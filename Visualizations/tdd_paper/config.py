"""Constants for the wave-1 paper build. No I/O happens here.

Everything that the report treats as a *design fact* (story order, baselines,
model labels, the confirmatory family, equivalence bands, sensitivity sets)
lives in this module so that ``numbers.json`` can cite one place for it.
"""
from __future__ import annotations

from dataclasses import dataclass
from pathlib import Path

VIZ = Path(__file__).resolve().parents[1]
REPO = VIZ.parent

TRUNK = "experiments/wave-1"              # harness to cite
EVIDENCE_BRANCH = "experiments/wave-1-metrics"
E2E_HARNESS_COMMIT = "e429a74d7"          # e2e validation added to the harness, 2026-08-03
TRUNK_HEAD_AT_PLANNING = "954048076"

# Canonical model keys (tdd_results.canonical_model) -> display labels.
MODEL_LABELS: dict[str, str] = {"kimik25": "Kimi-K2.5", "qwen37max": "Qwen3.7-max"}
MODEL_ORDER: list[str] = ["Kimi-K2.5", "Qwen3.7-max"]
MODEL_SHORT: dict[str, str] = {"Kimi-K2.5": "Kimi", "Qwen3.7-max": "Qwen"}
# Model ids that appear in run branches but are outside the study.
EXCLUDED_MODEL_KEYS: dict[str, str] = {
    "deepseekv4pro": "DeepSeek-V4-Pro: one story only (CPD-LC-001-001), pre-e2e harness, no E2E evidence",
    "grok43": "grok-4.3: one story only, pre-e2e harness, no E2E evidence",
    "grok45": "grok-4.5: one story only, pre-e2e harness, no E2E evidence",
}
INCLUDED_ITERATIONS = tuple(range(1, 7))
LAYERS = ("Domain", "Application", "Infrastructure", "Presentation")


@dataclass(frozen=True)
class StoryLink:
    story: str
    pos: int
    baseline_sha: str          # trunk commit the story's 12 runs were cut from
    merged_model: str          # display label of the run merged into the trunk
    merged_iteration: int
    merge_kind: str            # "squash-pr" | "squash" | "fast-forward"
    merge_ref: str             # PR number or commit


# Story chain in execution order. The merge of story N is the baseline of story N+1.
STORY_CHAIN: tuple[StoryLink, ...] = (
    StoryLink("CPD-LC-001-001", 1, "e0ae3a63a", "Qwen3.7-max", 3, "squash-pr", "PR #29 514992d8c"),
    StoryLink("SQL-LS-001-007", 2, "4aa60d917", "Qwen3.7-max", 5, "squash-pr", "PR #30 7bcac33b0"),
    StoryLink("CPD-LC-001-003", 3, "07c571e4e", "Qwen3.7-max", 4, "squash", "cb220b773"),
    StoryLink("CPD-LC-001-009", 4, "936d58ec5", "Kimi-K2.5", 4, "fast-forward", "9a2e1ab4a"),
    StoryLink("CPD-LC-001-005", 5, "b9431b228", "Kimi-K2.5", 6, "fast-forward", ""),
    StoryLink("SPT-UM-001-003", 6, "f8c9bb485", "Kimi-K2.5", 6, "fast-forward", ""),
    StoryLink("PQL-AE-001-003", 7, "d9c6130e4", "Qwen3.7-max", 1, "fast-forward", ""),
    StoryLink("PQL-LB-002-001", 8, "2f8ca9c54", "Kimi-K2.5", 2, "fast-forward", ""),
    StoryLink("PQL-AE-001-001", 9, "1ac4c9ed0", "Qwen3.7-max", 4, "fast-forward", ""),
    StoryLink("PQL-AE-001-002", 10, "2374db15d", "Kimi-K2.5", 3, "fast-forward", ""),
)
STORY_ORDER: list[str] = [s.story for s in STORY_CHAIN]
STORY_POS: dict[str, int] = {s.story: s.pos for s in STORY_CHAIN}
# Stories whose merge was selected by the composite score (stories 1 to 3); from story 4
# the merged run was picked by hand so that each model ends with five merged runs.
COMPOSITE_SELECTED_STORIES = ("CPD-LC-001-001", "SQL-LS-001-007", "CPD-LC-001-003")
ARTIFACT_CORE_STORIES = ("CPD-LC-001-001", "SQL-LS-001-007", "CPD-LC-001-003", "CPD-LC-001-009")

