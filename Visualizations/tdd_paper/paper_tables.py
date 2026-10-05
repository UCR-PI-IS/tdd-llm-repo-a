"""The report's tables, written as bare booktabs tabular bodies (tab_<slug>.tex)."""
from __future__ import annotations

from pathlib import Path

import numpy as np
import pandas as pd

from . import config as C
from .schema import COLUMN_CATALOG, catalog_frame
from .tables import fmt_p, fmt_value, latex_escape, to_booktabs, write_table

KIMI, QWEN = C.MODEL_ORDER
SHORT = C.MODEL_SHORT

DESCRIPTIVE_METRICS = [
    "build_execs", "build_failed_execs", "build_errors_burned", "build_peak_errors", "test_execs", "attempts_to_green",
    "test_failures_burned", "new_test_methods", "new_tests_per_intent", "new_layer_balance", "line_rate", "branch_rate",
    "story_line_rate", "median_mi", "min_mi", "max_cc", "max_coupling", "story_min_mi", "story_max_cc", "story_max_coupling",
    "story_pct_green", "min_mi_gain", "story_min_mi_gain", "e2e_execs", "e2e_attempts_to_pass_noinfra", "e2e_infra_failures",
    "active_min", "wall_to_green_min", "prod_loc_added",
]
BINARY_DESCRIPTIVES = ["build_first_pass", "red_first", "compile_red", "green_first_try", "e2e_first_pass",
                       "final_tests_green", "final_e2e_ok", "final_all_ok"]


def _median_iqr(desc: pd.DataFrame, metric: str, model: str) -> str:
    r = desc[(desc["metric"] == metric) & (desc["model"] == model)]
    if r.empty or r.iloc[0].get("n", 0) == 0:
        return "n/a"
    r = r.iloc[0]
    if r["binary"]:
        return f"{int(r['k'])}/{int(r['n'])} ({r['share']:.2f})"
    fmt = "{:.0f}" if COLUMN_CATALOG[metric].dtype == "Int64" else "{:.2f}"
    return f"{fmt.format(r['median'])} [{fmt.format(r['q1'])}, {fmt.format(r['q3'])}]"


def tab_stories(bundle, out_dir) -> Path:
    st = bundle["stories"].sort_values("story_pos").copy()
    st["label"] = st["story_pos"].map(lambda p: f"{int(p)}")
    st["intents_layers"] = st.apply(lambda r: f"{int(r['intents_Domain'])}/{int(r['intents_Application'])}/{int(r['intents_Infrastructure'])}/{int(r['intents_Presentation'])}", axis=1)
    st["baseline_author"] = st["baseline_author_model"].map(lambda m: SHORT.get(m, m))
    st["merged"] = st["merged_run"].map(lambda s: s.replace("Kimi-K2.5", "Kimi").replace("Qwen3.7-max", "Qwen") if isinstance(s, str) else "")
    cols = [("label", "Pos.", "str"), ("story", "Story", "str"), ("title", "Feature", "str"), ("n_scenarios", "Scen.", "int"),
            ("n_intents", "Intents", "int"), ("intents_layers", "D/A/I/P", "str"), ("effort_minutes", "Est. min", "int"),
            ("baseline_author", "Baseline by", "str"), ("baseline_prod_lines", "Prod. lines", "int"),
            ("baseline_test_attrs", "Tests inh.", "int"), ("merged", "Merged run", "str")]
    body = to_booktabs(st, cols, align="llp{4.6cm}rrrrlrrl")
    return write_table(out_dir, "tab_stories", body, meta={"rows": len(st), "provenance": "design + derived-from-git"})


# Metric catalog --------------------------------------------------------------------------------------------------
# The paper prints the compact catalog (Table 2 of the report): every metric that carries a confirmatory hypothesis,
# a secondary test, an equivalence band or a pass@k curve, sized for \textwidth (D16, 2026-10-04). The full catalog
# of the run table (112 metric and evidence columns) is written as tab_metric_catalog_full.{tex,csv} for the
# replication package; it is longer than a page and is not input by the paper.
_CATALOG_FAMILIES = ["C convergence", "V verification", "Q quality", "B behavioral", "R reliability", "P provenance",
                     "E evidence"]
