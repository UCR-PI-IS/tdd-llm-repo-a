"""Data loading and styling for the TDD-LLM experiment visualizations.

Result trees share the scheme ``<Tree>/<STORY>/<MODEL>/<ITERATION>/<TIMESTAMP>/``.
Each (story, model, iteration) cell holds many timestamped retry executions;
the *final* execution (max timestamp) is the iteration's outcome, while the
full set of attempts is itself an effort signal.

Every loader returns a tidy DataFrame keyed by
``story, model, iteration, ts, attempt`` and returns an empty frame with the
full column set when no data matches, so notebooks never crash on missing data.
"""

from __future__ import annotations

import json
import re
import xml.etree.ElementTree as ET
from datetime import datetime
from pathlib import Path

import matplotlib.transforms as mtransforms
import pandas as pd
import seaborn as sns

REPO_ROOT = Path(__file__).resolve().parent.parent
BUILD_TREE = REPO_ROOT / "BuildResults"
TEST_TREE = REPO_ROOT / "TestResults"
METRICS_TREE = REPO_ROOT / "MetricsResults"
E2E_TREE = REPO_ROOT / "E2EResults"
INTENTS_DIR = REPO_ROOT / "UserIntents"

TREES = {"build": BUILD_TREE, "test": TEST_TREE, "metrics": METRICS_TREE,
         "e2e": E2E_TREE}

#: Architectural layers used by the intent/test-coverage metrics.
LAYERS = ("Domain", "Application", "Infrastructure", "Presentation")

#: Non-fatal problems found while loading (bad JSON, odd folder names, ...).
load_warnings: list[str] = []

#: Iterations included in every loader; None = no filter. Iterations 7-8
#: (DeepSeek extras, no canonical results) are excluded per experiment design.
INCLUDED_ITERATIONS: set[int] | None = set(range(1, 7))


def _iteration_included(iteration: int) -> bool:
    return INCLUDED_ITERATIONS is None or iteration in INCLUDED_ITERATIONS

_TS_FORMATS = ("%Y-%m-%d_%H-%M-%S", "%Y%m%d%H%M%S")
# Folders that legitimately sit beside timestamp folders inside an iteration.
_NON_TS_DIRS = {"test-generator"}

_KEYS = ["story", "model", "iteration"]

# ---------------------------------------------------------------------------
# Discovery
# ---------------------------------------------------------------------------


_CANON_RE = re.compile(r"[^a-z0-9]+")
_label_cache: dict[str, dict[str, str]] = {}


def canonical_model(name: str) -> str:
    """Join key for a model: case- and punctuation-insensitive.

    Result trees disagree about spelling (``Kimi-K2.5`` under BuildResults,
    ``Kimi-k2.5`` under E2EResults) and the ``"model"`` field agents write
    inside their JSON disagrees with both. All variants collapse here, so a
    new story or model that arrives with yet another spelling still joins.
    """
    return _CANON_RE.sub("", str(name).lower())


def _labels_for(story: str) -> dict[str, str]:
    """canonical id -> the spelling used for display, chosen deterministically.

    Only *directory* names are considered: those are created by the harness,
    not written by the agent. Among the variants seen, the one with the most
    capitals wins (ties broken lexicographically), so the label is stable no
    matter which tree is scanned first.
    """
    if story in _label_cache:
        return _label_cache[story]
    best: dict[str, str] = {}
    for tree in TREES.values():
        story_dir = tree / story
        if not story_dir.is_dir():
            continue
        for name in sorted(p.name for p in story_dir.iterdir() if p.is_dir()):
            c = canonical_model(name)
            cur = best.get(c)
            rank = (sum(ch.isupper() for ch in name), name)
            if cur is None or rank > (sum(ch.isupper() for ch in cur), cur):
                best[c] = name
    _label_cache[story] = best
    return best


def resolve_model(story: str, name: str) -> str:
    """Any spelling of a model -> the one label every frame agrees on."""
    return _labels_for(story).get(canonical_model(name), str(name))


def reset_caches() -> None:
    """Drop the label cache; call after adding result folders mid-session."""
    _label_cache.clear()
    load_warnings.clear()


def pick(rec: dict | None, *aliases, default=None):
    """First alias present with a non-None value; warns when none resolve.

    Absorbs renames such as ``totalProbes`` -> ``probesTotal`` without a code
    change, so a schema revision degrades to a logged warning rather than a
    silently wrong number.
    """
    rec = rec or {}
    for a in aliases:
        if rec.get(a) is not None:
            return rec[a]
    load_warnings.append(f"no field matched {'|'.join(aliases)}")
    return default


def as_int(value, default=None):
    try:
        return int(str(value).strip())
    except (TypeError, ValueError):
        return default


def parse_ts(name: str) -> datetime | None:
    for fmt in _TS_FORMATS:
        try:
            return datetime.strptime(name, fmt)
        except ValueError:
            continue
    return None


def discover_stories() -> list[str]:
    stories: set[str] = set()
    for tree in TREES.values():
        if tree.is_dir():
            stories.update(p.name for p in tree.iterdir() if p.is_dir())
    return sorted(stories)


def discover_models(story: str) -> list[str]:
    """One entry per distinct model, spelling drift already folded away."""
    return sorted(_labels_for(story).values())


def _execution_dirs(tree: Path, story: str):
    """Yield (model, iteration, ts, path) for every timestamped execution dir."""
    story_dir = tree / story
    if not story_dir.is_dir():
        return
    for model_dir in sorted(p for p in story_dir.iterdir() if p.is_dir()):
        for iter_dir in sorted(p for p in model_dir.iterdir() if p.is_dir()):
            if not iter_dir.name.isdigit():
                load_warnings.append(f"non-numeric iteration folder skipped: {iter_dir}")
                continue
            if not _iteration_included(int(iter_dir.name)):
                continue
            for ts_dir in sorted(p for p in iter_dir.iterdir() if p.is_dir()):
                ts = parse_ts(ts_dir.name)
                if ts is None:
                    if ts_dir.name not in _NON_TS_DIRS:
                        load_warnings.append(f"unparseable timestamp folder skipped: {ts_dir}")
                    continue
                yield resolve_model(story, model_dir.name), int(iter_dir.name), ts, ts_dir


