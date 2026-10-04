"""Pure, seeded estimators for the wave-1 technical report (numpy / scipy / pandas only).

All functions are side-effect free. They accept numpy arrays, pandas Series or
plain sequences; the stratified functions take three parallel vectors
``(values, groups, strata)`` or the equivalent columns of a tidy frame. Every
stochastic function takes ``seed`` (default 20261003) and draws exclusively from
``numpy.random.default_rng(seed)``, so equal inputs and seeds give identical
output. Results are dataclasses with ``to_dict()`` (JSON friendly) or plain dicts.

Formulas and sources
--------------------
van Elteren stratified rank-sum test (van Elteren 1960):
    T = sum_k W_k / (n_k + 1), with W_k the mid-rank sum of group A in stratum k,
    E[T] = sum_k n_Ak (n_k + 1) / 2 / (n_k + 1) = sum_k n_Ak / 2,
    Var(W_k) = n_Ak n_Bk / 12 * [ (n_k + 1) - sum_j (t_j^3 - t_j) / (n_k (n_k - 1)) ]
    (tie correction as in Lehmann 1975, Nonparametrics, ch. 1), Var(T) = sum_k
    Var(W_k) / (n_k + 1)^2, z = (T - E) / sqrt(Var(T)), two-sided normal p.
    The weights 1 / (n_k + 1) are the locally most powerful choice for a
    location shift (van Elteren 1960). A within-stratum label permutation gives
    the Monte Carlo p (1 + #{|T* - E| >= |T - E|}) / (n_perm + 1).
Cliff's delta (Cliff 1993, Psychological Bulletin 114, 494-509):
    delta = [#(x_i > y_j) - #(x_i < y_j)] / (n_x n_y); ties count 0. Unbiased
    variance estimate (Cliff 1993 eq. 7; Cliff 1996 p. 138):
    s_d^2 = [ n_y^2 sum_i (d_i. - d)^2 + n_x^2 sum_j (d_.j - d)^2
              - sum_ij (d_ij - d)^2 ] / [ n_x n_y (n_x - 1)(n_y - 1) ].
    A12 = (delta + 1) / 2 (Vargha and Delaney 2000). Magnitude thresholds
    |delta| < 0.147 negligible, < 0.33 small, < 0.474 medium, else large
    (Romano, Kromrey, Coraggio and Skowronek 2006).
Stratified bootstrap: values are resampled with replacement within each
    (stratum, group) cell; percentile interval by default, BCa (Efron 1987)
    optional with a jackknife acceleration estimate.
Holm step-down (Holm 1979): p_holm(i) = max_{j <= i} min(1, (m - j + 1) p_(j)).
TOST (Schuirmann 1987): equivalence when the (1 - 2 alpha) interval lies inside
    (-band, +band); here the interval is a stratified bootstrap interval and the
    TOST p is the larger of the two one-sided bootstrap tail probabilities.
pass@k (Chen et al. 2021): 1 - C(n - c, k) / C(n, k); pass^k (Yao et al. 2024,
    tau-bench): C(c, k) / C(n, k); exact integer arithmetic via math.comb.
Heterogeneity: Cochran's Q = sum w_k (d_k - d_fixed)^2 with w_k = 1 / v_k,
    I^2 = max(0, (Q - df) / Q), DerSimonian and Laird (1986)
    tau^2 = max(0, (Q - df) / (sum w - sum w^2 / sum w)).
Paired stratum tests: exact Wilcoxon signed-rank (scipy, zero_method="wilcox")
    and the exact two-sided sign test (binomial, p = 1/2).
"""
from __future__ import annotations

import math
from collections.abc import Callable, Iterable, Mapping, Sequence
from dataclasses import asdict, dataclass, field
from typing import Any

import numpy as np
import pandas as pd
from scipy import stats as _sps

__all__ = [
    "DEFAULT_SEED",
    "EffectResult",
    "RankTestResult",
    "TostResult",
    "bootstrap_within_cells",
    "cliff_variance",
    "cliffs_delta",
    "cv",
    "heterogeneity",
    "holm",
    "magnitude_label",
    "paired_stratum_tests",
    "pass_at_k",
    "pass_hat_k",
    "passk_curves",
    "percentile_ci",
    "qcd",
    "stratified_binary_test",
    "stratified_cliffs_delta",
    "summarize",
    "tost_stratified",
    "van_elteren",
]

DEFAULT_SEED = 20261003

# Romano et al. (2006) thresholds on |delta|.
MAGNITUDE_THRESHOLDS = ((0.147, "negligible"), (0.33, "small"), (0.474, "medium"))


# --------------------------------------------------------------------------- #
# small helpers
# --------------------------------------------------------------------------- #
def _float_array(values: Any) -> np.ndarray:
    """Return a 1-D float64 array with non-finite entries removed."""
    arr = np.asarray(pd.Series(values, dtype="float64").to_numpy(), dtype="float64").ravel()
    return arr[np.isfinite(arr)]


def _object_array(values: Any) -> np.ndarray:
    """Return a 1-D object array (labels keep their identity, no dtype coercion)."""
    return np.asarray(pd.Series(values, dtype="object").to_numpy(), dtype="object").ravel()


def _ordered_unique(labels: Iterable[Any]) -> list:
    """Sorted distinct labels; falls back to string order for mixed types."""
    distinct = list(dict.fromkeys(labels))
    try:
        return sorted(distinct)
    except TypeError:
        return sorted(distinct, key=str)


