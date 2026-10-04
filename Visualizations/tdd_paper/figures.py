"""Paper figures. One function per figure, registered in ``FIGURE_REGISTRY``.

Every function takes a ``FigureContext`` (the dataset tables plus the analysis
frames) and writes ``fig_<slug>.pdf``, ``.png`` and a JSON sidecar with every
plotted number through ``theme.savefig_with_sidecar``. Figures plot what the
analysis computed; they never filter or recompute statistics.
"""
from __future__ import annotations

import math
from dataclasses import dataclass, field
from pathlib import Path

import matplotlib.pyplot as plt
import numpy as np
import pandas as pd
from matplotlib.lines import Line2D
from matplotlib.patches import Patch

from . import config as C
from . import theme as T

KIMI, QWEN = C.MODEL_ORDER


@dataclass
class FigureContext:
    runs: pd.DataFrame
    executions: pd.DataFrame
    types: pd.DataFrame
    cells: pd.DataFrame
    stories: pd.DataFrame
    frames: dict = field(default_factory=dict)
    out_dir: Path = Path("_build/paper")
    seed: int = C.SEED
    runs_sha256: str | None = None
    filters: dict = field(default_factory=dict)

    def frame(self, name: str) -> pd.DataFrame:
        return self.frames.get(name, pd.DataFrame())


def _save(ctx: FigureContext, fig, slug: str, **kw) -> dict:
    kw.setdefault("seed", ctx.seed)
    kw.setdefault("runs_sha256", ctx.runs_sha256)
    kw.setdefault("filters", ctx.filters)
    return T.savefig_with_sidecar(fig, ctx.out_dir, slug, **kw)


def _jitter(n: int, seed: int, width: float = 0.08) -> np.ndarray:
    return np.random.default_rng(seed).uniform(-width, width, size=n)


def wilson(k: int, n: int, z: float = 1.96) -> tuple[float, float, float]:
    if n == 0:
        return (math.nan, math.nan, math.nan)
    p = k / n
    den = 1 + z * z / n
    centre = (p + z * z / (2 * n)) / den
    half = z * math.sqrt(p * (1 - p) / n + z * z / (4 * n * n)) / den
    return (p, max(0.0, centre - half), min(1.0, centre + half))


def _story_labels(stories: pd.DataFrame) -> list[str]:
    return [f"{int(p)}" for p in stories.sort_values("story_pos")["story_pos"]]


def _model_legend(ax, loc="upper left", **kw):
    handles = [Line2D([0], [0], marker=T.MODEL_STYLE[m]["marker"], color=T.MODEL_STYLE[m]["color"],
                      linestyle="none", markersize=4, label=m) for m in C.MODEL_ORDER]
    ax.legend(handles=handles, loc=loc, **kw)


# ---------------------------------------------------------------------------
# fig_chain
# ---------------------------------------------------------------------------


# Baseline author encoding for fig_chain: fill colour plus hatching, so the author is never
# carried by hue alone (grey-scale prints still separate Kimi-K2.5, Qwen3.7-max and human).
# Hatch ink is chosen per fill so it stays legible in grey-scale: white on the dark blue,
# black on the light orange.
_CHAIN_HATCH = {KIMI: ("///", "white"), QWEN: ("...", "black")}
_CHAIN_MACRO_WORD = {1: "One", 10: "Ten"}