_CATALOG_EXCLUDED = ["tests_per_intent", "layer_coverage", "layer_balance"]
_DIRECTION_WORD = {1: "higher", -1: "lower", 0: "none"}
# column widths of the compact table, as fractions of \textwidth (checked by a probe compile: the tabular fits)
_W_ROLE, _W_PROV, _W_DEF = 0.105, 0.105, 0.35


def _band_text(band: float) -> str:
    return f"{int(band)}" if float(band).is_integer() else f"{band:.2f}"


def metric_roles() -> dict[str, list[str]]:
    """Metric -> its inferential roles in the order H1..H8, secondary, TOST band, pass@k (LaTeX strings)."""
    roles: dict[str, list[str]] = {}
    for i, m in enumerate(C.CONFIRMATORY_FAMILY, 1):
        roles.setdefault(m, []).append(f"H{i}")
    for m in C.SECONDARY_FAMILY:
        roles.setdefault(m, []).append("secondary")
    for m, band in C.TOST_BANDS.items():
        roles.setdefault(m, []).append(f"TOST $\\pm${_band_text(band)}")
    for m in C.SUCCESS_PREDICATES:
        roles.setdefault(m, []).append("pass@$k$")
    return roles


def _role_rank(role_list: list[str]) -> int:
    first = role_list[0] if role_list else ""
    if first.startswith("H") and first[1:].isdigit():
        return int(first[1:])
    return {"secondary": 20, "pass@$k$": 40}.get(first, 30)


def _pbox(width: float, text: str) -> str:
    return f"\\parbox[t]{{{width}\\textwidth}}{{\\raggedright\\strut {text}\\strut}}"


def _catalog_frame(compact: bool) -> pd.DataFrame:
    cf = catalog_frame()
    cf = cf[cf["family"].isin(_CATALOG_FAMILIES) & ~cf["name"].isin(_CATALOG_EXCLUDED)].copy()
    roles = metric_roles()
    cf["roles"] = cf["name"].map(lambda n: roles.get(n, []))
    if compact:
        cf = cf[cf["roles"].map(bool)].copy()
    cf["fam_rank"] = cf["family"].map(_CATALOG_FAMILIES.index)
    cf["role_rank"] = cf["roles"].map(_role_rank)
    cf = cf.sort_values(["fam_rank", "role_rank", "name"]).reset_index(drop=True)
    cf["name_tt"] = cf["name"].map(lambda n: f"\\texttt{{{latex_escape(n)}}}")
    cf["use"] = cf["roles"].map(lambda r: _pbox(_W_ROLE, "; ".join(r)) if r else "")
    cf["fam"] = cf["family"].str[0]
    cf["prov"] = cf["provenance"].map(lambda v: _pbox(_W_PROV, latex_escape(v)))
    cf["dir"] = cf["direction"].map(_DIRECTION_WORD)
    cf["unit_txt"] = cf.apply(lambda r: "0/1" if r["dtype"] == "boolean" else latex_escape(r["unit"]), axis=1)
    cf["definition"] = cf["description"].map(lambda d: _pbox(_W_DEF, latex_escape(d)))
    return cf


def _catalog_body(cf: pd.DataFrame) -> str:
    cols = [("name_tt", "Metric", "raw"), ("use", "Use", "raw"), ("fam", "Fam.", "str"), ("prov", "Provenance", "raw"),
            ("dir", "Better", "str"), ("unit_txt", "Unit", "raw"), ("definition", "Definition", "raw")]
    head = "\\footnotesize\n\\setlength{\\tabcolsep}{3pt}\n"
    return head + to_booktabs(cf, cols, align="lllllll", group_col="family")