def discover_cells(story: str) -> pd.DataFrame:
    """All (story, model, iteration) cells present in any of the three trees."""
    rows = {
        (story, model, iteration)
        for tree in TREES.values()
        for model, iteration, _, _ in _execution_dirs(tree, story)
    }
    return pd.DataFrame(sorted(rows), columns=_KEYS)


# ---------------------------------------------------------------------------
# Frame helpers
# ---------------------------------------------------------------------------


def _frame(rows: list[dict], columns: list[str]) -> pd.DataFrame:
    df = pd.DataFrame(rows, columns=columns)
    if "ts" in columns:
        df["ts"] = pd.to_datetime(df["ts"])
        df = _add_attempt(df)
    return df


def _add_attempt(df: pd.DataFrame) -> pd.DataFrame:
    """1-based chronological rank of each execution within its cell."""
    if df.empty:
        df["attempt"] = pd.Series(dtype="int64")
        return df
    df = df.sort_values(_KEYS + ["ts"]).reset_index(drop=True)
    df["attempt"] = df.groupby(_KEYS)["ts"].rank(method="dense").astype(int)
    return df


def _read_json(path: Path) -> dict | None:
    try:
        return json.loads(path.read_text())
    except (OSError, json.JSONDecodeError) as exc:
        load_warnings.append(f"unreadable JSON skipped: {path} ({exc})")
        return None


def finals(df: pd.DataFrame, keys: list[str] | None = None) -> pd.DataFrame:
    """Rows belonging to the last (max ts) execution of each cell."""
    if df.empty:
        return df
    keys = keys or _KEYS
    last = df.groupby(keys)["ts"].transform("max")
    return df[df["ts"] == last]


def attempt_counts(df: pd.DataFrame, tree: str) -> pd.DataFrame:
    """Executions per cell for one result tree — the retry/effort signal."""
    cols = _KEYS + ["tree", "n_attempts"]
    if df.empty:
        return pd.DataFrame(columns=cols)
    out = df.groupby(_KEYS)["ts"].nunique().reset_index(name="n_attempts")
    out["tree"] = tree
    return out[cols]


# ---------------------------------------------------------------------------
# Build results
# ---------------------------------------------------------------------------

_BUILD_COLS = _KEYS + ["ts", "status", "total_errors", "total_warnings",
                       "n_projects", "n_failed_projects"]
_BUILD_PROJ_COLS = _KEYS + ["ts", "project", "status", "errors", "warnings",
                            "n_error_messages"]


def load_build_executions(story: str) -> pd.DataFrame:
    rows = []
    for model, iteration, ts, ts_dir in _execution_dirs(BUILD_TREE, story):
        rec = _read_json(ts_dir / "build-summary.json") if (ts_dir / "build-summary.json").exists() else None
        if rec is None:
            continue
        projects = rec.get("projects") or []
        rows.append({
            "story": story, "model": model, "iteration": iteration, "ts": ts,
            "status": rec.get("status"),
            "total_errors": rec.get("totalErrors", 0),
            "total_warnings": rec.get("totalWarnings", 0),
            "n_projects": len(projects),
            "n_failed_projects": sum(1 for p in projects if p.get("status") != "success"),
        })
    return _frame(rows, _BUILD_COLS)


def load_build_projects(story: str) -> pd.DataFrame:
    rows = []
    for model, iteration, ts, ts_dir in _execution_dirs(BUILD_TREE, story):
        rec = _read_json(ts_dir / "build-summary.json") if (ts_dir / "build-summary.json").exists() else None
        if rec is None:
            continue
        for proj in rec.get("projects") or []:
            rows.append({
                "story": story, "model": model, "iteration": iteration, "ts": ts,
                "project": proj.get("name"),
                "status": proj.get("status"),
                "errors": proj.get("errors", 0),
                "warnings": proj.get("warnings", 0),
                "n_error_messages": len(proj.get("errorMessages") or []),
            })
    return _frame(rows, _BUILD_PROJ_COLS)


# ---------------------------------------------------------------------------
# Test results
# ---------------------------------------------------------------------------

_TEST_COLS = _KEYS + ["ts", "status", "total", "passed", "failed", "skipped",
                      "pass_rate"]
_TEST_PROJ_COLS = _KEYS + ["ts", "project", "total", "passed", "failed", "skipped"]


def load_test_executions(story: str) -> pd.DataFrame:
    rows = []
    for model, iteration, ts, ts_dir in _execution_dirs(TEST_TREE, story):
        summary = ts_dir / "test-summary.json"
        rec = _read_json(summary) if summary.exists() else None
        if rec is None:
            continue
        total = rec.get("totalTests", 0)
        passed = rec.get("totalPassed", 0)
        rows.append({
            "story": story, "model": model, "iteration": iteration, "ts": ts,
            "status": rec.get("status"),
            "total": total, "passed": passed,
            "failed": rec.get("totalFailed", 0),
            "skipped": rec.get("totalSkipped", 0),
            "pass_rate": passed / total if total else float("nan"),
        })
    return _frame(rows, _TEST_COLS)


def load_test_projects(story: str) -> pd.DataFrame:
    """Per-project test counts. Not part of :func:`load_all` -- no notebook
    charts it today; call it directly if you need the breakdown."""
    rows = []
    for model, iteration, ts, ts_dir in _execution_dirs(TEST_TREE, story):
        summary = ts_dir / "test-summary.json"
        rec = _read_json(summary) if summary.exists() else None
        if rec is None:
            continue
        for proj in rec.get("projects") or []:
            rows.append({
                "story": story, "model": model, "iteration": iteration, "ts": ts,
                "project": proj.get("name"),
                "total": proj.get("total", 0), "passed": proj.get("passed", 0),
                "failed": proj.get("failed", 0), "skipped": proj.get("skipped", 0),
            })
    return _frame(rows, _TEST_PROJ_COLS)