def fig_chain(ctx: FigureContext) -> dict:
    st = ctx.stories.sort_values("story_pos").reset_index(drop=True)
    with plt.rc_context({"hatch.linewidth": 0.7}):
        fig, (ax1, ax2) = plt.subplots(2, 1, figsize=T.fig_size("text", height=3.0), sharex=True,
                                       gridspec_kw={"height_ratios": [1, 1.1]})
        x = st["story_pos"].to_numpy()
        authors = list(st["baseline_author_model"])
        colors = [T.model_color(m) if m in T.MODEL_STYLE else T.NEUTRAL for m in authors]
        ax1.bar(x, st["n_intents"], color=colors, edgecolor="white", width=0.7)
        # Hatching goes on a second, transparent bar layer so the hatch ink differs from the white outline.
        for xi, n, m in zip(x, st["n_intents"], authors):
            if m in _CHAIN_HATCH:
                hatch, ink = _CHAIN_HATCH[m]
                ax1.bar(xi, n, width=0.7, fill=False, hatch=hatch, edgecolor=ink, linewidth=0)
        for xi, n in zip(x, st["n_intents"]):
            ax1.text(xi, n + 0.6, f"{int(n)}", ha="center", va="bottom", fontsize=plt.rcParams["font.size"] - 2)
        ax1.set_ylabel("confirmed intents")
        ax2.plot(x, st["baseline_prod_lines"], color="black", marker="o", markersize=3, label="production lines inherited")
        ax2.set_ylabel("production lines")
        ax3 = ax2.twinx()
        ax3.plot(x, st["baseline_test_attrs"], color=T.NEUTRAL, marker="^", markersize=3, linestyle=":", label="test attributes inherited")
        ax3.set_ylabel("test attributes", color=T.NEUTRAL)
        ax3.grid(False)
        ax2.set_xticks(x)
        ax2.set_xticklabels([f"{int(p)}\n{s}" for p, s in zip(st["story_pos"], st["story"])], fontsize=plt.rcParams["font.size"] - 2.5)
        ax2.set_xlabel("story, in chain order")
        handles = [Patch(facecolor=T.model_color(KIMI), edgecolor=_CHAIN_HATCH[KIMI][1], hatch=_CHAIN_HATCH[KIMI][0],
                         linewidth=0, label=f"baseline authored by {KIMI}"),
                   Patch(facecolor=T.model_color(QWEN), edgecolor=_CHAIN_HATCH[QWEN][1], hatch=_CHAIN_HATCH[QWEN][0],
                         linewidth=0, label=f"baseline authored by {QWEN}"),
                   Patch(facecolor=T.NEUTRAL, edgecolor=T.NEUTRAL, linewidth=0, label="human baseline")]
        ax1.legend(handles=handles, loc="upper right", ncol=3, fontsize=plt.rcParams["font.size"] - 2.5,
                   handleheight=1.3, handlelength=2.2, bbox_to_anchor=(1.0, 1.28))
        lines = ax2.get_legend_handles_labels()[0] + ax3.get_legend_handles_labels()[0]
        ax2.legend(handles=lines, loc="upper left", fontsize=plt.rcParams["font.size"] - 2)
        data = {"stories": st[["story", "story_pos", "n_intents", "baseline_author_model", "merged_run",
                               "baseline_prod_lines", "baseline_test_attrs", "effort_minutes"]].to_dict("list")}
        data["encoding"] = {"baseline_author_model": {KIMI: {"fill": T.model_color(KIMI), "hatch": _CHAIN_HATCH[KIMI][0]},
                                                      QWEN: {"fill": T.model_color(QWEN), "hatch": _CHAIN_HATCH[QWEN][0]},
                                                      "human": {"fill": T.NEUTRAL, "hatch": ""}}}
        # Baseline sizes quoted in the caption (macro names of numbers.tex), read from the stories frame.
        numbers = {}
        for pos, word in _CHAIN_MACRO_WORD.items():
            row = st.loc[st["story_pos"] == pos].iloc[0]
            numbers[f"Story{word}BaselineProdLines"] = row["baseline_prod_lines"]
            numbers[f"Story{word}BaselineTests"] = row["baseline_test_attrs"]
        return _save(ctx, fig, "fig_chain", data=data, numbers=numbers, width_class="text", rq=None, kind="chain")


# ---------------------------------------------------------------------------
# fig_evidence
# ---------------------------------------------------------------------------


def fig_evidence(ctx: FigureContext) -> dict:
    r = ctx.runs.sort_values(["story_pos", "model", "iteration"])
    rows = [(s, m) for s in C.STORY_ORDER for m in C.MODEL_ORDER if ((r["story"] == s) & (r["model"] == m)).any()]
    fig, ax = plt.subplots(figsize=T.fig_size("column", height=4.2))
    grid = np.full((len(rows), 6), np.nan)
    marks = {}
    for i, (s, m) in enumerate(rows):
        g = r[(r["story"] == s) & (r["model"] == m)]
        for _, row in g.iterrows():
            j = int(row["iteration"]) - 1
            grid[i, j] = float(row["evidence_completeness"]) if pd.notna(row["evidence_completeness"]) else np.nan
            glyph = ""
            if bool(row.get("identity_ambiguous")):
                glyph += "i"
            if pd.notna(row.get("compile_remove_added")) and int(row["compile_remove_added"]) > 0:
                glyph += "c"
            if str(row.get("exceptional_ending") or ""):
                glyph += "!"
            if bool(row.get("compile_red")):
                glyph += "r"
            if glyph:
                marks[(i, j)] = glyph
    cmap = plt.get_cmap("Greys")
    ax.imshow(1 - np.nan_to_num(grid, nan=0.0), cmap=cmap, vmin=-0.2, vmax=1.0, aspect="auto")
    for (i, j), g in marks.items():
        ax.text(j, i, g, ha="center", va="center", fontsize=plt.rcParams["font.size"] - 2, color="black")
    ax.set_xticks(range(6))
    ax.set_xticklabels([str(k) for k in range(1, 7)])
    ax.set_yticks(range(len(rows)))
    ax.set_yticklabels([f"{C.STORY_POS[s]} {C.MODEL_SHORT[m]}" for s, m in rows], fontsize=plt.rcParams["font.size"] - 2)
    ax.set_xlabel("iteration")
    ax.grid(False)
    for spine in ax.spines.values():
        spine.set_visible(False)
    ax.set_title("white = all four evidence trees; grey = three. i identity ambiguous, c compile exclusion, ! exceptional ending, r compile-red",
                 fontsize=plt.rcParams["font.size"] - 2.5, loc="left")
    data = {"rows": [f"{s}/{m}" for s, m in rows], "completeness": grid.tolist(),
            "marks": {f"{rows[i][0]}/{rows[i][1]}/{j + 1}": g for (i, j), g in marks.items()}}
    return _save(ctx, fig, "fig_evidence", data=data, width_class="column", kind="integrity")


