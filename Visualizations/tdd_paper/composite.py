"""Composite merge-decision score, ported from notebook 00 (code cell 18).

Purpose
-------
The composite answers one practical question per story: which single
(model, iteration) run is the best candidate to merge? It is a decision aid
and is labelled exploratory wherever it is reported. The confirmatory results
of the report never depend on it.

Two-tier scoring
----------------
Every run is scored on four pillars (build, tests & coverage, refactoring &
quality, behaviour). A pillar is the plain mean of its components and the
composite is the weighted mean of the pillars (equal weights by default).
Each component is normalised to [0, 1] in one of two ways:

* ``abs``: an absolute anchor fixed before this cohort existed. Rates and
  ratios are used raw, the maintainability index is divided by 100, the
  documented SQA threshold flags map GREEN=1 / YELLOW=0.5 / RED=0, and the
  efficiency anchors are 1/(attempts + offset) so that one attempt scores
  1.0. Values are clipped to [0, 1]. These components are comparable across
  stories the way a rate is.
* ``rel``: min-max within the runs being scored, for open-ended counts that
  have no absolute scale. A flat column scores 0.5 for everyone. A component
  whose label ends with ``LOW`` (the down arrow) is inverted: a lower raw
  value is better.

Missing evidence always scores 0, the worst value: producing no result is a
result. Only tool-measured artifacts are scored; agent self-reports are never
used. Quality components outnumber effort components by design, so verified
outcomes outweigh the cost of reaching them.

Story-local normalisation
-------------------------
``rel`` components are normalised within one story, so composites are only
comparable between runs of the same story. Never compare composite values
across stories. Cross-story statements must use ranks, wins, top-3 shares or
Pareto membership, which is what ``model_rank_summary`` reports.

Weight-independent diagnostics
------------------------------
``pareto_front`` marks the runs not dominated on all pillars; only these can
rank first under any positive weighting. ``weight_sweep`` re-ranks the runs
under every weighting of a deterministic simplex grid (each pillar weight
0.10 to 0.70 in 0.05 steps, keeping the combinations that sum to 1: 455
weightings for four pillars) and reports how often each run wins
(``top1_share``) and the ranks it spans (``rank_range``). ``rank_stability``
adds a seeded bootstrap of the runs within each model.

The module holds data (the pillar specification, weights and grid steps) plus
pure functions; it never reads files. Feed it the frames produced by
``tdd_results.iteration_summary`` and ``tdd_results.finals(metrics_types)``.
"""
from __future__ import annotations

from collections.abc import Mapping, Sequence
from itertools import product
from typing import Any

import numpy as np
import pandas as pd

__all__ = [
    "LOW", "FLAG", "STEPS", "KEY",
    "DEFAULT_PILLAR_WEIGHTS", "DEFAULT_PILLAR_SPEC",
    "EFFICIENCY_COLUMNS", "COLUMN_DIVISORS", "DERIVED_COLUMNS", "WORST_FLAG_COLUMNS",
    "worst_flag_score", "efficiency", "derive_components", "score",
    "pareto_front", "simplex_grid", "weight_sweep",
    "score_story", "score_all_stories", "model_rank_summary", "rank_stability",
]

# ---------------------------------------------------------------------------
# Data: the notebook's constants
# ---------------------------------------------------------------------------

#: Label suffix marking a component whose lower raw value is better. Only
#: ``rel`` components carry it; they are inverted after min-max scaling.
LOW = "↓"

#: Documented-threshold flag of a type's metric -> score.
FLAG = {"GREEN": 1.0, "YELLOW": 0.5, "RED": 0.0}

#: Weight grid for ``weight_sweep``: 0.10 to 0.70 in 0.05 steps.
STEPS: tuple[float, ...] = tuple(round(0.10 + 0.05 * k, 2) for k in range(13))

#: Default run key: one row per (model, iteration) within a story.
KEY: tuple[str, str] = ("model", "iteration")

DEFAULT_PILLAR_WEIGHTS: dict[str, float] = {
    "build": 1.0,
    "tests & coverage": 1.0,
    "refactoring & quality": 1.0,
    "behaviour": 1.0,
}

