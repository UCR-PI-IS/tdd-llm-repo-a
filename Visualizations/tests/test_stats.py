"""Synthetic-data tests for tdd_paper.stats (no evidence trees needed)."""
import json
import math

import numpy as np
import pandas as pd
import pytest
from scipy import stats as sps

from tdd_paper import stats as S


# --------------------------------------------------------------------------- #
# synthetic fixtures
# --------------------------------------------------------------------------- #
def _tidy(n_strata: int, n_per_cell: int, shift: float, seed: int, sd: float = 1.0) -> pd.DataFrame:
    """Balanced tidy frame (stratum, group, value); group A shifted by `shift`."""
    rng = np.random.default_rng(seed)
    rows = []
    for k in range(n_strata):
        for grp in ("A", "B"):
            for v in rng.normal(shift if grp == "A" else 0.0, sd, n_per_cell):
                rows.append((f"s{k}", grp, float(v)))
    return pd.DataFrame(rows, columns=["stratum", "group", "value"])


@pytest.fixture(scope="module")
def frame() -> pd.DataFrame:
    return _tidy(n_strata=5, n_per_cell=20, shift=0.15, seed=3)


@pytest.fixture(scope="module")
def frame_strong() -> pd.DataFrame:
    return _tidy(n_strata=5, n_per_cell=12, shift=0.8, seed=11)


# --------------------------------------------------------------------------- #
# Cliff's delta
# --------------------------------------------------------------------------- #
def test_cliffs_delta_separated_and_identical():
    assert S.cliffs_delta([1, 2, 3], [4, 5, 6]) == -1.0
    assert S.cliffs_delta([4, 5, 6], [1, 2, 3]) == 1.0
    assert S.cliffs_delta([1, 2, 3], [1, 2, 3]) == 0.0
    assert S.cliffs_delta([1, 1, 2], [1, 2, 2]) == pytest.approx((1 - 4) / 9)  # ties count zero
    assert math.isnan(S.cliffs_delta([], [1.0]))


def test_cliff_rows_matches_sign_count_with_ties():
    rng = np.random.default_rng(1)
    x = rng.integers(0, 5, 40).astype(float)
    y = rng.integers(0, 6, 35).astype(float)
    assert S._cliff_rows(x[None, :], y[None, :])[0] == pytest.approx(S.cliffs_delta(x, y), abs=1e-12)


def test_cliff_variance_edges_and_unbiasedness():
    assert S.cliff_variance([1, 2, 3], [4, 5, 6]) == 0.0           # complete separation
    assert math.isnan(S.cliff_variance([1.0], [1, 2, 3]))         # fewer than 2 values
    rng = np.random.default_rng(7)
    deltas, estimates = [], []
    for _ in range(1500):
        xa = rng.poisson(3, 6).astype(float)
        xb = rng.poisson(4, 6).astype(float)
        deltas.append(S.cliffs_delta(xa, xb))
        estimates.append(S.cliff_variance(xa, xb))
    assert np.mean(estimates) == pytest.approx(np.var(deltas, ddof=1), rel=0.10)


def test_magnitude_labels():
    assert S.magnitude_label(0.0) == "negligible"
    assert S.magnitude_label(-0.2) == "small"
    assert S.magnitude_label(0.4) == "medium"
    assert S.magnitude_label(-0.9) == "large"
    assert S.magnitude_label(0.147) == "small" and S.magnitude_label(0.474) == "large"
    assert S.magnitude_label(float("nan")) == "undefined"


def test_stratified_cliffs_delta_pooling_skips_and_dict(frame_strong):
    df = pd.concat([frame_strong, pd.DataFrame({"stratum": ["lonely"] * 3, "group": "A", "value": [1.0, 2.0, 3.0]})])
    res = S.stratified_cliffs_delta(df.value, df.group, df.stratum, "A", "B", n_boot=500)
    used = {k: v for k, v in res.per_stratum.items() if not v["skipped"]}
    assert res.per_stratum["lonely"]["skipped"] is True and len(used) == 5
    assert res.delta == pytest.approx(np.mean([v["delta"] for v in used.values()]))
    assert res.a12 == pytest.approx((res.delta + 1) / 2)
    assert res.n_a == 60 and res.n_b == 60 and res.magnitude == S.magnitude_label(res.delta)
    assert res.ci_low <= res.delta <= res.ci_high and res.delta > 0
    pairs = S.stratified_cliffs_delta(df.value, df.group, df.stratum, "A", "B", n_boot=0, pooling="pairs")
    assert pairs.delta == pytest.approx(res.delta) and math.isnan(pairs.ci_low)  # balanced: same pooled value
    json.dumps(res.to_dict())  # JSON friendly