def tab_metric_catalog(out_dir) -> Path:
    """Compact catalog for the paper (fits \\textwidth and one page)."""
    cf = _catalog_frame(compact=True)
    meta = {"rows": len(cf), "families": sorted(cf["family"].unique().tolist()),
            "selection": "metrics with a role: H1..H8, secondary family, TOST band, pass@k predicate",
            "full_catalog": "tab_metric_catalog_full", "width": "textwidth"}
    return write_table(out_dir, "tab_metric_catalog", _catalog_body(cf), meta=meta)


def tab_metric_catalog_full(out_dir) -> Path:
    """Full catalog for the replication package (longer than a page; not input by the paper)."""
    cf = _catalog_frame(compact=False)
    out = Path(out_dir) / "tables"
    out.mkdir(parents=True, exist_ok=True)
    cf[["name", "family", "provenance", "direction", "dtype", "unit", "description"]].assign(
        roles=cf["roles"].map(lambda r: "; ".join(r).replace("$\\pm$", "+-").replace("$k$", "k"))
    ).to_csv(out / "tab_metric_catalog_full.csv", index=False)
    return write_table(out_dir, "tab_metric_catalog_full", _catalog_body(cf),
                       meta={"rows": len(cf), "note": "full catalog for the replication package; longer than one page"})


def tab_integrity(bundle, out_dir) -> Path:
    runs = bundle["runs"]
    ex = bundle.get("excluded_refs")
    rows = [
        ("Cells (story x model x iteration)", len(runs), "design"),
        ("Cells with all four evidence trees", int((runs["evidence_completeness"] == 1.0).sum()), "tool-measured"),
        ("Cells without e2e evidence", int(runs["e2e_execs"].isna().sum()), "tool-measured"),
        ("Build executions recorded", int(pd.to_numeric(runs["build_execs"], errors="coerce").sum()), "tool-measured"),
        ("Test executions recorded", int(pd.to_numeric(runs["test_execs"], errors="coerce").sum()), "tool-measured"),
        ("Metrics snapshots recorded", int(pd.to_numeric(runs["metrics_snapshots"], errors="coerce").sum()), "tool-measured"),
        ("E2e executions recorded", int(pd.to_numeric(runs["e2e_execs"], errors="coerce").sum()), "tool-measured"),
        ("Run branches resolved (two run commits each)", int(runs["n_run_commits"].eq(2).sum()), "derived-from-git"),
        ("Cells whose agent files name the other model", int(runs["identity_ambiguous"].fillna(False).astype(bool).sum()), "derived-from-git"),
        ("Runs that excluded generated tests from compilation", int((pd.to_numeric(runs["compile_remove_added"], errors="coerce").fillna(0) > 0).sum()), "derived-from-git"),
        ("Runs with a first test execution of zero tests (compile-red)", int(runs["compile_red"].fillna(False).astype(bool).sum()), "tool-measured"),
        ("Runs not ending green on every check", int((runs["exceptional_ending"].fillna("") != "").sum()), "tool-measured"),
        ("Agent intent counts that disagree with the intent file", int((runs["intents_confirmed_ok"] == False).sum()), "self-reported"),
    ]
    # Excluded model arms are never mentioned in the report (STATUS.md decision D11, 2026-10-04); they stay in
    # data/excluded_refs.csv for provenance only.
    df = pd.DataFrame(rows, columns=["item", "value", "provenance"])
    body = to_booktabs(df, [("item", "Item", "str"), ("value", "Count", "int"), ("provenance", "Provenance", "str")], align="p{7cm}rl")
    return write_table(out_dir, "tab_integrity", body, meta={"rows": len(df)})