#: Derived efficiency columns: name -> (source column, offset). The anchor is
#: 1 / (source + offset): a run that needed one attempt scores 1.0.
EFFICIENCY_COLUMNS: dict[str, tuple[str, float]] = {
    "build_efficiency": ("build_failed_execs", 1),
    "green_efficiency": ("attempts_to_green", 0),
    "e2e_efficiency": ("e2e_attempts_to_pass", 0),
}

#: Columns of the final metrics-types frame that ``worst_flag_score`` reads.
WORST_FLAG_COLUMNS: list[str] = ["mi", "mi_flag", "cc", "cc_flag",
                                 "coupling", "coupling_flag", "dit", "dit_flag"]

#: Columns added by ``derive_components`` on top of the iteration summary.
DERIVED_COLUMNS: tuple[str, ...] = ("worst_flag", *EFFICIENCY_COLUMNS)

#: Raw columns whose natural range is not 0-1. They are divided by this value
#: before ``abs`` scoring (the notebook's ``median_mi / 100.0``).
COLUMN_DIVISORS: dict[str, float] = {"median_mi": 100.0, "min_mi": 100.0}

#: pillar -> [(component label, norm, column)]. ``norm`` is ``"abs"`` or
#: ``"rel"``; a label ending with ``LOW`` is inverted. Columns are those of
#: ``tdd_results.iteration_summary`` plus ``DERIVED_COLUMNS``.
DEFAULT_PILLAR_SPEC: dict[str, list[tuple[str, str, str]]] = {
    "build": [
        ("build efficiency", "abs", "build_efficiency"),
        (f"errors burned {LOW}", "rel", "build_errors_burned"),
        (f"peak errors {LOW}", "rel", "build_peak_errors"),
        (f"build warnings {LOW}", "rel", "build_warnings"),
    ],
    "tests & coverage": [
        ("green efficiency", "abs", "green_efficiency"),
        ("red-first discipline", "abs", "red_first"),
        ("line coverage", "abs", "line_rate"),
        ("branch coverage", "abs", "branch_rate"),
        ("layer balance", "abs", "layer_balance"),
        (f"failures burned {LOW}", "rel", "test_failures_burned"),
    ],
    "refactoring & quality": [
        ("% all-GREEN types", "abs", "pct_green_types"),
        ("median MI", "abs", "median_mi"),
        ("worst-type MI", "abs", "min_mi"),
        ("worst-type flags", "abs", "worst_flag"),
        (f"worst coupling {LOW}", "rel", "max_coupling"),
        ("coupling reduced", "rel", "worst_coupling_drop"),
        ("green share gained", "rel", "green_share_gain"),
    ],
    "behaviour": [
        ("e2e efficiency", "abs", "e2e_efficiency"),
        ("e2e probe pass rate", "abs", "e2e_probe_pass_rate"),
        (f"e2e infra failures {LOW}", "rel", "e2e_infra_failures"),
    ],
}

Spec = Mapping[str, Sequence[tuple[str, str, str]]]


# ---------------------------------------------------------------------------
# Derived components
# ---------------------------------------------------------------------------

def worst_flag_score(g: pd.DataFrame) -> float:
    """Documented-threshold flag of each metric's worst type, averaged.

    ``g`` holds the final metrics types of one run (columns
    ``WORST_FLAG_COLUMNS``). For MI the worst type is the minimum, for CC,
    coupling and DIT the maximum; its flag maps through ``FLAG`` (an unknown
    flag counts 0). Metrics without any value are skipped; NaN when none has
    a value. ``g`` must have a unique index.
    """
    vals = []
    for col, flag_col, worst in (("mi", "mi_flag", "idxmin"),
                                 ("cc", "cc_flag", "idxmax"),
                                 ("coupling", "coupling_flag", "idxmax"),
                                 ("dit", "dit_flag", "idxmax")):
        s = g[col].dropna()
        if not s.empty:
            vals.append(FLAG.get(g.loc[getattr(s, worst)(), flag_col], 0.0))
    return float(np.mean(vals)) if vals else float("nan")


def efficiency(series: pd.Series, offset: float = 0) -> pd.Series:
    """Absolute efficiency anchor 1 / (attempts + offset); one attempt -> 1.0.

    Bounded 0-1 without min-max, so it stays comparable across stories the
    way a rate does. Zero attempts with offset 0 give inf, which ``abs``
    scoring clips to 1.0; missing attempts stay NaN.
    """
    v = pd.to_numeric(series, errors="coerce")
    return 1.0 / (v + offset)