# ---------------------------------------------------------------------------
# fig_retry_paths
# ---------------------------------------------------------------------------


def fig_retry_paths(ctx: FigureContext, pick: tuple[str, str] | None = None) -> dict:
    ex = ctx.executions
    if pick is None:
        pick = ("CPD-LC-001-003/Kimi-K2.5/1", "CPD-LC-001-003/Qwen3.7-max/4")
    lanes = ["build", "test", "metrics", "e2e"]
    fig, axes = plt.subplots(2, 1, figsize=T.fig_size("text", height=2.6), sharex=False)
    data = {}
    for ax, run_id in zip(axes, pick):
        g = ex[ex["run_id"] == run_id].sort_values("ts").reset_index(drop=True)
        t0 = g["ts"].min()
        minutes = ((g["ts"] - t0) / pd.Timedelta(minutes=1)).to_numpy()
        xs = np.arange(len(g), dtype=float)  # execution order; wall-clock pauses would squash the path
        series = []
        for k, row in g.iterrows():
            lane = lanes.index(row["tree"])
            if row["tree"] == "metrics":
                color, marker = T.STATUS["snapshot"], "D"
            elif row["tree"] == "e2e" and not bool(row.get("backend_up", True)):
                color, marker = T.STATUS["infra"], "x"
            else:
                ok = row["status"] == "success"
                color, marker = (T.STATUS["green"] if ok else T.STATUS["red"]), ("o" if ok else "o")
            ax.scatter(xs[k], lane, color=color, marker=marker, s=14, zorder=3, edgecolor="white", linewidth=0.3)
            label = ""
            if row["tree"] == "build" and row["status"] != "success" and pd.notna(row.get("total_errors")):
                label = f"{int(row['total_errors'])}"
            if row["tree"] == "test" and pd.notna(row.get("failed")) and row["status"] != "success":
                label = f"{int(row['failed'])}f"
            if row["tree"] == "e2e" and pd.notna(row.get("probes_passed")) and pd.notna(row.get("total_probes")):
                label = f"{int(row['probes_passed'])}/{int(row['total_probes'])}"
            if label:
                ax.text(xs[k], lane + 0.32, label, ha="center", va="bottom", fontsize=plt.rcParams["font.size"] - 3)
            series.append({"order": int(k), "minute": float(minutes[k]), "tree": row["tree"], "status": row["status"], "label": label})
        ax.set_yticks(range(len(lanes)))
        ax.set_yticklabels(lanes)
        ax.set_ylim(-0.6, len(lanes) - 0.3)
        ax.set_xlim(-0.8, max(len(g) - 0.2, 1))
        span = float(minutes[-1]) if len(minutes) else 0.0
        ax.set_title(f"{run_id}   ({len(g)} recorded executions over {span:.0f} min of wall clock)", loc="left",
                     fontsize=plt.rcParams["font.size"] - 1)
        ax.grid(axis="x", color=T.LIGHTER)
        ax.grid(axis="y", visible=False)
        ax.set_xticks(xs)
        ax.set_xticklabels([str(int(x) + 1) for x in xs], fontsize=plt.rcParams["font.size"] - 2.5)
        data[run_id] = series
    axes[-1].set_xlabel("recorded executions in time order (all four trees interleaved)")
    handles = [Line2D([0], [0], marker="o", color=T.STATUS["red"], linestyle="none", label="failed execution"),
               Line2D([0], [0], marker="o", color=T.STATUS["green"], linestyle="none", label="passed execution"),
               Line2D([0], [0], marker="x", color=T.STATUS["infra"], linestyle="none", label="e2e infrastructure failure"),
               Line2D([0], [0], marker="D", color=T.STATUS["snapshot"], linestyle="none", label="metrics snapshot")]
    axes[0].legend(handles=handles, loc="lower center", bbox_to_anchor=(0.5, 1.12), ncol=4,
                   fontsize=plt.rcParams["font.size"] - 2.5)
    return _save(ctx, fig, "fig_retry_paths", data=data, width_class="text", rq="RQ3", kind="paths",
                 caveats=["numbers above failed builds are compiler errors; above test runs failed tests; above e2e probes passed/total"])


