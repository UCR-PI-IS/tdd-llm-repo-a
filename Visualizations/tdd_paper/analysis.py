"""Statistical analysis over the pooled runs table.

Implements the protocol of the plan (Appendix B): descriptives per model and per
story, within-story contrasts (Kimi minus Qwen) with Cliff's delta, stratified
bootstrap intervals and a van Elteren stratified rank test, Holm over the
primary family (the artifact's eight pre-specified contrasts) and separately
over the secondary family, TOST equivalence for the null claims, pass@k and
pass^k reliability, heterogeneity across stories, story-level paired tests, the
composite ranking (merge-decision aid) and the sensitivity sets S0 to S10.

Every number that may reach the report is registered in a ``Numbers`` registry
with its provenance tag; ``run_all`` returns the frames and the registry.
"""
from __future__ import annotations

import math
from dataclasses import dataclass, field

import numpy as np
import pandas as pd

from . import composite as comp
from . import config as C
from . import stats as S
from .schema import COLUMN_CATALOG, Provenance
from .tables import Numbers, macro_name

KIMI, QWEN = C.MODEL_ORDER

# Metrics with a within-story contrast (beyond the two families).
EXPLORATORY_METRICS: tuple[str, ...] = (
    "build_execs", "build_first_pass", "test_execs", "line_rate", "branch_rate", "median_mi", "min_mi",
    "max_cc", "max_coupling", "max_dit", "pct_green_types", "min_mi_gain", "worst_coupling_drop",
    "green_share_gain", "story_min_mi", "story_max_cc", "story_max_coupling", "story_max_dit",
    "story_pct_green", "story_line_rate", "story_branch_rate", "story_max_coupling_drop",
    "e2e_probe_pass_rate", "e2e_infra_failures", "e2e_first_pass", "new_test_methods", "new_test_cases",
    "new_cases_per_intent", "new_layer_balance", "assertion_free_new_tests", "prod_loc_added",
    "wall_to_green_min", "active_min", "compile_red", "green_first_try", "self_report_gap_methods",
    "intents_report_error", "refactor_cycles_measured", "new_test_methods_compiled",
)
DESCRIPTIVE_FAMILIES = ("C convergence", "V verification", "Q quality", "B behavioral", "R reliability", "P provenance")
DEPRECATED = ("tests_per_intent", "layer_coverage", "layer_balance")


def effect_metrics() -> list[str]:
    seen: list[str] = []
    for m in (*C.CONFIRMATORY_FAMILY, *C.SECONDARY_FAMILY, *EXPLORATORY_METRICS):
        if m not in seen:
            seen.append(m)
    return seen


def metric_family_label(metric: str) -> str:
    if metric in C.CONFIRMATORY_FAMILY:
        return "primary"
    if metric in C.SECONDARY_FAMILY:
        return "secondary"
    return "exploratory"


def is_binary(metric: str) -> bool:
    spec = COLUMN_CATALOG.get(metric)
    return metric in C.BINARY_METRICS or (spec is not None and spec.dtype == "boolean")


# ---------------------------------------------------------------------------
# Sensitivity sets
# ---------------------------------------------------------------------------


def snapshot_outliers(runs: pd.DataFrame) -> pd.Series:
    """Per story, final-snapshot n_types outside 1.5 IQR of the story's distribution."""
    flag = pd.Series(False, index=runs.index)
    for _, g in runs.groupby("story"):
        v = pd.to_numeric(g["n_types"], errors="coerce")
        q1, q3 = v.quantile(0.25), v.quantile(0.75)
        iqr = q3 - q1
        if pd.isna(iqr):
            continue
        flag.loc[g.index] = (v < q1 - 1.5 * iqr) | (v > q3 + 1.5 * iqr)
    return flag.fillna(False)


def filter_set(runs: pd.DataFrame, set_id: str) -> dict[str, pd.DataFrame]:
    """Return {subset label: frame}. Most sets yield one subset; S3 and S6 yield strata."""
    r = runs
    if set_id == "S0":
        return {"S0": r}
    if set_id == "S1":
        return {"S1": r[~r["identity_ambiguous"].fillna(False).astype(bool)]}
    if set_id == "S2":
        return {"S2": r[pd.to_numeric(r["compile_remove_added"], errors="coerce").fillna(0) == 0]}
    if set_id == "S4":
        return {"S4": r[r["story_pos"] <= 4]}
    if set_id == "S5":
        return {"S5": r[r["story_pos"] != 1]}
    if set_id == "S6":
        out = {"S6:pos1-3": r[r["story_pos"] <= 3], "S6:pos4-10": r[r["story_pos"] >= 4]}
        for author, label in ((KIMI, "S6:baseKimi"), (QWEN, "S6:baseQwen")):
            sub = r[r["baseline_author_model"] == author]
            if not sub.empty:
                out[label] = sub
        return out
    if set_id == "S7":  # the secondary e2e measure counts infrastructure executions as attempts
        r2 = r.copy()
        if "e2e_attempts_to_pass" in r2.columns:
            r2["e2e_attempts_to_pass_noinfra"] = r2["e2e_attempts_to_pass"]
        return {"S7": r2}
    if set_id == "S8":
        return {"S8": r[r["exceptional_ending"].fillna("") == ""]}
    if set_id == "S9":
        return {"S9": r[~snapshot_outliers(r)]}
    if set_id == "S10":
        return {"S10": r[r["story_pos"] >= 5]}
    raise KeyError(set_id)


