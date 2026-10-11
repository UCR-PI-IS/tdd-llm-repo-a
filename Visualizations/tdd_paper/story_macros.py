"""Descriptive per-story, per-model macros for the SANER 2027 paper only (decision P44 of its STATUS.md).

Off by default. When the environment variable ``TDD_PAPER_STORY_MACROS`` is ``1``, ``analysis.run_all`` (commands
``stats`` and ``all``) and the ``numbers`` command add, on S0, for every story (chain position One to Ten) and model:

* ``\\valStory<Pos><Metric><Model>Median`` and ``...N`` (known values) for ``MEDIAN_METRICS``;
* ``\\valStory<Pos><Flag><Model>K`` (count of True) and ``...N`` (known values) for ``FLAG_METRICS``;
* ``\\valStory<Pos>CompileRemove<Model>K|N`` and ``\\valStory<Pos>ExceptionalEnding<Model>K|N`` (runs of the group).

``<Metric>`` is the stem of the pooled macros (``macro_name(metric)``), the median format is the pooled one
(``analysis._fmt_for``) and the provenance tag is the column's. Each entry carries ``kind`` = ``KIND``. The values
describe six runs each: no test, no interval. The internal report never sets the variable, so its numbers are unchanged.
"""
from __future__ import annotations

import os

import pandas as pd

from . import config as C
from .analysis import _fmt_for, _short, _values
from .schema import COLUMN_CATALOG
from .tables import Numbers, macro_name

ENV = "TDD_PAPER_STORY_MACROS"
KIND = "descriptive-per-story"
CODE_REF = "tdd_paper.story_macros"
NOTES = "descriptive per-story value on S0 (one story, one model, six runs; no test, no interval); SANER paper only (P44)"

MEDIAN_METRICS: tuple[str, ...] = (
    "new_tests_per_intent", "new_layer_balance",
    "line_rate", "story_line_rate",
    "build_failed_execs", "build_peak_errors", "build_errors_burned",
    "attempts_to_green", "test_failures_burned",
    "e2e_execs", "e2e_attempts_to_pass_noinfra",
    "first_run_new_fail_share",
    "max_coupling", "median_mi", "story_min_mi", "story_max_cc", "story_min_mi_gain",
    "self_report_gap_methods_abs",
)
FLAG_METRICS: tuple[str, ...] = (
    "red_first", "build_first_pass", "green_first_try", "e2e_first_pass", "final_e2e_ok", "final_tests_green", "clean_run",
)


def enabled() -> bool:
    """True when ``TDD_PAPER_STORY_MACROS=1``. Read at call time."""
    return os.environ.get(ENV, "").strip() == "1"


def _story_counts(g: pd.DataFrame) -> dict[str, tuple[int, str, str, str]]:
    """{stem: (k, metric, family, provenance)} for the evidence counts of one story x model group."""
    cr = int((pd.to_numeric(g["compile_remove_added"], errors="coerce").fillna(0) > 0).sum())
    ex = int((g["exceptional_ending"].fillna("") != "").sum())
    return {"CompileRemove": (cr, "compile_remove_added", "E evidence", "derived-from-git"),
            "ExceptionalEnding": (ex, "exceptional_ending", "E evidence", "tool-measured")}


def register_story_macros(nums: Numbers, runs: pd.DataFrame) -> int:
    """Add the per-story macros to ``nums``; returns how many were added.

    Earlier entries of this kind are dropped first, so a re-emission recomputes them from ``runs``. A name that is
    already taken by any other macro raises ``ValueError`` (no existing macro is ever overwritten)."""
    for name in [k for k, e in nums.entries.items() if e.get("kind") == KIND]:
        del nums.entries[name]
    taken = set(nums.entries)
    added: list[str] = []

    def add(name: str, value, *, fmt: str, unit: str, family: str, metric: str, stat: str, group: dict,
            provenance: str, n: int) -> None:
        if name in taken:
            raise ValueError(f"per-story macro {name} collides with an existing macro")
        nums.add(name, value, fmt=fmt, unit=unit, family=family, metric=metric, stat=stat, group=group,
                 provenance=provenance, source_columns=[metric], n=n, set_id="S0", code_ref=CODE_REF, notes=NOTES)
        nums.entries[name]["kind"] = KIND
        added.append(name)

    for (story, model), g in runs.groupby(["story", "model"], sort=True):
        pos = C.STORY_POS[story]
        group = {"story": story, "story_pos": pos, "model": model}
        tag = _short(model)
        for m in MEDIAN_METRICS:
            spec = COLUMN_CATALOG[m]
            v = _values(g, m)[m]
            base = macro_name("Story", str(pos), m, tag)
            common = dict(unit=spec.unit, family=spec.family, metric=m, group=group, provenance=spec.provenance.value, n=len(v))
            add(base + "Median", float(v.median()) if len(v) else None, fmt=_fmt_for(m), stat="median", **common)
            add(base + "N", len(v), fmt="int", stat="n", **{**common, "unit": "runs"})
        for m in FLAG_METRICS:
            spec = COLUMN_CATALOG[m]
            v = _values(g, m)[m]
            base = macro_name("Story", str(pos), m, tag)
            common = dict(unit="runs", family=spec.family, metric=m, group=group, provenance=spec.provenance.value, n=len(v))
            add(base + "K", int(v.sum()), fmt="int", stat="count", **common)
            add(base + "N", len(v), fmt="int", stat="n", **common)
        for stem, (k, metric, family, prov) in _story_counts(g).items():
            base = macro_name("Story", str(pos), stem, tag)
            common = dict(unit="runs", family=family, metric=metric, group=group, provenance=prov, n=len(g))
            add(base + "K", k, fmt="int", stat="count", **common)
            add(base + "N", len(g), fmt="int", stat="n", **common)
    nums.meta["story_macros"] = {"env": ENV, "decision": "P44", "kind": KIND, "n_macros": len(added), "set": "S0",
                                 "median_metrics": list(MEDIAN_METRICS), "flag_metrics": list(FLAG_METRICS),
                                 "counts": ["compile_remove_added > 0", "exceptional_ending != ''"]}
    return len(added)


def register_if_enabled(nums: Numbers, runs: pd.DataFrame | None = None, *, out_dir=None) -> int:
    """No-op (returns 0) unless ``TDD_PAPER_STORY_MACROS=1``; then loads ``runs`` from ``out_dir`` when not given."""
    if not enabled():
        return 0
    if runs is None:
        from . import dataset as ds
        runs = ds.load_runs(out_dir)
    return register_story_macros(nums, runs)