def tab_descriptives(frames, out_dir) -> Path:
    desc = frames["descriptives"]
    rows = []
    for m in DESCRIPTIVE_METRICS + BINARY_DESCRIPTIVES:
        if m not in set(desc["metric"]):
            continue
        spec = COLUMN_CATALOG[m]
        rows.append({"metric": f"\\texttt{{{latex_escape(m)}}}", "family": spec.family.split(" ")[0],
                     "kimi": _median_iqr(desc, m, KIMI), "qwen": _median_iqr(desc, m, QWEN),
                     "prov": spec.provenance.value, "n": int(desc[(desc["metric"] == m)]["n"].sum())})
    df = pd.DataFrame(rows)
    body = to_booktabs(df, [("metric", "Metric", "raw"), ("family", "Fam.", "str"), ("kimi", KIMI, "str"), ("qwen", QWEN, "str"),
                            ("n", "n", "int"), ("prov", "Provenance", "str")], align="llrrrl", group_col="family")
    return write_table(out_dir, "tab_descriptives", body, meta={"rows": len(df), "note": "median [Q1, Q3]; binaries as k/n (share)"})


def _effects_rows(eff: pd.DataFrame, families: tuple[str, ...]) -> pd.DataFrame:
    e = eff[eff["family"].isin(families)].copy()
    e["metric_tt"] = e["metric"].map(lambda n: f"\\texttt{{{latex_escape(n)}}}")
    e["med"] = e.apply(lambda r: f"{r['median_a']:.2f} vs {r['median_b']:.2f}", axis=1)
    e["delta_ci"] = e.apply(lambda r: f"{r['delta']:+.2f} [{r['ci_low']:+.2f}, {r['ci_high']:+.2f}]", axis=1)
    e["z_txt"] = e["z"].map(lambda z: "" if pd.isna(z) else f"{z:+.2f}")
    e["p_txt"] = e["p_used"].map(fmt_p)
    e["ph_txt"] = e["p_holm"].map(lambda p: "" if pd.isna(p) else fmt_p(p))
    e["dir"] = e.apply(lambda r: f"{int(r['stories_kimi_higher'])}/{int(r['stories_qwen_higher'])}/{int(r['stories_tie'])}", axis=1)
    e["i2_txt"] = e["i2"].map(lambda v: "" if pd.isna(v) else f"{v:.2f}")
    e["rob"] = e.get("robustness", pd.Series("", index=e.index)).fillna("")
    order = {"primary": 0, "secondary": 1, "exploratory": 2}
    return e.sort_values(["family", "delta"], key=lambda s: s.map(order) if s.name == "family" else -s).reset_index(drop=True)


def tab_effects(frames, out_dir) -> Path:
    eff = frames["effects"]
    e = _effects_rows(eff, ("primary", "secondary"))
    cols = [("metric_tt", "Metric", "raw"), ("family", "Family", "str"), ("med", "Median K vs Q", "str"),
            ("delta_ci", "Cliff's $\\delta$ [95\\% CI]", "str"), ("z_txt", "$z$", "str"), ("p_txt", "$p$", "str"),
            ("ph_txt", "$p_{\\mathrm{Holm}}$", "str"), ("dir", "Stories K/Q/tie", "str"), ("i2_txt", "$I^2$", "str"), ("rob", "Robustness", "str")]
    body = to_booktabs(e, cols, align="llrrrrrrrl", group_col="family", escape=False)
    # escape only the string cells that need it: metric is raw, headers are already LaTeX
    return write_table(out_dir, "tab_effects", body, meta={"rows": len(e), "note": "Kimi minus Qwen within story; permutation p; Holm within family"})


def tab_effects_exploratory(frames, out_dir) -> Path:
    e = _effects_rows(frames["effects"], ("exploratory",))
    cols = [("metric_tt", "Metric", "raw"), ("med", "Median K vs Q", "str"), ("delta_ci", "Cliff's $\\delta$ [95\\% CI]", "str"),
            ("p_txt", "$p$ (unadjusted)", "str"), ("dir", "Stories K/Q/tie", "str"), ("i2_txt", "$I^2$", "str")]
    body = to_booktabs(e, cols, align="lrrrrr", escape=False)
    return write_table(out_dir, "tab_effects_exploratory", body, meta={"rows": len(e), "note": "exploratory contrasts, no correction"})


