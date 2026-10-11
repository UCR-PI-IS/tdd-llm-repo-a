"""Readable metric names of the SANER 2027 paper mapped to the columns of the run table (name-to-column table).

The paper names metrics by readable names only (decision P31 of its STATUS.md); its replication section says that
"a table in it maps each metric name of Table IV to its package column". This module holds that table. JP approved
it on 2026-10-06 under the same exception to rule R6 as decision P44 (paper only, behind a switch).

Off by default. When the environment variable ``TDD_PAPER_NAME_MAP`` is ``1``, ``paper_tables.make_tables``
(commands ``tables`` and ``all``) also writes ``tables/tab_metric_names.{csv,json,tex}``:

* one row per metric of Table IV (``sections/02-6-metrics.tex`` of the paper), in its order and family grouping,
  with the readable name exactly as printed, the ``runs.csv`` column, the family with its RQs, the unit and the
  better direction as printed, the provenance tag of ``schema.COLUMN_CATALOG``, the role of Table V
  (``sections/02-7-statistics.tex``: H1 to H8, secondary, equivalence band) or reliability flag or descriptive, and
  the macro stem of ``numbers.tex``;
* one descriptive row the paper also names (Layer balance, RQ1);
* one note row with the per-story macro grammar of P44.

Roles are computed from ``config`` (the families, bands and predicates the analysis uses), provenance from the
schema; the writer checks that every column exists in ``runs.csv`` and, when ``numbers.json`` is present, that
every stem carries macros. The internal report never sets the variable, so its tables are unchanged.
"""
from __future__ import annotations

import json
import os
from dataclasses import dataclass
from pathlib import Path

import pandas as pd

from . import config as C
from .schema import COLUMN_CATALOG
from .tables import latex_escape, macro_name, to_booktabs, write_table

ENV = "TDD_PAPER_NAME_MAP"
SLUG = "tab_metric_names"
DECISION = "name-to-column table, JP 2026-10-06 (R6 exception as P44); readable names P31"

_DIRECTION_WORD = {1: "higher", -1: "lower", 0: "none"}


@dataclass(frozen=True)
class PaperName:
    name: str      # readable name, exactly as printed in Table IV
    column: str    # runs.csv column
    family: str    # Table IV group header, with its RQs
    unit: str      # unit as printed in Table IV
    better: str    # better as printed in Table IV: higher | lower | none


_V, _C, _B = "Verification (RQ1, RQ2)", "Convergence (RQ2, RQ3)", "Behavioral (RQ3)"
_Q, _R, _P = "Quality (RQ4)", "Reliability (RQ5)", "Provenance (RQ6)"

# Table IV of the paper (tab:metrics), 24 rows in its order and grouping.
TABLE_IV: tuple[PaperName, ...] = (
    PaperName("New tests per intent", "new_tests_per_intent", _V, "methods per intent", "none"),
    PaperName("Solution line coverage", "line_rate", _V, "share", "higher"),
    PaperName("Story line coverage", "story_line_rate", _V, "share", "higher"),
    PaperName("New tests failing at first execution", "first_run_new_fail_share", _V, "share", "none"),
    PaperName("First-execution red", "red_first", _C, "flag", "none"),
    PaperName("Failed builds", "build_failed_execs", _C, "executions", "lower"),
    PaperName("Peak compiler errors", "build_peak_errors", _C, "errors", "lower"),
    PaperName("Total compiler errors", "build_errors_burned", _C, "errors", "lower"),
    PaperName("Test executions to green", "attempts_to_green", _C, "executions", "lower"),
    PaperName("Total test failures", "test_failures_burned", _C, "failures", "lower"),
    PaperName("End-to-end executions", "e2e_execs", _B, "executions", "lower"),
    PaperName("End-to-end attempts to pass", "e2e_attempts_to_pass_noinfra", _B, "executions", "lower"),
    PaperName("Refactoring gain in worst story MI", "story_min_mi_gain", _Q, "MI points", "higher"),
    PaperName("Maximum class coupling", "max_coupling", _Q, "classes", "lower"),
    PaperName("Median maintainability index", "median_mi", _Q, "MI points", "higher"),
    PaperName("Worst story maintainability index", "story_min_mi", _Q, "MI points", "higher"),
    PaperName("Worst story cyclomatic complexity", "story_max_cc", _Q, "CC", "lower"),
    PaperName("First build compiles", "build_first_pass", _R, "flag", "higher"),
    PaperName("Suite green at first execution", "green_first_try", _R, "flag", "higher"),
    PaperName("End-to-end passed first", "e2e_first_pass", _R, "flag", "higher"),
    PaperName("Final end-to-end passed", "final_e2e_ok", _R, "flag", "higher"),
    PaperName("Final suite green", "final_tests_green", _R, "flag", "higher"),
    PaperName("Clean run", "clean_run", _R, "flag", "higher"),
    PaperName("Self-report gap in test methods", "self_report_gap_methods_abs", _P, "methods", "lower"),
)

