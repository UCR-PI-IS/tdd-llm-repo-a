"""Pooled per-run dataset across the ten wave-1 stories.

``build_runs`` reuses the per-story loaders of ``tdd_results`` (one row per cell
from ``iteration_summary``), re-canonicalises model labels across stories, adds
the design facts of the story chain, the read-only git facts of each run branch
and the derived metrics defined in the analysis protocol, and returns the long
tables (executions, final-snapshot types) that figures need.

Nothing here scores anything: agent self-reports are carried with their
provenance tag and the statistics live in ``analysis.py``.
"""
from __future__ import annotations

import hashlib
import json
import re
import xml.etree.ElementTree as ET
from pathlib import Path

import numpy as np
import pandas as pd

from . import config as C
from . import gitfacts as gf
from .schema import COLUMN_CATALOG, coerce_dtypes, validate_runs

import tdd_results as tdd  # noqa: E402  (sys.path prepared by gitfacts)

KEY = ["story", "model_key", "iteration"]


# ---------------------------------------------------------------------------
# Story files
# ---------------------------------------------------------------------------

_EFFORT_INLINE = re.compile(r"Total estimate \(minutes\)\**\s*:?\**\s*:?\s*\**\s*(\d+)", re.IGNORECASE)
_EFFORT_ROW = re.compile(r"\|\s*\**Total Minutes\**\s*\|\s*\**\s*(\d+)", re.IGNORECASE)
_EFFORT_HEADER = re.compile(r"^\|.*(Total estimate \(minutes\)|Total Minutes)", re.IGNORECASE)
_TASK_MINUTES = re.compile(r"^\|[^|]+\|\s*(\d+)\s*\|\s*$")


def story_meta(story: str) -> dict:
    """Title, epic, team and the human effort estimate of a story file."""
    path = C.REPO / "UserStories" / f"{story}.md"
    text = path.read_text(encoding="utf-8", errors="replace") if path.exists() else ""
    lines = text.splitlines()
    epic = re.search(r"EPIC ID:\s*([A-Z0-9-]+)", text)
    feature = re.search(r"^\s*Feature:\s*(.+)$", text, flags=re.MULTILINE)
    title = feature.group(1).strip() if feature else None
    if not title:
        m = re.search(rf"^#+\s*(?:USER STORY|US)?\s*(?:ID)?:?\s*{re.escape(story)}\s*[-:]?\s*(.*)$", text, flags=re.MULTILINE)
        title = (m.group(1).strip() or None) if m else None
    effort, source = None, None
    m = _EFFORT_INLINE.search(text)
    if m:
        effort, source = int(m.group(1)), "inline"
    if effort is None:
        m = _EFFORT_ROW.search(text)
        if m:
            effort, source = int(m.group(1)), "total-row"
    if effort is None:
        for i, line in enumerate(lines):
            if _EFFORT_HEADER.match(line):
                for nxt in lines[i + 1:i + 4]:
                    if nxt.strip().startswith("|") and not set(nxt.strip()) <= set("|-: "):
                        num = re.search(r"(\d+)", nxt)
                        if num:
                            effort, source = int(num.group(1)), "header-next-row"
                            break
                if effort is not None:
                    break
    if effort is None:
        total = sum(int(m.group(1)) for m in (_TASK_MINUTES.match(l) for l in lines) if m)
        if total:
            effort, source = total, "task-sum"
    scenarios = len(re.findall(r"^\s*Scenario(?: Outline)?:", text, flags=re.MULTILINE))
    return {"story": story, "title": title, "epic": epic.group(1) if epic else None,
            "team": story.split("-")[0], "effort_minutes": effort, "effort_source": source,
            "n_scenarios": scenarios}


def story_intents(story: str) -> dict:
    intents = tdd.load_user_intents(story)
    per_layer = {f"intents_{l}": 0 for l in C.LAYERS}
    for _, r in intents.iterrows():
        if r["layer"] in C.LAYERS:
            per_layer[f"intents_{r['layer']}"] = int(r["n_intents"])
    return {"n_intents": int(intents["n_intents"].sum()) if not intents.empty else 0, **per_layer}


# ---------------------------------------------------------------------------
# Helpers over the per-story frames
# ---------------------------------------------------------------------------