# ---------------------------------------------------------------------------
# fig_convergence
# ---------------------------------------------------------------------------


def _strip_with_pairs(ax, runs: pd.DataFrame, metric: str, seed: int, log: bool = False) -> dict:
    out = {}
    pos = {KIMI: 0, QWEN: 1}
    for m in C.MODEL_ORDER:
        v = pd.to_numeric(runs.loc[runs["model"] == m, metric], errors="coerce").dropna()
        x = pos[m] + _jitter(len(v), seed + pos[m])
        ax.scatter(x, v, s=7, color=T.MODEL_STYLE[m]["color"], marker=T.MODEL_STYLE[m]["marker"], alpha=0.55,
                   edgecolor="none", zorder=2)
        out[m] = {"values": v.tolist(), "median": float(v.median()) if len(v) else None, "n": int(len(v))}
    med = runs.groupby(["story", "model"])[metric].median().unstack("model")
    for story, row in med.iterrows():
        if KIMI in row and QWEN in row and pd.notna(row[KIMI]) and pd.notna(row[QWEN]):
            ax.plot([0, 1], [row[KIMI], row[QWEN]], color=T.NEUTRAL, linewidth=0.5, alpha=0.6, zorder=1)
    for m in C.MODEL_ORDER:
        if out[m]["median"] is not None:
            ax.plot([pos[m] - 0.22, pos[m] + 0.22], [out[m]["median"]] * 2, color="black", linewidth=1.4, zorder=4)
    ax.set_xticks([0, 1])
    ax.set_xticklabels([C.MODEL_SHORT[m] for m in C.MODEL_ORDER])
    ax.set_xlim(-0.5, 1.5)
    if log:
        ax.set_yscale("symlog", linthresh=1)
        ax.set_ylim(bottom=-0.3)
    out["story_medians"] = med.reset_index().to_dict("list")
    return out


def fig_convergence(ctx: FigureContext) -> dict:
    panels = [("build_failed_execs", "failed build executions", False), ("build_errors_burned", "compiler errors burned", True),
              ("attempts_to_green", "test executions to first green", False), ("e2e_execs", "e2e executions", False)]
    fig, axes = plt.subplots(1, 4, figsize=T.fig_size("text", height=2.0))
    data = {}
    eff = ctx.frame("effects")
    for ax, (metric, label, log) in zip(axes, panels):
        data[metric] = _strip_with_pairs(ax, ctx.runs, metric, ctx.seed, log=log)
        row = eff[eff["metric"] == metric] if not eff.empty else pd.DataFrame()
        title = label
        if not row.empty:
            r = row.iloc[0]
            title += f"\nδ = {r['delta']:+.2f} [{r['ci_low']:+.2f}, {r['ci_high']:+.2f}]"
            data[metric]["delta"] = {"delta": r["delta"], "ci_low": r["ci_low"], "ci_high": r["ci_high"]}
        ax.set_title(title, fontsize=plt.rcParams["font.size"] - 1, loc="left")
    return _save(ctx, fig, "fig_convergence", data=data, width_class="text", rq="RQ3", kind="strips",
                 caveats=["grey lines join per-story medians; black bars are pooled medians"])


# ---------------------------------------------------------------------------
# fig_forest_delta
# ---------------------------------------------------------------------------