_TRX_NS = "{http://microsoft.com/schemas/VisualStudio/TeamTest/2010}"
_TRX_COLS = _KEYS + ["ts", "project", "test_name", "class_name", "outcome",
                     "duration_s"]


def load_trx_tests(story: str, finals_only: bool = True) -> pd.DataFrame:
    """Per-test rows from VSTest .trx files (parse cost => finals by default)."""
    dirs = [(m, i, ts, d) for m, i, ts, d in _execution_dirs(TEST_TREE, story)]
    if finals_only and dirs:
        last = {}
        for m, i, ts, d in dirs:
            if (m, i) not in last or ts > last[(m, i)][0]:
                last[(m, i)] = (ts, d)
        dirs = [(m, i, ts, d) for (m, i), (ts, d) in last.items()]
    rows = []
    for model, iteration, ts, ts_dir in dirs:
        for trx in sorted(ts_dir.rglob("*.trx")):
            try:
                root = ET.parse(trx).getroot()
            except ET.ParseError as exc:
                load_warnings.append(f"unparseable trx skipped: {trx} ({exc})")
                continue
            class_by_id = {
                ut.get("id"): tm.get("className", "").split(",")[0]
                for ut in root.iter(f"{_TRX_NS}UnitTest")
                for tm in ut.iter(f"{_TRX_NS}TestMethod")
            }
            for res in root.iter(f"{_TRX_NS}UnitTestResult"):
                duration = res.get("duration")
                rows.append({
                    "story": story, "model": model, "iteration": iteration, "ts": ts,
                    "project": trx.parent.name,
                    "test_name": res.get("testName"),
                    "class_name": class_by_id.get(res.get("testId"), ""),
                    "outcome": res.get("outcome"),
                    "duration_s": pd.to_timedelta(duration).total_seconds() if duration else float("nan"),
                })
    return _frame(rows, _TRX_COLS)


_COV_COLS = _KEYS + ["ts", "line_rate", "branch_rate", "lines_covered",
                     "lines_valid"]


def load_coverage(story: str, finals_only: bool = True) -> pd.DataFrame:
    """Line/branch coverage from the combined Cobertura report."""
    rows = []
    for model, iteration, ts, ts_dir in _execution_dirs(TEST_TREE, story):
        cob = ts_dir / "Coverage" / "Combined" / "Cobertura.xml"
        if not cob.exists():
            continue
        try:
            root = ET.parse(cob).getroot()
        except ET.ParseError as exc:
            load_warnings.append(f"unparseable cobertura skipped: {cob} ({exc})")
            continue
        rows.append({
            "story": story, "model": model, "iteration": iteration, "ts": ts,
            "line_rate": float(root.get("line-rate", "nan")),
            "branch_rate": float(root.get("branch-rate", "nan")),
            "lines_covered": int(root.get("lines-covered", 0)),
            "lines_valid": int(root.get("lines-valid", 0)),
        })
    df = _frame(rows, _COV_COLS)
    return finals(df) if finals_only else df


# ---------------------------------------------------------------------------
# Code metrics
# ---------------------------------------------------------------------------

_MET_TYPE_COLS = _KEYS + ["ts", "project", "type", "mi", "mi_flag", "cc", "cc_flag",
                          "coupling", "coupling_flag", "dit", "dit_flag",
                          "source_lines", "executable_lines"]
_MET_METHOD_COLS = _KEYS + ["ts", "project", "type", "method", "mi", "mi_flag",
                            "cc", "cc_flag", "source_lines", "executable_lines"]


def _metric(entry: dict | None, key: str):
    val = (entry or {}).get(key) or {}
    return val.get("value"), val.get("flag")


def load_metrics_types(story: str) -> pd.DataFrame:
    rows = []
    for model, iteration, ts, ts_dir in _execution_dirs(METRICS_TREE, story):
        summary = ts_dir / "metrics-summary.json"
        rec = _read_json(summary) if summary.exists() else None
        if rec is None:
            continue
        for proj in rec.get("projects") or []:
            for typ in proj.get("types") or []:
                mi, mi_f = _metric(typ, "maintainabilityIndex")
                cc, cc_f = _metric(typ, "cyclomaticComplexity")
                cp, cp_f = _metric(typ, "classCoupling")
                dit, dit_f = _metric(typ, "depthOfInheritance")
                rows.append({
                    "story": story, "model": model, "iteration": iteration, "ts": ts,
                    "project": proj.get("name"), "type": typ.get("name"),
                    "mi": mi, "mi_flag": mi_f, "cc": cc, "cc_flag": cc_f,
                    "coupling": cp, "coupling_flag": cp_f, "dit": dit, "dit_flag": dit_f,
                    "source_lines": typ.get("sourceLines"),
                    "executable_lines": typ.get("executableLines"),
                })
    return _frame(rows, _MET_TYPE_COLS)


def load_metrics_methods(story: str) -> pd.DataFrame:
    rows = []
    for model, iteration, ts, ts_dir in _execution_dirs(METRICS_TREE, story):
        summary = ts_dir / "metrics-summary.json"
        rec = _read_json(summary) if summary.exists() else None
        if rec is None:
            continue
        for proj in rec.get("projects") or []:
            for typ in proj.get("types") or []:
                for meth in typ.get("methods") or []:
                    mi, mi_f = _metric(meth, "maintainabilityIndex")
                    cc, cc_f = _metric(meth, "cyclomaticComplexity")
                    rows.append({
                        "story": story, "model": model, "iteration": iteration, "ts": ts,
                        "project": proj.get("name"), "type": typ.get("name"),
                        "method": meth.get("name"),
                        "mi": mi, "mi_flag": mi_f, "cc": cc, "cc_flag": cc_f,
                        "source_lines": meth.get("sourceLines"),
                        "executable_lines": meth.get("executableLines"),
                    })
    return _frame(rows, _MET_METHOD_COLS)


