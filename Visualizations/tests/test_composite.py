"""Tests for tdd_paper.composite: hand-computed synthetic frames plus a
repo-gated regression against notebook 00, cell 18 (story PQL-AE-001-002)."""
import numpy as np
import pandas as pd
import pytest
from conftest import requires_repo

from tdd_paper import composite as cs
from tdd_paper.composite import (
    DEFAULT_PILLAR_SPEC, LOW, STEPS, derive_components, model_rank_summary,
    pareto_front, rank_stability, score, score_all_stories, score_story,
    simplex_grid, weight_sweep, worst_flag_score,
)

STORY = "PQL-AE-001-002"
PILLARS = ["build", "tests & coverage", "refactoring & quality", "behaviour"]


# ---------------------------------------------------------------------------
# Synthetic fixtures
# ---------------------------------------------------------------------------

@pytest.fixture
def synthetic_summ() -> pd.DataFrame:
    """Two models x two iterations with hand-computable scores."""
    return pd.DataFrame({
        "model": ["A", "A", "B", "B"],
        "iteration": [1, 2, 1, 2],
        "line_rate": [0.8, 0.5, 1.2, np.nan],      # abs, clipped: .8 .5 1.0 0
        "build_failed_execs": [0, 1, 3, 0],         # 1/(v+1): 1 .5 .25 1
        "build_errors_burned": [0, 4, 2, 8],        # rel, inverted: 1 .5 .75 0
        "attempts_to_green": [1, 2, 4, 1],          # 1/v: 1 .5 .25 1
        "e2e_attempts_to_pass": [1, 1, 1, 1],
    })


REDUCED_SPEC = {
    "build": [
        ("build efficiency", "abs", "build_efficiency"),
        (f"errors burned {LOW}", "rel", "build_errors_burned"),
    ],
    "tests": [
        ("line coverage", "abs", "line_rate"),
        ("green efficiency", "abs", "green_efficiency"),
    ],
}

EMPTY_MT = pd.DataFrame(columns=["model", "iteration", *cs.WORST_FLAG_COLUMNS])


# ---------------------------------------------------------------------------
# (1) hand-computed composite on the synthetic frame
# ---------------------------------------------------------------------------

def test_score_story_hand_computed(synthetic_summ):
    r = score_story(synthetic_summ, EMPTY_MT, spec=REDUCED_SPEC)

    assert r.index.name == "rank"
    assert list(r.index) == [1, 2, 3, 4]
    assert list(r.columns) == ["model", "iteration", "composite", "build", "tests",
                               "pareto", "top1_share", "rank_range",
                               "score:build efficiency", f"score:errors burned {LOW}",
                               "score:line coverage", "score:green efficiency"]

    # A/1: build mean(1, 1) = 1, tests mean(.8, 1) = .9 -> .95
    # B/1: build mean(.25, .75) = .5, tests mean(1, .25) = .625 -> .5625
    # A/2 and B/2 both .5; tie broken by model then iteration (ascending)
    assert list(zip(r["model"], r["iteration"])) == [("A", 1), ("B", 1), ("A", 2), ("B", 2)]
    assert r["composite"].tolist() == pytest.approx([0.95, 0.5625, 0.5, 0.5])
    assert r["build"].tolist() == pytest.approx([1.0, 0.5, 0.5, 0.5])
    assert r["tests"].tolist() == pytest.approx([0.9, 0.625, 0.5, 0.5])

    assert r["score:build efficiency"].tolist() == pytest.approx([1.0, 0.25, 0.5, 1.0])
    assert r[f"score:errors burned {LOW}"].tolist() == pytest.approx([1.0, 0.75, 0.5, 0.0])
    assert r["score:line coverage"].tolist() == pytest.approx([0.8, 1.0, 0.5, 0.0])
    assert r["score:green efficiency"].tolist() == pytest.approx([1.0, 0.25, 0.5, 1.0])

    # A/1 dominates everyone; identical pillar vectors (A/2, B/2) keep order.
    assert r["pareto"].tolist() == [True, False, False, False]
    assert r["top1_share"].tolist() == pytest.approx([1.0, 0.0, 0.0, 0.0])
    assert r["rank_range"].tolist() == ["1–1", "2–2", "3–3", "4–4"]