# Descriptive measures the paper names outside Table IV (defined where reported).
DESCRIPTIVE: tuple[PaperName, ...] = (
    PaperName("Layer balance", "new_layer_balance", "Verification (RQ1)", "index (1 = balanced)", "higher"),
)
_DESCRIPTIVE_ROLE = {"new_layer_balance": "descriptive (not in Table IV; defined in the text of RQ1)"}

# Per-story macros of P44 (story_macros.py), written as one note row and in the JSON.
STORY_GRAMMAR = "Story<Pos><Stem><Model><Median|K|N>"
STORY_NOTE = ("per-story values on S0 (P44, descriptive, six runs per story and model, no test): "
              "\\valStory<Pos><Stem><Model><Median|K|N>, Pos = chain position One to Ten (anonymous label US1 to US10), "
              "Model = Kimi or Qwen, Median and N (known values) for measures, K (runs with yes) and N for flags; "
              "also \\valStory<Pos>CompileRemove<Model><K|N> (runs with compile_remove_added > 0) and "
              "\\valStory<Pos>ExceptionalEnding<Model><K|N> (runs with a non-empty exceptional_ending)")
POOLED_NOTE = ("pooled macros: \\val<Stem><Model><Stat> (Median, Mean, QOne, QThree, Min, Max, Sd, Iqr, N for "
               "measures; K, Share, Pct, N for flags), \\val<Stem><Stat>[<Set>] for the model contrast (CliffDelta, "
               "CliffLo, CliffHi, Atwelve, Magnitude, PPerm, VanElterenZ, PHolm, Reject; no <Set> suffix on S0), "
               "\\val<Stem>Tost<Stat> for equivalence, \\val<Stem><Model><PassAt|PassHat><k>[Lo|Hi] for reliability "
               "flags (k spelled One to Six); example \\valBuildFailedExecsKimiMedian")

CSV_COLUMNS = ("readable_name", "package_column", "family", "unit", "better", "provenance", "role", "macro_stem", "row_kind")


def enabled() -> bool:
    """True when ``TDD_PAPER_NAME_MAP=1``. Read at call time."""
    return os.environ.get(ENV, "").strip() == "1"


def _band_text(band: float, unit: str) -> str:
    """Band as Table V prints it: shares without a unit, other bands with the unit of Table IV."""
    num = f"{int(band)}" if float(band).is_integer() else f"{band:.2f}"
    return num if unit == "share" else f"{num} {unit}"


def role(column: str, unit: str) -> str:
    """Role of a metric in the analysis (Table V and the reliability curves), from ``config``."""
    parts: list[str] = []
    if column in C.CONFIRMATORY_FAMILY:
        parts.append(f"H{C.CONFIRMATORY_FAMILY.index(column) + 1} (confirmatory)")
    if column in C.SECONDARY_FAMILY:
        parts.append("secondary")
    if column in C.TOST_BANDS:
        band = _band_text(C.TOST_BANDS[column], unit)
        parts.append(f"equivalence band {band}" if parts else f"equivalence only, band {band}")
    if column in C.SUCCESS_PREDICATES:
        parts.append("reliability flag (pass@k, pass^k)")
    return "; ".join(parts) or _DESCRIPTIVE_ROLE.get(column, "descriptive")


def name_map() -> pd.DataFrame:
    """The table as a frame: 24 Table IV rows, the descriptive rows, then the note row."""
    rows = []
    for kind, entries in (("table-iv", TABLE_IV), ("descriptive", DESCRIPTIVE)):
        for e in entries:
            spec = COLUMN_CATALOG[e.column]
            rows.append({"readable_name": e.name, "package_column": e.column, "family": e.family, "unit": e.unit,
                         "better": e.better, "provenance": spec.provenance.value, "role": role(e.column, e.unit),
                         "macro_stem": macro_name(e.column), "row_kind": kind})
    rows.append({"readable_name": "Per-story value of any metric above", "package_column": "story, model and the column",
                 "family": "any", "unit": "as the metric", "better": "as the metric", "provenance": "as the metric",
                 "role": "descriptive (P44)", "macro_stem": STORY_GRAMMAR, "row_kind": "note"})
    return pd.DataFrame(rows, columns=list(CSV_COLUMNS))


