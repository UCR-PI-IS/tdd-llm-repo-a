"""Analysis layer: sensitivity filters, robustness labels and the macro grammar (repo-gated end to end)."""
import numpy as np
import pandas as pd
import pytest

from tdd_paper import analysis as an
from tdd_paper import config as C
from tdd_paper import dataset as ds

from conftest import requires_repo


def test_robustness_labels_rules():
    rows = []
    def add(metric, set_id, delta, reject):
        rows.append({"metric": metric, "set": set_id, "delta": delta, "reject": reject, "family": "primary"})
    for s in ["S0", *C.ROBUSTNESS_SETS, "S4", "S10", "S6:pos1-3", "S6:pos4-10"]:
        add("robust_metric", s, 0.5, True)
        add("null_metric", s, 0.01 if s != "S1" else -0.01, False)      # sign flip of a null is ignored
        add("subset_metric", s, 0.4, s not in ("S4",))                   # only the 4-story subset changes
        add("fragile_metric", s, 0.4, s != "S1")                          # a robustness set changes the decision
        add("regime_metric", s, -0.3 if s == "S6:pos1-3" else 0.3, True)
    lab = an.robustness_labels(pd.DataFrame(rows)).set_index("metric")["robustness"]
    assert lab["robust_metric"] == "robust"
    assert lab["null_metric"] == "robust-null"
    assert lab["subset_metric"] == "subset-dependent"
    assert lab["fragile_metric"] == "fragile"
    assert lab["regime_metric"] == "regime-dependent"



def test_s7_changes_the_tested_secondary_e2e_measure():
    """The secondary family tests the e2e index without infrastructure executions; S7 counts them (Q19)."""
    assert "e2e_attempts_to_pass_noinfra" in C.SECONDARY_FAMILY
    assert "e2e_attempts_to_pass" not in C.SECONDARY_FAMILY
    runs = pd.DataFrame({"e2e_attempts_to_pass": [3, 1, 2], "e2e_attempts_to_pass_noinfra": [1, 1, 2]})
    s7 = an.filter_set(runs, "S7")["S7"]
    assert s7["e2e_attempts_to_pass_noinfra"].tolist() == [3, 1, 2]
    assert runs["e2e_attempts_to_pass_noinfra"].tolist() == [1, 1, 2]  # S0 frame untouched

@pytest.fixture(scope="module")
def result():
    bundle = ds.build_runs()
    res = an.run_all(bundle, n_boot=200, n_perm=300, with_stability=False, sets=("S0", "S1", "S4"))
    return bundle, res


@requires_repo
def test_filter_sets(result):
    bundle, _ = result
    runs = bundle["runs"]
    assert len(an.filter_set(runs, "S1")["S1"]) == C.EXPECTED_CELLS - len(C.IDENTITY_AMBIGUOUS_CELLS)
    assert len(an.filter_set(runs, "S2")["S2"]) == C.EXPECTED_CELLS - len(C.COMPILE_REMOVE_CELLS)
    assert len(an.filter_set(runs, "S4")["S4"]) == 48
    assert len(an.filter_set(runs, "S10")["S10"]) == 72
    assert "S3" not in C.SENSITIVITY_SETS  # retired with the harness-version split (D18)
    with pytest.raises(KeyError):
        an.filter_set(runs, "S3")
    assert len(an.filter_set(runs, "S8")["S8"]) == C.EXPECTED_CELLS - len(C.EXCEPTIONAL_ENDING_CELLS)


@requires_repo
def test_effects_and_macro_grammar(result):
    _, res = result
    eff = res.frames["effects"].set_index("metric")
    assert set(C.CONFIRMATORY_FAMILY) <= set(eff.index)
    prim = eff.loc[list(C.CONFIRMATORY_FAMILY)]
    assert prim["p_holm"].notna().all()
    # red-first is a null effect over ten stories; the build-cost metrics are not
    assert abs(eff.loc["red_first", "delta"]) < 0.1
    assert eff.loc["build_failed_execs", "delta"] > 0.3 and bool(eff.loc["build_failed_execs", "reject"])
    macros = res.numbers.entries
    for name in ("NStories", "NCells", "NRunsEndToEnd", "BuildFailedExecsKimiMedian", "BuildFailedExecsKimiIqr",
                 "RedFirstQwenShare", "BuildFailedExecsCliffDelta", "BuildFailedExecsPHolm", "BuildFailedExecsDirection",
                 "BuildFailedExecsCliffDeltaSFour", "NewTestsPerIntentTostSmallestBand", "BuildFirstPassKimiPassAtThree",
                 "CompositeQwenWins", "ExampleNewTestMethods", "StoryThreeIntents"):
        assert name in macros, name
    assert macros["NCells"]["value"] == 120 and macros["ExampleNewTestMethods"]["value"] == 25
    assert macros["BuildFailedExecsKimiMedian"]["provenance"] == "tool-measured"
    assert all(e["provenance"] for e in macros.values())