def test_score_story_weights(synthetic_summ):
    r = score_story(synthetic_summ, EMPTY_MT, spec=REDUCED_SPEC,
                    weights={"build": 3.0, "tests": 1.0})
    # A/1: (3*1 + .9)/4 = .975 ; B/1: (3*.5 + .625)/4 = .53125
    assert r.loc[1, "composite"] == pytest.approx(0.975)
    assert r.loc[2, "composite"] == pytest.approx(0.53125)
    with pytest.raises(ValueError):
        score_story(synthetic_summ, EMPTY_MT, spec=REDUCED_SPEC, weights={"build": 1.0})


def test_spec_errors(synthetic_summ):
    with pytest.raises(KeyError):
        score_story(synthetic_summ, EMPTY_MT,
                    spec={"p": [("missing", "abs", "no_such_column")]})
    with pytest.raises(ValueError):
        score_story(synthetic_summ, EMPTY_MT, spec={"p": [("x", "weird", "line_rate")]})
    with pytest.raises(ValueError):
        score_story(synthetic_summ, EMPTY_MT, spec={"p": []})


def test_default_spec_shape():
    assert list(DEFAULT_PILLAR_SPEC) == PILLARS
    assert sum(len(v) for v in DEFAULT_PILLAR_SPEC.values()) == 20
    assert set(cs.DEFAULT_PILLAR_WEIGHTS) == set(PILLARS)
    for items in DEFAULT_PILLAR_SPEC.values():
        for label, norm, _column in items:
            assert norm in ("abs", "rel")
            assert not (norm == "abs" and label.endswith(LOW))


def test_empty_summary_gives_empty_ranking():
    empty = pd.DataFrame(columns=["model", "iteration"])
    r = score_story(empty, EMPTY_MT)
    assert r.empty and r.index.name == "rank"
    assert "composite" in r.columns and "rank_range" in r.columns


# ---------------------------------------------------------------------------
# derived components
# ---------------------------------------------------------------------------

def test_derive_components_and_worst_flag(synthetic_summ):
    mt = pd.DataFrame({
        "model": ["A", "A", "A", "B"], "iteration": [1, 1, 1, 1],
        "mi": [80, 40, 60, np.nan], "mi_flag": ["GREEN", "RED", "YELLOW", None],
        "cc": [5, 30, 12, np.nan], "cc_flag": ["GREEN", "RED", "YELLOW", None],
        "coupling": [3, 3, 15, np.nan], "coupling_flag": ["GREEN", "GREEN", "YELLOW", None],
        "dit": [1, 2, 1, np.nan], "dit_flag": ["GREEN", "GREEN", "GREEN", None],
    })
    # A/1: worst MI 40 RED (0), worst CC 30 RED (0), worst coupling 15 YELLOW
    # (.5), worst DIT 2 GREEN (1) -> mean .375
    assert worst_flag_score(mt.iloc[:3]) == pytest.approx(0.375)
    assert np.isnan(worst_flag_score(mt.iloc[3:]))

    runs = derive_components(synthetic_summ, mt)
    assert list(runs.index.names) == ["model", "iteration"]
    assert runs.loc[("A", 1), "worst_flag"] == pytest.approx(0.375)
    assert np.isnan(runs.loc[("B", 1), "worst_flag"])   # types without values
    assert np.isnan(runs.loc[("A", 2), "worst_flag"])   # no types at all
    assert runs["build_efficiency"].tolist() == pytest.approx([1.0, 0.5, 0.25, 1.0])
    assert runs["green_efficiency"].tolist() == pytest.approx([1.0, 0.5, 0.25, 1.0])
    assert runs["e2e_efficiency"].tolist() == pytest.approx([1.0] * 4)

    no_types = derive_components(synthetic_summ, EMPTY_MT)
    assert no_types["worst_flag"].isna().all()
    no_types = derive_components(synthetic_summ, None)
    assert no_types["worst_flag"].isna().all()


# ---------------------------------------------------------------------------
# (2) Pareto front
# ---------------------------------------------------------------------------

def test_pareto_front_three_points():
    P = np.array([[1.0, 0.2], [0.3, 0.9], [0.8, 0.1]])   # third dominated by first
    assert pareto_front(P).tolist() == [True, True, False]
    assert pareto_front(np.array([[1.0, 1.0], [1.0, 1.0]])).tolist() == [True, True]
    assert pareto_front(np.zeros((0, 3))).tolist() == []