def _py(value: Any) -> Any:
    """Convert numpy scalars / arrays nested in dicts and lists to plain Python."""
    if isinstance(value, dict):
        return {str(k): _py(v) for k, v in value.items()}
    if isinstance(value, (list, tuple)):
        return [_py(v) for v in value]
    if isinstance(value, np.ndarray):
        return [_py(v) for v in value.tolist()]
    if isinstance(value, np.generic):
        return value.item()
    return value


def _check_level(level: float) -> None:
    if not (0.0 < level < 1.0):
        raise ValueError(f"level must lie in (0, 1), got {level!r}")


def percentile_ci(samples: np.ndarray, level: float = 0.95) -> tuple[float, float]:
    """Percentile interval of a bootstrap distribution (nan, nan when empty)."""
    _check_level(level)
    arr = np.asarray(samples, dtype="float64")
    arr = arr[np.isfinite(arr)]
    if arr.size == 0:
        return (math.nan, math.nan)
    alpha = (1.0 - level) / 2.0
    lo, hi = np.quantile(arr, [alpha, 1.0 - alpha])
    return (float(lo), float(hi))


def _split_cells(values: Any, groups: Any, strata: Any) -> tuple[dict, list]:
    """Return ({stratum: {group: float array}}, ordered strata); drops non-finite values."""
    vals = np.asarray(pd.Series(values, dtype="float64").to_numpy(), dtype="float64").ravel()
    grp = _object_array(groups)
    strat = _object_array(strata)
    if not (vals.shape == grp.shape == strat.shape):
        raise ValueError("values, groups and strata must have the same length")
    keep = np.isfinite(vals)
    vals, grp, strat = vals[keep], grp[keep], strat[keep]
    cells: dict = {}
    for k in _ordered_unique(strat):
        mask_k = strat == k
        cells[k] = {g: vals[mask_k & (grp == g)] for g in _ordered_unique(grp[mask_k])}
    return cells, list(cells)


# --------------------------------------------------------------------------- #
# Cliff's delta
# --------------------------------------------------------------------------- #
def magnitude_label(delta: float) -> str:
    """Romano et al. (2006) label for |delta|: negligible, small, medium or large.

    A non-finite delta returns "undefined" (it cannot be classified).
    """
    if delta is None or not np.isfinite(delta):
        return "undefined"
    size = abs(float(delta))
    for threshold, label in MAGNITUDE_THRESHOLDS:
        if size < threshold:
            return label
    return "large"


def cliffs_delta(x: Any, y: Any) -> float:
    """Cliff's delta of x over y: P(x > y) - P(x < y), ties count zero; nan if a side is empty."""
    xa, ya = _float_array(x), _float_array(y)
    if xa.size == 0 or ya.size == 0:
        return math.nan
    ys = np.sort(ya)
    greater = np.searchsorted(ys, xa, side="left").sum()            # #(x_i > y_j)
    less = (ya.size - np.searchsorted(ys, xa, side="right")).sum()  # #(x_i < y_j)
    return float((int(greater) - int(less)) / (xa.size * ya.size))


def _dominance_matrix(x: np.ndarray, y: np.ndarray) -> np.ndarray:
    """sign(x_i - y_j) as an (n_x, n_y) float matrix."""
    return np.sign(x[:, None] - y[None, :]).astype("float64")


def cliff_variance(x: Any, y: Any) -> float:
    """Cliff (1993) unbiased estimate of Var(delta); nan if either side has fewer than 2 values.

    The estimate is exactly 0 under complete separation (|delta| = 1); callers
    that need a positive variance (for example inverse-variance weights) must
    floor it, see ``heterogeneity``.
    """
    xa, ya = _float_array(x), _float_array(y)
    n, m = xa.size, ya.size
    if n < 2 or m < 2:
        return math.nan
    d_ij = _dominance_matrix(xa, ya)
    d = d_ij.mean()
    d_i = d_ij.mean(axis=1)
    d_j = d_ij.mean(axis=0)
    num = (m * m * np.sum((d_i - d) ** 2) + n * n * np.sum((d_j - d) ** 2)
           - np.sum((d_ij - d) ** 2))
    return float(max(num / (n * m * (n - 1) * (m - 1)), 0.0))


def _cliff_rows(xa: np.ndarray, xb: np.ndarray) -> np.ndarray:
    """Cliff's delta for each row of xa (B, n_a) against the same row of xb (B, n_b).

    Uses the identity delta = 2 U / (n_a n_b) - 1 with U the mid-rank
    Mann-Whitney statistic, which equals the sign-count definition.
    """
    n_a, n_b = xa.shape[1], xb.shape[1]
    ranks = _sps.rankdata(np.concatenate([xa, xb], axis=1), axis=1)
    w_a = ranks[:, :n_a].sum(axis=1)
    u_a = w_a - n_a * (n_a + 1) / 2.0
    return 2.0 * u_a / (n_a * n_b) - 1.0


def _resample_rows(rng: np.random.Generator, x: np.ndarray, n_boot: int) -> np.ndarray:
    """(n_boot, len(x)) matrix of within-cell resamples with replacement."""
    idx = rng.integers(0, x.size, size=(n_boot, x.size))
    return x[idx]


def _pool(deltas: np.ndarray, weights: np.ndarray, axis: int | None = None) -> np.ndarray | float:
    return np.sum(deltas * weights, axis=axis) / np.sum(weights, axis=axis)