def fig_forest_delta(ctx: FigureContext, metrics: list[str] | None = None) -> dict:
    eff = ctx.frame("effects")
    bys = ctx.frame("effects_by_story")
    if eff.empty:
        raise ValueError("effects frame missing")
    if metrics is None:
        fam = eff[eff["family"].isin(["primary", "secondary"])]
        extra = eff[(eff["family"] == "exploratory") & eff["metric"].isin(
            ["median_mi", "line_rate", "max_coupling", "story_min_mi", "story_pct_green", "min_mi_gain", "green_share_gain",
             "e2e_probe_pass_rate", "new_layer_balance"])]
        rows = pd.concat([fam, extra])
    else:
        rows = eff[eff["metric"].isin(metrics)]
    rows = rows.dropna(subset=["delta"]).copy()
    rows["order"] = rows["family"].map({"primary": 0, "secondary": 1, "exploratory": 2})
    rows = rows.sort_values(["order", "delta"], ascending=[True, False]).reset_index(drop=True)
    fig, ax = plt.subplots(figsize=T.fig_size("text", height=0.22 * len(rows) + 0.9))
    y = np.arange(len(rows))[::-1]
    for yi, (_, r) in zip(y, rows.iterrows()):
        if not bys.empty:
            per = bys[(bys["metric"] == r["metric"]) & (bys["set"] == "S0")] if "set" in bys.columns else bys[bys["metric"] == r["metric"]]
            ax.scatter(per["delta"], np.full(len(per), yi) + _jitter(len(per), ctx.seed, 0.12), s=5, color=T.LIGHT, zorder=1)
        ax.plot([r["ci_low"], r["ci_high"]], [yi, yi], color="black", linewidth=0.9, zorder=2)
        filled = bool(r.get("reject")) if pd.notna(r.get("reject")) else False
        ax.scatter([r["delta"]], [yi], s=22, facecolor="black" if filled else "white", edgecolor="black", zorder=3,
                   marker="D" if r["family"] == "primary" else ("s" if r["family"] == "secondary" else "o"))
        note = f"{r['delta']:+.2f}"
        if pd.notna(r.get("p_holm")):
            note += f"  p$_H$={r['p_holm']:.3f}" if r["p_holm"] >= 0.001 else "  p$_H$<0.001"
        if pd.notna(r.get("i2")):
            note += f"  I²={r['i2']:.2f}"
        ax.text(1.03, yi, note, va="center", ha="left", fontsize=plt.rcParams["font.size"] - 2.5, transform=ax.get_yaxis_transform())
    ax.axvline(0, color=T.NEUTRAL, linewidth=0.8)
    ax.set_yticks(y)
    ax.set_yticklabels([f"{m}  ({f[0]})" for m, f in zip(rows["metric"], rows["family"])], fontsize=plt.rcParams["font.size"] - 2)
    ax.set_xlim(-1.05, 1.05)
    ax.set_xlabel("Cliff's δ (Kimi minus Qwen), 95% stratified bootstrap interval; grey dots = per-story δ")
    ax.grid(axis="x", color=T.LIGHTER)
    ax.grid(axis="y", visible=False)
    data = {"rows": rows[["metric", "family", "delta", "ci_low", "ci_high", "p_used", "p_holm", "reject", "i2",
                          "stories_kimi_higher", "stories_qwen_higher"]].to_dict("list")}
    return _save(ctx, fig, "fig_forest_delta", data=data, width_class="text", rq="all", kind="forest",
                 caveats=["(p) primary family, (s) secondary family, (e) exploratory; filled markers survive Holm within their family"])


# ---------------------------------------------------------------------------
# fig_fidelity
# ---------------------------------------------------------------------------


def _by_story_strips(ax, runs: pd.DataFrame, metric: str, seed: int, ref_line: float | None = None) -> dict:
    data = {}
    offs = {KIMI: -0.18, QWEN: 0.18}
    for m in C.MODEL_ORDER:
        g = runs[runs["model"] == m]
        x = g["story_pos"].astype(float) + offs[m] + _jitter(len(g), seed + offs[m].__hash__() % 97, 0.06)
        v = pd.to_numeric(g[metric], errors="coerce")
        ax.scatter(x, v, s=6, color=T.MODEL_STYLE[m]["color"], marker=T.MODEL_STYLE[m]["marker"], alpha=0.6, edgecolor="none")
        med = g.groupby("story_pos")[metric].median()
        ax.plot(med.index.astype(float) + offs[m], med.values, color=T.MODEL_STYLE[m]["color"],
                linestyle=T.MODEL_STYLE[m]["linestyle"], linewidth=0.9)
        data[m] = {"story_pos": g["story_pos"].astype(int).tolist(), "values": v.tolist(), "medians": med.to_dict()}
    if ref_line is not None:
        ax.axhline(ref_line, color=T.NEUTRAL, linewidth=0.7, linestyle=":")
    ax.set_xticks(range(1, 11))
    ax.set_xticklabels([str(i) for i in range(1, 11)])
    ax.set_xlabel("story position in the chain")
    return data


def fig_fidelity(ctx: FigureContext) -> dict:
    fig, axes = plt.subplots(2, 1, figsize=T.fig_size("column", height=3.4), sharex=True)
    d1 = _by_story_strips(axes[0], ctx.runs, "new_tests_per_intent", ctx.seed, ref_line=1.0)
    axes[0].set_ylabel("story-new test methods\nper confirmed intent")
    d2 = _by_story_strips(axes[1], ctx.runs, "new_layer_balance", ctx.seed + 1)
    axes[1].set_ylabel("layer balance")
    axes[1].set_xlabel("story (chain order)")
    _model_legend(axes[0], loc="upper right")
    return _save(ctx, fig, "fig_fidelity", data={"new_tests_per_intent": d1, "new_layer_balance": d2},
                 width_class="column", rq="RQ1", kind="strips")


# ---------------------------------------------------------------------------
# fig_red_first
# ---------------------------------------------------------------------------