def tab_equivalence(frames, out_dir) -> Path:
    t = frames["tost"].copy()
    t["metric_tt"] = t["metric"].map(lambda n: f"\\texttt{{{latex_escape(n)}}}")
    t["ci"] = t.apply(lambda r: f"{r['estimate']:+.3f} [{r['ci_low']:+.3f}, {r['ci_high']:+.3f}]", axis=1)
    t["band_txt"] = t["band"].map(lambda b: f"$\\pm${b:g}")
    t["dec"] = t["equivalent"].map(lambda v: "equivalent" if bool(v) else "not shown")
    t["small"] = t["smallest_equivalent_band"].map(lambda v: f"{v:.3f}")
    cols = [("metric_tt", "Metric", "raw"), ("ci", "$\\Delta$ [90\\% CI]", "str"), ("band_txt", "Band", "str"),
            ("dec", "Decision", "str"), ("small", "Smallest band", "str")]
    body = to_booktabs(t, cols, align="lrrlr", escape=False)
    return write_table(out_dir, "tab_equivalence", body, meta={"rows": len(t), "note": "mean over stories of per-story median differences"})


def tab_sensitivity(frames, out_dir) -> Path:
    sens = frames["sensitivity"]
    if sens.empty:
        return write_table(out_dir, "tab_sensitivity", "% no sensitivity frame\n")
    sets = [s for s in ["S0", "S1", "S2", "S5", "S7", "S8", "S4", "S10", "S6:pos1-3", "S6:pos4-10"] if s in set(sens["set"])]
    rows = []
    for metric, g in sens.groupby("metric", sort=False):
        fam = g["family"].iloc[0]
        row = {"metric": f"\\texttt{{{latex_escape(metric)}}}", "family": fam}
        for s in sets:
            r = g[g["set"] == s]
            if r.empty or pd.isna(r.iloc[0]["delta"]):
                row[s] = ""
            else:
                rr = r.iloc[0]
                star = "*" if (pd.notna(rr.get("reject")) and bool(rr["reject"])) else ""
                row[s] = f"{rr['delta']:+.2f}{star}"
        rows.append(row)
    df = pd.DataFrame(rows)
    order = {"primary": 0, "secondary": 1}
    df = df.sort_values("family", key=lambda s: s.map(order)).reset_index(drop=True)
    cols = [("metric", "Metric", "raw")] + [(s, s.replace(":", " "), "str") for s in sets]
    body = to_booktabs(df, cols, align="l" + "r" * len(sets), group_col="family", escape=False)
    return write_table(out_dir, "tab_sensitivity", body, meta={"rows": len(df), "sets": sets, "note": "* survives Holm within the set"})


def tab_paired_story(frames, out_dir) -> Path:
    e = _effects_rows(frames["effects"], ("primary", "secondary"))
    e["sign_txt"] = e["sign_p"].map(fmt_p) if "sign_p" in e.columns else ""
    e["wil_txt"] = e["wilcoxon_p"].map(fmt_p) if "wilcoxon_p" in e.columns else ""
    cols = [("metric_tt", "Metric", "raw"), ("family", "Family", "str"), ("dir", "Stories K/Q/tie", "str"),
            ("sign_txt", "Sign test $p$", "str"), ("wil_txt", "Wilcoxon $p$", "str"), ("i2_txt", "$I^2$", "str")]
    body = to_booktabs(e, cols, align="llrrrr", group_col="family", escape=False)
    return write_table(out_dir, "tab_paired_story", body,
                       meta={"rows": len(e), "note": "story-level paired tests on per-cell medians, n = 10 stories"})