# ---------------------------------------------------------------------------
# Pipeline stage results
# ---------------------------------------------------------------------------

_STAGE_COLS = _KEYS + ["ts", "stage", "status", "n_warnings", "notes",
                       "n_files_created", "n_files_modified",
                       "tg_intents_confirmed", "tg_test_methods",
                       "ref_loop_iterations", "ref_max_loop_iterations",
                       "ref_all_green", "ref_n_remaining_violations",
                       "ref_baseline_minMI", "ref_final_minMI",
                       "ref_baseline_maxCC", "ref_final_maxCC",
                       "ref_baseline_maxCoupling", "ref_final_maxCoupling",
                       "ref_baseline_maxDIT", "ref_final_maxDIT"]
_VIOLATION_COLS = _KEYS + ["ts", "type", "metric", "value", "flag"]


def _stage_files(story: str):
    for tree in (TEST_TREE, METRICS_TREE):
        story_dir = tree / story
        if story_dir.is_dir():
            yield from sorted(story_dir.rglob("pipeline-stage-result.json"))


def load_stage_results(story: str) -> pd.DataFrame:
    """One row per (cell, stage) — deduped to the latest result when agents
    committed several (code-generation has duplicates in the raw data)."""
    rows = []
    for path in _stage_files(story):
        rec = _read_json(path)
        if rec is None or "stage" not in rec:
            continue
        # Identity comes from the path (harness-created), never from the
        # payload: agents spell their own name inconsistently.
        model = resolve_model(story, path.parts[-4])
        iteration = as_int(path.parts[-3], as_int(rec.get("iteration"), 0))
        if not _iteration_included(iteration):
            continue
        m = rec.get("metrics") or {}
        row = {
            "story": story,
            "model": model,
            "iteration": iteration,
            "ts": parse_ts(path.parent.name),
            "stage": rec["stage"], "status": rec.get("status"),
            "n_warnings": len(rec.get("warnings") or []),
            "notes": rec.get("notes"),
            "n_files_created": len(rec.get("filesCreated") or []),
            "n_files_modified": len(rec.get("filesModified") or []),
            "tg_intents_confirmed": m.get("intentsConfirmed"),
            "tg_test_methods": m.get("testMethodsEmitted"),
            "ref_loop_iterations": m.get("loopIterationsPerformed"),
            "ref_max_loop_iterations": m.get("maxLoopIterations"),
            "ref_all_green": m.get("allGreenAchieved"),
            "ref_n_remaining_violations": len(m.get("remainingViolations") or []),
        }
        for k in ("minMI", "maxCC", "maxCoupling", "maxDIT"):
            row[f"ref_baseline_{k}"] = (m.get("baseline") or {}).get(k)
            row[f"ref_final_{k}"] = (m.get("final") or {}).get(k)
        rows.append(row)
    df = pd.DataFrame(rows, columns=_STAGE_COLS)
    if df.empty:
        return df
    df["ts"] = pd.to_datetime(df["ts"])
    # Latest result wins per (cell, stage); NaT-ts rows (test-generator dir) sort first.
    df = (df.sort_values("ts", na_position="first")
            .groupby(_KEYS + ["stage"], as_index=False, dropna=False)
            .tail(1)
            .reset_index(drop=True))
    return df


def load_remaining_violations(story: str) -> pd.DataFrame:
    """Exploded refactoring `remainingViolations` (latest refactoring result per cell)."""
    per_cell: dict[tuple, tuple] = {}
    for path in _stage_files(story):
        rec = _read_json(path)
        if rec is None or rec.get("stage") != "refactoring":
            continue
        iteration = as_int(path.parts[-3], as_int(rec.get("iteration"), 0))
        if not _iteration_included(iteration):
            continue
        key = (resolve_model(story, path.parts[-4]), iteration)
        ts = parse_ts(path.parent.name)
        if key not in per_cell or (ts or datetime.min) > (per_cell[key][0] or datetime.min):
            per_cell[key] = (ts, rec)
    rows = []
    for (model, iteration), (ts, rec) in per_cell.items():
        for v in (rec.get("metrics") or {}).get("remainingViolations") or []:
            rows.append({
                "story": story, "model": model, "iteration": iteration, "ts": ts,
                "type": v.get("type"), "metric": v.get("metric"),
                "value": v.get("value"), "flag": v.get("flag"),
            })
    df = pd.DataFrame(rows, columns=_VIOLATION_COLS)
    df["ts"] = pd.to_datetime(df["ts"])
    return df


# ---------------------------------------------------------------------------
# User intents (denominator for "intents covered")
# ---------------------------------------------------------------------------

_INTENT_COLS = ["story", "layer", "n_intents"]


def load_user_intents(story: str) -> pd.DataFrame:
    path = INTENTS_DIR / f"{story}.json"
    rec = _read_json(path) if path.exists() else None
    if rec is None:
        return pd.DataFrame(columns=_INTENT_COLS)
    counts: dict[str, int] = {}
    for intent in rec.get("intents") or []:
        if intent.get("status") == "confirmed":
            counts[intent.get("layer", "?")] = counts.get(intent.get("layer", "?"), 0) + 1
    rows = [{"story": story, "layer": layer, "n_intents": n}
            for layer, n in sorted(counts.items())]
    return pd.DataFrame(rows, columns=_INTENT_COLS)


# ---------------------------------------------------------------------------
# Bundles
# ---------------------------------------------------------------------------