# --------------------------------------------------------------------------- #
# stratified Cliff's delta with bootstrap interval
# --------------------------------------------------------------------------- #
@dataclass
class EffectResult:
    """Pooled Cliff's delta over strata with a stratified bootstrap interval."""

    delta: float
    a12: float
    ci_low: float
    ci_high: float
    level: float
    n_a: int
    n_b: int
    n_boot: int
    seed: int
    method: str
    magnitude: str
    per_stratum: dict[str, dict] = field(default_factory=dict)
    pooling: str = "mean"
    bca_fallback: bool = False

    def to_dict(self) -> dict:
        """Plain-Python dict (JSON friendly)."""
        return _py(asdict(self))


def _bca_interval(theta_hat: float, boot: np.ndarray, jack: np.ndarray,
                  level: float) -> tuple[float, float] | None:
    """BCa interval (Efron 1987); None when bias or acceleration is degenerate."""
    boot = boot[np.isfinite(boot)]
    jack = jack[np.isfinite(jack)]
    if boot.size == 0 or jack.size < 3:
        return None
    prop = np.mean(boot < theta_hat)
    if prop <= 0.0 or prop >= 1.0:
        return None
    z0 = _sps.norm.ppf(prop)
    centred = jack.mean() - jack
    denom = 6.0 * np.sum(centred ** 2) ** 1.5
    if not np.isfinite(denom) or denom <= 0.0:
        return None
    accel = np.sum(centred ** 3) / denom
    alpha = (1.0 - level) / 2.0
    z_lo, z_hi = _sps.norm.ppf(alpha), _sps.norm.ppf(1.0 - alpha)
    adj = []
    for z in (z_lo, z_hi):
        scale = 1.0 - accel * (z0 + z)
        if scale <= 0.0:
            return None
        adj.append(_sps.norm.cdf(z0 + (z0 + z) / scale))
    a_lo, a_hi = adj
    if not (0.0 < a_lo < a_hi < 1.0):
        return None
    lo, hi = np.quantile(boot, [a_lo, a_hi])
    return (float(lo), float(hi))


def stratified_cliffs_delta(values: Any, groups: Any, strata: Any, group_a: Any, group_b: Any, *,
                            n_boot: int = 5000, level: float = 0.95, seed: int = DEFAULT_SEED,
                            method: str = "percentile", pooling: str = "mean") -> EffectResult:
    """Cliff's delta of group_a over group_b per stratum, pooled, with a stratified bootstrap CI.

    pooling="mean" takes the unweighted mean over strata (balanced design);
    pooling="pairs" weights each stratum by n_a * n_b. The interval resamples
    values with replacement within every (stratum, group) cell. method="bca"
    uses a jackknife acceleration and falls back to the percentile interval
    (``bca_fallback=True``) when the bias or acceleration term is degenerate.
    Strata that miss one of the two groups are skipped and listed in
    ``per_stratum`` with ``"skipped": True``.
    """
    _check_level(level)
    if method not in {"percentile", "bca"}:
        raise ValueError("method must be 'percentile' or 'bca'")
    if pooling not in {"mean", "pairs"}:
        raise ValueError("pooling must be 'mean' or 'pairs'")
    if n_boot < 0:
        raise ValueError("n_boot must be non-negative")
    cells, order = _split_cells(values, groups, strata)
    per_stratum: dict[str, dict] = {}
    used: list = []
    for k in order:
        xa = cells[k].get(group_a, np.empty(0))
        xb = cells[k].get(group_b, np.empty(0))
        if xa.size == 0 or xb.size == 0:
            per_stratum[str(k)] = {"delta": math.nan, "var": math.nan,
                                   "n_a": int(xa.size), "n_b": int(xb.size), "skipped": True}
            continue
        per_stratum[str(k)] = {"delta": cliffs_delta(xa, xb), "var": cliff_variance(xa, xb),
                               "n_a": int(xa.size), "n_b": int(xb.size), "skipped": False}
        used.append(k)

    def _weights(n_a: np.ndarray, n_b: np.ndarray) -> np.ndarray:
        return n_a * n_b if pooling == "pairs" else np.ones_like(n_a, dtype="float64")

    if not used:
        return EffectResult(math.nan, math.nan, math.nan, math.nan, level, 0, 0, n_boot, seed,
                            method, "undefined", per_stratum, pooling, False)

    n_a = np.array([cells[k][group_a].size for k in used], dtype="float64")
    n_b = np.array([cells[k][group_b].size for k in used], dtype="float64")
    deltas = np.array([per_stratum[str(k)]["delta"] for k in used])
    weights = _weights(n_a, n_b)
    delta = float(_pool(deltas, weights))

    ci_low = ci_high = math.nan
    bca_fallback = False
    used_method = method
    if n_boot > 0:
        rng = np.random.default_rng(seed)
        boot = np.zeros((n_boot, len(used)))
        for j, k in enumerate(used):
            boot[:, j] = _cliff_rows(_resample_rows(rng, cells[k][group_a], n_boot),
                                     _resample_rows(rng, cells[k][group_b], n_boot))
        pooled = _pool(boot, weights[None, :], axis=1)
        interval = None
        if method == "bca":
            interval = _bca_interval(delta, pooled, _jackknife_pooled(cells, used, group_a, group_b,
                                                                       deltas, pooling), level)
            if interval is None:
                bca_fallback, used_method = True, "percentile"
        if interval is None:
            interval = percentile_ci(pooled, level)
        ci_low, ci_high = interval

    return EffectResult(delta=delta, a12=(delta + 1.0) / 2.0, ci_low=ci_low, ci_high=ci_high,
                        level=level, n_a=int(n_a.sum()), n_b=int(n_b.sum()), n_boot=n_boot,
                        seed=seed, method=used_method, magnitude=magnitude_label(delta),
                        per_stratum=per_stratum, pooling=pooling, bca_fallback=bca_fallback)