# ---------------------------------------------------------------------------
# (3) simplex grid: 455 weightings for four pillars
# ---------------------------------------------------------------------------

def test_weight_sweep_grid():
    grid = simplex_grid(4)
    assert grid.shape == (455, 4)
    assert np.allclose(grid.sum(axis=1), 1.0, atol=1e-9)
    assert set(np.round(grid.ravel(), 2)) <= set(STEPS)
    assert len(STEPS) == 13 and STEPS[0] == 0.10 and STEPS[-1] == 0.70

    P = np.array([[1.0, 0.9, 0.8, 1.0], [0.5, 1.0, 0.5, 0.5], [0.2, 0.2, 0.2, 0.2]])
    top1, lo, hi, g = weight_sweep(P)
    assert g.shape == (455, 4)
    assert top1.sum() == pytest.approx(1.0)
    assert (lo <= hi).all() and lo.min() == 1 and hi.max() == 3
    assert lo.tolist()[2] == 3 and hi.tolist()[2] == 3      # always last
    with pytest.raises(ValueError):
        weight_sweep(np.ones((2, 1)))                        # no 1-pillar weighting


# ---------------------------------------------------------------------------
# (4) score rules
# ---------------------------------------------------------------------------

def test_score_abs_clips_and_fills():
    s = score(pd.Series([-0.5, 0.3, 1.7, np.nan, np.inf]), "abs")
    assert s.tolist() == [0.0, 0.3, 1.0, 0.0, 1.0]
    assert score(pd.Series([True, False]), "abs").tolist() == [1.0, 0.0]
    assert score(pd.Series([True, False]), "abs").dtype == float


def test_score_rel_rules():
    assert score(pd.Series([3, 3, 3]), "rel").tolist() == [0.5, 0.5, 0.5]
    assert score(pd.Series([3, 3, np.nan]), "rel").tolist() == [0.5, 0.5, 0.0]
    assert score(pd.Series([0, 2, 4, np.nan]), "rel").tolist() == [0.0, 0.5, 1.0, 0.0]
    assert score(pd.Series([0, 2, 4, np.nan]), "rel", invert=True).tolist() == [1.0, 0.5, 0.0, 0.0]
    assert score(pd.Series([np.nan, np.nan]), "rel").tolist() == [0.0, 0.0]
    assert score(pd.Series([7]), "rel").tolist() == [0.5]


def test_score_rejects_bad_arguments():
    with pytest.raises(ValueError):
        score(pd.Series([0.5]), "minmax")
    with pytest.raises(ValueError):
        score(pd.Series([0.5]), "abs", invert=True)


# ---------------------------------------------------------------------------
# multi-story helpers
# ---------------------------------------------------------------------------

def test_score_all_stories_and_model_rank_summary(synthetic_summ):
    swapped = synthetic_summ.assign(model=synthetic_summ["model"].map({"A": "B", "B": "A"}))
    all_runs = score_all_stories({"S1": (synthetic_summ, EMPTY_MT), "S2": (swapped, EMPTY_MT)},
                                 spec=REDUCED_SPEC)
    assert list(all_runs.columns[:3]) == ["story", "rank", "model"]
    assert len(all_runs) == 8
    assert all_runs.groupby("story")["rank"].apply(list).tolist() == [[1, 2, 3, 4]] * 2

    summary = model_rank_summary(all_runs)
    # S1 ranks: A/1=1, B/1=2, A/2=3, B/2=4 ; S2: B/1=1, A/1=2, A/2=3, B/2=4
    assert summary.loc["A", "wins"] == 1 and summary.loc["B", "wins"] == 1
    assert summary.loc["A", "mean_rank"] == pytest.approx(2.25)
    assert summary.loc["B", "mean_rank"] == pytest.approx(2.75)
    assert summary.loc["A", "top3_share"] == pytest.approx(1.0)
    assert summary.loc["B", "top3_share"] == pytest.approx(0.5)
    assert summary.loc["A", "pareto_share"] == pytest.approx(0.25)
    assert summary.loc["B", "pareto_share"] == pytest.approx(0.25)
    assert summary["n_stories"].tolist() == [2, 2]
    assert summary["n_runs"].tolist() == [4, 4]
    assert list(summary.index) == ["A", "B"]          # equal wins, better mean rank first

    single = model_rank_summary(score_story(synthetic_summ, EMPTY_MT, spec=REDUCED_SPEC))
    assert single.loc["A", "wins"] == 1 and single["n_stories"].tolist() == [1, 1]