def load_all(story: str) -> dict[str, pd.DataFrame]:
    return {
        "build": load_build_executions(story),
        "build_projects": load_build_projects(story),
        "test": load_test_executions(story),
        "trx": load_trx_tests(story),
        "coverage": load_coverage(story),
        "metrics_types": load_metrics_types(story),
        "metrics_methods": load_metrics_methods(story),
        "stages": load_stage_results(story),
        "violations": load_remaining_violations(story),
        "intents": load_user_intents(story),
        "e2e": load_e2e_executions(story),
    }


def iteration_summary(story: str, data: dict[str, pd.DataFrame] | None = None) -> pd.DataFrame:
    """Master wide frame: one row per (model, iteration), outer-joined across
    all sources. Missing cells stay NaN (iteration counts differ per model)."""
    d = data or load_all(story)
    out = discover_cells(story)

    def merge(df: pd.DataFrame, cols: dict[str, str]):
        nonlocal out
        if df.empty:
            for target in cols.values():
                out[target] = float("nan")
            return
        sub = df[_KEYS + list(cols)].rename(columns=cols)
        out = out.merge(sub, on=_KEYS, how="left")

    merge(finals(d["build"]), {"status": "build_status", "total_errors": "build_errors",
                               "total_warnings": "build_warnings"})
    merge(finals(d["test"]), {"status": "test_status", "total": "total_tests",
                              "passed": "passed", "failed": "failed",
                              "skipped": "skipped", "pass_rate": "pass_rate"})
    merge(finals(d["coverage"]), {"line_rate": "line_rate", "branch_rate": "branch_rate"})

    mt = finals(d["metrics_types"])
    if not mt.empty:
        all_green = ((mt["mi_flag"] == "GREEN") & (mt["cc_flag"] == "GREEN")
                     & (mt["coupling_flag"] == "GREEN") & (mt["dit_flag"] == "GREEN"))
        agg = (mt.assign(all_green=all_green)
                 .groupby(_KEYS)
                 .agg(median_mi=("mi", "median"), min_mi=("mi", "min"),
                      max_cc=("cc", "max"), max_coupling=("coupling", "max"),
                      max_dit=("dit", "max"), n_types=("type", "count"),
                      pct_green_types=("all_green", "mean"),
                      source_lines=("source_lines", "sum"),
                      executable_lines=("executable_lines", "sum"))
                 .reset_index())
        out = out.merge(agg, on=_KEYS, how="left")
    else:
        for c in ["median_mi", "min_mi", "max_cc", "max_coupling", "max_dit",
                  "n_types", "pct_green_types", "source_lines", "executable_lines"]:
            out[c] = float("nan")

    stages = d["stages"]
    stage_cols = {
        "test-generation": {"status": "tg_status", "tg_test_methods": "tg_test_methods"},
        "code-generation": {"status": "cg_status", "n_files_created": "cg_files_created",
                            "n_files_modified": "cg_files_modified"},
        "refactoring": {"status": "ref_status", "ref_all_green": "ref_all_green",
                        "ref_loop_iterations": "ref_loop_iterations",
                        "ref_n_remaining_violations": "ref_n_remaining_violations"},
    }
    for stage, cols in stage_cols.items():
        merge(stages[stages["stage"] == stage] if not stages.empty else stages, cols)

    # NOTE: per-tree execution counts are not merged here -- build_path,
    # test_path and quality_movement already carry them as build_execs,
    # test_execs and metrics_snapshots. attempt_counts() stays available for
    # the long-format effort charts, which need one row per (cell, tree).

    # Path-based and tool-measured families (see the section above): these are
    # what stays informative once the saturated final-state columns flatten.
    for part in (evidence(story),
                 build_path(story, d["build"]),
                 test_path(story, d["test"]),
                 layer_coverage(story, d["trx"]),
                 quality_movement(story, d["metrics_types"]),
                 e2e_path(story, d["e2e"])):
        # Columns must exist even with no data behind them, so a notebook run
        # against a brand-new story degrades to empty charts, not KeyErrors.
        if part.empty:
            for col in (c for c in part.columns if c not in _KEYS):
                out[col] = float("nan")
        else:
            out = out.merge(part, on=_KEYS, how="left")

    return out


# ---------------------------------------------------------------------------
# End-to-end validation results
# ---------------------------------------------------------------------------

_E2E_COLS = _KEYS + ["ts", "status", "total_probes", "probes_passed",
                     "probes_failed", "backend_up", "n_errors"]


def load_e2e_executions(story: str) -> pd.DataFrame:
    """One row per e2e execution: a real database, a published API, HTTP probes.

    ``totalProbes`` here and ``probesTotal`` in the pipeline stage files name
    the same quantity, so both spellings are accepted.
    """
    rows = []
    for model, iteration, ts, ts_dir in _execution_dirs(E2E_TREE, story):
        summary = ts_dir / "e2e-summary.json"
        rec = _read_json(summary) if summary.exists() else None
        if rec is None:
            continue
        backend = rec.get("backend") or {}
        rows.append({
            "story": story, "model": model, "iteration": iteration, "ts": ts,
            "status": rec.get("status"),
            "total_probes": as_int(pick(rec, "totalProbes", "probesTotal", default=0), 0),
            "probes_passed": as_int(pick(rec, "probesPassed", "passedProbes", default=0), 0),
            "probes_failed": as_int(pick(rec, "probesFailed", "failedProbes", default=0), 0),
            "backend_up": bool(backend.get("started")),
            "n_errors": len(rec.get("errors") or []),
        })
    return _frame(rows, _E2E_COLS)


# ---------------------------------------------------------------------------
# Derived metrics — measured over the whole retry path, not the final state
#
# The pipeline does not stop until the build compiles and the tests are green,
# so any metric read off the final execution is saturated by construction: in
# wave-1 the final build status, build errors, pass rate and failed count are
# each constant across all runs. These read the path instead, which is where
# models actually differ, and prefer tool-written artifacts over the values
# agents report about themselves.
# ---------------------------------------------------------------------------