def fig_red_first(ctx: FigureContext) -> dict:
    r = ctx.runs
    fig, axes = plt.subplots(2, 1, figsize=T.fig_size("text", height=2.8), sharex=True)
    data = {"red_first": {}, "first_run_new_fail_share": {}}
    offs = {KIMI: -0.15, QWEN: 0.15}
    for m in C.MODEL_ORDER:
        xs, ps, lo, hi = [], [], [], []
        for pos, g in r[r["model"] == m].groupby("story_pos"):
            v = g["red_first"].dropna().astype(bool)
            p, l, h = wilson(int(v.sum()), int(len(v)))
            xs.append(float(pos) + offs[m]); ps.append(p); lo.append(l); hi.append(h)
        yerr = [np.clip(np.array(ps) - np.array(lo), 0, None), np.clip(np.array(hi) - np.array(ps), 0, None)]
        axes[0].errorbar(xs, ps, yerr=yerr, fmt=T.MODEL_STYLE[m]["marker"],
                         color=T.MODEL_STYLE[m]["color"], markersize=3.5, capsize=1.5, linewidth=0.8)
        data["red_first"][m] = {"story_pos": xs, "share": ps, "lo": lo, "hi": hi}
    axes[0].axvspan(0.5, 4.5, color=T.LIGHTER, zorder=0)
    axes[0].text(2.5, 1.04, "stories analysed in the artifact", ha="center", va="bottom", fontsize=plt.rcParams["font.size"] - 2.5, color=T.NEUTRAL)
    axes[0].set_ylim(-0.03, 1.08)
    axes[0].set_ylabel("first-execution red\n(share of runs, Wilson 95%)")
    _model_legend(axes[0], loc="lower left")
    d2 = _by_story_strips(axes[1], r, "first_run_new_fail_share", ctx.seed + 7)
    cr = r[r["compile_red"].fillna(False).astype(bool)]
    for _, row in cr.iterrows():
        ha = "right" if row["model"] == KIMI else "left"
        axes[1].annotate("compile-red", (float(row["story_pos"]) + offs[row["model"]], 1.0),
                         fontsize=plt.rcParams["font.size"] - 3, ha=ha, va="bottom", color=T.NEUTRAL)
    axes[1].set_ylabel("share of the run's own\ntest cases not passing")
    axes[1].set_xlabel("story (chain order)")
    data["first_run_new_fail_share"] = d2
    return _save(ctx, fig, "fig_red_first", data=data, width_class="text", rq="RQ2", kind="rates",
                 caveats=["first-execution red: the first recorded suite execution, after an implementation attempt, was not green"])


# ---------------------------------------------------------------------------
# fig_quality
# ---------------------------------------------------------------------------


def fig_quality(ctx: FigureContext) -> dict:
    panels = [("story_min_mi", "mi", "worst MI (story types)"), ("story_max_cc", "cc", "worst CC (story types)"),
              ("story_max_coupling", "coupling", "worst coupling (story types)"), ("story_min_mi_gain", None, "worst-MI gain from refactoring")]
    fig, axes = plt.subplots(1, 4, figsize=T.fig_size("text", height=2.0))
    data = {}
    for ax, (metric, band_metric, label) in zip(axes, panels):
        if metric not in ctx.runs.columns:
            continue
        data[metric] = _strip_with_pairs(ax, ctx.runs, metric, ctx.seed)
        ax.set_title(label, fontsize=plt.rcParams["font.size"] - 1, loc="left")
        if band_metric:
            T.threshold_bands(ax, band_metric, axis="y")
    return _save(ctx, fig, "fig_quality", data=data, width_class="text", rq="RQ4", kind="strips",
                 caveats=["MI is the harness variant without Halstead volume; grey bands are the harness thresholds"])


# ---------------------------------------------------------------------------
# fig_equivalence
# ---------------------------------------------------------------------------


def fig_equivalence(ctx: FigureContext) -> dict:
    tost = ctx.frame("tost")
    if tost.empty:
        raise ValueError("tost frame missing")
    fig, axes = plt.subplots(len(tost), 1, figsize=T.fig_size("column", height=0.42 * len(tost) + 0.6))
    axes = np.atleast_1d(axes)
    data = {}
    for ax, (_, r) in zip(axes, tost.iterrows()):
        band = float(r["band"])
        ax.axvspan(-band, band, color=T.LIGHTER, zorder=0)
        ax.plot([r["ci_low"], r["ci_high"]], [0, 0], color="black", linewidth=1.2)
        ax.scatter([r["estimate"]], [0], s=18, color="black", zorder=3)
        span = max(2 * band, abs(r["ci_low"]) * 1.1, abs(r["ci_high"]) * 1.1)
        ax.set_xlim(-span, span)
        ax.set_yticks([])
        ax.set_ylabel(r["metric"], rotation=0, ha="right", va="center", fontsize=plt.rcParams["font.size"] - 2)
        ax.text(1.02, 0, "equivalent" if bool(r["equivalent"]) else "not shown", transform=ax.get_yaxis_transform(),
                va="center", fontsize=plt.rcParams["font.size"] - 2.5)
        ax.grid(False)
        for s in ("top", "right", "left"):
            ax.spines[s].set_visible(False)
        data[r["metric"]] = {"estimate": r["estimate"], "ci_low": r["ci_low"], "ci_high": r["ci_high"], "band": band,
                             "equivalent": bool(r["equivalent"]), "smallest_equivalent_band": r["smallest_equivalent_band"]}
    axes[-1].set_xlabel("Kimi minus Qwen, mean of per-story median differences (90% interval) against the band")
    return _save(ctx, fig, "fig_equivalence", data=data, width_class="column", rq="RQ4", kind="tost")