# ---------------------------------------------------------------------------
# Core contrasts
# ---------------------------------------------------------------------------


def _values(df: pd.DataFrame, metric: str) -> pd.DataFrame:
    sub = df[["story", "model", metric]].copy()
    if is_binary(metric):
        sub[metric] = sub[metric].map(lambda v: np.nan if pd.isna(v) else float(bool(v)))
    else:
        sub[metric] = pd.to_numeric(sub[metric], errors="coerce").astype(float)
    return sub.dropna(subset=[metric])


def contrast(df: pd.DataFrame, metric: str, *, n_boot: int = C.N_BOOT, n_perm: int = C.N_PERM,
             seed: int = C.SEED) -> dict:
    """Kimi minus Qwen within story for one metric: effect size, test, per-story deltas."""
    sub = _values(df, metric)
    out = {"metric": metric, "family": metric_family_label(metric), "binary": is_binary(metric),
           "n_a": int((sub["model"] == KIMI).sum()), "n_b": int((sub["model"] == QWEN).sum()),
           "n_stories": int(sub["story"].nunique())}
    if out["n_a"] < 2 or out["n_b"] < 2 or sub[metric].nunique() < 2:
        out.update({"delta": np.nan, "ci_low": np.nan, "ci_high": np.nan, "a12": np.nan, "magnitude": "n/a",
                    "z": np.nan, "p_asymptotic": np.nan, "p_permutation": np.nan, "p_used": np.nan,
                    "median_a": np.nan, "median_b": np.nan, "mean_a": np.nan, "mean_b": np.nan,
                    "stories_kimi_higher": 0, "stories_qwen_higher": 0, "stories_tie": 0,
                    "i2": np.nan, "q": np.nan, "tau2": np.nan, "per_story": {}})
        return out
    eff = S.stratified_cliffs_delta(sub[metric].to_numpy(), sub["model"].to_numpy(), sub["story"].to_numpy(),
                                    KIMI, QWEN, n_boot=n_boot, level=0.95, seed=seed)
    out.update({"delta": eff.delta, "ci_low": eff.ci_low, "ci_high": eff.ci_high, "a12": eff.a12,
                "magnitude": eff.magnitude, "per_story": eff.per_stratum})
    # One engine for every contrast (analysis-protocol 2.1, STATUS.md decision D14 of 2026-10-04): the van Elteren
    # test on the metric values. For a 0/1 metric the mid-rank sum of a story is affine in its success count, so
    # the test is a stratified comparison of success counts with story weight N_k / (N_k + 1); with equal strata it
    # matches stats.stratified_binary_test to the last digit of the permutation p (see tests/test_stats.py).
    # Binary metrics add the Mantel-Haenszel odds ratio and the mean risk difference as descriptives, not a test.
    rt = S.van_elteren(sub[metric].to_numpy(), sub["model"].to_numpy(), sub["story"].to_numpy(), KIMI,
                       n_perm=n_perm, seed=seed)
    out.update({"z": rt.z, "p_asymptotic": rt.p_asymptotic, "p_permutation": rt.p_permutation})
    if out["binary"]:
        bt = S.stratified_binary_test(sub[metric].to_numpy() > 0.5, sub["model"].to_numpy(), sub["story"].to_numpy(),
                                      KIMI, n_perm=0, seed=seed)
        out.update({"mh_odds_ratio": bt.get("mh_odds_ratio"), "risk_difference_mean": bt.get("risk_difference_mean")})
    out["p_used"] = out["p_permutation"] if out["p_permutation"] is not None and not pd.isna(out["p_permutation"]) else out["p_asymptotic"]
    a = sub[sub["model"] == KIMI][metric]
    b = sub[sub["model"] == QWEN][metric]
    out.update({"median_a": float(a.median()), "median_b": float(b.median()),
                "mean_a": float(a.mean()), "mean_b": float(b.mean())})
    # per-story direction on medians (proportions for binaries)
    med = sub.groupby(["story", "model"])[metric].median().unstack("model")
    if KIMI in med.columns and QWEN in med.columns:
        d = (med[KIMI] - med[QWEN]).dropna()
        out.update({"stories_kimi_higher": int((d > 0).sum()), "stories_qwen_higher": int((d < 0).sum()),
                    "stories_tie": int((d == 0).sum())})
        paired = S.paired_stratum_tests(med[KIMI].dropna().to_dict(), med[QWEN].dropna().to_dict())
        out.update({"wilcoxon_p": paired.get("wilcoxon_p"), "sign_p": paired.get("sign_p"),
                    "direction": paired.get("direction")})
    deltas, variances = [], []
    for st in eff.per_stratum.values():
        if st.get("skipped"):
            continue
        deltas.append(st["delta"])
        variances.append(st.get("var", np.nan))
    het = S.heterogeneity(np.array(deltas, dtype=float), np.array(variances, dtype=float)) if len(deltas) >= 2 else {}
    out.update({"i2": het.get("i2", np.nan), "q": het.get("Q", np.nan), "tau2": het.get("tau2", np.nan)})
    return out