def _jackknife_pooled(cells: dict, used: list, group_a: Any, group_b: Any, deltas: np.ndarray,
                      pooling: str) -> np.ndarray:
    """Leave-one-observation-out values of the pooled delta (for the BCa acceleration)."""
    n_a = np.array([cells[k][group_a].size for k in used], dtype="float64")
    n_b = np.array([cells[k][group_b].size for k in used], dtype="float64")
    out: list[float] = []
    for j, k in enumerate(used):
        xa, xb = cells[k][group_a], cells[k][group_b]
        if xa.size < 2 or xb.size < 2:
            return np.array([])  # degenerate: a stratum would lose a group
        dom = _dominance_matrix(xa, xb)
        total = dom.sum()
        for row_sum in dom.sum(axis=1):          # leave out one value of group A
            d_loo = deltas.copy()
            d_loo[j] = (total - row_sum) / ((xa.size - 1) * xb.size)
            na_loo = n_a.copy()
            na_loo[j] -= 1
            w = na_loo * n_b if pooling == "pairs" else np.ones_like(n_a)
            out.append(float(_pool(d_loo, w)))
        for col_sum in dom.sum(axis=0):          # leave out one value of group B
            d_loo = deltas.copy()
            d_loo[j] = (total - col_sum) / (xa.size * (xb.size - 1))
            nb_loo = n_b.copy()
            nb_loo[j] -= 1
            w = n_a * nb_loo if pooling == "pairs" else np.ones_like(n_a)
            out.append(float(_pool(d_loo, w)))
    return np.array(out)


def bootstrap_within_cells(frame: pd.DataFrame, value_col: str, cell_cols: Sequence[str] | str,
                           stat_fn: Callable[[pd.DataFrame], Any], n_boot: int = 5000,
                           seed: int = DEFAULT_SEED) -> np.ndarray:
    """Generic stratified bootstrap: resample rows with replacement within each cell.

    Cells are the distinct combinations of ``cell_cols``; every replicate keeps
    each cell at its original size. ``stat_fn`` receives the resampled frame
    (same columns, fresh RangeIndex) and returns a scalar or a 1-D array; the
    result has shape (n_boot,) or (n_boot, d). ``value_col`` must exist and is
    passed through untouched (it is named so callers document what they
    resample). Rows with a missing ``value_col`` are dropped first.
    """
    if isinstance(cell_cols, str):
        cell_cols = [cell_cols]
    if value_col not in frame.columns:
        raise KeyError(f"value_col {value_col!r} not in frame")
    missing = [c for c in cell_cols if c not in frame.columns]
    if missing:
        raise KeyError(f"cell columns missing from frame: {missing}")
    base = frame.loc[frame[value_col].notna()].reset_index(drop=True)
    positions = [np.asarray(idx, dtype="int64")
                 for _, idx in sorted(base.groupby(list(cell_cols), sort=True, observed=True)
                                      .indices.items(), key=lambda kv: str(kv[0]))]
    rng = np.random.default_rng(seed)
    results: list[np.ndarray] = []
    for _ in range(int(n_boot)):
        take = np.concatenate([pos[rng.integers(0, pos.size, size=pos.size)] for pos in positions]) \
            if positions else np.empty(0, dtype="int64")
        sample = base.take(take).reset_index(drop=True)
        results.append(np.asarray(stat_fn(sample), dtype="float64"))
    if not results:
        return np.empty((0,))
    return np.vstack(results) if results[0].ndim else np.array(results, dtype="float64")


# --------------------------------------------------------------------------- #
# van Elteren stratified rank-sum test and the binary analogue
# --------------------------------------------------------------------------- #
@dataclass
class RankTestResult:
    """van Elteren statistic with asymptotic and permutation p-values."""

    statistic: float
    expected: float
    variance: float
    z: float
    p_asymptotic: float
    p_permutation: float
    n_perm: int
    strata_used: list
    strata_dropped: list
    seed: int

    def to_dict(self) -> dict:
        """Plain-Python dict (JSON friendly)."""
        return _py(asdict(self))


def _stratum_arrays(values: Any, groups: Any, strata: Any, group_a: Any):
    """Per stratum (A values, B values) where B is everything that is not group_a."""
    vals = np.asarray(pd.Series(values, dtype="float64").to_numpy(), dtype="float64").ravel()
    grp = _object_array(groups)
    strat = _object_array(strata)
    if not (vals.shape == grp.shape == strat.shape):
        raise ValueError("values, groups and strata must have the same length")
    keep = np.isfinite(vals)
    vals, grp, strat = vals[keep], grp[keep], strat[keep]
    out: dict = {}
    for k in _ordered_unique(strat):
        mask_k = strat == k
        is_a = grp == group_a
        out[k] = (vals[mask_k & is_a], vals[mask_k & ~is_a])
    return out


def _permute_weighted_sums(rng: np.random.Generator, per_stratum: Sequence[tuple[np.ndarray, int, float]],
                           n_perm: int) -> np.ndarray:
    """Monte Carlo null distribution of sum_k w_k * (sum of n_Ak values drawn without replacement).

    ``per_stratum`` holds (scores, n_Ak, w_k). Labels are shuffled independently
    within every stratum, which is the exact permutation null of a stratified design.
    """
    total = np.zeros(n_perm)
    for scores, n_a, weight in per_stratum:
        shuffled = rng.permuted(np.broadcast_to(scores, (n_perm, scores.size)).copy(), axis=1)
        total += weight * shuffled[:, :n_a].sum(axis=1)
    return total


