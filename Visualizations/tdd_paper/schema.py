"""Column catalogue and validation for the pooled runs table.

Every column that may reach the report is declared here with its provenance so
that ``numbers.json`` can carry the tag and writers can tell measured values from
agent self-reports. ``validate_runs`` refuses frames with unknown or missing
catalogued columns.
"""
from __future__ import annotations

from dataclasses import dataclass
from enum import Enum

import pandas as pd

from .config import LAYERS


class Provenance(str, Enum):
    DESIGN = "design"
    TOOL = "tool-measured"
    GIT = "derived-from-git"
    SELF = "self-reported"
    DERIVED = "derived"
    ORACLE = "agent-authored-oracle"


@dataclass(frozen=True)
class ColumnSpec:
    name: str
    family: str          # design | C convergence | V verification | Q quality | B behavioral | R reliability | P provenance | E evidence
    provenance: Provenance
    direction: int       # +1 higher is better, -1 lower is better, 0 no direction
    dtype: str           # pandas dtype string
    unit: str
    description: str


class SchemaError(ValueError):
    pass


def _spec(name, family, prov, direction, dtype, unit, description) -> ColumnSpec:
    return ColumnSpec(name, family, prov, direction, dtype, unit, description)


D, T, G, S, X, O = (Provenance.DESIGN, Provenance.TOOL, Provenance.GIT, Provenance.SELF,
                    Provenance.DERIVED, Provenance.ORACLE)