def test_rank_stability_synthetic(synthetic_summ):
    res = rank_stability(synthetic_summ, EMPTY_MT, n_boot=100, seed=7, spec=REDUCED_SPEC)
    assert set(res["p_win"]) == {"A", "B"}
    assert sum(res["p_win"].values()) == pytest.approx(1.0)
    assert res["n_runs"] == {"A": 2, "B": 2}
    assert res["mean_rank"]["A"]["observed"] == pytest.approx(2.0)   # ranks 1 and 3
    assert res["mean_rank"]["B"]["observed"] == pytest.approx(3.0)   # ranks 2 and 4
    for iv in res["mean_rank"].values():
        assert 1.0 <= iv["lo"] <= iv["hi"] <= 4.0
    assert res["observed_winner"] == "A"
    assert rank_stability(synthetic_summ, EMPTY_MT, n_boot=30, seed=7, spec=REDUCED_SPEC) == \
        rank_stability(synthetic_summ, EMPTY_MT, n_boot=30, seed=7, spec=REDUCED_SPEC)


# ---------------------------------------------------------------------------
# (5), (6) repo-gated regression against notebook 00, cell 18
# ---------------------------------------------------------------------------

@pytest.fixture(scope="module")
def pql_frames():
    import tdd_results as tdd
    tdd.reset_caches()
    data = tdd.load_all(STORY)
    summ = tdd.iteration_summary(STORY, data)
    return summ, tdd.finals(data["metrics_types"])


@requires_repo
def test_regression_pql_ae_001_002(pql_frames):
    summ, mt = pql_frames
    r = score_story(summ, mt)

    assert r.index.name == "rank" and list(r.index) == list(range(1, 13))
    top = r.loc[1]
    assert (top["model"], int(top["iteration"])) == ("Qwen3.7-max", 1)
    assert top["composite"] == pytest.approx(0.832, abs=5e-4)
    for pillar, expected in zip(PILLARS, [1.000, 0.675, 0.654, 1.000]):
        assert top[pillar] == pytest.approx(expected, abs=5e-4), pillar
    assert bool(top["pareto"]) is True
    assert top["top1_share"] == 1.0
    assert top["rank_range"] == "1–1"

    expected = [("Qwen3.7-max", 6, 0.803), ("Qwen3.7-max", 5, 0.760),
                ("Kimi-K2.5", 3, 0.745), ("Kimi-K2.5", 4, 0.740), ("Qwen3.7-max", 3, 0.738)]
    for rank, (model, iteration, comp) in zip(range(2, 7), expected):
        row = r.loc[rank]
        assert (row["model"], int(row["iteration"])) == (model, iteration), rank
        assert row["composite"] == pytest.approx(comp, abs=5e-4), rank

    # The notebook's full weight-sweep column for this story.
    assert r["rank_range"].tolist() == [
        "1–1", "2–5", "3–7", "3–10", "2–8", "4–9",
        "3–9", "4–12", "5–11", "8–12", "5–12", "4–12"]
    assert r["pareto"].tolist() == [True, True, False, False, True, False,
                                    False, False, False, False, True, False]
    assert {f"score:{label}" for items in DEFAULT_PILLAR_SPEC.values()
            for label, _, _ in items} <= set(r.columns)
    assert ((r[PILLARS] >= 0) & (r[PILLARS] <= 1)).all().all()


@requires_repo
def test_rank_stability_pql(pql_frames):
    summ, mt = pql_frames
    res = rank_stability(summ, mt, n_boot=200)
    assert sum(res["p_win"].values()) == pytest.approx(1.0)
    assert set(res["p_win"]) == {"Kimi-K2.5", "Qwen3.7-max"}
    assert res["n_boot"] == 200 and res["seed"] == 20261003
    assert res["observed_winner"] == "Qwen3.7-max"
    for iv in res["mean_rank"].values():
        assert 1.0 <= iv["lo"] <= iv["hi"] <= 12.0