def tab_passk(frames, out_dir) -> Path:
    pk = frames["passk"]
    rows = []
    for pred in C.SUCCESS_PREDICATES:
        for model in C.MODEL_ORDER:
            g = pk[(pk["predicate"] == pred) & (pk["group"] == model)]
            if g.empty:
                continue

            def val(est, k):
                r = g[(g["estimator"] == est) & (g["k"] == k)]
                return "" if r.empty else f"{r.iloc[0]['mean']:.2f}"
            rows.append({"pred": pred.replace("_", " "), "model": SHORT[model], "rate": f"{g['pooled_rate'].iloc[0]:.2f}",
                         "a1": val("pass_at_k", 1), "a3": val("pass_at_k", 3), "a6": val("pass_at_k", 6),
                         "h3": val("pass_hat_k", 3), "h6": val("pass_hat_k", 6)})
    df = pd.DataFrame(rows)
    cols = [("pred", "Outcome", "str"), ("model", "Model", "str"), ("rate", "Pooled rate", "str"), ("a1", "pass@1", "str"),
            ("a3", "pass@3", "str"), ("a6", "pass@6", "str"), ("h3", "pass$^3$", "str"), ("h6", "pass$^6$", "str")]
    body = to_booktabs(df, cols, align="llrrrrrr", group_col="pred", escape=False)
    return write_table(out_dir, "tab_passk", body, meta={"rows": len(df), "note": "means over stories of per-cell estimators from six runs"})


def tab_rankings(frames, out_dir) -> Path:
    rk = frames["rankings"].copy()
    rk["story_pos"] = rk["story"].map(C.STORY_POS)
    rows = []
    for story, g in sorted(rk.groupby("story"), key=lambda kv: C.STORY_POS[kv[0]]):
        g = g.sort_values("rank")
        top = g.head(3)
        rows.append({"story": f"{C.STORY_POS[story]}. {story}",
                     **{f"r{i + 1}": f"{SHORT[r['model']]}/{int(r['iteration'])} ({r['composite']:.2f}{'*' if r.get('pareto') else ''})"
                        for i, (_, r) in enumerate(top.iterrows())},
                     "kimi_top3": int((top["model"] == KIMI).sum()), "pareto_n": int(g["pareto"].sum()) if "pareto" in g else 0})
    df = pd.DataFrame(rows)
    cols = [("story", "Story", "str"), ("r1", "Rank 1", "str"), ("r2", "Rank 2", "str"), ("r3", "Rank 3", "str"),
            ("pareto_n", "Pareto set", "int")]
    body = to_booktabs(df, cols, align="lllll r"[::1].replace(" ", ""))
    return write_table(out_dir, "tab_rankings", body, meta={"rows": len(df), "note": "* Pareto-optimal; composite is story-local"})


def tab_provenance(bundle, frames, out_dir) -> Path:
    runs = bundle["runs"]
    rows = []
    for model in C.MODEL_ORDER:
        g = runs[runs["model"] == model]
        gap = pd.to_numeric(g["self_report_gap_methods"], errors="coerce")
        gapc = pd.to_numeric(g["self_report_gap_cases"], errors="coerce")
        rows.append({"model": SHORT[model], "n": int(gap.notna().sum()),
                     "exact": f"{int((gap == 0).sum())}/{int(gap.notna().sum())}",
                     "under": int((gap < 0).sum()), "over": int((gap > 0).sum()),
                     "abs_med": f"{gap.abs().median():.1f}", "abs_max": int(gap.abs().max()),
                     "cases_gap_med": f"{gapc.median():.1f}",
                     "intents_wrong": f"{int((g['intents_confirmed_ok'] == False).sum())}/{int(g['intents_confirmed_ok'].notna().sum())}",
                     "loop_gap_nonzero": int((pd.to_numeric(g["refactor_loop_gap"], errors="coerce").fillna(0) != 0).sum())})
    df = pd.DataFrame(rows)
    cols = [("model", "Model", "str"), ("exact", "Methods reported exactly", "str"), ("under", "Under", "int"), ("over", "Over", "int"),
            ("abs_med", "|gap| median", "str"), ("abs_max", "|gap| max", "int"), ("cases_gap_med", "Gap vs cases (median)", "str"),
            ("intents_wrong", "Intent count wrong", "str"), ("loop_gap_nonzero", "Loop count off", "int")]
    body = to_booktabs(df, cols, align="lrrrrrrrr")
    return write_table(out_dir, "tab_provenance", body, meta={"rows": len(df), "note": "self-reported fields compared with tool and git counts"})