def _indexed(summ: pd.DataFrame, key: list[str]) -> pd.DataFrame:
    """``summ`` indexed by ``key`` (a frame already indexed by it is copied)."""
    if list(summ.index.names) == key:
        return summ.copy()
    missing = [k for k in key if k not in summ.columns]
    if missing:
        raise KeyError(f"key columns missing from the summary frame: {missing}")
    return summ.set_index(key)


def derive_components(summ: pd.DataFrame, metrics_types_final: pd.DataFrame | None,
                      key: Sequence[str] = KEY) -> pd.DataFrame:
    """``summ`` indexed by ``key`` with the four derived columns added.

    ``worst_flag`` is ``worst_flag_score`` of the final metrics types grouped
    by ``key`` (NaN for runs without types, and for every run when the frame
    is empty or None). The efficiency columns follow ``EFFICIENCY_COLUMNS``;
    a missing source column yields NaN, which scores 0 like any missing
    evidence. Existing columns of the same names are overwritten.
    """
    key = list(key)
    runs = _indexed(summ, key)
    mt = metrics_types_final
    if mt is None or mt.empty:
        runs["worst_flag"] = np.nan
    else:
        mt = mt.reset_index(drop=True)  # worst_flag_score needs unique labels
        runs["worst_flag"] = mt.groupby(key)[WORST_FLAG_COLUMNS].apply(worst_flag_score)
    for name, (source, offset) in EFFICIENCY_COLUMNS.items():
        runs[name] = efficiency(runs[source], offset) if source in runs.columns else np.nan
    return runs


# ---------------------------------------------------------------------------
# Scoring
# ---------------------------------------------------------------------------

def score(series: pd.Series, norm: str, invert: bool = False) -> pd.Series:
    """Normalise one component to [0, 1].

    ``abs``: raw 0-1 anchor, clipped. ``rel``: min-max within these runs
    (flat -> 0.5), inverted when ``invert`` (lower raw value is better).
    Missing evidence always scores 0 (worst). Values are coerced to float
    first, so boolean columns score 1.0 / 0.0. Inversion is defined for
    ``rel`` components only, as in the notebook; asking for it on an ``abs``
    component is an error rather than a silent no-op.
    """
    v = pd.to_numeric(series, errors="coerce").astype(float)
    if norm == "abs":
        if invert:
            raise ValueError("inversion (the LOW suffix) applies to 'rel' components only")
        return v.clip(0.0, 1.0).fillna(0.0)
    if norm != "rel":
        raise ValueError(f"unknown norm {norm!r}: expected 'abs' or 'rel'")
    lo, hi = v.min(), v.max()
    if pd.isna(lo):
        return pd.Series(0.0, index=v.index)
    n = (v - lo) / (hi - lo) if hi > lo else v.where(v.isna(), 0.5)
    return (1.0 - n if invert else n).fillna(0.0)


def _validate_spec(spec: Spec) -> dict[str, list[tuple[str, str, str]]]:
    if not spec:
        raise ValueError("the pillar spec is empty")
    out: dict[str, list[tuple[str, str, str]]] = {}
    seen: set[str] = set()
    for pillar, items in spec.items():
        items = list(items)
        if not items:
            raise ValueError(f"pillar {pillar!r} has no components")
        for item in items:
            if len(item) != 3:
                raise ValueError(f"component {item!r} of pillar {pillar!r} is not "
                                 "(label, norm, column)")
            label, norm, _column = item
            if norm not in ("abs", "rel"):
                raise ValueError(f"component {label!r}: unknown norm {norm!r}")
            if label in seen:
                raise ValueError(f"component label {label!r} is used twice")
            seen.add(label)
        out[str(pillar)] = [tuple(item) for item in items]
    return out


def _resolve_weights(spec: Mapping[str, Any], weights: Mapping[str, float] | None) -> pd.Series:
    """Pillar weights as a Series in spec order (defaults: 1.0 per pillar)."""
    if weights is None:
        weights = {p: DEFAULT_PILLAR_WEIGHTS.get(p, 1.0) for p in spec}
    if set(weights) != set(spec):
        raise ValueError("weights must name exactly the pillars of the spec: "
                         f"{sorted(spec)} vs {sorted(weights)}")
    w = pd.Series({p: float(weights[p]) for p in spec})
    if (w < 0).any() or not w.sum() > 0:
        raise ValueError("weights must be non-negative with a positive sum")
    return w