def contrasts_table(df: pd.DataFrame, metrics: list[str], *, set_id: str = "S0", **kw) -> tuple[pd.DataFrame, pd.DataFrame]:
    rows, by_story = [], []
    for m in metrics:
        if m not in df.columns:
            continue
        c = contrast(df, m, **kw)
        per = c.pop("per_story", {})
        c["set"] = set_id
        rows.append(c)
        for story, st in per.items():
            by_story.append({"metric": m, "set": set_id, "story": story, **{k: v for k, v in st.items()}})
    eff = pd.DataFrame(rows)
    if not eff.empty:
        for fam in ("primary", "secondary"):
            mask = eff["family"] == fam
            if mask.any():
                h = S.holm({r["metric"]: r["p_used"] for _, r in eff[mask].iterrows()}, alpha=C.ALPHA)
                hm = h.set_index("name")
                eff.loc[mask, "p_holm"] = eff.loc[mask, "metric"].map(hm["p_holm"]).to_numpy()
                eff.loc[mask, "reject"] = eff.loc[mask, "metric"].map(hm["reject"]).to_numpy()
                eff.loc[mask, "holm_rank"] = eff.loc[mask, "metric"].map(hm["rank"]).to_numpy()
        eff["p_holm"] = eff.get("p_holm", pd.Series(np.nan, index=eff.index))
        eff["reject"] = eff.get("reject", pd.Series(pd.NA, index=eff.index))
    bs = pd.DataFrame(by_story)
    if not bs.empty:
        bs["story_pos"] = bs["story"].map(C.STORY_POS)
    return eff, bs


# ---------------------------------------------------------------------------
# Other layers
# ---------------------------------------------------------------------------


def descriptives(runs: pd.DataFrame, metrics: list[str] | None = None, *, set_id: str = "S0") -> pd.DataFrame:
    metrics = metrics or [n for n, s in COLUMN_CATALOG.items()
                          if s.family in DESCRIPTIVE_FAMILIES and s.dtype in ("Int64", "float64", "boolean")
                          and n not in DEPRECATED and n in runs.columns]
    rows = []
    for m in metrics:
        for model, g in runs.groupby("model"):
            v = _values(g, m)[m]
            s = S.summarize(v.to_numpy()) if len(v) else {"n": 0}
            rows.append({"metric": m, "model": model, "set": set_id, "provenance": COLUMN_CATALOG[m].provenance.value,
                         "family": COLUMN_CATALOG[m].family, "binary": is_binary(m),
                         "n_missing": int(g[m].isna().sum()), **s,
                         "k": int(v.sum()) if is_binary(m) else None, "share": float(v.mean()) if is_binary(m) and len(v) else None})
    return pd.DataFrame(rows)


def descriptives_by_story(runs: pd.DataFrame, metrics: list[str]) -> pd.DataFrame:
    rows = []
    for m in metrics:
        if m not in runs.columns:
            continue
        for (story, model), g in runs.groupby(["story", "model"]):
            v = _values(g, m)[m]
            if len(v):
                rows.append({"metric": m, "story": story, "story_pos": C.STORY_POS.get(story), "model": model,
                             "n": int(len(v)), "median": float(v.median()), "mean": float(v.mean()),
                             "q1": float(v.quantile(0.25)), "q3": float(v.quantile(0.75)),
                             "share": float(v.mean()) if is_binary(m) else None,
                             "qcd": S.qcd(v.to_numpy()), "cv": S.cv(v.to_numpy())})
    return pd.DataFrame(rows)


def equivalence(runs: pd.DataFrame, *, set_id: str = "S0", n_boot: int = C.N_BOOT, seed: int = C.SEED) -> pd.DataFrame:
    rows = []
    for metric, band in C.TOST_BANDS.items():
        if metric not in runs.columns:
            continue
        sub = _values(runs, metric)
        if sub.empty:
            continue
        t = S.tost_stratified(sub[metric].to_numpy(), sub["model"].to_numpy(), sub["story"].to_numpy(), KIMI, QWEN,
                              band, statistic="median", n_boot=n_boot, level=C.TOST_LEVEL, seed=seed)
        rows.append({"metric": metric, "set": set_id, "band": band, **t.to_dict()})
    return pd.DataFrame(rows)


def reliability(cells: pd.DataFrame, *, n_boot: int = C.N_BOOT, seed: int = C.SEED) -> pd.DataFrame:
    frames = []
    for pred in C.SUCCESS_PREDICATES:
        if f"n_{pred}" not in cells.columns:
            continue
        cf = cells.rename(columns={"story": "stratum", "model": "group"})[["stratum", "group"]].copy()
        cf["n"] = cells[f"n_known_{pred}"].to_numpy()
        cf["c"] = cells[f"n_{pred}"].to_numpy()
        cf = cf[cf["n"] > 0]
        if cf.empty:
            continue
        pk = S.passk_curves(cf, ks=range(1, 7), n_boot=n_boot, level=0.95, seed=seed)
        pk["predicate"] = pred
        pooled = cf.groupby("group")[["n", "c"]].sum()
        pk["pooled_rate"] = pk["group"].map((pooled["c"] / pooled["n"]).to_dict())
        frames.append(pk)
    return pd.concat(frames, ignore_index=True) if frames else pd.DataFrame()