def test_bca_runs_and_falls_back_on_separation(frame_strong):
    res = S.stratified_cliffs_delta(frame_strong.value, frame_strong.group, frame_strong.stratum, "A", "B",
                                    method="bca", n_boot=2000)
    assert res.method == "bca" and res.bca_fallback is False and res.ci_low < res.ci_high
    sep = S.stratified_cliffs_delta([1, 2, 3, 4, 5, 6], ["A"] * 3 + ["B"] * 3, ["s"] * 6, "A", "B",
                                    method="bca", n_boot=300)
    assert sep.delta == -1.0 and sep.method == "percentile" and sep.bca_fallback is True


def test_seed_reproducibility_and_overlap(frame):
    kw = dict(group_a="A", group_b="B", n_boot=2000)
    r1 = S.stratified_cliffs_delta(frame.value, frame.group, frame.stratum, **kw)
    r2 = S.stratified_cliffs_delta(frame.value, frame.group, frame.stratum, **kw)
    r3 = S.stratified_cliffs_delta(frame.value, frame.group, frame.stratum, seed=1, **kw)
    assert (r1.ci_low, r1.ci_high) == (r2.ci_low, r2.ci_high)
    assert max(r1.ci_low, r3.ci_low) <= min(r1.ci_high, r3.ci_high)  # overlapping intervals
    t1 = S.tost_stratified(frame.value, frame.group, frame.stratum, "A", "B", 0.5, n_boot=2000)
    t2 = S.tost_stratified(frame.value, frame.group, frame.stratum, "A", "B", 0.5, n_boot=2000)
    assert (t1.ci_low, t1.ci_high) == (t2.ci_low, t2.ci_high)


# --------------------------------------------------------------------------- #
# van Elteren and the binary analogue
# --------------------------------------------------------------------------- #
def test_one_stratum_van_elteren_equals_tie_corrected_mann_whitney():
    xa = np.array([1, 2, 2, 3, 4, 4, 4, 5.0])
    xb = np.array([2, 3, 3, 5, 6, 6, 7.0])
    values = np.concatenate([xa, xb])
    groups = ["A"] * xa.size + ["B"] * xb.size
    res = S.van_elteren(values, groups, ["only"] * values.size, "A", n_perm=0)
    mwu = sps.mannwhitneyu(xa, xb, method="asymptotic", use_continuity=False, alternative="two-sided")
    z_scipy = sps.norm.isf(mwu.pvalue / 2)
    assert abs(res.z) == pytest.approx(z_scipy, abs=1e-9)
    assert res.p_asymptotic == pytest.approx(mwu.pvalue, abs=1e-9)
    assert res.z < 0  # group A has the lower values
    assert math.isnan(res.p_permutation) and res.strata_used == ["only"] and res.strata_dropped == []
    # hand formula for T, E and V in a single stratum with weight 1/(n+1)
    n_a, n_b = xa.size, xb.size
    n = n_a + n_b
    ranks = sps.rankdata(values)
    _, ties = np.unique(values, return_counts=True)
    var_w = n_a * n_b / 12 * ((n + 1) - np.sum(ties ** 3 - ties) / (n * (n - 1)))
    assert res.statistic == pytest.approx(ranks[:n_a].sum() / (n + 1))
    assert res.expected == pytest.approx(n_a / 2)
    assert res.variance == pytest.approx(var_w / (n + 1) ** 2)


def test_van_elteren_permutation_close_to_asymptotic(frame):
    res = S.van_elteren(frame.value, frame.group, frame.stratum, "A", n_perm=20000)
    assert 0.001 < res.p_asymptotic < 0.999
    assert abs(res.p_permutation - res.p_asymptotic) < 0.02
    assert res.n_perm == 20000 and len(res.strata_used) == 5
    json.dumps(res.to_dict())