def _component_values(frame: pd.DataFrame, label: str, column: str) -> pd.Series:
    """Raw values of one component as float, with ``COLUMN_DIVISORS`` applied."""
    if column not in frame.columns:
        raise KeyError(f"component {label!r} needs column {column!r}, "
                       "which the runs frame does not have")
    v = pd.to_numeric(frame[column], errors="coerce").astype(float)
    divisor = COLUMN_DIVISORS.get(column)
    return v / divisor if divisor is not None else v


def _pillar_scores(frame: pd.DataFrame, spec: Spec) -> tuple[pd.DataFrame, pd.DataFrame]:
    """(pillar scores, component scores prefixed ``score:``) for ``frame``."""
    pillars: dict[str, pd.Series] = {}
    comps: dict[str, pd.Series] = {}
    for pillar, items in spec.items():
        scored = pd.DataFrame({label: score(_component_values(frame, label, column),
                                            norm, label.endswith(LOW))
                               for label, norm, column in items})
        pillars[pillar] = scored.mean(axis=1)
        for label in scored.columns:
            comps[f"score:{label}"] = scored[label]
    return pd.DataFrame(pillars), pd.DataFrame(comps)


def _score_runs(frame: pd.DataFrame, spec: Spec, w: pd.Series, steps: Sequence[float],
                key: list[str], diagnostics: bool) -> pd.DataFrame:
    """Score and rank ``frame`` (key as columns, unique row index).

    ``diagnostics=False`` skips the Pareto front and the weight sweep, which
    do not affect the ranking; the bootstrap uses it.
    """
    pillars, comps = _pillar_scores(frame, spec)
    out = pd.concat([frame[key], pillars, comps], axis=1)
    out["composite"] = (pillars * w).sum(axis=1) / w.sum()
    ranking = (out.sort_values(["composite", *key], ascending=[False, *([True] * len(key))])
                  .reset_index(drop=True))
    ranking.index = pd.RangeIndex(1, len(ranking) + 1, name="rank")
    cols = [*key, "composite", *spec]
    if diagnostics:
        P = ranking[list(spec)].to_numpy(dtype=float)
        ranking["pareto"] = pareto_front(P)
        top1, lo, hi, _grid = weight_sweep(P, steps)
        ranking["top1_share"] = top1
        ranking["rank_range"] = [f"{int(a)}–{int(b)}" for a, b in zip(lo, hi)]
        cols += ["pareto", "top1_share", "rank_range"]
    return ranking[cols + list(comps.columns)]


def _empty_ranking(spec: Spec, key: list[str]) -> pd.DataFrame:
    cols = [*key, "composite", *spec, "pareto", "top1_share", "rank_range",
            *(f"score:{label}" for items in spec.values() for label, _, _ in items)]
    out = pd.DataFrame(columns=cols)
    out.index = pd.RangeIndex(1, 1, name="rank")
    return out


# ---------------------------------------------------------------------------
# Weight-independent diagnostics
# ---------------------------------------------------------------------------

def pareto_front(P: np.ndarray) -> np.ndarray:
    """Boolean mask of the non-dominated rows of ``P`` (runs x pillars).

    Row i is dominated when some other row is >= on every pillar and > on at
    least one. Identical rows do not dominate each other.
    """
    P = np.asarray(P, dtype=float)
    if P.ndim != 2:
        raise ValueError("P must be two-dimensional (runs x pillars)")
    if len(P) == 0:
        return np.zeros(0, dtype=bool)
    ge = (P[None, :, :] >= P[:, None, :]).all(axis=2)  # ge[i, j]: j >= i everywhere
    gt = (P[None, :, :] > P[:, None, :]).any(axis=2)   # gt[i, j]: j > i somewhere
    dominated = (ge & gt).any(axis=1)
    return ~dominated


def simplex_grid(n_pillars: int, steps: Sequence[float] = STEPS) -> np.ndarray:
    """Deterministic weight grid: every combination of ``steps`` over
    ``n_pillars`` pillars whose weights sum to 1 (455 rows for 4 pillars)."""
    steps = [float(s) for s in steps]
    pts = [c for c in product(steps, repeat=n_pillars) if abs(sum(c) - 1.0) < 1e-9]
    return np.array(pts, dtype=float).reshape(len(pts), n_pillars)