def _with_key(df: pd.DataFrame) -> pd.DataFrame:
    out = df.copy()
    if "model" not in out.columns:
        return out
    if out.empty:
        out["model_key"] = pd.Series(dtype=object)
        return out
    out["model_key"] = out["model"].map(tdd.canonical_model)
    return out


def _cell_rows(df: pd.DataFrame, model_key: str, iteration: int) -> pd.DataFrame:
    if df.empty:
        return df
    sub = df[(df["model_key"] == model_key) & (df["iteration"] == iteration)]
    return sub.sort_values("ts") if "ts" in sub.columns else sub


def _minutes(a, b) -> float:
    return float((b - a) / pd.Timedelta(minutes=1))


def _time_metrics(ts_list: list[pd.Timestamp]) -> dict:
    ts = sorted(t for t in ts_list if pd.notna(t))
    if len(ts) < 2:
        return {"wall_clock_min": 0.0 if ts else np.nan, "active_min": 0.0 if ts else np.nan,
                "n_pauses": 0 if ts else pd.NA, "n_artifacts": len(ts)}
    gaps = [_minutes(a, b) for a, b in zip(ts, ts[1:])]
    return {"wall_clock_min": _minutes(ts[0], ts[-1]),
            "active_min": float(sum(g for g in gaps if g <= C.GAP_CAP_MIN)),
            "n_pauses": int(sum(g > C.GAP_CAP_MIN for g in gaps)), "n_artifacts": len(ts)}


def _simple_class(name: str) -> str:
    return str(name).split(".")[-1]


def _method_base(name: str) -> str:
    return str(name).split("(")[0]


def _find_cobertura(story: str, model_key: str, iteration: int, ts: pd.Timestamp) -> Path | None:
    if pd.isna(ts):
        return None
    stamp = ts.strftime("%Y-%m-%d_%H-%M-%S")
    story_dir = C.REPO / "TestResults" / story
    if not story_dir.is_dir():
        return None
    for model_dir in story_dir.iterdir():
        if model_dir.is_dir() and tdd.canonical_model(model_dir.name) == model_key:
            cand = model_dir / str(iteration) / stamp / "Coverage" / "Combined" / "Cobertura.xml"
            if cand.exists():
                return cand
    return None


def story_coverage(cobertura: Path | None, prod_files: list[str]) -> dict:
    """Line and branch coverage restricted to the production files a run touched."""
    out = {"story_line_rate": np.nan, "story_branch_rate": np.nan, "story_lines_valid": pd.NA,
           "story_classes_matched": pd.NA}
    if cobertura is None or not prod_files:
        return out
    suffixes = tuple(p.replace("\\", "/") for p in prod_files)
    lines_valid = lines_covered = br_valid = br_covered = matched = 0
    try:
        root = ET.parse(cobertura).getroot()
    except ET.ParseError:
        return out
    for cls in root.iter("class"):
        fname = (cls.get("filename") or "").replace("\\", "/")
        if not any(fname.endswith(s) for s in suffixes):
            continue
        matched += 1
        for line in cls.iter("line"):
            lines_valid += 1
            hits = int(line.get("hits") or 0)
            lines_covered += int(hits > 0)
            cc = line.get("condition-coverage")
            if cc:
                m = re.search(r"\((\d+)/(\d+)\)", cc)
                if m:
                    br_covered += int(m.group(1))
                    br_valid += int(m.group(2))
    if matched:
        out.update({"story_line_rate": lines_covered / lines_valid if lines_valid else np.nan,
                    "story_branch_rate": br_covered / br_valid if br_valid else np.nan,
                    "story_lines_valid": lines_valid, "story_classes_matched": matched})
    return out


def _story_types(prod_files: list[str]) -> list[str]:
    return sorted({Path(p).stem for p in prod_files if p.endswith(".cs")})