def build_path(story: str, build: pd.DataFrame | None = None) -> pd.DataFrame:
    """Cost of reaching a compiling build (replaces the constant final status).

    ``build_failed_execs`` counts executions that did not compile,
    ``build_errors_burned`` the compiler errors fought through in total, and
    ``build_peak_errors`` the worst single execution.
    """
    df = load_build_executions(story) if build is None else build
    cols = _KEYS + ["build_execs", "build_failed_execs", "build_errors_burned",
                    "build_peak_errors", "build_first_pass"]
    if df.empty:
        return pd.DataFrame(columns=cols)
    df = df.sort_values(_KEYS + ["ts"])
    return (df.groupby(_KEYS)
              .agg(build_execs=("ts", "nunique"),
                   build_failed_execs=("status", lambda s: int((s != "success").sum())),
                   build_errors_burned=("total_errors", "sum"),
                   build_peak_errors=("total_errors", "max"),
                   build_first_pass=("status", lambda s: bool(s.iloc[0] == "success")))
              .reset_index())


def test_path(story: str, test: pd.DataFrame | None = None) -> pd.DataFrame:
    """Cost of reaching a green suite (replaces the constant final pass rate).

    ``red_first`` records that the first test execution was *not* green, the
    shape test-first development is supposed to produce; a suite green on its
    first run is evidence the tests followed the code.
    """
    df = load_test_executions(story) if test is None else test
    cols = _KEYS + ["test_execs", "attempts_to_green", "test_failures_burned",
                    "red_first"]
    if df.empty:
        return pd.DataFrame(columns=cols)
    df = df.sort_values(_KEYS + ["ts"]).copy()
    df["green"] = (df["failed"] == 0) & (df["total"] > 0)
    rows = []
    for key, g in df.groupby(_KEYS):
        g = g.reset_index(drop=True)
        rows.append(dict(zip(_KEYS, key)) | {
            "test_execs": int(g["ts"].nunique()),
            "attempts_to_green": int(g["green"].idxmax()) + 1 if g["green"].any() else pd.NA,
            "test_failures_burned": int(g["failed"].sum()),
            "red_first": bool(not g["green"].iloc[0]),
        })
    return pd.DataFrame(rows, columns=cols)


def intents_by_layer(story: str) -> dict[str, int]:
    """Ground-truth confirmed intents per architectural layer."""
    df = load_user_intents(story)
    return {} if df.empty else dict(zip(df["layer"], df["n_intents"]))


_LAYER_RE = re.compile(r"Backend\.(" + "|".join(LAYERS) + r")\b")


def layer_coverage(story: str, trx: pd.DataFrame | None = None) -> pd.DataFrame:
    """Intent coverage measured from .trx, not from the agent's own count.

    The test-generation stage reports ``intentsConfirmed`` itself; in wave-1 it
    says 20 where the story's intents file lists 21. Both sides here are
    tool-measured: emitted test methods come from the final run's VSTest
    output, attributed to a layer by assembly name.

    ``layer_balance`` is 1 minus the mean absolute deviation of the per-layer
    test:intent ratio — it catches suites that pile onto Domain and leave
    Infrastructure thin, which a single overall ratio hides.
    """
    df = load_trx_tests(story) if trx is None else trx
    want = intents_by_layer(story)
    total_intents = sum(want.values())
    cols = _KEYS + ["test_methods", "tests_per_intent", "layer_coverage",
                    "layer_balance"] + [f"tests_{l}" for l in LAYERS]
    if df.empty or not total_intents:
        return pd.DataFrame(columns=cols)

    def _layer(row):
        for candidate in (row["project"], row["class_name"]):
            m = _LAYER_RE.search(str(candidate))
            if m:
                return m.group(1)
        return None

    df = df.copy()
    df["layer"] = df.apply(_layer, axis=1)
    unattributed = int(df["layer"].isna().sum())
    if unattributed:
        load_warnings.append(f"{unattributed} tests could not be attributed to a layer")
    rows = []
    for key, g in df.groupby(_KEYS):
        per_layer = {l: int((g["layer"] == l).sum()) for l in LAYERS}
        ratios = [per_layer[l] / want[l] for l in LAYERS if want.get(l)]
        mad = sum(abs(r - 1.0) for r in ratios) / len(ratios) if ratios else float("nan")
        rows.append(dict(zip(_KEYS, key)) | {
            "test_methods": len(g),
            "tests_per_intent": len(g) / total_intents,
            "layer_coverage": sum(r >= 1.0 for r in ratios) / len(ratios) if ratios else float("nan"),
            "layer_balance": max(0.0, 1.0 - mad),
            **{f"tests_{l}": per_layer[l] for l in LAYERS},
        })
    return pd.DataFrame(rows, columns=cols)


def quality_movement(story: str, metrics_types: pd.DataFrame | None = None) -> pd.DataFrame:
    """Refactoring effect measured from consecutive metrics snapshots.

    Replaces the agent-reported ``allGreenAchieved`` / ``loopIterationsPerformed``
    / ``remainingViolations``: those describe what the agent believes it did,
    these measure what the analyser found before and after.
    """
    df = load_metrics_types(story) if metrics_types is None else metrics_types
    cols = _KEYS + ["metrics_snapshots", "worst_coupling_drop", "green_share_gain",
                    "min_mi_gain"]
    if df.empty:
        return pd.DataFrame(columns=cols)
    df = df.copy()
    df["all_green"] = ((df["mi_flag"] == "GREEN") & (df["cc_flag"] == "GREEN")
                       & (df["coupling_flag"] == "GREEN") & (df["dit_flag"] == "GREEN"))
    rows = []
    for key, g in df.groupby(_KEYS):
        first, last = g[g["ts"] == g["ts"].min()], g[g["ts"] == g["ts"].max()]
        rows.append(dict(zip(_KEYS, key)) | {
            "metrics_snapshots": int(g["ts"].nunique()),
            "worst_coupling_drop": first["coupling"].max() - last["coupling"].max(),
            "green_share_gain": last["all_green"].mean() - first["all_green"].mean(),
            "min_mi_gain": last["mi"].min() - first["mi"].min(),
        })
    return pd.DataFrame(rows, columns=cols)