def _mc_pvalue(null: np.ndarray, observed: float, centre: float) -> float:
    """(1 + #{|T* - E| >= |T - E|}) / (n_perm + 1) with a small float tolerance."""
    dev = abs(observed - centre)
    hits = np.count_nonzero(np.abs(null - centre) >= dev - 1e-9)
    return float((1 + hits) / (null.size + 1))


def van_elteren(values: Any, groups: Any, strata: Any, group_a: Any, *,
                n_perm: int = 20000, seed: int = DEFAULT_SEED) -> RankTestResult:
    """van Elteren (1960) stratified rank-sum test of group_a against the rest.

    T = sum_k W_k / (n_k + 1) with W_k the mid-rank sum of group_a in stratum k;
    E = sum_k n_Ak / 2; tie-corrected Var(W_k) per Lehmann (1975); z = (T - E) /
    sqrt(V); two-sided asymptotic p; Monte Carlo permutation p by shuffling
    labels within strata (``n_perm=0`` skips it, p_permutation = nan). Strata
    with an empty group are dropped and listed. A degenerate variance (all
    values tied) gives z = nan and p = 1.
    """
    if n_perm < 0:
        raise ValueError("n_perm must be non-negative")
    per = _stratum_arrays(values, groups, strata, group_a)
    used, dropped, parts = [], [], []
    statistic = expected = variance = 0.0
    for k, (xa, xb) in per.items():
        n_a, n_b = xa.size, xb.size
        n_k = n_a + n_b
        if n_a == 0 or n_b == 0:
            dropped.append(k)
            continue
        used.append(k)
        pooled = np.concatenate([xa, xb])
        ranks = _sps.rankdata(pooled)
        w_k = ranks[:n_a].sum()
        _, ties = np.unique(pooled, return_counts=True)
        tie_term = np.sum(ties.astype("float64") ** 3 - ties) / (n_k * (n_k - 1)) if n_k > 1 else 0.0
        var_w = n_a * n_b / 12.0 * ((n_k + 1) - tie_term)
        weight = 1.0 / (n_k + 1)
        statistic += weight * w_k
        expected += weight * n_a * (n_k + 1) / 2.0
        variance += weight * weight * var_w
        parts.append((ranks, n_a, weight))
    if not used:
        return RankTestResult(math.nan, math.nan, math.nan, math.nan, math.nan, math.nan,
                              n_perm, [], dropped, seed)
    if variance > 1e-12:
        z = (statistic - expected) / math.sqrt(variance)
        p_asym = float(2.0 * _sps.norm.sf(abs(z)))
    else:
        variance, z, p_asym = 0.0, math.nan, 1.0
    p_perm = math.nan
    if n_perm > 0:
        if variance == 0.0:
            p_perm = 1.0
        else:
            null = _permute_weighted_sums(np.random.default_rng(seed), parts, n_perm)
            p_perm = _mc_pvalue(null, statistic, expected)
    return RankTestResult(float(statistic), float(expected), float(variance), float(z), p_asym,
                          p_perm, n_perm, used, dropped, seed)


def stratified_binary_test(success: Any, groups: Any, strata: Any, group_a: Any, *,
                           n_perm: int = 20000, seed: int = DEFAULT_SEED) -> dict:
    """Stratified permutation test for a binary outcome (group_a against the rest).

    statistic = sum over strata of (successes_A - successes_B); its permutation
    p shuffles labels within strata and centres on the permutation expectation
    sum_k s_k (n_Ak - n_Bk) / n_k (zero for balanced cells). Also returns the
    Mantel-Haenszel odds ratio (0.5 added to the four cells of a stratum only
    when that stratum has a zero cell) and the mean over strata of the risk
    difference p_A - p_B. Strata with an empty group are dropped.
    """
    if n_perm < 0:
        raise ValueError("n_perm must be non-negative")
    succ = pd.Series(success)
    if succ.dtype == bool or str(succ.dtype) == "boolean":
        succ = succ.astype("float64")
    per = _stratum_arrays(succ, groups, strata, group_a)
    used, dropped, parts = [], [], []
    statistic = expected = 0.0
    mh_num = mh_den = 0.0
    risk_diffs: list[float] = []
    n_corrected = 0
    for k, (xa, xb) in per.items():
        if xa.size == 0 or xb.size == 0:
            dropped.append(k)
            continue
        if not (np.isin(xa, (0.0, 1.0)).all() and np.isin(xb, (0.0, 1.0)).all()):
            raise ValueError("success must be binary (0/1 or bool)")
        used.append(k)
        n_a, n_b = xa.size, xb.size
        n_k = n_a + n_b
        s_a, s_b = xa.sum(), xb.sum()
        s_k = s_a + s_b
        statistic += s_a - s_b
        expected += s_k * (n_a - n_b) / n_k
        # statistic contribution = 2 * s_A - s_k, so permute with weight 2 (constant drops out)
        parts.append((np.concatenate([xa, xb]), n_a, 2.0))
        a, b, c, d = s_a, n_a - s_a, s_b, n_b - s_b
        if min(a, b, c, d) == 0:  # continuity correction only for tables with a zero cell
            a, b, c, d = a + 0.5, b + 0.5, c + 0.5, d + 0.5
            n_corrected += 1
        table_total = a + b + c + d
        mh_num += a * d / table_total
        mh_den += b * c / table_total
        risk_diffs.append(float(s_a / n_a - s_b / n_b))
    result = {"statistic": float(statistic), "expected": float(expected), "p_permutation": math.nan,
              "mh_odds_ratio": math.nan, "risk_difference_mean": math.nan, "n_perm": n_perm,
              "strata_used": used, "strata_dropped": dropped, "n_corrected_strata": n_corrected,
              "seed": seed}
    if not used:
        return result
    result["mh_odds_ratio"] = float(mh_num / mh_den) if mh_den > 0 else math.inf
    result["risk_difference_mean"] = float(np.mean(risk_diffs))
    if n_perm > 0:
        null = _permute_weighted_sums(np.random.default_rng(seed), parts, n_perm)
        # null holds 2 * sum_k s_Ak*; shift by the constant sum_k s_k to get sum_k (s_Ak* - s_Bk*)
        null -= sum(xa.sum() + xb.sum() for xa, xb in (per[k] for k in used))
        result["p_permutation"] = _mc_pvalue(null, statistic, expected)
    return result