def test_van_elteren_drops_strata_and_handles_all_ties():
    values = [1, 2, 3, 4, 7, 7, 7, 8, 9, 9]
    groups = ["A", "A", "B", "B", "A", "A", "B", "B", "B", "B"]
    strata = ["s1"] * 4 + ["s2"] * 4 + ["s3"] * 2  # s3 has no group A
    res = S.van_elteren(values, groups, strata, "A", n_perm=500)
    assert res.strata_dropped == ["s3"] and res.strata_used == ["s1", "s2"]
    tied = S.van_elteren([5, 5, 5, 5], ["A", "A", "B", "B"], ["s"] * 4, "A", n_perm=200)
    assert math.isnan(tied.z) and tied.p_asymptotic == 1.0 and tied.p_permutation == 1.0


def test_stratified_binary_test_hand_example():
    success = [1, 1, 1, 0, 1, 0, 0, 0, 1, 1, 0, 0, 1, 1, 0, 0]
    groups = (["A"] * 4 + ["B"] * 4) * 2
    strata = ["s1"] * 8 + ["s2"] * 8
    res = S.stratified_binary_test(success, groups, strata, "A", n_perm=4000)
    assert res["statistic"] == 2.0                      # (3 - 1) + (2 - 2)
    assert res["mh_odds_ratio"] == pytest.approx(13 / 5)  # (9/8 + 4/8) / (1/8 + 4/8), no zero cell
    assert res["risk_difference_mean"] == pytest.approx(0.25)
    assert res["n_corrected_strata"] == 0 and res["strata_used"] == ["s1", "s2"]
    assert 0 < res["p_permutation"] <= 1
    zero = S.stratified_binary_test([1, 1, 0, 0], ["A", "A", "B", "B"], ["s"] * 4, "A", n_perm=100)
    assert zero["n_corrected_strata"] == 1 and zero["mh_odds_ratio"] == pytest.approx(2.5 * 2.5 / (0.5 * 0.5))


# --------------------------------------------------------------------------- #
# Holm, TOST
# --------------------------------------------------------------------------- #
def test_holm_textbook_example():
    out = S.holm({"h1": 0.01, "h2": 0.02, "h3": 0.03, "h4": 0.04}, alpha=0.05)
    assert list(out.columns) == ["name", "p", "rank", "p_holm", "reject"]
    assert list(out["p_holm"]) == pytest.approx([0.04, 0.06, 0.06, 0.06])
    assert list(out["reject"]) == [True, False, False, False]
    assert list(out["rank"]) == [1, 2, 3, 4]


def test_holm_nan_handling_and_order():
    out = S.holm({"b": 0.04, "a": float("nan"), "c": 0.001})
    assert list(out["name"]) == ["c", "b", "a"]
    assert list(out["p_holm"][:2]) == pytest.approx([0.002, 0.04])
    assert pd.isna(out.loc[2, "rank"]) and math.isnan(out.loc[2, "p_holm"]) and not out.loc[2, "reject"]
    empty = S.holm({"x": float("nan")})
    assert len(empty) == 1 and not empty["reject"].iloc[0]


def test_tost_identical_vs_shifted():
    rng = np.random.default_rng(5)
    rows = []
    for k in range(10):
        base = rng.normal(0, 1, 12)
        for grp in ("A", "B"):
            rows.extend((f"s{k}", grp, float(v)) for v in base)
    same = pd.DataFrame(rows, columns=["stratum", "group", "value"])
    res = S.tost_stratified(same.value, same.group, same.stratum, "A", "B", 0.5)
    assert res.estimate == 0.0 and res.equivalent is True and res.p_tost < 0.05
    assert res.smallest_equivalent_band == pytest.approx(max(abs(res.ci_low), abs(res.ci_high)))
    assert res.level == 0.90 and res.statistic == "median" and len(res.strata_used) == 10
    shifted = same.copy()
    shifted.loc[shifted.group == "B", "value"] += 3.0  # three standard deviations
    res2 = S.tost_stratified(shifted.value, shifted.group, shifted.stratum, "A", "B", 0.5)
    assert res2.estimate == pytest.approx(-3.0) and res2.equivalent is False and res2.p_tost == 1.0
    json.dumps(res2.to_dict())


# --------------------------------------------------------------------------- #
# pass@k, pass^k
# --------------------------------------------------------------------------- #
def test_pass_at_k_closed_forms():
    assert S.pass_at_k(6, 3, 1) == pytest.approx(0.5)
    assert S.pass_at_k(6, 3, 4) == pytest.approx(1.0)
    for k in range(1, 4):
        assert S.pass_hat_k(6, 3, k) == pytest.approx(math.comb(3, k) / math.comb(6, k))
    assert S.pass_hat_k(6, 3, 4) == 0.0
    assert math.isnan(S.pass_at_k(5, 2, 6)) and math.isnan(S.pass_hat_k(5, 2, 6))
    with pytest.raises(ValueError):
        S.pass_at_k(6, 7, 1)