def e2e_path(story: str, e2e: pd.DataFrame | None = None) -> pd.DataFrame:
    """Behavioural verification: does the story work against a real stack?

    Unit tests pass by construction, so this is the only evidence the feature
    actually runs. Saturation applies here too — wave-1's final e2e run is a
    success for every cell while 14 of 26 executions failed — so the outcome is
    scored through the path. ``e2e_infra_failures`` isolates runs where the API
    never came up, keeping harness flakiness off the model's record.
    """
    df = load_e2e_executions(story) if e2e is None else e2e
    cols = _KEYS + ["e2e_execs", "e2e_attempts_to_pass", "e2e_probe_depth",
                    "e2e_probe_pass_rate", "e2e_infra_failures"]
    if df.empty:
        return pd.DataFrame(columns=cols)
    df = df.sort_values(_KEYS + ["ts"])
    rows = []
    for key, g in df.groupby(_KEYS):
        g = g.reset_index(drop=True)
        ok = g["status"] == "success"
        last = g.iloc[-1]
        rows.append(dict(zip(_KEYS, key)) | {
            "e2e_execs": len(g),
            "e2e_attempts_to_pass": int(ok.idxmax()) + 1 if ok.any() else pd.NA,
            "e2e_probe_depth": int(last["total_probes"]),
            "e2e_probe_pass_rate": (last["probes_passed"] / last["total_probes"]
                                    if last["total_probes"] else float("nan")),
            "e2e_infra_failures": int((~g["backend_up"]).sum()),
        })
    return pd.DataFrame(rows, columns=cols)


#: Artifact each tree is expected to leave behind for a run.
EXPECTED_ARTIFACTS = {"build": "build-summary.json", "test": "test-summary.json",
                      "metrics": "metrics-summary.json", "e2e": "e2e-summary.json"}


def evidence(story: str) -> pd.DataFrame:
    """How much of the expected evidence a run actually produced.

    Scoring a missing artifact as zero reads as "the model did badly" when it
    can equally mean the stage never ran. Completeness is reported beside the
    scores so the two stay distinguishable.
    """
    cells, have = set(), {}
    for tree, fname in EXPECTED_ARTIFACTS.items():
        for model, iteration, ts, ts_dir in _execution_dirs(TREES[tree], story):
            cells.add((model, iteration))
            if (ts_dir / fname).exists():
                have.setdefault((model, iteration), set()).add(tree)
    rows = [{"story": story, "model": m, "iteration": i,
             "evidence_completeness": len(have.get((m, i), set())) / len(EXPECTED_ARTIFACTS),
             "missing_trees": ",".join(sorted(set(EXPECTED_ARTIFACTS) - have.get((m, i), set()))) or None}
            for m, i in sorted(cells)]
    return pd.DataFrame(rows, columns=_KEYS + ["evidence_completeness", "missing_trees"])


# ---------------------------------------------------------------------------
# Guard rails — run these in every notebook's sanity cell
# ---------------------------------------------------------------------------

#: metric -> (family, provenance, +1 if higher is better). ``self-reported``
#: values come from files the agent wrote about itself and are never scored.
METRIC_CATALOG = {
    "evidence_completeness": ("E evidence", "derived", +1),
    "build_failed_execs": ("C convergence", "tool-measured", -1),
    "build_errors_burned": ("C convergence", "tool-measured", -1),
    "build_peak_errors": ("C convergence", "tool-measured", -1),
    "build_execs": ("C convergence", "tool-measured", -1),
    "attempts_to_green": ("C convergence", "tool-measured", -1),
    "test_failures_burned": ("C convergence", "tool-measured", -1),
    "red_first": ("C convergence", "tool-measured", +1),
    "tests_per_intent": ("V verification", "tool-measured", +1),
    "layer_coverage": ("V verification", "tool-measured", +1),
    "layer_balance": ("V verification", "tool-measured", +1),
    "line_rate": ("V verification", "tool-measured", +1),
    "branch_rate": ("V verification", "tool-measured", +1),
    "worst_coupling_drop": ("Q quality", "tool-measured", +1),
    "green_share_gain": ("Q quality", "tool-measured", +1),
    "min_mi_gain": ("Q quality", "tool-measured", +1),
    "pct_green_types": ("Q quality", "tool-measured", +1),
    "median_mi": ("Q quality", "tool-measured", +1),
    "e2e_attempts_to_pass": ("B behavioural", "tool-measured", -1),
    "e2e_probe_pass_rate": ("B behavioural", "tool-measured", +1),
    "e2e_probe_depth": ("B behavioural", "self-reported", +1),
    "e2e_infra_failures": ("B behavioural", "tool-measured", -1),
    "tg_test_methods": ("V verification", "self-reported", +1),
    "ref_all_green": ("Q quality", "self-reported", +1),
}


def discrimination(summary: pd.DataFrame) -> pd.DataFrame:
    """Flag every metric that carries no signal across the current cohort.

    A metric that is constant or all-NA cannot separate two models, so it is a
    defect rather than a data point — either the join broke or the pipeline
    saturates it. This is the check whose absence let a broken join zero four
    scorecard inputs while every assertion still passed.
    """
    rows = []
    for name, (family, prov, _) in METRIC_CATALOG.items():
        if name not in summary.columns:
            rows.append({"metric": name, "family": family, "provenance": prov,
                         "status": "ABSENT", "distinct": 0, "na_share": 1.0})
            continue
        col = summary[name]
        na = float(col.isna().mean())
        distinct = int(col.nunique(dropna=True))
        status = ("ALL-NA" if na == 1.0 else "CONSTANT" if distinct <= 1
                  else "WEAK" if distinct <= 2 else "ok")
        rows.append({"metric": name, "family": family, "provenance": prov,
                     "status": status, "distinct": distinct, "na_share": round(na, 3)})
    return (pd.DataFrame(rows)
              .sort_values(["family", "metric"])
              .reset_index(drop=True))