def rankings(runs: pd.DataFrame, types: pd.DataFrame, *, n_boot: int = 2000, seed: int = C.SEED,
             with_stability: bool = True) -> tuple[pd.DataFrame, pd.DataFrame, pd.DataFrame]:
    frames: dict[str, tuple[pd.DataFrame, pd.DataFrame]] = {}
    for story, g in runs.groupby("story"):
        mt = types[types["story"] == story] if not types.empty else pd.DataFrame()
        frames[story] = (g.reset_index(drop=True), mt)
    rank = comp.score_all_stories(frames)
    summary = comp.model_rank_summary(rank)
    if "model" not in summary.columns:
        summary = summary.reset_index().rename(columns={"index": "model"})
    stab_rows = []
    if with_stability:
        for story, (summ, mt) in frames.items():
            st = comp.rank_stability(summ, mt, n_boot=n_boot, seed=seed)
            for model, p in st["p_win"].items():
                mr = st.get("mean_rank", {}).get(model, {})
                stab_rows.append({"story": story, "story_pos": C.STORY_POS.get(story), "model": model, "p_win": p,
                                  "mean_rank": mr.get("observed", np.nan), "mean_rank_lo": mr.get("lo", np.nan),
                                  "mean_rank_hi": mr.get("hi", np.nan), "observed_winner": st.get("observed_winner"),
                                  "n_boot": st["n_boot"]})
    return rank, summary, pd.DataFrame(stab_rows)


def sensitivity(runs: pd.DataFrame, metrics: list[str], *, sets: tuple[str, ...] = tuple(C.SENSITIVITY_SETS),
                n_boot: int, n_perm: int, seed: int) -> pd.DataFrame:
    frames = []
    for set_id in sets:
        for label, sub in filter_set(runs, set_id).items():
            if sub["story"].nunique() < 2:
                continue
            eff, _ = contrasts_table(sub, metrics, set_id=label, n_boot=n_boot, n_perm=n_perm, seed=seed)
            frames.append(eff)
    out = pd.concat(frames, ignore_index=True) if frames else pd.DataFrame()
    return out


def robustness_labels(sens: pd.DataFrame) -> pd.DataFrame:
    """Label each primary/secondary metric from the sensitivity matrix.

    * ``robust``: S0 rejects and every robustness set (S1, S2, S5, S7, S8) keeps the
      sign and the Holm decision.
    * ``robust-null``: S0 does not reject and no robustness or subset set rejects.
    * ``fragile``: a robustness set changes the Holm decision, or flips the sign of a
      rejected effect.
    * ``subset-dependent``: only the story subsets (S4 four stories, S10 holdout)
      change the Holm decision.
    * ``regime-dependent``: the sign of a rejected effect differs between the
      composite-selected merges (stories 1 to 3) and the hand-picked merges (4 to 10).
    Sign flips of a null effect are not counted: a delta near zero changes sign by noise.
    """
    rows = []
    if sens.empty:
        return pd.DataFrame()
    for metric, g in sens.groupby("metric"):
        base = g[g["set"] == "S0"]
        if base.empty:
            continue
        b = base.iloc[0]
        sign0 = np.sign(b["delta"]) if pd.notna(b["delta"]) else 0
        rej0 = bool(b["reject"]) if pd.notna(b.get("reject")) else False
        reasons = []
        fragile = False
        subset = False
        regime = False

        def _rej(row) -> bool | None:
            return bool(row["reject"]) if pd.notna(row.get("reject")) else None

        for s in C.ROBUSTNESS_SETS:
            r = g[g["set"] == s]
            if r.empty:
                continue
            rr = r.iloc[0]
            if _rej(rr) is not None and _rej(rr) != rej0:
                fragile = True
                reasons.append(f"Holm decision changes in {s}")
            if rej0 and pd.notna(rr["delta"]) and np.sign(rr["delta"]) != sign0:
                fragile = True
                reasons.append(f"sign flips in {s}")
        for s in ("S4", "S10"):
            r = g[g["set"] == s]
            if not r.empty and _rej(r.iloc[0]) is not None and _rej(r.iloc[0]) != rej0:
                subset = True
                reasons.append(f"Holm decision changes in {s}")
        a, c = g[g["set"] == "S6:pos1-3"], g[g["set"] == "S6:pos4-10"]
        if rej0 and not a.empty and not c.empty and pd.notna(a.iloc[0]["delta"]) and pd.notna(c.iloc[0]["delta"]) \
                and np.sign(a.iloc[0]["delta"]) != np.sign(c.iloc[0]["delta"]):
            regime = True
            reasons.append("sign differs between merge regimes")
        if fragile:
            label = "fragile"
        elif subset:
            label = "subset-dependent"
        elif regime:
            label = "regime-dependent"
        else:
            label = "robust" if rej0 else "robust-null"
        rows.append({"metric": metric, "family": b["family"], "robustness": label, "reasons": "; ".join(reasons),
                     "s0_delta": b["delta"], "s0_reject": rej0})
    return pd.DataFrame(rows)


# ---------------------------------------------------------------------------
# Numbers registration
# ---------------------------------------------------------------------------


def _short(model: str) -> str:
    return C.MODEL_SHORT.get(model, model)


def _fmt_for(metric: str) -> str:
    spec = COLUMN_CATALOG.get(metric)
    if spec is None:
        return "{:.2f}"
    if spec.dtype == "Int64":
        return "{:.1f}"
    if spec.unit == "share":
        return "{:.2f}"
    return "{:.2f}"