def test_passk_curves_shape_and_monotonicity():
    cells = pd.DataFrame({"stratum": [f"s{i}" for i in range(10)] * 2,
                          "group": ["A"] * 10 + ["B"] * 10,
                          "n": 6,
                          "c": [0, 1, 2, 3, 4, 5, 6, 6, 3, 2] + [1, 2, 3, 4, 5, 6, 6, 6, 4, 3]})
    out = S.passk_curves(cells, ks=range(1, 7), n_boot=1000)
    assert list(out.columns) == ["group", "k", "estimator", "mean", "ci_low", "ci_high", "n_strata"]
    assert out.shape == (2 * 6 * 2, 7)
    for (_, est), sub in out.groupby(["group", "estimator"]):
        means = sub.sort_values("k")["mean"].to_numpy()
        if est == "pass_at_k":
            assert np.all(np.diff(means) >= -1e-12)
        else:
            assert np.all(np.diff(means) <= 1e-12)
    assert ((out["ci_low"] <= out["mean"] + 1e-12) & (out["mean"] <= out["ci_high"] + 1e-12)).all()
    assert out.loc[(out.group == "A") & (out.k == 1) & (out.estimator == "pass_at_k"), "mean"].iloc[0] == pytest.approx(32 / 60)
    with pytest.raises(ValueError):
        S.passk_curves(pd.concat([cells, cells.iloc[:1]]))


# --------------------------------------------------------------------------- #
# dispersion, heterogeneity, paired tests, descriptives, generic bootstrap
# --------------------------------------------------------------------------- #
def test_qcd_cv_summarize():
    assert S.qcd([1, 2, 3, 4, 5]) == pytest.approx((4 - 2) / (4 + 2))
    assert S.cv([1, 2, 3, 4, 5]) == pytest.approx(np.std([1, 2, 3, 4, 5], ddof=1) / 3)
    assert math.isnan(S.cv([0, 0, 0])) and math.isnan(S.qcd([7.0])) and math.isnan(S.cv([-1, 1]))
    summ = S.summarize([1, 2, 3, np.nan, 4])
    assert summ == {"n": 4, "mean": 2.5, "sd": pytest.approx(np.std([1, 2, 3, 4], ddof=1)), "median": 2.5,
                    "q1": 1.75, "q3": 3.25, "min": 1.0, "max": 4.0, "n_missing": 1}
    assert S.summarize([])["n"] == 0


def test_heterogeneity_identical_and_divergent():
    same = S.heterogeneity([0.3] * 5, [0.02] * 5)
    assert same["i2"] == 0.0 and same["Q"] == pytest.approx(0.0) and same["tau2"] == 0.0
    assert same["pooled_fixed"] == pytest.approx(0.3) and same["k"] == 5 and same["df"] == 4
    div = S.heterogeneity([-0.9, 0.9, -0.8, 0.85, float("nan")], [0.02, 0.02, 0.02, 0.02, 0.02])
    assert div["k"] == 4 and div["i2"] > 0.5 and div["tau2"] > 0 and div["p_q"] < 0.001
    floored = S.heterogeneity([1.0, 0.2], [0.0, 0.03])
    assert floored["n_floored"] == 1 and np.isfinite(floored["Q"])
    assert S.heterogeneity([0.1], [0.01])["i2"] == 0.0


def test_paired_stratum_tests_known_example():
    a = {f"s{i}": float(i) for i in range(10)}
    b = {k: v - 1.0 for k, v in a.items()}
    b["s0"] = a["s0"] + 1.0  # one stratum goes the other way
    res = S.paired_stratum_tests(a, b)
    assert res["n"] == 10 and res["n_nonzero"] == 10
    assert (res["sign_k_pos"], res["sign_k_neg"], res["sign_ties"]) == (9, 1, 0)
    assert res["sign_p"] == pytest.approx(22 / 1024)  # 2 * P(X >= 9 | n = 10, p = 1/2) = 0.0215
    assert res["direction"] == "9 of 10 strata favour A"
    assert 0 < res["wilcoxon_p"] < 0.05
    flipped = S.paired_stratum_tests(a, b, label_a="Kimi", label_b="Qwen", higher_is_better=False)
    assert flipped["direction"] == "9 of 10 strata favour Qwen"
    none = S.paired_stratum_tests({"x": 1.0, "y": 2.0}, {"x": 1.0, "y": 2.0})
    assert none["n_nonzero"] == 0 and none["sign_p"] == 1.0 and none["wilcoxon_p"] == 1.0