def audit_identity(story: str) -> pd.DataFrame:
    """Files whose self-reported model/iteration disagrees with their path.

    Every disagreement should reconcile under :func:`canonical_model`; a row
    with ``reconciles = False`` is a genuinely misfiled result that the
    canonical key cannot rescue, and needs looking at by hand.
    """
    rows = []
    for tree_name, tree in TREES.items():
        for model, iteration, ts, ts_dir in _execution_dirs(tree, story):
            for path in sorted(ts_dir.glob("*.json")):
                rec = _read_json(path)
                if rec is None:
                    continue
                said = rec.get("model")
                if said is not None and str(said) != model:
                    rows.append({"tree": tree_name,
                                 "file": str(path.relative_to(REPO_ROOT)),
                                 "field": "model", "path_says": model,
                                 "file_says": said,
                                 "reconciles": canonical_model(said) == canonical_model(model)})
                said_i = rec.get("iteration")
                if said_i is not None and as_int(said_i) != iteration:
                    rows.append({"tree": tree_name,
                                 "file": str(path.relative_to(REPO_ROOT)),
                                 "field": "iteration", "path_says": iteration,
                                 "file_says": said_i, "reconciles": False})
    return pd.DataFrame(rows, columns=["tree", "file", "field", "path_says",
                                       "file_says", "reconciles"])


# ---------------------------------------------------------------------------
# Styling — palette validated with the dataviz skill's six checks
# ---------------------------------------------------------------------------

#: Fixed categorical slot order (identity = model). Assigned to sorted model
#: names, first N slots, never cycled. Validated on white: worst CVD dE 24.2.
CATEGORICAL_SLOTS = ["#2a78d6", "#1baf7a", "#eda100", "#008300",
                     "#4a3aa7", "#e34948", "#e87ba4", "#eb6834"]

#: Reserved status colors (never reused for models); always pair with labels.
STATUS_COLORS = {"GREEN": "#0ca30c", "YELLOW": "#fab219", "RED": "#d03b3b"}
OUTCOME_COLORS = {"passed": "#0ca30c", "failed": "#d03b3b", "skipped": "#898781"}
STAGE_STATUS_COLORS = {"success": "#0ca30c", "partial": "#fab219", "failure": "#d03b3b"}

INK = {"primary": "#0b0b0b", "secondary": "#52514e", "muted": "#898781",
       "grid": "#e1e0d9", "baseline": "#c3c2b7"}


def setup_theme() -> None:
    # seaborn's FacetGrid calls tight_layout, which harmlessly overrides the
    # constrained-layout rc default and emits a UserWarning each time.
    import warnings
    warnings.filterwarnings("ignore", message="The figure layout has changed to tight")
    sns.set_theme(style="whitegrid", context="notebook", rc={
        "figure.constrained_layout.use": True,
        "figure.facecolor": "white",
        "axes.facecolor": "white",
        "grid.color": INK["grid"],
        "grid.linewidth": 0.8,
        "axes.edgecolor": INK["baseline"],
        "text.color": INK["primary"],
        "axes.labelcolor": INK["secondary"],
        "xtick.color": INK["secondary"],
        "ytick.color": INK["secondary"],
        "legend.frameon": False,
        "figure.dpi": 110,
    })


def model_palette(models) -> dict[str, str]:
    """Fixed model -> color mapping; pass with hue_order=sorted(models) everywhere."""
    models = sorted(models)
    if len(models) > len(CATEGORICAL_SLOTS):
        raise ValueError(f"{len(models)} models but only {len(CATEGORICAL_SLOTS)} "
                         "categorical slots; fold extras into 'Other'")
    return dict(zip(models, CATEGORICAL_SLOTS))


#: (start, end, flag) bands per metric; None = unbounded (clipped to axis).
THRESHOLDS = {
    "mi": [(0, 10, "RED"), (10, 20, "YELLOW"), (20, None, "GREEN")],
    "cc": [(0, 10, "GREEN"), (10, 25, "YELLOW"), (25, None, "RED")],
    "coupling": [(0, 10, "GREEN"), (10, 40, "YELLOW"), (40, None, "RED")],
    "dit": [(0, 4.5, "GREEN"), (4.5, 5.5, "YELLOW"), (5.5, None, "RED")],
}


def add_threshold_bands(ax, metric: str, axis: str = "y", label: bool = True) -> None:
    """Faint GREEN/YELLOW/RED background bands at the documented thresholds."""
    lo, hi = ax.get_ylim() if axis == "y" else ax.get_xlim()
    span = ax.axhspan if axis == "y" else ax.axvspan
    for start, end, flag in THRESHOLDS[metric]:
        end = hi if end is None else end
        if end <= lo or start >= hi:
            continue
        span(max(start, lo), min(end, hi), color=STATUS_COLORS[flag],
             alpha=0.07, zorder=0)
        if label and axis == "y":
            trans = mtransforms.blended_transform_factory(ax.transAxes, ax.transData)
            ax.text(0.995, (max(start, lo) + min(end, hi)) / 2, flag,
                    transform=trans, ha="right", va="center",
                    fontsize=7, color=INK["muted"])
    if axis == "y":
        ax.set_ylim(lo, hi)
    else:
        ax.set_xlim(lo, hi)


def label_bars(ax, fmt: str = "%.2f", **kwargs) -> None:
    """Direct value labels on every bar container (contrast relief rule)."""
    for container in ax.containers:
        ax.bar_label(container, fmt=fmt, fontsize=8, color=INK["secondary"], **kwargs)