def register_numbers(nums: Numbers, runs: pd.DataFrame, desc: pd.DataFrame, eff: pd.DataFrame, tost: pd.DataFrame,
                     passk: pd.DataFrame, rank_summary: pd.DataFrame, sens: pd.DataFrame, robust: pd.DataFrame,
                     excluded: pd.DataFrame, stories: pd.DataFrame, executions: pd.DataFrame | None = None) -> None:
    code = "tdd_paper.analysis"
    # design counts ---------------------------------------------------------------
    nums.add("NStories", runs["story"].nunique(), fmt="int", unit="stories", family="design", stat="count",
             provenance="design", n=len(runs), code_ref=code)
    nums.add("NModels", runs["model"].nunique(), fmt="int", unit="models", family="design", stat="count", provenance="design")
    nums.add("NIterations", int(runs["iteration"].max()), fmt="int", unit="iterations", family="design", provenance="design")
    nums.add("NCells", len(runs), fmt="int", unit="runs", family="design", stat="count", provenance="design")
    nums.add("NRunsPerModel", int((runs["model"] == KIMI).sum()), fmt="int", unit="runs", family="design", provenance="design")
    nums.add("NRunsEndToEnd", int(runs["e2e_execs"].notna().sum()), fmt="int", unit="runs", family="E evidence", provenance="tool-measured")
    if executions is not None and not executions.empty and "ts" in executions.columns:
        ts = pd.to_datetime(executions["ts"], errors="coerce").dropna()
        if not ts.empty:  # execution folder names (local time), first and last across the four trees of all cells
            nums.add("RunWindowFirst", ts.min().strftime("%Y-%m-%d"), fmt="{}", unit="date", family="E evidence",
                     stat="min", provenance="tool-measured", n=int(ts.size), source_columns=["executions.ts"], code_ref=code)
            nums.add("RunWindowLast", ts.max().strftime("%Y-%m-%d"), fmt="{}", unit="date", family="E evidence",
                     stat="max", provenance="tool-measured", n=int(ts.size), source_columns=["executions.ts"], code_ref=code)
    if "ref" in runs.columns:
        origin_only = runs["ref"].fillna("").str.startswith(("origin/", "refs/remotes/origin/"))
        nums.add("NCellsOriginOnly", int(origin_only.sum()), fmt="int", unit="cells", family="E evidence", stat="count",
                 provenance="derived-from-git", n=len(runs), source_columns=["ref"], code_ref=code,
                 notes="cells whose resolved run branch exists only under the remote refs")
    nums.add("NIntentsTotal", int(stories["n_intents"].sum()), fmt="int", unit="intents", family="design", provenance="design")
    nums.add("NIdentityAmbiguous", int(runs["identity_ambiguous"].fillna(False).astype(bool).sum()), fmt="int", unit="runs",
             family="E evidence", provenance="derived-from-git", notes="agent files on the run branch name the other model")
    nums.add("NCompileRemove", int((pd.to_numeric(runs["compile_remove_added"], errors="coerce").fillna(0) > 0).sum()), fmt="int",
             unit="runs", family="E evidence", provenance="derived-from-git")
    nums.add("NExceptionalEnding", int((runs["exceptional_ending"].fillna("") != "").sum()), fmt="int", unit="runs",
             family="E evidence", provenance="tool-measured")
    nums.add("NCompileRed", int(runs["compile_red"].fillna(False).astype(bool).sum()), fmt="int", unit="runs",
             family="V verification", provenance="tool-measured")
    nums.add("NRunBranches", int(len(excluded) + runs["ref"].notna().sum()) if not excluded.empty else int(runs["ref"].notna().sum()),
             fmt="int", unit="branches", family="E evidence", provenance="derived-from-git",
             notes="distinct run branches considered (used plus excluded)")
    if not excluded.empty:
        for reason, g in excluded.groupby("reason"):
            nums.add(macro_name("NExcluded", reason), len(g), fmt="int", unit="branches", family="E evidence",
                     provenance="derived-from-git")
    ok = runs["intents_confirmed_ok"]
    nums.add("NIntentsReportWrong", int((ok == False).sum()), fmt="int", unit="runs", family="P provenance",
             provenance="self-reported", notes="agent intentsConfirmed differs from the intents file")
    nums.add("NIntentsReportMissing", int(ok.isna().sum()), fmt="int", unit="runs", family="P provenance", provenance="self-reported")
    gap = pd.to_numeric(runs["self_report_gap_methods"], errors="coerce")
    nums.add("SelfReportGapZeroK", int((gap == 0).sum()), fmt="int", unit="runs", family="P provenance", provenance="self-reported",
             n=int(gap.notna().sum()), notes="runs where testMethodsEmitted equals the methods counted in the branch diff")
    nums.add("SelfReportGapZeroShare", float((gap == 0).mean()), fmt="{:.2f}", unit="share", family="P provenance",
             provenance="self-reported", n=int(gap.notna().sum()))
    nums.add("SelfReportGapAbsMedian", float(gap.abs().median()), fmt="{:.1f}", unit="methods", family="P provenance",
             provenance="self-reported", n=int(gap.notna().sum()))
    for model, g in runs.groupby("model"):
        gg = pd.to_numeric(g["self_report_gap_methods"], errors="coerce")
        nums.add(macro_name("SelfReportGap", _short(model), "ZeroK"), int((gg == 0).sum()), fmt="int", unit="runs",
                 family="P provenance", provenance="self-reported", n=int(gg.notna().sum()), group={"model": model})
        nums.add(macro_name("SelfReportGap", _short(model), "ZeroShare"), float((gg == 0).mean()), fmt="{:.2f}", unit="share",
                 family="P provenance", provenance="self-reported", n=int(gg.notna().sum()), group={"model": model})
        nums.add(macro_name("SelfReportGap", _short(model), "Median"), float(gg.median()), fmt="{:.1f}", unit="methods",
                 family="P provenance", provenance="self-reported", n=int(gg.notna().sum()), group={"model": model})
    # self-consistency of the stage files (SANER P117): contradictions of a report with itself, per model -----
    for col, stem, what in (("tg_layers_mismatch", "SelfConsistLayers", "per-layer counts that do not sum to the reported total"),
                            ("cg_success_contradiction", "SelfConsistCodeSuccess", "code-generation success with failures, build errors or a failed end-to-end check"),
                            ("ref_success_not_green", "SelfConsistRefSuccess", "refactoring success with allGreenAchieved false"),
                            ("ref_green_with_violations", "SelfConsistRefGreen", "refactoring allGreenAchieved with remaining violations")):
        if col not in runs.columns:
            continue
        for model, g in runs.groupby("model"):
            v = g[col].astype("boolean")
            applies = v.notna()
            k = int(v[applies].astype(bool).sum())
            nums.add(macro_name(stem, _short(model), "K"), k, fmt="int", unit="runs", family="P provenance",
                     provenance="self-reported", n=int(applies.sum()), group={"model": model},
                     notes=f"stage files that contradict themselves: {what}")
            nums.add(macro_name(stem, _short(model), "N"), int(applies.sum()), fmt="int", unit="runs",
                     family="P provenance", provenance="self-reported", group={"model": model},
                     notes=f"runs where the check applies: {what}")
    # example runs (the paired story-3 runs used throughout the report) --------------------------------
    for run_id, tag in (("CPD-LC-001-003/Kimi-K2.5/1", "Kimi"), ("CPD-LC-001-003/Qwen3.7-max/4", "Qwen")):
        row = runs[runs["run_id"] == run_id]
        if row.empty:
            continue
        row = row.iloc[0]
        for col in ("build_execs", "build_failed_execs", "build_errors_burned", "attempts_to_green", "test_execs",
                    "e2e_execs", "e2e_attempts_to_pass", "e2e_attempts_to_pass_noinfra", "new_test_methods", "new_test_cases", "tg_test_methods",
                    "n_intents", "first_run_new_fail_share", "story_min_mi", "story_max_coupling", "metrics_snapshots"):
            if col in row.index and pd.notna(row[col]):
                spec = COLUMN_CATALOG[col]
                nums.add(macro_name("Example", tag, col), row[col], fmt="int" if spec.dtype in ("Int64",) else "{:.2f}",
                         unit=spec.unit, family=spec.family, metric=col, stat="value", provenance=spec.provenance.value,
                         n=1, example_run=run_id, group={"run": run_id})
    cal = runs[runs["run_id"] == "CPD-LC-001-003/Kimi-K2.5/1"]
    if not cal.empty:
        c0 = cal.iloc[0]
        nums.add("ExampleRqThreeKimiErrors", c0["build_errors_burned"], fmt="int", unit="errors", family="C convergence",
                 metric="build_errors_burned", stat="value", provenance="tool-measured", n=1, example_run=c0["run_id"])
        nums.add("ExampleTgTestMethods", c0["tg_test_methods"], fmt="int", unit="methods", family="P provenance",
                 metric="tg_test_methods", stat="value", provenance="self-reported", n=1, example_run=c0["run_id"])
        nums.add("ExampleNewTestMethods", c0["new_test_methods"], fmt="int", unit="methods", family="V verification",
                 metric="new_test_methods", stat="value", provenance="derived-from-git", n=1, example_run=c0["run_id"])
        nums.add("ExampleNewTestCases", c0["new_test_cases"], fmt="int", unit="cases", family="V verification",
                 metric="new_test_cases", stat="value", provenance="derived-from-git", n=1, example_run=c0["run_id"])
    # final state sanity -------------------------------------------------------------
    for col in ("final_build_ok", "final_tests_green", "final_e2e_ok", "final_all_ok"):
        v = runs[col]
        nums.add(macro_name(col, "K"), int(v.fillna(False).astype(bool).sum()), fmt="int", unit="runs", family="R reliability",
                 provenance="tool-measured", n=int(v.notna().sum()))
        nums.add(macro_name(col, "N"), int(v.notna().sum()), fmt="int", unit="runs", family="R reliability", provenance="tool-measured")
    # descriptives per model -------------------------------------------------------------
    # grammar: \val<Metric><Group><Stat>, e.g. \valBuildFailedExecsKimiMedian, \valRedFirstQwenShare
    for _, r in desc.iterrows():
        base = macro_name(r["metric"], _short(r["model"]))
        prov = r["provenance"]
        if r.get("n", 0) == 0:
            continue
        common = dict(family=r["family"], metric=r["metric"], group={"model": r["model"]}, provenance=prov,
                      n=int(r["n"]), source_columns=[r["metric"]], code_ref=code)
        if r["binary"]:
            nums.add(base + "K", r["k"], fmt="int", unit="runs", stat="count", **common)
            nums.add(base + "Share", r["share"], fmt="{:.2f}", unit="share", stat="proportion", **common)
            nums.add(base + "Pct", r["share"], fmt="pct", unit="percent", stat="proportion", **common)
        else:
            fmt = _fmt_for(r["metric"])
            unit = COLUMN_CATALOG[r["metric"]].unit
            for stat in ("median", "mean", "q1", "q3", "min", "max", "sd"):
                nums.add(base + macro_name(stat), r.get(stat), fmt=fmt, unit=unit, stat=stat, **common)
            iqr_text = f"[{fmt.format(r['q1'])}, {fmt.format(r['q3'])}]" if pd.notna(r.get("q1")) else "n/a"
            nums.add(base + "Iqr", iqr_text, fmt="{}", unit=unit, stat="iqr", **common)
        nums.add(base + "N", int(r["n"]), fmt="int", unit="runs", stat="n", **common)
    # effects -----------------------------------------------------------------------------
    def _add_effects(frame: pd.DataFrame, suffix: str = "") -> None:
        for _, r in frame.iterrows():
            if pd.isna(r.get("delta")):
                continue
            base = macro_name(r["metric"])
            prov = "statistic"
            common = dict(family="STAT", metric=r["metric"], provenance=prov, n=int(r["n_a"] + r["n_b"]),
                          set_id=r["set"], code_ref=code, test={"family": r["family"]})

            def add(stat_name, value, **kw):  # grammar: \val<Metric><Stat>[<Set>]
                nums.add(base + stat_name + suffix, value, **kw)

            add("CliffDelta", r["delta"], fmt="{:+.2f}", unit="delta", stat="cliffs_delta", **common)
            add("CliffLo", r["ci_low"], fmt="{:+.2f}", unit="delta", stat="ci_lo", **common)
            add("CliffHi", r["ci_high"], fmt="{:+.2f}", unit="delta", stat="ci_hi", **common)
            add("Atwelve", r["a12"], fmt="{:.2f}", unit="probability", stat="a12", **common)
            add("Magnitude", r.get("magnitude", ""), fmt="{}", unit="label", stat="magnitude", **common)
            add("PPerm", r["p_used"], fmt="p", unit="p", stat="p_perm", **common)
            if pd.notna(r.get("z")):
                add("VanElterenZ", r["z"], fmt="{:+.2f}", unit="z", stat="z", **common)
            if pd.notna(r.get("p_holm")):
                add("PHolm", r["p_holm"], fmt="p", unit="p", stat="p_holm", **common)
                add("Reject", "yes" if bool(r.get("reject")) else "no", fmt="{}", unit="decision", stat="holm_reject", **common)
            add("StoriesKimiHigher", r["stories_kimi_higher"], fmt="int", unit="stories", stat="count", **common)
            add("StoriesQwenHigher", r["stories_qwen_higher"], fmt="int", unit="stories", stat="count", **common)
            add("StoriesTie", r.get("stories_tie", 0), fmt="int", unit="stories", stat="count", **common)
            add("Direction", r["stories_kimi_higher"], fmt="int", unit="stories", stat="count", **common,
                notes="stories whose per-story median is higher for Kimi")
            add("StoriesAgainst", r["stories_qwen_higher"], fmt="int", unit="stories", stat="count", **common,
                notes="stories whose per-story median is higher for Qwen")
            if pd.notna(r.get("i2")):
                add("Isq", r["i2"], fmt="{:.2f}", unit="share", stat="i2", **common)
            if pd.notna(r.get("sign_p")):
                add("SignP", r["sign_p"], fmt="p", unit="p", stat="sign_p", **common)
            if pd.notna(r.get("wilcoxon_p")):
                add("WilcoxonP", r["wilcoxon_p"], fmt="p", unit="p", stat="wilcoxon_p", **common)
            if suffix:  # per-set medians; the S0 medians come from the descriptives with their own provenance
                add("KimiMedian", r["median_a"], fmt=_fmt_for(r["metric"]), unit="", stat="median", **common)
                add("QwenMedian", r["median_b"], fmt=_fmt_for(r["metric"]), unit="", stat="median", **common)
    _add_effects(eff)
    if not sens.empty:
        for set_label, g in sens.groupby("set"):
            if set_label == "S0":
                continue
            _add_effects(g, macro_name(set_label.replace(":", " ")))
    for _, r in robust.iterrows():
        nums.add(macro_name(r["metric"], "Robustness"), r["robustness"], fmt="{}", unit="label", family="STAT", metric=r["metric"],
                 stat="robustness", provenance="statistic", notes=r["reasons"])
    # equivalence ------------------------------------------------------------------------------
    for _, r in tost.iterrows():
        base = macro_name(r["metric"]) + "Tost"
        common = dict(family="STAT", metric=r["metric"], provenance="statistic", set_id=r["set"], code_ref=code)
        nums.add(base + "Estimate", r["estimate"], fmt="{:+.3f}", unit="", stat="tost_delta", **common)
        nums.add(base + "Lo", r["ci_low"], fmt="{:+.3f}", unit="", stat="ci_lo", **common)
        nums.add(base + "Hi", r["ci_high"], fmt="{:+.3f}", unit="", stat="ci_hi", **common)
        nums.add(base + "Band", r["band"], fmt="{:g}", unit="", stat="band", **common)
        nums.add(base + "Equivalent", "yes" if r["equivalent"] else "no", fmt="{}", unit="", stat="decision", **common)
        nums.add(base + "P", r["p_tost"], fmt="p", unit="p", stat="p_tost", **common)
        nums.add(base + "SmallestBand", r["smallest_equivalent_band"], fmt="{:.3f}", unit="", stat="smallest_band", **common)
    # reliability -----------------------------------------------------------------------------------
    for _, r in passk.iterrows():
        est = "PassAt" if r["estimator"] == "pass_at_k" else "PassHat"
        base = macro_name(r["predicate"], _short(r["group"]), est, str(int(r["k"])))
        common = dict(family="R reliability", metric=r["predicate"], provenance="derived", group={"model": r["group"], "k": int(r["k"])},
                      stat=r["estimator"], code_ref=code)
        nums.add(base, r["mean"], fmt="{:.2f}", unit="probability", **common)
        nums.add(base + "Lo", r["ci_low"], fmt="{:.2f}", unit="probability", **common)
        nums.add(base + "Hi", r["ci_high"], fmt="{:.2f}", unit="probability", **common)
    # composite ---------------------------------------------------------------------------------------
    for _, r in rank_summary.iterrows():
        base = macro_name("Composite", _short(r["model"]))
        nums.add(base + "Wins", r["wins"], fmt="int", unit="stories", family="R reliability", provenance="derived",
                 stat="wins", notes="composite is a merge-decision aid, exploratory")
        nums.add(base + "MeanRank", r["mean_rank"], fmt="{:.1f}", unit="rank", family="R reliability", provenance="derived", stat="mean_rank")
        nums.add(base + "TopThreeShare", r["top3_share"], fmt="{:.2f}", unit="share", family="R reliability", provenance="derived", stat="top3_share")
    # per story facts --------------------------------------------------------------------------------------
    for _, r in stories.iterrows():
        base = macro_name("Story", str(int(r["story_pos"])))
        nums.add(base + "Intents", r["n_intents"], fmt="int", unit="intents", family="design", provenance="design", group={"story": r["story"]})
        if pd.notna(r.get("effort_minutes")):
            nums.add(base + "EffortMin", r["effort_minutes"], fmt="int", unit="min", family="design", provenance="design", group={"story": r["story"]})
        if pd.notna(r.get("baseline_test_attrs")):
            nums.add(base + "BaselineTests", r["baseline_test_attrs"], fmt="int", unit="attributes", family="design",
                     provenance="derived-from-git", group={"story": r["story"]})
            nums.add(base + "BaselineProdLines", r["baseline_prod_lines"], fmt="int", unit="lines", family="design",
                     provenance="derived-from-git", group={"story": r["story"]})