_SPECS: list[ColumnSpec] = [
    # --- design / identity --------------------------------------------------
    _spec("run_id", "design", D, 0, "str", "", "story/model/iteration"),
    _spec("story", "design", D, 0, "str", "", "user story id"),
    _spec("story_pos", "design", D, 0, "Int64", "", "position in the story chain, 1 to 10"),
    _spec("model", "design", D, 0, "str", "", "canonical display label"),
    _spec("model_key", "design", D, 0, "str", "", "canonical join key"),
    _spec("iteration", "design", D, 0, "Int64", "", "iteration 1 to 6"),
    _spec("n_intents", "design", D, 0, "Int64", "intents", "confirmed intents in UserIntents/<story>.json"),
    *[_spec(f"intents_{l}", "design", D, 0, "Int64", "intents", f"confirmed intents in the {l} layer") for l in LAYERS],
    _spec("effort_minutes", "design", D, 0, "Int64", "min", "human effort estimate from UserStories/<story>.md"),
    _spec("effort_source", "design", D, 0, "str", "", "which pattern produced effort_minutes"),
    _spec("baseline_sha", "design", G, 0, "str", "", "trunk commit the run was cut from (parent of commit A)"),
    _spec("baseline_author_model", "design", D, 0, "str", "", "model whose merged run produced the baseline (human for story 1)"),
    _spec("own_baseline", "design", D, 0, "boolean", "", "the run's model authored its baseline"),
    _spec("baseline_test_attrs", "design", G, 0, "Int64", "attributes", "NUnit test attributes inherited at the baseline"),
    _spec("baseline_prod_lines", "design", G, 0, "Int64", "lines", "production .cs lines inherited at the baseline"),
    _spec("merged_into_trunk", "design", D, 0, "boolean", "", "this run was merged as the next baseline"),
    _spec("ref", "design", G, 0, "str", "", "git ref resolved for the cell"),
    _spec("tip_sha", "design", G, 0, "str", "", "branch tip (commit B)"),
    _spec("commit_a_sha", "design", G, 0, "str", "", "results commit"),
    _spec("commit_b_sha", "design", G, 0, "str", "", "code commit"),
    _spec("n_run_commits", "design", G, 0, "Int64", "", "run commits on the branch (expected 2)"),
    _spec("harness_pre_e2e", "design", G, 0, "boolean", "", "run branch predates the e2e harness commit"),
    _spec("identity_declared_model", "design", G, 0, "str", "", "model key named by the agent files on the branch"),
    _spec("identity_agents_agree", "design", G, 0, "boolean", "", "all agent files on the branch name the same model"),
    _spec("identity_ambiguous", "design", G, 0, "boolean", "", "agent files name the other model"),
    _spec("compile_remove_added", "design", G, 0, "Int64", "lines", "<Compile Remove> lines added by the run"),
    _spec("compile_remove_files", "design", G, 0, "str", "", "files excluded from compilation"),
    _spec("evidence_completeness", "E evidence", X, 0, "float64", "share", "trees with a summary file / 4"),
    _spec("missing_trees", "E evidence", X, 0, "str", "", "trees without evidence"),
    _spec("exceptional_ending", "E evidence", X, 0, "str", "", "why the run does not end green (empty if it does)"),
    _spec("included_core", "design", X, 0, "boolean", "", "not identity_ambiguous and compile_remove_added == 0"),
    # --- convergence (tool) --------------------------------------------------
    _spec("build_execs", "C convergence", T, -1, "Int64", "executions", "build executions recorded"),
    _spec("build_failed_execs", "C convergence", T, -1, "Int64", "executions", "build executions that did not compile"),
    _spec("build_first_pass", "C convergence", T, +1, "boolean", "", "first build execution succeeded"),
    _spec("build_errors_burned", "C convergence", T, -1, "Int64", "errors", "compiler errors summed over all build executions"),
    _spec("build_peak_errors", "C convergence", T, -1, "Int64", "errors", "largest number of compiler errors in one build execution"),
    _spec("build_status", "C convergence", T, 0, "str", "", "status of the final build"),
    _spec("build_errors", "C convergence", T, -1, "Int64", "errors", "errors of the final build"),
    _spec("build_warnings", "C convergence", T, -1, "Int64", "warnings", "warnings of the final build"),
    _spec("test_execs", "C convergence", T, -1, "Int64", "executions", "test executions recorded"),
    _spec("attempts_to_green", "C convergence", T, -1, "Int64", "executions", "index of the first test execution with the whole suite green (1 = first execution)"),
    _spec("test_failures_burned", "C convergence", T, -1, "Int64", "failures", "failed test cases summed over all test executions"),
    _spec("red_first", "C convergence", T, 0, "boolean", "", "first recorded test execution was not green (first-execution red)"),
    _spec("green_first_try", "R reliability", T, +1, "boolean", "", "whole suite green at the first recorded test execution"),
    _spec("test_status", "C convergence", T, 0, "str", "", "status of the final test execution"),
    _spec("total_tests", "V verification", T, 0, "Int64", "tests", "tests in the final execution (whole suite)"),
    _spec("passed", "V verification", T, 0, "Int64", "tests", "passed in the final execution"),
    _spec("failed", "V verification", T, -1, "Int64", "tests", "failed in the final execution"),
    _spec("skipped", "V verification", T, 0, "Int64", "tests", "skipped in the final execution"),
    _spec("pass_rate", "V verification", T, +1, "float64", "share", "passed / total in the final execution"),
    _spec("wall_clock_min", "C convergence", X, -1, "float64", "min", "first to last artifact of the run"),
    _spec("active_min", "C convergence", X, -1, "float64", "min", "wall clock with pauses above GAP_CAP_MIN removed"),
    _spec("n_pauses", "C convergence", X, 0, "Int64", "", "inter-artifact gaps above GAP_CAP_MIN"),
    _spec("wall_to_green_min", "C convergence", X, -1, "float64", "min", "first build to first green suite"),
    # --- verification --------------------------------------------------------
    _spec("line_rate", "V verification", T, +1, "float64", "share", "line coverage of the whole solution at the final test execution"),
    _spec("branch_rate", "V verification", T, +1, "float64", "share", "branch coverage, whole solution, final execution"),
    _spec("tests_per_intent", "V verification", T, 0, "float64", "tests/intent", "DEPRECATED: whole final suite / intents (inherited-suite inflated)"),
    _spec("layer_coverage", "V verification", T, 0, "float64", "share", "DEPRECATED: constant"),
    _spec("layer_balance", "V verification", T, 0, "float64", "", "DEPRECATED: constant"),
    *[_spec(f"tests_{l}", "V verification", T, 0, "Int64", "tests", f"final-run TRX tests in the {l} test project (whole suite)") for l in LAYERS],
    _spec("new_test_methods", "V verification", G, 0, "Int64", "methods", "test methods added by the run relative to its baseline"),
    _spec("new_test_cases", "V verification", G, 0, "Int64", "cases", "NUnit test attributes added by the run (cases after parameter expansion)"),
    *[_spec(f"new_test_methods_{l}", "V verification", G, 0, "Int64", "methods", f"story-new test methods in {l}") for l in LAYERS],
    *[_spec(f"new_test_cases_{l}", "V verification", G, 0, "Int64", "cases", f"story-new test cases in {l}") for l in LAYERS],
    _spec("new_test_files", "V verification", G, 0, "Int64", "files", "test files added by the run"),
    _spec("modified_test_files", "V verification", G, 0, "Int64", "files", "test files modified by the run"),
    _spec("new_test_classes", "V verification", G, 0, "str", "", "test classes declared in story-new test files"),
    _spec("new_test_methods_compiled", "V verification", G, 0, "Int64", "methods", "new test methods not excluded by <Compile Remove>"),
    _spec("prod_loc_added", "C convergence", G, 0, "Int64", "lines", "production .cs lines added by the run (git numstat)"),
    _spec("prod_loc_deleted", "C convergence", G, 0, "Int64", "lines", "production .cs lines deleted by the run"),
    _spec("prod_files_added", "C convergence", G, 0, "Int64", "files", "production .cs files added by the run"),
    _spec("prod_files_modified", "C convergence", G, 0, "Int64", "files", "production .cs files modified by the run"),
    _spec("test_loc_added", "C convergence", G, 0, "Int64", "lines", "test .cs lines added by the run"),
    _spec("test_loc_deleted", "C convergence", G, 0, "Int64", "lines", "test .cs lines deleted by the run"),
    _spec("new_tests_per_intent", "V verification", X, 0, "float64", "methods/intent", "story-new test methods in the branch diff per confirmed intent"),
    _spec("new_cases_per_intent", "V verification", X, 0, "float64", "cases/intent", "story-new test cases per confirmed intent"),
    _spec("new_layer_balance", "V verification", X, +1, "float64", "", "1 - mean |ratio - 1| over layers with intents"),
    _spec("story_line_rate", "V verification", X, +1, "float64", "share", "line coverage of production files the run created or modified"),
    _spec("story_branch_rate", "V verification", X, +1, "float64", "share", "branch coverage of production files the run created or modified"),
    _spec("first_run_new_fail_share", "V verification", X, 0, "float64", "share", "share of the run's own test cases not passing at the first recorded execution"),
    _spec("compile_red", "V verification", T, 0, "boolean", "", "first test execution had zero tests (test projects failed to compile)"),
    _spec("assertion_free_new_tests", "V verification", G, -1, "Int64", "methods", "story-new test methods without an assertion token"),
    # --- quality -----------------------------------------------------------------
    _spec("median_mi", "Q quality", T, +1, "float64", "MI", "median maintainability index over all types of the solution (harness formula, no Halstead term)"),
    _spec("min_mi", "Q quality", T, +1, "float64", "MI", "worst type MI, whole solution"),
    _spec("max_cc", "Q quality", T, -1, "float64", "", "worst type cyclomatic complexity, whole solution"),
    _spec("max_coupling", "Q quality", T, -1, "float64", "classes", "largest class coupling of any type in the solution"),
    _spec("max_dit", "Q quality", T, -1, "float64", "", "worst type depth of inheritance, whole solution"),
    _spec("n_types", "Q quality", T, 0, "Int64", "types", "types measured in the final snapshot"),
    _spec("pct_green_types", "Q quality", T, +1, "float64", "share", "types with all four flags GREEN"),
    _spec("source_lines", "Q quality", T, 0, "Int64", "lines", "source lines, whole solution"),
    _spec("executable_lines", "Q quality", T, 0, "Int64", "lines", "executable lines, whole solution"),
    _spec("metrics_snapshots", "Q quality", T, 0, "Int64", "snapshots", "metrics executions recorded"),
    _spec("min_mi_gain", "Q quality", T, +1, "float64", "MI", "last minus first snapshot min MI, whole solution"),
    _spec("worst_coupling_drop", "Q quality", T, +1, "float64", "classes", "first minus last snapshot max coupling"),
    _spec("green_share_gain", "Q quality", T, +1, "float64", "share", "last minus first snapshot all-green share"),
    _spec("story_new_types", "Q quality", G, 0, "str", "", "metrics types matched to story-new production files"),
    _spec("story_n_types", "Q quality", X, 0, "Int64", "types", "number of story-new types measured"),
    _spec("story_min_mi", "Q quality", X, +1, "float64", "MI", "worst maintainability index over the story's new types, final snapshot"),
    _spec("story_max_cc", "Q quality", X, -1, "float64", "", "worst CC over story-new types"),
    _spec("story_max_coupling", "Q quality", X, -1, "float64", "classes", "worst coupling over story-new types"),
    _spec("story_max_dit", "Q quality", X, -1, "float64", "", "worst DIT over story-new types"),
    _spec("story_pct_green", "Q quality", X, +1, "float64", "share", "all-green share over story-new types"),
    _spec("story_min_mi_gain", "Q quality", X, +1, "float64", "MI", "worst maintainability index over the story's new types, last minus first snapshot"),
    _spec("story_max_coupling_drop", "Q quality", X, +1, "float64", "classes", "first minus last snapshot worst coupling over story-new types"),
    _spec("refactor_cycles_measured", "Q quality", T, 0, "Int64", "", "metrics snapshots minus one"),
    # --- behavioral --------------------------------------------------------------
    _spec("e2e_execs", "B behavioral", T, -1, "Int64", "executions", "end-to-end executions recorded"),
    _spec("e2e_attempts_to_pass", "B behavioral", O, -1, "Int64", "executions", "index of the first end-to-end execution whose agent-authored probes all passed"),
    _spec("e2e_attempts_to_pass_noinfra", "B behavioral", O, -1, "Int64", "executions", "same, counting only executions where the API started"),
    _spec("e2e_first_pass", "R reliability", O, +1, "boolean", "", "first end-to-end execution that reached the API passed its probes"),
    _spec("e2e_probe_pass_rate", "B behavioral", O, +1, "float64", "share", "probes passed / total, final execution"),
    _spec("e2e_infra_failures", "B behavioral", T, -1, "Int64", "executions", "e2e executions where the API never started"),
    _spec("e2e_probe_depth", "B behavioral", S, 0, "Int64", "probes", "probes in the final execution (agent-authored)"),
    # --- final state -----------------------------------------------------------
    _spec("final_build_ok", "R reliability", T, +1, "boolean", "", "final build succeeded"),
    _spec("final_tests_green", "R reliability", T, +1, "boolean", "", "last test execution green"),
    _spec("final_e2e_ok", "R reliability", O, +1, "boolean", "", "last end-to-end execution passed its probes"),
    _spec("final_all_ok", "R reliability", X, +1, "boolean", "", "all three final checks succeeded"),
    _spec("clean_run", "R reliability", X, +1, "boolean", "", "first build, first test execution and first end-to-end execution all succeeded"),
    # --- self-reported (never scored) -------------------------------------------
    _spec("tg_status", "P provenance", S, 0, "str", "", "test-generation stage status (agent)"),
    _spec("tg_test_methods", "P provenance", S, 0, "Int64", "methods", "testMethodsEmitted (agent)"),
    _spec("tg_intents_confirmed", "P provenance", S, 0, "Int64", "intents", "intentsConfirmed (agent)"),
    _spec("cg_status", "P provenance", S, 0, "str", "", "code-generation stage status (agent)"),
    _spec("cg_files_created", "P provenance", S, 0, "Int64", "files", "filesCreated count (agent)"),
    _spec("cg_files_modified", "P provenance", S, 0, "Int64", "files", "filesModified count (agent)"),
    _spec("ref_status", "P provenance", S, 0, "str", "", "refactoring stage status (agent)"),
    _spec("ref_all_green", "P provenance", S, 0, "boolean", "", "allGreenAchieved (agent)"),
    _spec("ref_loop_iterations", "P provenance", S, 0, "Int64", "", "loopIterationsPerformed (agent)"),
    _spec("ref_n_remaining_violations", "P provenance", S, 0, "Int64", "", "remainingViolations count (agent)"),
    # --- provenance gaps (derived from self-report vs tool) ------------------------
    _spec("self_report_gap_methods", "P provenance", X, 0, "Int64", "methods", "tg_test_methods minus new_test_methods (like for like)"),
    _spec("self_report_gap_methods_abs", "P provenance", X, -1, "Int64", "methods", "absolute difference between the test methods the test generator reported and those counted in the branch diff"),
    _spec("self_report_gap_cases", "P provenance", X, 0, "Int64", "cases", "tg_test_methods minus new_test_cases (unit mismatch illustration)"),
    _spec("self_report_ratio", "P provenance", X, 0, "float64", "", "tg_test_methods / new_test_methods"),
    _spec("intents_report_error", "P provenance", X, 0, "Int64", "intents", "tg_intents_confirmed minus n_intents"),
    _spec("intents_confirmed_ok", "P provenance", X, +1, "boolean", "", "agent intent count equals the file"),
    _spec("refactor_loop_gap", "P provenance", X, 0, "Int64", "", "ref_loop_iterations minus refactor_cycles_measured"),
]