def _story_quality(mt: pd.DataFrame, model_key: str, iteration: int, types: list[str]) -> dict:
    """Story-scope quality: worst flags over the types the run created or modified,
    at the final snapshot, plus first-to-last movement."""
    out = {"story_n_types": pd.NA, "story_min_mi": np.nan, "story_max_cc": np.nan,
           "story_max_coupling": np.nan, "story_max_dit": np.nan, "story_pct_green": np.nan,
           "story_min_mi_gain": np.nan, "story_max_coupling_drop": np.nan}
    if mt.empty or not types:
        return out
    cell = _cell_rows(mt, model_key, iteration)
    cell = cell[cell["type"].isin(types)]
    if cell.empty:
        return out
    last_ts, first_ts = cell["ts"].max(), cell["ts"].min()
    last = cell[cell["ts"] == last_ts]
    first = cell[cell["ts"] == first_ts]
    all_green = ((last["mi_flag"] == "GREEN") & (last["cc_flag"] == "GREEN")
                 & (last["coupling_flag"] == "GREEN") & (last["dit_flag"] == "GREEN"))
    out.update({"story_n_types": int(len(last)), "story_min_mi": float(last["mi"].min()),
                "story_max_cc": float(last["cc"].max()), "story_max_coupling": float(last["coupling"].max()),
                "story_max_dit": float(last["dit"].max()), "story_pct_green": float(all_green.mean())})
    if last_ts != first_ts and not first.empty:
        out["story_min_mi_gain"] = float(last["mi"].min() - first["mi"].min())
        out["story_max_coupling_drop"] = float(first["coupling"].max() - last["coupling"].max())
    else:
        out["story_min_mi_gain"] = 0.0
        out["story_max_coupling_drop"] = 0.0
    return out


# ---------------------------------------------------------------------------
# Main assembly
# ---------------------------------------------------------------------------


def load_story(story: str) -> tuple[dict, pd.DataFrame]:
    tdd.reset_caches()
    data = tdd.load_all(story)
    summ = tdd.iteration_summary(story, data)
    return data, summ


def _is_success(status) -> bool:
    return isinstance(status, str) and status.strip().lower() == "success"


def _report_self_consistency(row) -> dict:
    """Contradictions inside a stage file (SANER P117): the agent's report against itself, no tool record needed.

    Each flag is NA where the check does not apply, True where the report contradicts itself, False otherwise:
    tg_layers_mismatch (test generation: the per-layer counts do not sum to testMethodsEmitted),
    cg_success_contradiction (code generation: status success with failed tests, build errors or a failed
    end-to-end status), ref_success_not_green (refactoring: status success with allGreenAchieved false),
    ref_green_with_violations (refactoring: allGreenAchieved true with remaining violations listed)."""
    out = {}
    tls, tgm = row.get("tg_layers_sum"), row.get("tg_test_methods")
    out["tg_layers_mismatch"] = bool(int(tls) != int(tgm)) if pd.notna(tls) and pd.notna(tgm) else pd.NA
    if _is_success(row.get("cg_status")):
        tf, be, es = row.get("cg_tests_failed"), row.get("cg_build_errors"), row.get("cg_e2e_status")
        out["cg_success_contradiction"] = bool((pd.notna(tf) and int(tf) > 0) or (pd.notna(be) and int(be) > 0)
                                               or (isinstance(es, str) and es.strip().lower() != "success"))
    else:
        out["cg_success_contradiction"] = pd.NA
    ag = row.get("ref_all_green")
    ag = bool(ag) if ag in (True, False) else None
    out["ref_success_not_green"] = (ag is False) if _is_success(row.get("ref_status")) and ag is not None else pd.NA
    rv = row.get("ref_n_remaining_violations")
    out["ref_green_with_violations"] = bool(pd.notna(rv) and int(rv) > 0) if ag is True else pd.NA
    return out