# ---------------------------------------------------------------------------
# fig_passk
# ---------------------------------------------------------------------------


def fig_passk(ctx: FigureContext, predicates: tuple[str, ...] = ("build_first_pass", "green_first_try", "e2e_first_pass", "clean_run")) -> dict:
    pk = ctx.frame("passk")
    if pk.empty:
        raise ValueError("passk frame missing")
    preds = [p for p in predicates if p in set(pk["predicate"])]
    fig, axes = plt.subplots(2, len(preds), figsize=T.fig_size("text", height=2.6), sharex=True, sharey=True)
    axes = np.atleast_2d(axes)
    data = {}
    for j, pred in enumerate(preds):
        for i, est in enumerate(("pass_at_k", "pass_hat_k")):
            ax = axes[i, j]
            for m in C.MODEL_ORDER:
                g = pk[(pk["predicate"] == pred) & (pk["estimator"] == est) & (pk["group"] == m)].sort_values("k")
                ax.plot(g["k"], g["mean"], color=T.MODEL_STYLE[m]["color"], marker=T.MODEL_STYLE[m]["marker"],
                        linestyle=T.MODEL_STYLE[m]["linestyle"], markersize=3)
                ax.fill_between(g["k"], g["ci_low"], g["ci_high"], color=T.MODEL_STYLE[m]["color"], alpha=0.12, linewidth=0)
                data[f"{pred}/{est}/{m}"] = g[["k", "mean", "ci_low", "ci_high"]].to_dict("list")
            if i == 0:
                ax.set_title(pred.replace("_", " "), fontsize=plt.rcParams["font.size"] - 1, loc="left")
            if j == 0:
                ax.set_ylabel("pass@k" if est == "pass_at_k" else "pass^k")
            ax.set_ylim(0, 1.02)
            ax.set_xticks(range(1, 7))
    for ax in axes[-1]:
        ax.set_xlabel("k runs")
    handles = [Line2D([0], [0], marker=T.MODEL_STYLE[m]["marker"], color=T.MODEL_STYLE[m]["color"],
                      linestyle=T.MODEL_STYLE[m]["linestyle"], markersize=4, label=m) for m in C.MODEL_ORDER]
    fig.legend(handles=handles, loc="lower center", bbox_to_anchor=(0.5, -0.06), ncol=2, frameon=False)
    return _save(ctx, fig, "fig_passk", data=data, width_class="text", rq="RQ5", kind="curves",
                 caveats=["mean over stories of the per-cell estimator from six runs; ribbons are story-bootstrap 95% intervals"])


# ---------------------------------------------------------------------------
# fig_rankings
# ---------------------------------------------------------------------------


def fig_rankings(ctx: FigureContext) -> dict:
    rk = ctx.frame("rankings")
    if rk.empty:
        raise ValueError("rankings frame missing")
    rk = rk.copy()
    rk["story_pos"] = rk["story"].map(C.STORY_POS)
    stories = sorted(rk["story"].unique(), key=lambda s: C.STORY_POS[s])
    fig, ax = plt.subplots(figsize=T.fig_size("text", height=2.6))
    data = {}
    for i, story in enumerate(stories):
        g = rk[rk["story"] == story].sort_values("rank")
        for _, r in g.iterrows():
            x = int(r["rank"]) - 1
            ax.add_patch(plt.Rectangle((x - 0.5, i - 0.45), 1, 0.9, color=T.model_color(r["model"]), alpha=0.85 if r.get("pareto") else 0.45))
            ax.text(x, i, f"{int(r['iteration'])}{'*' if r.get('pareto') else ''}", ha="center", va="center",
                    fontsize=plt.rcParams["font.size"] - 2, color="white")
        data[story] = g[["rank", "model", "iteration", "composite", "pareto", "top1_share"]].to_dict("list")
    ax.set_xlim(-0.5, 11.5)
    ax.set_ylim(-0.6, len(stories) - 0.4)
    ax.set_xticks(range(12))
    ax.set_xticklabels([str(k + 1) for k in range(12)])
    ax.set_yticks(range(len(stories)))
    ax.set_yticklabels([f"{C.STORY_POS[s]}. {s}" for s in stories], fontsize=plt.rcParams["font.size"] - 2)
    ax.invert_yaxis()
    ax.set_xlabel("composite rank within the story (1 = best); cell label = iteration, * = Pareto-optimal")
    ax.grid(False)
    handles = [Patch(color=T.model_color(m), label=m) for m in C.MODEL_ORDER]
    ax.legend(handles=handles, loc="upper center", bbox_to_anchor=(0.5, -0.28), ncol=2)
    return _save(ctx, fig, "fig_rankings", data=data, width_class="text", rq="RQ5", kind="heatmap",
                 caveats=["composite score is a merge-decision aid (exploratory); composites are story-local and must not be compared across stories"])