COLUMN_CATALOG: dict[str, ColumnSpec] = {s.name: s for s in _SPECS}
assert len(COLUMN_CATALOG) == len(_SPECS), "duplicate column in catalogue"

REQUIRED_COLUMNS: tuple[str, ...] = (
    "run_id", "story", "story_pos", "model", "model_key", "iteration", "n_intents",
    "build_execs", "build_failed_execs", "build_errors_burned", "build_peak_errors",
    "test_execs", "attempts_to_green", "test_failures_burned", "red_first",
    "line_rate", "branch_rate", "median_mi", "min_mi", "max_cc", "max_coupling", "max_dit",
    "e2e_execs", "evidence_completeness",
)

SELF_REPORTED_COLUMNS: tuple[str, ...] = tuple(
    s.name for s in _SPECS if s.provenance is Provenance.SELF)


def coerce_dtypes(df: pd.DataFrame) -> pd.DataFrame:
    """Cast catalogued columns to their declared pandas dtypes (nullable)."""
    out = df.copy()
    for name, spec in COLUMN_CATALOG.items():
        if name not in out.columns:
            continue
        try:
            if spec.dtype == "Int64":
                out[name] = pd.to_numeric(out[name], errors="coerce").round().astype("Int64")
            elif spec.dtype == "boolean":
                out[name] = out[name].map(
                    lambda v: pd.NA if pd.isna(v) else bool(v) if not isinstance(v, str)
                    else v.strip().lower() in ("true", "1", "yes")).astype("boolean")
            elif spec.dtype == "float64":
                out[name] = pd.to_numeric(out[name], errors="coerce").astype("float64")
            elif spec.dtype == "str":
                out[name] = out[name].astype("object").where(out[name].notna(), None)
        except (TypeError, ValueError) as exc:  # pragma: no cover - defensive
            raise SchemaError(f"cannot coerce column {name!r} to {spec.dtype}: {exc}") from exc
    return out


def validate_runs(df: pd.DataFrame, *, strict: bool = True) -> list[str]:
    """Return a list of warnings; raise SchemaError on missing required columns.

    ``strict`` also treats unknown (uncatalogued) columns as errors.
    """
    missing = [c for c in REQUIRED_COLUMNS if c not in df.columns]
    if missing:
        raise SchemaError(f"runs table is missing required columns: {missing}")
    unknown = [c for c in df.columns if c not in COLUMN_CATALOG]
    if unknown and strict:
        raise SchemaError(f"runs table has uncatalogued columns: {unknown}")
    warnings = [f"uncatalogued column: {c}" for c in unknown]
    if "run_id" in df.columns and df["run_id"].duplicated().any():
        raise SchemaError("duplicate run_id values")
    return warnings


def catalog_frame() -> pd.DataFrame:
    """The catalogue as a DataFrame (feeds tab_metric_catalog)."""
    return pd.DataFrame([{
        "name": s.name, "family": s.family, "provenance": s.provenance.value,
        "direction": s.direction, "dtype": s.dtype, "unit": s.unit, "description": s.description,
    } for s in _SPECS])