def build_runs(stories: list[str] | None = None, *, with_git: bool = True, refresh_cache: bool = False,
               progress=None) -> dict:
    """Build every table. Returns a dict with ``runs``, ``executions``, ``types``,
    ``cells``, ``stories``, ``excluded_refs`` DataFrames and ``warnings``."""
    stories = list(stories or C.STORY_ORDER)
    chain = {r["story"]: r for r in gf.story_chain_facts()} if with_git else {}
    refs = gf.list_run_refs() if with_git else []
    run_rows: list[dict] = []
    exec_rows: list[pd.DataFrame] = []
    type_rows: list[pd.DataFrame] = []
    warnings: list[str] = []
    story_rows: list[dict] = []
    present: set[tuple[str, str, int]] = set()

    for story in stories:
        data, summ = load_story(story)
        warnings.extend(f"{story}: {w}" for w in tdd.load_warnings)
        link = next((l for l in C.STORY_CHAIN if l.story == story), None)
        meta = {**story_meta(story), **story_intents(story)}
        prev = next((l for l in C.STORY_CHAIN if link and l.pos == link.pos - 1), None)
        baseline_author = prev.merged_model if prev else "human"
        story_rows.append({**meta, "story_pos": link.pos if link else pd.NA,
                           "baseline_author_model": baseline_author,
                           "merged_run": f"{link.merged_model}/{link.merged_iteration}" if link else None,
                           "merge_kind": link.merge_kind if link else None,
                           **{k: v for k, v in chain.get(story, {}).items()
                              if k in ("baseline_sha", "baseline_full_sha", "baseline_date", "baseline_test_attrs",
                                       "baseline_test_files", "baseline_prod_files", "baseline_prod_lines")}})

        summ = _with_key(summ)
        summ = summ[summ["model_key"].isin(C.MODEL_LABELS)].copy()
        frames = {k: _with_key(v) for k, v in data.items()}
        trx_all = _with_key(tdd.load_trx_tests(story, finals_only=False))
        mt_all = frames["metrics_types"]

        # long tables -----------------------------------------------------
        for tree, cols in (("build", ["status", "total_errors", "total_warnings"]),
                           ("test", ["status", "total", "passed", "failed"]),
                           ("e2e", ["status", "total_probes", "probes_passed", "backend_up"])):
            df = frames[tree]
            if df.empty:
                continue
            sub = df[KEY + ["ts", "attempt"] + [c for c in cols if c in df.columns]].copy()
            sub["tree"] = tree
            exec_rows.append(sub)
        if not mt_all.empty:
            snaps = mt_all[KEY + ["ts"]].drop_duplicates().copy()
            snaps["tree"] = "metrics"
            snaps["status"] = "snapshot"
            snaps = snaps.sort_values("ts")
            snaps["attempt"] = snaps.groupby(KEY).cumcount() + 1
            exec_rows.append(snaps)

        for _, row in summ.iterrows():
            mk, it = row["model_key"], int(row["iteration"])
            model = C.MODEL_LABELS[mk]
            present.add((story, mk, it))
            run_id = f"{story}/{model}/{it}"
            rec: dict = {c: row[c] for c in summ.columns if c not in ("model_key",)}
            rec.update({"run_id": run_id, "model": model, "model_key": mk, "iteration": it,
                        "story_pos": link.pos if link else pd.NA, **{k: meta[k] for k in
                        ("n_intents", "effort_minutes", "effort_source", *[f"intents_{l}" for l in C.LAYERS])},
                        "baseline_author_model": baseline_author, "own_baseline": baseline_author == model,
                        "merged_into_trunk": bool(link and link.merged_model == model and link.merged_iteration == it),
                        "baseline_test_attrs": chain.get(story, {}).get("baseline_test_attrs", pd.NA),
                        "baseline_prod_lines": chain.get(story, {}).get("baseline_prod_lines", pd.NA)})

            build = _cell_rows(frames["build"], mk, it)
            test = _cell_rows(frames["test"], mk, it)
            e2e = _cell_rows(frames["e2e"], mk, it)
            snaps_ts = sorted(mt_all[(mt_all["model_key"] == mk) & (mt_all["iteration"] == it)]["ts"].unique()) if not mt_all.empty else []

            # time ----------------------------------------------------------
            all_ts = list(build["ts"]) + list(test["ts"]) + list(e2e["ts"]) + [pd.Timestamp(t) for t in snaps_ts]
            rec.update(_time_metrics(all_ts))
            green = test[(test["failed"] == 0) & (test["total"] > 0)]
            rec["wall_to_green_min"] = (_minutes(build["ts"].min(), green["ts"].min())
                                        if not build.empty and not green.empty else np.nan)
            # first execution facts ------------------------------------------
            first_test = test.iloc[0] if not test.empty else None
            rec["compile_red"] = bool(first_test is not None and int(first_test["total"] or 0) == 0)
            rec["green_first_try"] = (pd.NA if pd.isna(row.get("attempts_to_green")) else int(row["attempts_to_green"]) == 1)
            # e2e ---------------------------------------------------------------
            if e2e.empty:
                rec.update({"e2e_first_pass": pd.NA, "e2e_attempts_to_pass_noinfra": pd.NA, "final_e2e_ok": pd.NA,
                            "e2e_last_backend_up": pd.NA})
            else:
                noinfra = e2e[e2e["backend_up"].fillna(False).astype(bool)]
                rec["e2e_first_pass"] = bool(noinfra.iloc[0]["status"] == "success") if not noinfra.empty else False
                succ = noinfra.reset_index(drop=True).index[noinfra["status"].eq("success").to_numpy()]
                rec["e2e_attempts_to_pass_noinfra"] = int(succ[0]) + 1 if len(succ) else pd.NA
                rec["final_e2e_ok"] = bool(e2e.iloc[-1]["status"] == "success")
                rec["e2e_last_backend_up"] = bool(e2e.iloc[-1]["backend_up"])
            # final state -----------------------------------------------------------
            rec["final_build_ok"] = (row["build_status"] == "success") if pd.notna(row.get("build_status")) else pd.NA
            rec["final_tests_green"] = (bool(row["failed"] == 0 and row["total_tests"] > 0)
                                        if pd.notna(row.get("failed")) and pd.notna(row.get("total_tests")) else pd.NA)
            finals = [rec["final_build_ok"], rec["final_tests_green"], rec["final_e2e_ok"]]
            rec["final_all_ok"] = pd.NA if any(pd.isna(f) for f in finals) else all(bool(f) for f in finals)
            ending = []  # any failed final check (build, tests, e2e) is an exceptional ending (STATUS.md Q21, D27)
            if rec["final_build_ok"] is False:
                ending.append("final build failed")
            if rec["final_tests_green"] is False:
                ending.append("final test execution red")
            if rec["final_e2e_ok"] is False:
                ending.append("final e2e failed (infrastructure)" if rec["e2e_last_backend_up"] is False
                              else "final e2e failed (probe)")
            rec["exceptional_ending"] = "; ".join(ending)
            # self-reports ------------------------------------------------------------
            st = frames["stages"]
            tg = st[(st["model_key"] == mk) & (st["iteration"] == it) & (st["stage"] == "test-generation")]
            rec["tg_intents_confirmed"] = tg["tg_intents_confirmed"].iloc[-1] if not tg.empty else pd.NA
            rec["refactor_cycles_measured"] = (int(row["metrics_snapshots"]) - 1
                                               if pd.notna(row.get("metrics_snapshots")) and row["metrics_snapshots"] > 0 else pd.NA)
            # git facts -----------------------------------------------------------------
            if with_git:
                f = gf.cell_facts(story, mk, it, refs=refs, refresh=refresh_cache)
                keep = ("ref", "tip_sha", "commit_a_sha", "commit_b_sha", "n_run_commits", "baseline_sha",
                        "identity_declared_model", "identity_agents_agree", "identity_ambiguous", "harness_pre_e2e",
                        "new_test_methods", "new_test_cases", "new_test_files", "modified_test_files",
                        "assertion_free_new_tests", "compile_remove_added",
                        "prod_loc_added", "prod_loc_deleted", "prod_files_added", "prod_files_modified",
                        "test_loc_added", "test_loc_deleted",
                        *[f"new_test_methods_{l}" for l in C.LAYERS], *[f"new_test_cases_{l}" for l in C.LAYERS])
                rec.update({k: f.get(k) for k in keep})
                rec["compile_remove_files"] = ";".join(f.get("compile_remove_files") or [])
                rec["new_test_classes"] = ";".join(f.get("new_test_classes") or [])
                if f.get("ref_partial") or f.get("ref_ambiguous"):
                    warnings.append(f"{run_id}: ref resolution partial={f.get('ref_partial')} ambiguous={f.get('ref_ambiguous')}")
                if f.get("n_run_commits") not in (None, 2):
                    warnings.append(f"{run_id}: {f.get('n_run_commits')} run commits on {f.get('ref')}")
                removed = {Path(x).name for x in (f.get("compile_remove_files") or [])}
                per_file = f.get("per_file_method_delta") or {}
                rec["new_test_methods_compiled"] = (f.get("new_test_methods") or 0) - sum(
                    v for p, v in per_file.items() if Path(p).name in removed and v > 0)
                prod_files = f.get("prod_files") or []
                types = _story_types(prod_files)
                rec["story_new_types"] = ";".join(types)
                rec.update(_story_quality(mt_all, mk, it, types))
                cov_final = _cell_rows(frames["coverage"], mk, it)
                cob = _find_cobertura(story, mk, it, cov_final["ts"].iloc[-1]) if not cov_final.empty else None
                rec.update(story_coverage(cob, prod_files))
                # first-execution outcome of the run's own tests
                if first_test is not None and not trx_all.empty:
                    fx = trx_all[(trx_all["model_key"] == mk) & (trx_all["iteration"] == it) & (trx_all["ts"] == first_test["ts"])]
                    classes = set(f.get("new_test_classes") or [])
                    names = set(f.get("new_test_method_names") or [])
                    own = fx[fx["class_name"].map(_simple_class).isin(classes) & fx["test_name"].map(_method_base).isin(names)]
                    if rec["compile_red"]:
                        rec["first_run_new_fail_share"] = 1.0
                    elif not own.empty:
                        rec["first_run_new_fail_share"] = float((own["outcome"] != "Passed").mean())
                    else:
                        rec["first_run_new_fail_share"] = np.nan
                else:
                    rec["first_run_new_fail_share"] = np.nan
                # derived fidelity and provenance
                n_int = meta["n_intents"] or np.nan
                rec["new_tests_per_intent"] = (f.get("new_test_methods") or 0) / n_int if n_int else np.nan
                rec["new_cases_per_intent"] = (f.get("new_test_cases") or 0) / n_int if n_int else np.nan
                ratios = [(f.get(f"new_test_methods_{l}") or 0) / meta[f"intents_{l}"]
                          for l in C.LAYERS if meta[f"intents_{l}"]]
                rec["new_layer_balance"] = max(0.0, 1.0 - float(np.mean([abs(r - 1) for r in ratios]))) if ratios else np.nan
                tgm = row.get("tg_test_methods")
                if pd.notna(tgm) and f.get("new_test_methods") is not None:
                    rec["self_report_gap_methods"] = int(tgm) - int(f["new_test_methods"])
                    rec["self_report_gap_methods_abs"] = abs(rec["self_report_gap_methods"])
                    rec["self_report_gap_cases"] = int(tgm) - int(f["new_test_cases"])
                    rec["self_report_ratio"] = int(tgm) / f["new_test_methods"] if f["new_test_methods"] else np.nan
            tic = rec["tg_intents_confirmed"]
            rec["intents_report_error"] = int(tic) - meta["n_intents"] if pd.notna(tic) else pd.NA
            rec["intents_confirmed_ok"] = (int(tic) == meta["n_intents"]) if pd.notna(tic) else pd.NA
            rl = row.get("ref_loop_iterations")
            rec["refactor_loop_gap"] = (int(rl) - rec["refactor_cycles_measured"]
                                        if pd.notna(rl) and not pd.isna(rec["refactor_cycles_measured"]) else pd.NA)
            rec.update(_report_self_consistency(row))
            rec["clean_run"] = (bool(rec.get("build_first_pass")) and bool(rec["green_first_try"]) and bool(rec["e2e_first_pass"])
                                if not any(pd.isna(x) for x in (rec.get("build_first_pass"), rec["green_first_try"], rec["e2e_first_pass"])) else pd.NA)
            rec["included_core"] = (not bool(rec.get("identity_ambiguous"))) and int(rec.get("compile_remove_added") or 0) == 0
            run_rows.append(rec)

        # final snapshot types (for the quality panel) ----------------------------
        if not mt_all.empty:
            fin = tdd.finals(mt_all, keys=["story", "model_key", "iteration"]).copy()
            fin["model"] = fin["model_key"].map(C.MODEL_LABELS)
            type_rows.append(fin.drop(columns=[c for c in ("attempt",) if c in fin.columns]))
        if progress:
            progress(story)

    runs = pd.DataFrame(run_rows)
    runs = runs.drop(columns=[c for c in ("test_methods", "e2e_last_backend_up", "n_artifacts",
                                          "story_lines_valid", "story_classes_matched") if c in runs.columns])
    runs = coerce_dtypes(runs)
    runs = runs.sort_values(["story_pos", "model", "iteration"]).reset_index(drop=True)
    warnings.extend(validate_runs(runs, strict=False))

    executions = pd.concat(exec_rows, ignore_index=True) if exec_rows else pd.DataFrame()
    if not executions.empty:
        executions["model"] = executions["model_key"].map(C.MODEL_LABELS)
        executions = executions[executions["model"].notna()]
        executions["run_id"] = executions["story"] + "/" + executions["model"] + "/" + executions["iteration"].astype(int).astype(str)
        executions = executions.sort_values(["story", "model", "iteration", "ts"]).reset_index(drop=True)
    types = pd.concat(type_rows, ignore_index=True) if type_rows else pd.DataFrame()
    if not types.empty:
        types = types[types["model"].notna()].copy()
        types["run_id"] = types["story"] + "/" + types["model"] + "/" + types["iteration"].astype(int).astype(str)
        new_types = runs.set_index("run_id")["story_new_types"].fillna("").str.split(";").to_dict() if "story_new_types" in runs.columns else {}
        types["story_new"] = [t in set(new_types.get(r, [])) for r, t in zip(types["run_id"], types["type"])]

    stories_df = pd.DataFrame(story_rows).sort_values("story_pos").reset_index(drop=True)
    cells = _cells(runs)
    used_refs = set(runs["ref"].dropna()) if "ref" in runs.columns else set()
    excluded = (pd.DataFrame(gf.excluded_refs(present, set(stories), used_refs))
                if with_git else pd.DataFrame())
    return {"runs": runs, "executions": executions, "types": types, "cells": cells,
            "stories": stories_df, "excluded_refs": excluded, "warnings": warnings}


