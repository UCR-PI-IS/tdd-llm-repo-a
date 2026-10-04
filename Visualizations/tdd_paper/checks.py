"""Integrity gate, regression checks and the output manifest.

``run_checks`` returns a list of ``Check`` results; ``cli check`` exits non-zero
when any required check fails. The expectations live in ``config``.
"""
from __future__ import annotations

import hashlib
import json
import platform
import re
from dataclasses import asdict, dataclass
from datetime import datetime, timezone
from pathlib import Path

import pandas as pd

from . import __version__
from . import config as C
from . import gitfacts as gf
from .schema import COLUMN_CATALOG, validate_runs


@dataclass
class Check:
    name: str
    ok: bool
    detail: str = ""
    required: bool = True

    def to_dict(self) -> dict:
        return {"name": self.name, "ok": bool(self.ok), "detail": str(self.detail), "required": bool(self.required)}


def _sha256(path: Path) -> str:
    return hashlib.sha256(path.read_bytes()).hexdigest()


def dataset_checks(bundle: dict) -> list[Check]:
    runs = bundle["runs"]
    out: list[Check] = []
    out.append(Check("rows", len(runs) == C.EXPECTED_CELLS, f"{len(runs)} rows, expected {C.EXPECTED_CELLS}"))
    per = runs.groupby("story").size()
    out.append(Check("twelve-per-story", bool((per == 12).all()), per.to_dict().__repr__()))
    out.append(Check("model-set", set(runs["model"]) == set(C.MODEL_ORDER), str(sorted(set(runs["model"])))))
    try:
        warn = validate_runs(runs, strict=False)
        out.append(Check("schema", True, f"{len(warn)} warnings"))
    except Exception as exc:
        out.append(Check("schema", False, str(exc)))
    out.append(Check("run-id-unique", bool(runs["run_id"].is_unique)))
    na_e2e = int(runs["e2e_execs"].isna().sum())
    out.append(Check("e2e-missing", na_e2e == C.EXPECTED_CELLS - C.EXPECTED_E2E_CELLS, f"{na_e2e} runs without e2e evidence"))
    amb = set(runs.loc[runs["identity_ambiguous"].fillna(False).astype(bool), "run_id"])
    out.append(Check("identity-ambiguous", amb == set(C.IDENTITY_AMBIGUOUS_CELLS), f"{sorted(amb)}"))
    cr = set(runs.loc[pd.to_numeric(runs["compile_remove_added"], errors="coerce").fillna(0) > 0, "run_id"])
    out.append(Check("compile-remove", cr == set(C.COMPILE_REMOVE_CELLS), f"{len(cr)} cells"))
    exc_ = set(runs.loc[runs["exceptional_ending"].fillna("") != "", "run_id"])
    out.append(Check("exceptional-endings", exc_ == set(C.EXCEPTIONAL_ENDING_CELLS), f"{sorted(exc_)}"))
    out.append(Check("no-pre-e2e", not runs["harness_pre_e2e"].fillna(False).astype(bool).any()))
    out.append(Check("two-run-commits", bool(runs["n_run_commits"].eq(2).all()), runs["n_run_commits"].value_counts(dropna=False).to_dict().__repr__()))
    wrong = int((runs["intents_confirmed_ok"] == False).sum())
    missing = int(runs["intents_confirmed_ok"].isna().sum())
    out.append(Check("intents-report", wrong == C.INTENTS_CONFIRMED_WRONG_EXPECTED and missing == C.INTENTS_CONFIRMED_MISSING_EXPECTED,
                     f"wrong={wrong} missing={missing}"))
    cell, methods, cases = C.CALIBRATION_CELL
    row = runs[runs["run_id"] == cell]
    ok = (not row.empty and int(row["new_test_methods"].iloc[0]) == methods and int(row["new_test_cases"].iloc[0]) == cases)
    out.append(Check("calibration-run", ok, f"{cell}: methods={row['new_test_methods'].iloc[0] if not row.empty else None} cases={row['new_test_cases'].iloc[0] if not row.empty else None}"))
    incomplete = int((runs["evidence_completeness"] < 1.0).sum())
    out.append(Check("evidence-completeness", incomplete == len(C.E2E_MISSING_CELLS),
                     f"{incomplete} runs with fewer than four trees, expected {len(C.E2E_MISSING_CELLS)}"))
    scored = [m for m, s in COLUMN_CATALOG.items() if s.family in ("C convergence", "V verification", "Q quality", "B behavioral")
              and s.dtype in ("Int64", "float64", "boolean") and m in runs.columns and m not in ("tests_per_intent", "layer_coverage", "layer_balance")]
    dead = [m for m in scored if runs[m].nunique(dropna=True) <= 1]
    out.append(Check("scored-metrics-discriminate", not dead, f"constant or all-NA: {dead}", required=False))
    ex = bundle.get("excluded_refs", pd.DataFrame())
    if not ex.empty:
        out.append(Check("excluded-models", int((ex["reason"] == "model-not-in-study").sum()) == 20, ex["reason"].value_counts().to_dict().__repr__(), required=False))
    return out