# ---------------------------------------------------------------------------
# fig_self_report
# ---------------------------------------------------------------------------


def fig_self_report(ctx: FigureContext) -> dict:
    r = ctx.runs
    fig, axes = plt.subplots(1, 2, figsize=T.fig_size("column", height=1.9), sharey=True)
    data = {}
    for ax, xcol, label in zip(axes, ("new_test_methods", "new_test_cases"), ("test methods added (branch diff)", "test cases added (attributes)")):
        for m in C.MODEL_ORDER:
            g = r[r["model"] == m]
            ax.scatter(g[xcol], g["tg_test_methods"], s=8, color=T.MODEL_STYLE[m]["color"], marker=T.MODEL_STYLE[m]["marker"],
                       alpha=0.65, edgecolor="none")
            data[f"{xcol}/{m}"] = {"x": pd.to_numeric(g[xcol], errors="coerce").tolist(), "y": pd.to_numeric(g["tg_test_methods"], errors="coerce").tolist()}
        lim = max(float(pd.to_numeric(r[xcol], errors="coerce").max()), float(pd.to_numeric(r["tg_test_methods"], errors="coerce").max())) + 2
        ax.plot([0, lim], [0, lim], color=T.NEUTRAL, linewidth=0.7, linestyle=":")
        ax.set_xlabel(label)
        ax.set_xlim(0, lim)
        ax.set_ylim(0, lim)
    axes[0].set_ylabel("testMethodsEmitted (agent report)")
    _model_legend(axes[0], loc="upper left")
    return _save(ctx, fig, "fig_self_report", data=data, width_class="column", rq="RQ6", kind="scatter",
                 caveats=["left: like-for-like (methods); right: cases after [TestCase] expansion, the unit mismatch that inflates an apparent under-report"])


# ---------------------------------------------------------------------------
# fig_behavioural
# ---------------------------------------------------------------------------


def fig_behavioural(ctx: FigureContext) -> dict:
    fig, axes = plt.subplots(1, 3, figsize=T.fig_size("text", height=1.9))
    data = {}
    for ax, (metric, label) in zip(axes, (("e2e_attempts_to_pass_noinfra", "e2e attempts to pass\n(infrastructure failures removed)"),
                                           ("e2e_infra_failures", "e2e infrastructure failures"),
                                           ("e2e_probe_depth", "probes in the final e2e run\n(agent-authored)"))):
        if metric in ctx.runs.columns:
            data[metric] = _strip_with_pairs(ax, ctx.runs, metric, ctx.seed)
            ax.set_title(label, fontsize=plt.rcParams["font.size"] - 1, loc="left")
    return _save(ctx, fig, "fig_behavioural", data=data, width_class="text", rq="RQ3", kind="strips",
                 caveats=["probes and seeds are written by the same agent that wrote the code; probe body assertions are not recorded"])


FIGURE_REGISTRY = {
    "fig_chain": fig_chain, "fig_evidence": fig_evidence, "fig_retry_paths": fig_retry_paths,
    "fig_convergence": fig_convergence, "fig_forest_delta": fig_forest_delta, "fig_fidelity": fig_fidelity,
    "fig_red_first": fig_red_first, "fig_quality": fig_quality, "fig_equivalence": fig_equivalence,
    "fig_passk": fig_passk, "fig_rankings": fig_rankings, "fig_self_report": fig_self_report,
    "fig_behavioural": fig_behavioural,
}


def make_figures(ctx: FigureContext, only: list[str] | None = None, progress=None) -> dict[str, dict]:
    T.paper_theme()
    out = {}
    for slug, fn in FIGURE_REGISTRY.items():
        if only and slug not in only:
            continue
        try:
            out[slug] = fn(ctx)
        except Exception as exc:  # keep going; report at the end
            out[slug] = {"figure": slug, "error": f"{type(exc).__name__}: {exc}"}
            plt.close("all")
        if progress:
            progress(slug, "error" in out[slug])
    return out