def _cells(runs: pd.DataFrame) -> pd.DataFrame:
    if runs.empty:
        return pd.DataFrame()
    rows = []
    for (story, model), g in runs.groupby(["story", "model"], sort=False):
        row = {"story": story, "story_pos": int(g["story_pos"].iloc[0]), "model": model, "n_runs": int(len(g))}
        for pred in C.SUCCESS_PREDICATES:
            if pred in g.columns:
                col = g[pred]
                row[f"n_{pred}"] = int(col.fillna(False).astype(bool).sum())
                row[f"n_known_{pred}"] = int(col.notna().sum())
        rows.append(row)
    return pd.DataFrame(rows).sort_values(["story_pos", "model"]).reset_index(drop=True)


# ---------------------------------------------------------------------------
# Persistence
# ---------------------------------------------------------------------------


def _sha256(path: Path) -> str:
    return hashlib.sha256(path.read_bytes()).hexdigest()


def write_dataset(bundle: dict, out_dir: Path) -> dict:
    data_dir = Path(out_dir) / "data"
    data_dir.mkdir(parents=True, exist_ok=True)
    written = {}
    for name in ("runs", "executions", "types", "cells", "stories", "excluded_refs"):
        df = bundle.get(name)
        if df is None:
            continue
        path = data_dir / f"{name}.csv"
        df.to_csv(path, index=False)
        written[name] = {"path": str(path), "rows": int(len(df)), "sha256": _sha256(path)}
        if name == "runs":
            try:
                import pyarrow  # noqa: F401
                df.to_parquet(data_dir / "runs.parquet", index=False)
            except Exception:  # pragma: no cover - optional dependency
                pass
    (data_dir / "warnings.txt").write_text("\n".join(bundle.get("warnings", [])) + "\n")
    (data_dir / "dataset-meta.json").write_text(json.dumps(written, indent=1))
    return written


def load_runs(out_dir: Path) -> pd.DataFrame:
    runs = pd.read_csv(Path(out_dir) / "data" / "runs.csv")
    return coerce_dtypes(runs)


def load_table(out_dir: Path, name: str) -> pd.DataFrame:
    path = Path(out_dir) / "data" / f"{name}.csv"
    df = pd.read_csv(path)
    if "ts" in df.columns:
        df["ts"] = pd.to_datetime(df["ts"])
    return df
