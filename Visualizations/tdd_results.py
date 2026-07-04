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
INTENTS_DIR = REPO_ROOT / "UserIntents"

TREES = {"build": BUILD_TREE, "test": TEST_TREE, "metrics": METRICS_TREE}

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
    models: set[str] = set()
    for tree in TREES.values():
        story_dir = tree / story
        if story_dir.is_dir():
            models.update(p.name for p in story_dir.iterdir() if p.is_dir())
    return sorted(models)


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
                yield model_dir.name, int(iter_dir.name), ts, ts_dir


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
        if not _iteration_included(int(rec.get("iteration", 0))):
            continue
        m = rec.get("metrics") or {}
        row = {
            "story": story,
            "model": rec.get("model", path.parts[-4] if len(path.parts) >= 4 else None),
            "iteration": int(rec.get("iteration", 0)),
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
        if not _iteration_included(int(rec.get("iteration", 0))):
            continue
        key = (rec.get("model"), int(rec.get("iteration", 0)))
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
        "test_projects": load_test_projects(story),
        "trx": load_trx_tests(story),
        "coverage": load_coverage(story),
        "metrics_types": load_metrics_types(story),
        "metrics_methods": load_metrics_methods(story),
        "stages": load_stage_results(story),
        "violations": load_remaining_violations(story),
        "intents": load_user_intents(story),
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

    for tree, source in (("build", "build"), ("test", "test"), ("metrics", "metrics_types")):
        counts = attempt_counts(d[source], tree)
        merge(counts, {"n_attempts": f"{tree}_attempts"})

    return out


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