def test_bootstrap_within_cells_shape_and_seed(frame):
    def stat(f: pd.DataFrame) -> float:
        means = f.groupby("group")["value"].mean()
        return means["A"] - means["B"]

    b1 = S.bootstrap_within_cells(frame, "value", ["stratum", "group"], stat, n_boot=100, seed=2)
    b2 = S.bootstrap_within_cells(frame, "value", ["stratum", "group"], stat, n_boot=100, seed=2)
    assert b1.shape == (100,) and np.array_equal(b1, b2)
    vec = S.bootstrap_within_cells(frame, "value", "stratum", lambda f: f.groupby("group")["value"].mean().to_numpy(),
                                   n_boot=20, seed=2)
    assert vec.shape == (20, 2)
    # within-cell resampling keeps every cell at its size
    sizes = S.bootstrap_within_cells(frame, "value", ["stratum", "group"],
                                     lambda f: f.groupby(["stratum", "group"]).size().to_numpy(), n_boot=3, seed=0)
    assert (sizes == 20).all()


# --------------------------------------------------------------------------- #
# Binary outcomes go through the van Elteren engine (STATUS.md decision D14)
# --------------------------------------------------------------------------- #
def _cmh_z(values, groups, strata, group_a):
    """Cochran-Mantel-Haenszel z without continuity correction, by hand, for the two tests below."""
    import numpy as _np
    values, groups, strata = (_np.asarray(a) for a in (values, groups, strata))
    num = var = 0.0
    for k in dict.fromkeys(strata.tolist()):
        m = strata == k
        a, b = values[m & (groups == group_a)].astype(float), values[m & (groups != group_a)].astype(float)
        n_a, n_b = a.size, b.size
        n, s = n_a + n_b, a.sum() + b.sum()
        num += a.sum() - n_a * s / n
        var += n_a * n_b * s * (n - s) / (n * n * (n - 1))
    return num / var ** 0.5


def test_van_elteren_on_binary_equals_success_count_test_when_strata_are_balanced():
    import numpy as _np
    rng = _np.random.default_rng(7)
    strata = _np.repeat([f"s{k}" for k in range(5)], 12)
    groups = _np.tile(["A"] * 6 + ["B"] * 6, 5)
    values = rng.integers(0, 2, size=60).astype(float)
    ve = S.van_elteren(values, groups, strata, "A", n_perm=3000, seed=11)
    bt = S.stratified_binary_test(values > 0.5, groups, strata, "A", n_perm=3000, seed=11)
    # Same seed, stratum order and sizes give identical permutations; equal strata make the two statistics
    # affine transforms of each other, so the permutation p-values agree to the last digit.
    assert ve.p_permutation == pytest.approx(bt["p_permutation"], abs=1e-12)
    # and the tie-corrected rank variance equals the hypergeometric one: z is the CMH z without correction
    assert ve.z == pytest.approx(_cmh_z(values, groups, strata, "A"), abs=1e-9)


def test_van_elteren_on_binary_differs_from_success_count_test_when_strata_are_unequal():
    # s1: 12 runs, A 5/6 vs B 1/6; s0: 4 runs, A 2/2 vs B 0/2. Story weights N_k/(N_k+1) now differ
    # (12/13 vs 4/5), so van Elteren is no longer an affine transform of the equal-weight success-count test.
    values = [1, 1, 1, 1, 1, 0, 1, 0, 0, 0, 0, 0, 1, 1, 0, 0]
    groups = ["A"] * 6 + ["B"] * 6 + ["A", "A", "B", "B"]
    strata = ["s1"] * 12 + ["s0"] * 4
    ve = S.van_elteren(values, groups, strata, "A", n_perm=0)
    cmh = _cmh_z(values, groups, strata, "A")
    assert ve.z == pytest.approx(2.7732, abs=1e-3)
    assert cmh == pytest.approx(2.7957, abs=1e-3)
    assert abs(ve.z - cmh) > 1e-3