def tab_exceptional_runs(bundle, out_dir) -> Path:
    runs = bundle["runs"]
    r = runs[(runs["exceptional_ending"].fillna("") != "") | runs["compile_red"].fillna(False).astype(bool)
             | runs["identity_ambiguous"].fillna(False).astype(bool) | (pd.to_numeric(runs["compile_remove_added"], errors="coerce").fillna(0) > 0)].copy()
    r["flags"] = r.apply(lambda x: "; ".join(filter(None, [
        x["exceptional_ending"] if isinstance(x["exceptional_ending"], str) and x["exceptional_ending"] else "",
        "compile-red first execution" if bool(x["compile_red"]) else "",
        "agent files name the other model" if bool(x["identity_ambiguous"]) else "",
        f"excluded {int(x['compile_remove_added'])} test file(s) from compilation" if pd.notna(x["compile_remove_added"]) and int(x["compile_remove_added"]) > 0 else "",
        "merged into the trunk" if bool(x["merged_into_trunk"]) else ""])), axis=1)
    r["run"] = r["run_id"].map(lambda s: f"\\texttt{{{latex_escape(s)}}}")
    r = r.sort_values(["story_pos", "model", "iteration"])
    body = to_booktabs(r, [("run", "Run", "raw"), ("flags", "Flags", "str")], align="lp{9cm}")
    return write_table(out_dir, "tab_exceptional_runs", body, meta={"rows": len(r)})


def tab_final_state(bundle, out_dir) -> Path:
    runs = bundle["runs"]
    rows = []
    for col, label in (("final_build_ok", "final build compiles"), ("final_tests_green", "final test execution green"),
                       ("final_e2e_ok", "final e2e execution passes"), ("final_all_ok", "all three")):
        row = {"outcome": label}
        for model in C.MODEL_ORDER:
            v = runs.loc[runs["model"] == model, col]
            row[model] = f"{int(v.fillna(False).astype(bool).sum())}/{int(v.notna().sum())}"
        rows.append(row)
    df = pd.DataFrame(rows)
    body = to_booktabs(df, [("outcome", "Final-state outcome", "str"), (KIMI, KIMI, "str"), (QWEN, QWEN, "str")], align="lrr")
    return write_table(out_dir, "tab_final_state", body, meta={"rows": len(df), "note": "nearly saturated; every run with a failed final check is listed in tab_exceptional_runs"})


def make_tables(bundle: dict, frames: dict, out_dir: Path) -> dict[str, str]:
    written = {}
    jobs = [("tab_stories", lambda: tab_stories(bundle, out_dir)), ("tab_metric_catalog", lambda: tab_metric_catalog(out_dir)),
            ("tab_metric_catalog_full", lambda: tab_metric_catalog_full(out_dir)),
            ("tab_integrity", lambda: tab_integrity(bundle, out_dir)), ("tab_descriptives", lambda: tab_descriptives(frames, out_dir)),
            ("tab_effects", lambda: tab_effects(frames, out_dir)), ("tab_effects_exploratory", lambda: tab_effects_exploratory(frames, out_dir)),
            ("tab_equivalence", lambda: tab_equivalence(frames, out_dir)), ("tab_sensitivity", lambda: tab_sensitivity(frames, out_dir)),
            ("tab_paired_story", lambda: tab_paired_story(frames, out_dir)),
            ("tab_passk", lambda: tab_passk(frames, out_dir)), ("tab_rankings", lambda: tab_rankings(frames, out_dir)),
            ("tab_provenance", lambda: tab_provenance(bundle, frames, out_dir)), ("tab_exceptional_runs", lambda: tab_exceptional_runs(bundle, out_dir)),
            ("tab_final_state", lambda: tab_final_state(bundle, out_dir))]
    for slug, fn in jobs:
        try:
            written[slug] = str(fn())
        except Exception as exc:
            written[slug] = f"ERROR {type(exc).__name__}: {exc}"
    return written