# Expected integrity facts (asserted by `tdd_paper check`).
EXPECTED_CELLS = 120
EXPECTED_E2E_CELLS = 119
E2E_MISSING_CELLS = ("SPT-UM-001-003/Qwen3.7-max/4",)
IDENTITY_AMBIGUOUS_CELLS = (
    "CPD-LC-001-009/Qwen3.7-max/1", "SPT-UM-001-003/Qwen3.7-max/4",
    "CPD-LC-001-003/Kimi-K2.5/5", "CPD-LC-001-009/Kimi-K2.5/2", "PQL-AE-001-002/Kimi-K2.5/3",
)
COMPILE_REMOVE_CELLS = (
    "CPD-LC-001-009/Qwen3.7-max/1", "CPD-LC-001-009/Kimi-K2.5/4",
    "SPT-UM-001-003/Kimi-K2.5/1", "SPT-UM-001-003/Kimi-K2.5/3", "SPT-UM-001-003/Kimi-K2.5/4",
    "SPT-UM-001-003/Kimi-K2.5/5", "SPT-UM-001-003/Qwen3.7-max/1", "SPT-UM-001-003/Qwen3.7-max/3",
    "SPT-UM-001-003/Qwen3.7-max/4", "SPT-UM-001-003/Qwen3.7-max/5",
)
EXCEPTIONAL_ENDING_CELLS = {
    "CPD-LC-001-003/Qwen3.7-max/4": "last recorded test execution red (1 of 88), later metrics snapshot",
    "SQL-LS-001-007/Kimi-K2.5/6": "final e2e execution ended on a Docker timeout (infrastructure)",
    "CPD-LC-001-005/Qwen3.7-max/1": "final e2e execution failed its probe (0 of 1)",
}
CALIBRATION_CELL = ("CPD-LC-001-003/Kimi-K2.5/1", 25, 35)  # (cell, new test methods, new test cases)
INTENTS_CONFIRMED_WRONG_EXPECTED = 25   # agent intentsConfirmed differs from UserIntents/<story>.json
INTENTS_CONFIRMED_MISSING_EXPECTED = 1  # cells without a readable test-generation stage report

# Statistics.
SEED = 20261003
N_BOOT = 5000
N_PERM = 20000
ALPHA = 0.05
TOST_LEVEL = 0.90
GAP_CAP_MIN = 60.0   # inter-artifact gaps above this are treated as pauses

# Primary confirmatory family (Holm): the artifact's eight pre-specified contrasts.
CONFIRMATORY_FAMILY: tuple[str, ...] = (
    "new_tests_per_intent", "red_first", "build_failed_execs", "build_peak_errors",
    "build_errors_burned", "attempts_to_green", "test_failures_burned", "e2e_execs",
)
# Secondary family (own Holm correction), labelled as such everywhere.
SECONDARY_FAMILY: tuple[str, ...] = (
    "first_run_new_fail_share", "story_min_mi_gain", "self_report_gap_methods_abs", "e2e_attempts_to_pass",
)
BINARY_METRICS: tuple[str, ...] = ("red_first", "build_first_pass", "compile_red", "e2e_first_pass",
                                   "final_tests_green", "final_e2e_ok", "final_all_ok")
# Equivalence bands (TOST on the mean over stories of the per-story median difference), in raw metric units.
# Fixed from measurement granularity during protocol design (2026-10-03) and ratified unchanged by JP on
# 2026-10-04 (STATUS.md decision D7). Change only with a new dated decision; the numbers then need `stats` re-run.
TOST_BANDS: dict[str, float] = {
    "median_mi": 2.0, "line_rate": 0.02, "max_coupling": 2.0,
    "new_tests_per_intent": 0.20, "story_line_rate": 0.05, "story_min_mi": 3.0,
}
# Reliability outcomes: column -> description. Columns are booleans in runs.csv.
SUCCESS_PREDICATES: dict[str, str] = {
    "build_first_pass": "first compiler invocation succeeded",
    "green_first_try": "whole suite green at the first recorded test execution",
    "e2e_first_pass": "agent probes passed at the first non-infrastructure e2e attempt",
    "final_e2e_ok": "last e2e execution succeeded",
    "final_tests_green": "last recorded test execution green",
    "clean_run": "build_first_pass and green_first_try and e2e_first_pass",
}
# Sensitivity sets: id -> (label, description). Filters are implemented in analysis.filter_set.
SENSITIVITY_SETS: dict[str, tuple[str, str]] = {
    "S0": ("primary", "all 120 runs (119 for e2e metrics)"),
    "S1": ("identity", "drop the 5 cells whose agent files name the other model"),
    "S2": ("compile-remove", "drop the 10 runs that excluded generated tests from compilation"),
    "S3": ("harness", "stratify by harness version (reported per stratum)"),
    "S4": ("core-4", "stories 1 to 4 only (the artifact's 48 runs)"),
    "S5": ("no-story-1", "drop story 1 (empty baseline, first harness use, re-run batch)"),
    "S6": ("regime", "stories 1 to 3 (composite-selected merges) vs stories 4 to 10 (hand-picked merges, five per model)"),
    "S7": ("infra", "count infrastructure e2e executions as attempts"),
    "S8": ("exceptional", "drop the 3 runs with exceptional endings"),
    "S9": ("snapshots", "drop runs whose final metrics snapshot is an n_types outlier"),
    "S10": ("holdout", "stories 5 to 10 only (not used to form F1 to F5)"),
}
ROBUSTNESS_SETS = ("S1", "S2", "S5", "S7", "S8")

# Figures (ACM acmart sigconf; verified by a LaTeX probe during the scaffold build).
COLUMN_WIDTH_IN = 3.33
TEXT_WIDTH_IN = 7.0
FIGURE_DPI = 300

# Macro naming.
MACRO_PREFIX = "val"