def composite_regression() -> list[Check]:
    import tdd_results as tdd
    from . import composite as comp
    tdd.reset_caches()
    data = tdd.load_all("PQL-AE-001-002")
    summ = tdd.iteration_summary("PQL-AE-001-002", data)
    r = comp.score_story(summ, tdd.finals(data["metrics_types"]))
    top = r.iloc[0]
    expected = [("Qwen3.7-max", 1, 0.832), ("Qwen3.7-max", 6, 0.803), ("Qwen3.7-max", 5, 0.760), ("Kimi-K2.5", 3, 0.745),
                ("Kimi-K2.5", 4, 0.740), ("Qwen3.7-max", 3, 0.738)]
    ok = True
    details = []
    for (m, it, comp_val), (_, row) in zip(expected, r.head(6).iterrows()):
        good = tdd.canonical_model(row["model"]) == tdd.canonical_model(m) and int(row["iteration"]) == it and abs(row["composite"] - comp_val) < 5e-4
        ok &= good
        details.append(f"{row['model']}/{int(row['iteration'])}={row['composite']:.3f}")
    ok &= bool(top["pareto"]) and abs(top["top1_share"] - 1.0) < 1e-9
    return [Check("composite-regression", bool(ok), "; ".join(details))]


def stage_identity_checks() -> list[Check]:
    import tdd_results as tdd
    bad = []
    for story in C.STORY_ORDER:
        tdd.reset_caches()
        st = tdd.load_stage_results(story)
        stray = sorted(set(st["model"]) - set(C.MODEL_ORDER) - {"qwen3.7-max", "Kimi-k2.5", "Kimi-K2-5"})
        if stray:
            bad.append(f"{story}: {stray}")
    return [Check("stage-identity", not bad, "; ".join(bad) or "no stray stage models")]


def latest_duplicate_check() -> list[Check]:
    problems = []
    for latest in (C.REPO / "MetricsResults").glob("*/*/*/latest/metrics-summary.json"):
        siblings = sorted(p for p in latest.parent.parent.iterdir() if p.is_dir() and p.name != "latest")
        if not siblings:
            problems.append(f"{latest}: no timestamped sibling")
            continue
        last = siblings[-1] / "metrics-summary.json"
        try:
            a = json.loads(latest.read_text())
            b = json.loads(last.read_text())
        except Exception as exc:
            problems.append(f"{latest}: {exc}")
            continue
        if a != b:
            problems.append(f"{latest}: differs from {last.parent.name}")
    return [Check("latest-duplicate-snapshot", not problems, "; ".join(problems) or "all latest/ folders duplicate the last snapshot")]


def stats_selftest() -> list[Check]:
    import numpy as np
    from . import stats as S
    out = []
    try:
        d = S.cliffs_delta(np.array([1, 2, 3]), np.array([4, 5, 6]))
        out.append(Check("stats-cliff", abs(d + 1) < 1e-12, f"delta={d}"))
        h = S.holm({"a": 0.01, "b": 0.02, "c": 0.03, "d": 0.04})
        hp = h.set_index("name")["p_holm"]
        out.append(Check("stats-holm", abs(hp["a"] - 0.04) < 1e-9 and abs(hp["b"] - 0.06) < 1e-9, hp.round(3).to_dict().__repr__()))
        out.append(Check("stats-passk", abs(S.pass_at_k(6, 3, 1) - 0.5) < 1e-12 and abs(S.pass_at_k(6, 3, 4) - 1.0) < 1e-12))
    except Exception as exc:
        out.append(Check("stats-selftest", False, f"{type(exc).__name__}: {exc}"))
    return out


