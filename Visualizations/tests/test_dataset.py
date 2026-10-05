"""Pooled dataset contract (repo-gated) and story-file parsing."""
import pandas as pd
import pytest

from tdd_paper import config as C
from tdd_paper import dataset as ds
from tdd_paper.schema import COLUMN_CATALOG, validate_runs

from conftest import requires_repo


@requires_repo
def test_story_meta_effort_formats():
    expected = {"CPD-LC-001-001": 720, "CPD-LC-001-003": 480, "CPD-LC-001-009": 225, "SQL-LS-001-007": 495,
                "PQL-AE-001-001": 1181, "PQL-AE-001-002": 1425, "PQL-LB-002-001": 620, "SPT-UM-001-003": 365}
    for story, minutes in expected.items():
        meta = ds.story_meta(story)
        assert meta["effort_minutes"] == minutes, (story, meta)
        assert meta["title"], story


@pytest.fixture(scope="module")
def pooled():
    return ds.build_runs()


@requires_repo
def test_runs_contract(pooled):
    runs = pooled["runs"]
    assert len(runs) == C.EXPECTED_CELLS
    assert runs.groupby("story").size().eq(12).all()
    assert set(runs["model"]) == set(C.MODEL_ORDER)
    assert validate_runs(runs, strict=False) == []
    assert runs["run_id"].is_unique
    assert runs["e2e_execs"].isna().sum() == C.EXPECTED_CELLS - C.EXPECTED_E2E_CELLS
    assert set(runs.loc[runs["identity_ambiguous"] == True, "run_id"]) == set(C.IDENTITY_AMBIGUOUS_CELLS)
    assert set(runs.loc[runs["compile_remove_added"] > 0, "run_id"]) == set(C.COMPILE_REMOVE_CELLS)
    assert int((runs["intents_confirmed_ok"] == False).sum()) == C.INTENTS_CONFIRMED_WRONG_EXPECTED
    assert int(runs["intents_confirmed_ok"].isna().sum()) == C.INTENTS_CONFIRMED_MISSING_EXPECTED
    assert (runs["evidence_completeness"] < 1.0).sum() == len(C.E2E_MISSING_CELLS)  # only cells without e2e evidence lack a tree
    assert not runs["harness_pre_e2e"].fillna(False).any()
    assert runs["n_run_commits"].eq(2).all()
    ending = runs.loc[runs["exceptional_ending"].fillna("") != "", "run_id"]
    assert set(ending) == set(C.EXCEPTIONAL_ENDING_CELLS)
    # an exceptional ending is exactly a failed final check (build, tests or e2e), D27
    assert set(ending) == set(runs.loc[runs["final_all_ok"] == False, "run_id"])


@requires_repo
def test_other_tables(pooled):
    assert len(pooled["stories"]) == 10 and list(pooled["stories"]["story_pos"]) == list(range(1, 11))
    assert len(pooled["cells"]) == 20
    ex = pooled["executions"]
    assert set(ex["tree"]) == {"build", "test", "metrics", "e2e"}
    assert pooled["types"]["story_new"].any()
    ex = pooled["excluded_refs"]
    assert set(ex["reason"]) <= {"model-not-in-study", "iteration-out-of-range", "no-evidence-in-working-tree",
                                 "unused-duplicate-ref"}
    assert (ex["reason"] == "model-not-in-study").sum() == 20      # DeepSeek 8, grok-4.3 6, grok-4.5 6
    assert ex.loc[ex["reason"] == "model-not-in-study", "pre_e2e"].all()
    assert (ex["reason"] == "no-evidence-in-working-tree").sum() == 0