def weight_sweep(P: np.ndarray, steps: Sequence[float] = STEPS
                 ) -> tuple[np.ndarray, np.ndarray, np.ndarray, np.ndarray]:
    """Re-rank the runs of ``P`` (runs x pillars) under every grid weighting.

    Returns ``(top1_share, rank_lo, rank_hi, grid)``: the share of weightings
    each run wins, the best and worst rank it reaches, and the grid itself.
    Ties keep the row order of ``P`` (stable sort), so pass rows in ranking
    order as ``score_story`` does.
    """
    P = np.asarray(P, dtype=float)
    if P.ndim != 2:
        raise ValueError("P must be two-dimensional (runs x pillars)")
    n_runs, n_pillars = P.shape
    grid = simplex_grid(n_pillars, steps)
    if len(grid) == 0:
        raise ValueError(f"no weighting of {n_pillars} pillars sums to 1 on steps {list(steps)}")
    top1 = np.zeros(n_runs)
    rank_lo, rank_hi = np.full(n_runs, n_runs), np.ones(n_runs)
    if n_runs == 0:
        return top1, rank_lo.astype(int), rank_hi.astype(int), grid
    for wv in grid:
        comp = P @ wv
        order = np.argsort(-comp, kind="stable")
        rnk = np.empty(n_runs, dtype=int)
        rnk[order] = np.arange(1, n_runs + 1)
        top1[order[0]] += 1
        rank_lo, rank_hi = np.minimum(rank_lo, rnk), np.maximum(rank_hi, rnk)
    return top1 / len(grid), rank_lo.astype(int), rank_hi.astype(int), grid


# ---------------------------------------------------------------------------
# Story-level ranking
# ---------------------------------------------------------------------------

def score_story(summ: pd.DataFrame, metrics_types_final: pd.DataFrame | None,
                spec: Spec | None = None, weights: Mapping[str, float] | None = None,
                steps: Sequence[float] = STEPS, key: Sequence[str] = KEY) -> pd.DataFrame:
    """Rank the runs of one story by composite score.

    ``summ`` is ``tdd_results.iteration_summary`` of the story and
    ``metrics_types_final`` is ``tdd_results.finals(data["metrics_types"])``.
    Returns a frame indexed by ``rank`` (1 = best) with the key columns,
    ``composite``, one column per pillar, ``pareto``, ``top1_share``,
    ``rank_range`` ("lo-hi" with an en dash, as in the notebook) and the
    component scores prefixed ``score:``. Ties on the composite are broken
    by the key, ascending. Numbers match notebook 00, cell 18.

    ``rel`` components are normalised within this story: do not compare the
    composite across stories.
    """
    spec = _validate_spec(DEFAULT_PILLAR_SPEC if spec is None else spec)
    w = _resolve_weights(spec, weights)
    key = list(key)
    runs = derive_components(summ, metrics_types_final, key)
    if runs.empty:
        return _empty_ranking(spec, key)
    return _score_runs(runs.reset_index(), spec, w, steps, key, diagnostics=True)


def score_all_stories(frames: Mapping[str, tuple[pd.DataFrame, pd.DataFrame | None]],
                      **kw: Any) -> pd.DataFrame:
    """``score_story`` for every story, stacked with ``story`` and ``rank`` columns.

    ``frames`` maps story id -> (iteration summary, final metrics types);
    ``kw`` is forwarded to ``score_story``. Normalisation stays story-local
    by design, so the ``composite`` column is comparable within a story only.
    Use ranks, wins and Pareto membership across stories
    (``model_rank_summary``).
    """
    parts = []
    for story, (summ, mt) in frames.items():
        ranked = score_story(summ, mt, **kw).reset_index()
        ranked.insert(0, "story", story)
        parts.append(ranked)
    if not parts:
        spec = _validate_spec(kw.get("spec") or DEFAULT_PILLAR_SPEC)
        empty = _empty_ranking(spec, list(kw.get("key", KEY))).reset_index()
        empty.insert(0, "story", pd.Series(dtype=str))
        return empty
    return pd.concat(parts, ignore_index=True)