def _check(df: pd.DataFrame, runs: pd.DataFrame | None, out_dir: Path) -> dict:
    """Refuse a table whose names or columns are not unique, or whose columns are missing; check macro stems."""
    m = df[df["row_kind"] != "note"]
    for col in ("readable_name", "package_column", "macro_stem"):
        dup = m[col][m[col].duplicated()].tolist()
        if dup:
            raise ValueError(f"{SLUG}: duplicate {col}: {dup}")
    if runs is not None:
        missing = [c for c in m["package_column"] if c not in runs.columns]
        if missing:
            raise ValueError(f"{SLUG}: columns missing from runs.csv: {missing}")
    facts = {"columns_verified_against": "data/runs.csv" if runs is not None else None}
    jpath = Path(out_dir) / "numbers.json"
    if jpath.exists():
        nums = json.loads(jpath.read_text())["numbers"]
        counts = {}
        for col, stem in zip(m["package_column"], m["macro_stem"]):
            counts[stem] = sum(1 for k, v in nums.items() if v.get("metric") == col and k.startswith(stem))
        empty = [s for s, n in counts.items() if n == 0]
        if empty:
            raise ValueError(f"{SLUG}: no macro in numbers.json carries the stem of {empty}")
        facts.update({"stems_verified_against": "numbers.json", "pooled_macros_per_stem": counts})
    else:
        facts["stems_verified_against"] = None
    return facts


def tab_metric_names(bundle: dict, out_dir) -> Path:
    """Write tables/tab_metric_names.{csv,json,tex} (paper only; the paper does not input the .tex)."""
    out_dir = Path(out_dir)
    df = name_map()
    facts = _check(df, bundle.get("runs") if bundle else None, out_dir)
    out = out_dir / "tables"
    out.mkdir(parents=True, exist_ok=True)
    df.to_csv(out / f"{SLUG}.csv", index=False)
    tex = df.copy()
    tex["name_tt"] = tex["package_column"].map(lambda c: f"\\texttt{{{latex_escape(c)}}}")
    tex["stem_tt"] = tex["macro_stem"].map(lambda s: f"\\texttt{{{latex_escape(s)}}}")
    tex["role_tex"] = tex["role"].map(latex_escape)
    for col in ("readable_name", "family", "unit", "better", "provenance"):
        tex[col] = tex[col].map(latex_escape)
    tex.loc[tex["row_kind"] == "note", "name_tt"] = latex_escape("story, model and the column")
    cols = [("readable_name", "Readable name", "raw"), ("name_tt", "Package column", "raw"), ("family", "Family", "raw"),
            ("unit", "Unit", "raw"),
            ("better", "Better", "raw"), ("provenance", "Provenance", "raw"), ("role_tex", "Role", "raw"),
            ("stem_tt", "Macro stem", "raw")]
    body = "\\footnotesize\n\\setlength{\\tabcolsep}{3pt}\n" + to_booktabs(tex, cols, align="llllllll", group_col="family")
    body += f"% {POOLED_NOTE}\n% {STORY_NOTE}\n"
    meta = {"rows": len(df), "table_iv_rows": len(TABLE_IV), "descriptive_rows": len(DESCRIPTIVE), "note_rows": 1,
            "env": ENV, "decision": DECISION, "csv": f"{SLUG}.csv", "columns": list(CSV_COLUMNS),
            "sources": {"readable_name, family, unit, better": "Table IV, sections/02-6-metrics.tex of the SANER paper",
                        "provenance": "schema.COLUMN_CATALOG",
                        "role": "config.CONFIRMATORY_FAMILY, SECONDARY_FAMILY, TOST_BANDS, SUCCESS_PREDICATES (Table V)",
                        "macro_stem": "tables.macro_name(column)"},
            "macro_grammar": {"pooled": POOLED_NOTE, "per_story": STORY_NOTE},
            **facts,
            "mapping": df.to_dict(orient="records")}
    return write_table(out_dir, SLUG, body, meta=meta)