# --------------------------------------------------------------------------- #
# multiplicity, equivalence
# --------------------------------------------------------------------------- #
def holm(pvalues: Mapping[str, float], alpha: float = 0.05) -> pd.DataFrame:
    """Holm (1979) step-down adjustment; returns name, p, rank, p_holm, reject sorted by p.

    NaN p-values are excluded from m, placed last with rank <NA>, p_holm nan
    and reject False. Ties in p are ordered by name for determinism.
    """
    names = list(pvalues)
    raw = np.array([float(pvalues[n]) if pvalues[n] is not None else math.nan for n in names])
    frame = pd.DataFrame({"name": names, "p": raw})
    valid = frame[np.isfinite(frame["p"])].sort_values(["p", "name"], kind="stable").reset_index(drop=True)
    invalid = frame[~np.isfinite(frame["p"])].sort_values("name", kind="stable").reset_index(drop=True)
    m = len(valid)
    if m:
        ranks = np.arange(1, m + 1)
        stepped = np.minimum(1.0, (m - ranks + 1) * valid["p"].to_numpy())
        valid["rank"] = ranks
        valid["p_holm"] = np.maximum.accumulate(stepped)
        valid["reject"] = valid["p_holm"] <= alpha
    else:
        valid["rank"], valid["p_holm"], valid["reject"] = [], [], []
    invalid["rank"] = pd.array([pd.NA] * len(invalid), dtype="Int64")
    invalid["p_holm"] = math.nan
    invalid["reject"] = False
    out = pd.concat([valid, invalid], ignore_index=True)
    out["rank"] = out["rank"].astype("Int64")
    out["p_holm"] = out["p_holm"].astype("float64")
    out["reject"] = out["reject"].astype(bool)
    return out[["name", "p", "rank", "p_holm", "reject"]]


@dataclass
class TostResult:
    """Bootstrap TOST (Schuirmann 1987) for a stratified difference of medians or means."""

    estimate: float
    ci_low: float
    ci_high: float
    level: float
    band: float
    equivalent: bool
    p_tost: float
    smallest_equivalent_band: float
    statistic: str
    n_boot: int
    seed: int
    strata_used: list = field(default_factory=list)
    strata_skipped: list = field(default_factory=list)

    def to_dict(self) -> dict:
        """Plain-Python dict (JSON friendly)."""
        return _py(asdict(self))


_STATS = {"median": np.median, "mean": np.mean}


def tost_stratified(values: Any, groups: Any, strata: Any, group_a: Any, group_b: Any, band: float, *,
                    statistic: str = "median", n_boot: int = 5000, level: float = 0.90,
                    seed: int = DEFAULT_SEED) -> TostResult:
    """Equivalence of group_a and group_b within +/- band on the mean over strata of stat_A - stat_B.

    The interval is a stratified percentile bootstrap (default 90 %, which is
    the TOST convention for alpha = 0.05); equivalent when ci_low > -band and
    ci_high < band. p_tost = max(P*(est* <= -band), P*(est* >= band)) over the
    bootstrap distribution; smallest_equivalent_band = max(|ci_low|, |ci_high|).
    """
    _check_level(level)
    if statistic not in _STATS:
        raise ValueError("statistic must be 'median' or 'mean'")
    if not (band > 0):
        raise ValueError("band must be positive")
    fn = _STATS[statistic]
    cells, order = _split_cells(values, groups, strata)
    used = [k for k in order if cells[k].get(group_a, np.empty(0)).size and cells[k].get(group_b, np.empty(0)).size]
    skipped = [k for k in order if k not in used]
    if not used:
        return TostResult(math.nan, math.nan, math.nan, level, band, False, math.nan, math.nan,
                          statistic, n_boot, seed, [], skipped)
    diffs = np.array([fn(cells[k][group_a]) - fn(cells[k][group_b]) for k in used])
    estimate = float(diffs.mean())
    ci_low = ci_high = p_tost = math.nan
    if n_boot > 0:
        rng = np.random.default_rng(seed)
        boot = np.zeros((n_boot, len(used)))
        for j, k in enumerate(used):
            boot[:, j] = (fn(_resample_rows(rng, cells[k][group_a], n_boot), axis=1)
                          - fn(_resample_rows(rng, cells[k][group_b], n_boot), axis=1))
        est_star = boot.mean(axis=1)
        ci_low, ci_high = percentile_ci(est_star, level)
        p_tost = float(max(np.mean(est_star <= -band), np.mean(est_star >= band)))
    equivalent = bool(np.isfinite(ci_low) and np.isfinite(ci_high) and ci_low > -band and ci_high < band)
    smallest = float(max(abs(ci_low), abs(ci_high))) if np.isfinite(ci_low) and np.isfinite(ci_high) else math.nan
    return TostResult(estimate, ci_low, ci_high, level, band, equivalent, p_tost, smallest,
                      statistic, n_boot, seed, used, skipped)