def model_rank_summary(rankings: pd.DataFrame) -> pd.DataFrame:
    """Per-model rank statistics over one or many stories.

    Accepts the output of ``score_all_stories`` (or of ``score_story``, whose
    rank index is used). Columns: ``wins`` (rank-1 count), ``mean_rank``,
    ``top3_share`` (share of the model's runs ranked 3 or better),
    ``pareto_share``, ``n_stories`` and ``n_runs``. Sorted by wins, then mean
    rank. Model labels must already be canonical.
    """
    r = rankings if "rank" in rankings.columns else rankings.reset_index()
    if "rank" not in r.columns:
        raise KeyError("rankings need a 'rank' column or index")
    r = r.assign(_win=r["rank"] == 1, _top3=r["rank"] <= 3)
    g = r.groupby("model", sort=True)
    out = pd.DataFrame({
        "wins": g["_win"].sum().astype(int),
        "mean_rank": g["rank"].mean(),
        "top3_share": g["_top3"].mean(),
        "pareto_share": (r["pareto"].astype(float).groupby(r["model"]).mean()
                         if "pareto" in r.columns else np.nan),
        "n_stories": g["story"].nunique() if "story" in r.columns else 1,
        "n_runs": g.size(),
    })
    return out.sort_values(["wins", "mean_rank"], ascending=[False, True])


def rank_stability(summ: pd.DataFrame, metrics_types_final: pd.DataFrame | None, *,
                   n_boot: int = 2000, seed: int = 20261003, spec: Spec | None = None,
                   weights: Mapping[str, float] | None = None, steps: Sequence[float] = STEPS,
                   key: Sequence[str] = KEY, level: float = 0.90) -> dict[str, Any]:
    """Bootstrap the ranking of one story.

    Each replicate resamples the runs with replacement within each model
    (the first key field), keeping every model's number of runs, and
    re-scores them exactly as ``score_story`` does (``rel`` normalisation
    included; the Pareto front and weight sweep are skipped because they do
    not affect ranks). Returns ``p_win`` (share of replicates each model
    supplies the rank-1 run; sums to 1), ``mean_rank`` per model with the
    observed value and a ``level`` percentile interval (90% by default),
    ``observed_winner``, ``n_runs`` per model, ``n_boot`` and ``seed``.
    Uses ``numpy.random.default_rng(seed)``.
    """
    if n_boot < 1:
        raise ValueError("n_boot must be at least 1")
    spec = _validate_spec(DEFAULT_PILLAR_SPEC if spec is None else spec)
    w = _resolve_weights(spec, weights)
    key = list(key)
    runs = derive_components(summ, metrics_types_final, key).reset_index()
    if runs.empty:
        raise ValueError("no runs to resample")
    model_col = key[0]
    labels = runs[model_col].to_numpy()
    models = sorted(set(labels))
    groups = [np.flatnonzero(labels == m) for m in models]

    observed = _score_runs(runs, spec, w, steps, key, diagnostics=False)
    obs_labels = observed[model_col].to_numpy()
    obs_ranks = np.arange(1, len(observed) + 1)

    rng = np.random.default_rng(seed)
    wins = dict.fromkeys(models, 0)
    boot_mean_rank = np.empty((n_boot, len(models)))
    for b in range(n_boot):
        pos = np.concatenate([rng.choice(g, size=g.size, replace=True) for g in groups])
        sample = runs.iloc[pos].reset_index(drop=True)
        ranked = _score_runs(sample, spec, w, steps, key, diagnostics=False)
        lab = ranked[model_col].to_numpy()
        wins[lab[0]] += 1
        rk = np.arange(1, len(ranked) + 1)
        for i, m in enumerate(models):
            boot_mean_rank[b, i] = rk[lab == m].mean()

    lo_q, hi_q = (1.0 - level) / 2.0, 1.0 - (1.0 - level) / 2.0
    return {
        "p_win": {str(m): wins[m] / n_boot for m in models},
        "mean_rank": {
            str(m): {"observed": float(obs_ranks[obs_labels == m].mean()),
                     "lo": float(np.quantile(boot_mean_rank[:, i], lo_q)),
                     "hi": float(np.quantile(boot_mean_rank[:, i], hi_q))}
            for i, m in enumerate(models)
        },
        "observed_winner": str(obs_labels[0]),
        "n_runs": {str(m): int(g.size) for m, g in zip(models, groups)},
        "level": level,
        "n_boot": n_boot,
        "seed": seed,
    }