# ---------------------------------------------------------------------------
# Orchestration
# ---------------------------------------------------------------------------


@dataclass
class AnalysisResult:
    frames: dict[str, pd.DataFrame] = field(default_factory=dict)
    numbers: Numbers | None = None


def run_all(bundle: dict, *, n_boot: int = C.N_BOOT, n_perm: int = C.N_PERM, seed: int = C.SEED,
            sets: tuple[str, ...] = tuple(C.SENSITIVITY_SETS), with_stability: bool = True,
            meta: dict | None = None, progress=None) -> AnalysisResult:
    runs, cells, types = bundle["runs"], bundle["cells"], bundle.get("types", pd.DataFrame())
    stories, excluded = bundle["stories"], bundle.get("excluded_refs", pd.DataFrame())
    metrics = effect_metrics()
    log = progress or (lambda *_: None)

    log("descriptives")
    desc = descriptives(runs)
    desc_story = descriptives_by_story(runs, metrics)
    log("contrasts")
    eff, by_story = contrasts_table(runs, metrics, set_id="S0", n_boot=n_boot, n_perm=n_perm, seed=seed)
    log("equivalence")
    tost = equivalence(runs, n_boot=n_boot, seed=seed)
    log("reliability")
    passk = reliability(cells, n_boot=n_boot, seed=seed)
    log("rankings")
    rank, rank_summary, stability = rankings(runs, types, seed=seed, with_stability=with_stability)
    log("sensitivity")
    fam_metrics = [*C.CONFIRMATORY_FAMILY, *C.SECONDARY_FAMILY]
    sens = sensitivity(runs, fam_metrics, sets=sets, n_boot=max(1000, n_boot // 5), n_perm=max(2000, n_perm // 5), seed=seed)
    if not sens.empty:
        base = eff[eff["metric"].isin(fam_metrics)].copy()
        sens = pd.concat([base, sens[sens["set"] != "S0"]], ignore_index=True)
    robust = robustness_labels(sens)
    if not robust.empty:
        eff = eff.merge(robust[["metric", "robustness"]], on="metric", how="left")
    log("numbers")
    nums = Numbers(meta or {})
    nums.meta.update({"seed": seed, "n_boot": n_boot, "n_perm": n_perm, "alpha": C.ALPHA,
                      "confirmatory_family": list(C.CONFIRMATORY_FAMILY), "secondary_family": list(C.SECONDARY_FAMILY),
                      "tost_bands": C.TOST_BANDS, "tost_level": C.TOST_LEVEL,
                      "sensitivity_sets": {k: v[1] for k, v in C.SENSITIVITY_SETS.items()}})
    register_numbers(nums, runs, desc, eff, tost, passk, rank_summary, sens, robust, excluded, stories,
                     executions=bundle.get("executions"))
    from . import story_macros  # SANER paper only (P44): no-op unless TDD_PAPER_STORY_MACROS=1
    story_macros.register_if_enabled(nums, runs)
    frames = {"descriptives": desc, "descriptives_by_story": desc_story, "effects": eff, "effects_by_story": by_story,
              "tost": tost, "passk": passk, "rankings": rank, "rank_summary": rank_summary, "rank_stability": stability,
              "sensitivity": sens, "robustness": robust}
    return AnalysisResult(frames=frames, numbers=nums)


def write_frames(result: AnalysisResult, out_dir) -> dict:
    from pathlib import Path
    out = Path(out_dir) / "stats"
    out.mkdir(parents=True, exist_ok=True)
    written = {}
    for name, df in result.frames.items():
        path = out / f"{name}.csv"
        df.to_csv(path, index=name == "rankings" and df.index.name == "rank")
        written[name] = str(path)
    return written