# --------------------------------------------------------------------------- #
# pass@k, pass^k
# --------------------------------------------------------------------------- #
def _check_nck(n: int, c: int, k: int) -> None:
    if n < 0 or c < 0 or k < 0 or c > n:
        raise ValueError(f"need 0 <= c <= n and k >= 0, got n={n}, c={c}, k={k}")


def pass_at_k(n: int, c: int, k: int) -> float:
    """Chen et al. (2021) pass@k = 1 - C(n - c, k) / C(n, k); nan when k > n (undefined)."""
    n, c, k = int(n), int(c), int(k)
    _check_nck(n, c, k)
    if k > n:
        return math.nan
    return 1.0 - math.comb(n - c, k) / math.comb(n, k)


def pass_hat_k(n: int, c: int, k: int) -> float:
    """Yao et al. (2024) pass^k = C(c, k) / C(n, k): all k draws succeed; nan when k > n."""
    n, c, k = int(n), int(c), int(k)
    _check_nck(n, c, k)
    if k > n:
        return math.nan
    return math.comb(c, k) / math.comb(n, k)


def passk_curves(cells: pd.DataFrame, ks: Iterable[int] = range(1, 7), *, n_boot: int = 5000,
                 level: float = 0.95, seed: int = DEFAULT_SEED) -> pd.DataFrame:
    """pass@k and pass^k per group, averaged over strata, with a stratum bootstrap CI.

    ``cells`` has columns stratum, group, n, c (one row per cell). The mean
    ignores strata where the estimator is undefined (k > n). The CI resamples
    strata with replacement within each group. Output columns: group, k,
    estimator, mean, ci_low, ci_high, n_strata.
    """
    _check_level(level)
    required = {"stratum", "group", "n", "c"}
    if not required.issubset(cells.columns):
        raise KeyError(f"cells needs columns {sorted(required)}")
    if cells.duplicated(["stratum", "group"]).any():
        raise ValueError("cells must have one row per (stratum, group)")
    ks = [int(k) for k in ks]
    rng = np.random.default_rng(seed)
    rows: list[dict] = []
    for g in _ordered_unique(cells["group"].tolist()):
        sub = cells[cells["group"] == g]
        sub = sub.iloc[np.argsort(sub["stratum"].astype(str).to_numpy(), kind="stable")]
        n_vals, c_vals = sub["n"].astype(int).to_numpy(), sub["c"].astype(int).to_numpy()
        mats = {"pass_at_k": np.array([[pass_at_k(n, c, k) for k in ks] for n, c in zip(n_vals, c_vals)]),
                "pass_hat_k": np.array([[pass_hat_k(n, c, k) for k in ks] for n, c in zip(n_vals, c_vals)])}
        n_strata = len(sub)
        idx = rng.integers(0, n_strata, size=(n_boot, n_strata)) if n_boot > 0 else None
        for est, mat in mats.items():
            with np.errstate(invalid="ignore"):
                means = np.nanmean(mat, axis=0) if n_strata else np.full(len(ks), math.nan)
                if idx is not None and n_strata:
                    boot = np.nanmean(mat[idx], axis=1)  # (n_boot, n_k)
            for j, k in enumerate(ks):
                lo, hi = percentile_ci(boot[:, j], level) if idx is not None and n_strata else (math.nan, math.nan)
                rows.append({"group": g, "k": k, "estimator": est, "mean": float(means[j]),
                             "ci_low": lo, "ci_high": hi,
                             "n_strata": int(np.count_nonzero(np.isfinite(mat[:, j]))) if n_strata else 0})
    return pd.DataFrame(rows, columns=["group", "k", "estimator", "mean", "ci_low", "ci_high", "n_strata"])


# --------------------------------------------------------------------------- #
# dispersion, heterogeneity, paired story-level tests, descriptives
# --------------------------------------------------------------------------- #
def qcd(values: Any) -> float:
    """Quartile coefficient of dispersion (Q3 - Q1) / (Q3 + Q1); nan if n < 2 or Q3 + Q1 == 0."""
    arr = _float_array(values)
    if arr.size < 2:
        return math.nan
    q1, q3 = np.quantile(arr, [0.25, 0.75])
    return float((q3 - q1) / (q3 + q1)) if (q3 + q1) != 0 else math.nan


def cv(values: Any) -> float:
    """Coefficient of variation sd(ddof=1) / mean; nan if n < 2 or mean == 0."""
    arr = _float_array(values)
    if arr.size < 2:
        return math.nan
    mean = arr.mean()
    return float(arr.std(ddof=1) / mean) if mean != 0 else math.nan