def generated_hash_check(out_dir: Path) -> list[Check]:
    """Detect hand edits of generated outputs by comparing with the manifest."""
    out_dir = Path(out_dir)
    mpath = out_dir / "manifest.json"
    if not mpath.exists():
        return [Check("manifest-present", False, "no manifest.json; run `tdd_paper all` first", required=False)]
    manifest = json.loads(mpath.read_text())
    changed = []
    missing = []
    for rel, sha in manifest.get("files", {}).items():
        p = out_dir / rel
        if not p.exists():
            missing.append(rel)
        elif _sha256(p) != sha:
            changed.append(rel)
    return [Check("generated-unmodified", not changed and not missing,
                  f"changed={changed[:5]} missing={missing[:5]}")]


def write_manifest(out_dir: Path, *, extra: dict | None = None) -> Path:
    out_dir = Path(out_dir)
    files = {}
    for p in sorted(out_dir.rglob("*")):
        if p.is_file() and p.name != "manifest.json":
            files[str(p.relative_to(out_dir))] = _sha256(p)
    manifest = {
        "build_id": datetime.now(timezone.utc).strftime("%Y%m%dT%H%M%SZ"),
        "generated_at": datetime.now(timezone.utc).isoformat(timespec="seconds"),
        "package_version": __version__,
        "python": platform.python_version(),
        "evidence_head": gf.git("rev-parse", "HEAD").strip(),
        "evidence_branch": gf.git("rev-parse", "--abbrev-ref", "HEAD").strip(),
        "harness_head": gf.git("rev-parse", C.TRUNK, check=False).strip() or None,
        "n_files": len(files),
        "files": files,
        **(extra or {}),
    }
    (out_dir / "manifest.json").write_text(json.dumps(manifest, indent=1))
    return out_dir / "manifest.json"


def untracked_execution_check() -> list[Check]:
    """Every timestamped execution folder in the four result trees must hold at least one tracked file.

    A folder with only git-ignored files (logs, raw coverage, packages) is the residue of a discarded attempt
    (for example after ``git reset``); the loaders would read it as an execution and pick it as a cell's final one
    (2026-10-04: 23 such folders from a mislabelled re-run sat in SPT-UM-001-003/Qwen3.7-max/1).
    """
    trees = ("BuildResults", "TestResults", "MetricsResults", "E2EResults")
    tracked = set()
    for line in gf.git("ls-files", "--", *trees, check=False).splitlines():
        parts = line.split("/")
        if len(parts) >= 5:
            tracked.add("/".join(parts[:5]))
    stray = []
    for tree in trees:
        root = C.REPO / tree
        if not root.is_dir():
            continue
        for d in root.glob("*/*/*/*"):
            if d.is_dir() and re.match(r"\d{4}-\d{2}-\d{2}_", d.name) and str(d.relative_to(C.REPO)) not in tracked:
                stray.append(str(d.relative_to(C.REPO)))
    return [Check("no-untracked-executions", not stray, f"{len(stray)} execution folders without tracked files"
                  + (f", e.g. {stray[:3]}" if stray else ""))]


def run_checks(bundle: dict | None = None, *, out_dir: Path | None = None, with_composite: bool = True,
               with_latest: bool = True) -> list[Check]:
    checks: list[Check] = []
    if bundle is not None:
        checks += dataset_checks(bundle)
    checks += stage_identity_checks()
    if with_latest:
        checks += latest_duplicate_check()
    checks += untracked_execution_check()
    if with_composite:
        try:
            checks += composite_regression()
        except Exception as exc:
            checks.append(Check("composite-regression", False, f"{type(exc).__name__}: {exc}"))
    checks += stats_selftest()
    if out_dir is not None:
        checks += generated_hash_check(out_dir)
    return checks


def report(checks: list[Check]) -> str:
    lines = []
    for c in checks:
        flag = "PASS" if c.ok else ("FAIL" if c.required else "WARN")
        lines.append(f"[{flag}] {c.name}: {c.detail}")
    n_fail = sum(1 for c in checks if not c.ok and c.required)
    lines.append(f"{len(checks)} checks, {n_fail} required failures")
    return "\n".join(lines)