def heterogeneity(deltas: Any, variances: Any) -> dict:
    """Cochran's Q, I^2 (proportion in [0, 1]) and DerSimonian-Laird tau^2 over strata.

    Strata with a non-finite delta or variance are dropped. Non-positive
    variances (complete separation gives exactly 0) would carry infinite
    weight; they are floored to the smallest positive variance among the
    remaining strata (``n_floored`` reports how many), or to 1 when none is
    positive. Returns Q, df, p_q, i2, tau2, pooled_fixed, pooled_random, k plus
    se_fixed, se_random and n_floored.
    """
    d = np.asarray(pd.Series(deltas, dtype="float64").to_numpy(), dtype="float64").ravel()
    v = np.asarray(pd.Series(variances, dtype="float64").to_numpy(), dtype="float64").ravel()
    if d.shape != v.shape:
        raise ValueError("deltas and variances must have the same length")
    keep = np.isfinite(d) & np.isfinite(v)
    d, v = d[keep], v[keep]
    k = int(d.size)
    nan_result = {"Q": math.nan, "df": max(k - 1, 0), "p_q": math.nan, "i2": math.nan, "tau2": math.nan,
                  "pooled_fixed": math.nan, "pooled_random": math.nan, "k": k,
                  "se_fixed": math.nan, "se_random": math.nan, "n_floored": 0}
    if k == 0:
        return nan_result
    positive = v[v > 0]
    floor = float(positive.min()) if positive.size else 1.0
    n_floored = int(np.count_nonzero(v <= 0))
    v = np.where(v <= 0, floor, v)
    w = 1.0 / v
    pooled_fixed = float(np.sum(w * d) / np.sum(w))
    q = float(np.sum(w * (d - pooled_fixed) ** 2))
    df = k - 1
    if df == 0:
        return {**nan_result, "Q": 0.0, "p_q": math.nan, "i2": 0.0, "tau2": 0.0,
                "pooled_fixed": pooled_fixed, "pooled_random": pooled_fixed,
                "se_fixed": float(math.sqrt(1.0 / np.sum(w))), "se_random": float(math.sqrt(1.0 / np.sum(w))),
                "n_floored": n_floored}
    p_q = float(_sps.chi2.sf(q, df))
    i2 = float(max(0.0, (q - df) / q)) if q > 0 else 0.0
    denom = np.sum(w) - np.sum(w ** 2) / np.sum(w)
    tau2 = float(max(0.0, (q - df) / denom)) if denom > 0 else 0.0
    w_star = 1.0 / (v + tau2)
    pooled_random = float(np.sum(w_star * d) / np.sum(w_star))
    return {"Q": q, "df": df, "p_q": p_q, "i2": i2, "tau2": tau2, "pooled_fixed": pooled_fixed,
            "pooled_random": pooled_random, "k": k, "se_fixed": float(math.sqrt(1.0 / np.sum(w))),
            "se_random": float(math.sqrt(1.0 / np.sum(w_star))), "n_floored": n_floored}


def paired_stratum_tests(a: Mapping[Any, float], b: Mapping[Any, float], *, label_a: str = "A",
                         label_b: str = "B", higher_is_better: bool = True) -> dict:
    """Exact Wilcoxon signed-rank and sign test on per-stratum paired values a - b.

    Only strata present in both mappings with finite values are used.
    Wilcoxon: scipy ``wilcoxon`` with zero_method="wilcox" (zeros dropped),
    method="exact" when n_nonzero <= 25 else "approx". Sign test: exact
    two-sided binomial on the non-tied strata. ``direction`` reads like
    "7 of 10 strata favour A"; a stratum favours A when a > b if
    higher_is_better else when a < b. Returns n, n_nonzero, wilcoxon_stat,
    wilcoxon_p, sign_k_pos, sign_k_neg, sign_ties, sign_p, direction.
    """
    keys = [k for k in a if k in b]
    diffs = np.array([float(a[k]) - float(b[k]) for k in keys], dtype="float64")
    diffs = diffs[np.isfinite(diffs)]
    n = int(diffs.size)
    k_pos = int(np.count_nonzero(diffs > 0))
    k_neg = int(np.count_nonzero(diffs < 0))
    ties = n - k_pos - k_neg
    n_nonzero = k_pos + k_neg
    if n_nonzero == 0:
        w_stat, w_p = math.nan, 1.0
    else:
        res = _sps.wilcoxon(diffs, zero_method="wilcox", alternative="two-sided",
                            method="exact" if n_nonzero <= 25 else "approx")
        w_stat, w_p = float(res.statistic), float(res.pvalue)
    sign_p = float(_sps.binomtest(k_pos, n_nonzero, 0.5, alternative="two-sided").pvalue) if n_nonzero else 1.0
    fav_a, fav_b = (k_pos, k_neg) if higher_is_better else (k_neg, k_pos)
    if fav_a >= fav_b:
        direction = f"{fav_a} of {n} strata favour {label_a}"
    else:
        direction = f"{fav_b} of {n} strata favour {label_b}"
    if ties:
        direction += f" ({ties} tied)"
    return {"n": n, "n_nonzero": n_nonzero, "wilcoxon_stat": w_stat, "wilcoxon_p": w_p,
            "sign_k_pos": k_pos, "sign_k_neg": k_neg, "sign_ties": ties, "sign_p": sign_p,
            "direction": direction}


def summarize(values: Any) -> dict:
    """n, mean, sd (ddof=1), median, q1, q3, min, max and n_missing of a sample."""
    raw = np.asarray(pd.Series(values, dtype="float64").to_numpy(), dtype="float64").ravel()
    arr = raw[np.isfinite(raw)]
    n = int(arr.size)
    if n == 0:
        return {"n": 0, "mean": math.nan, "sd": math.nan, "median": math.nan, "q1": math.nan,
                "q3": math.nan, "min": math.nan, "max": math.nan, "n_missing": int(raw.size)}
    q1, med, q3 = np.quantile(arr, [0.25, 0.5, 0.75])
    return {"n": n, "mean": float(arr.mean()), "sd": float(arr.std(ddof=1)) if n > 1 else math.nan,
            "median": float(med), "q1": float(q1), "q3": float(q3), "min": float(arr.min()),
            "max": float(arr.max()), "n_missing": int(raw.size - n)}
