"""Paper figures. One function per figure, registered in ``FIGURE_REGISTRY``.

Every function takes a ``FigureContext`` (the dataset tables plus the analysis
frames) and writes ``fig_<slug>.pdf``, ``.png`` and a JSON sidecar with every
plotted number through ``theme.savefig_with_sidecar``. Figures plot what the
analysis computed; they never filter or recompute statistics.
"""
from __future__ import annotations

import ast
import json
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
    if T.ieee_mode() and metrics is None:
        return fig_forest_delta_ieee(ctx)
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
    if T.ieee_mode():
        return fig_fidelity_ieee(ctx)
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
    if T.ieee_mode():
        return fig_red_first_ieee(ctx)
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
    if T.ieee_mode():
        return fig_equivalence_ieee(ctx)
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
    if T.ieee_mode():
        return fig_passk_ieee(ctx)
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
    if T.ieee_mode():
        return fig_self_report_ieee(ctx)
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


# ---------------------------------------------------------------------------
# IEEE variants (TDD_PAPER_THEME=ieee): the figures of the SANER 2027 paper at one IEEEtran column
# (3.486 in), Times face, readable metric names (P31), no robustness marks (P37), stories as the anonymous
# labels US1 to US10 (P43). Only drawing code lives here; the rows, intervals and bands are exactly what
# analysis.py produced. These functions run only under the switch, so the report's figures above are untouched.
# ---------------------------------------------------------------------------

IEEE_ADAPTED = ("fig_forest_delta", "fig_equivalence", "fig_passk", "fig_self_report", "fig_red_first", "fig_fidelity", "fig_survival")

# Readable metric names (P31, JP 2026-10-05): the paper never prints a package id.
IEEE_METRIC_NAMES = {
    "new_tests_per_intent": "New tests per intent",
    "new_layer_balance": "Layer balance",   # not a Table IV metric; defined in the text of RQ1 (P43)
    "red_first": "First-execution red",
    "build_failed_execs": "Failed builds",
    "build_peak_errors": "Peak compiler errors",
    "build_errors_burned": "Total compiler errors",
    "attempts_to_green": "Test executions to green",
    "test_failures_burned": "Total test failures",
    "e2e_execs": "End-to-end executions",
    "first_run_new_fail_share": "New tests failing at first execution",
    "story_min_mi_gain": "Refactoring gain in worst story MI",
    "self_report_gap_methods_abs": "Self-report gap in test methods",
    "e2e_attempts_to_pass_noinfra": "End-to-end attempts to pass",
    "line_rate": "Solution line coverage",
    "story_line_rate": "Story line coverage",
    "max_coupling": "Maximum class coupling",
    "median_mi": "Median maintainability index",
    "story_min_mi": "Worst story maintainability index",
}
IEEE_FLAG_NAMES = {
    "build_first_pass": "First build compiles",
    "green_first_try": "Suite green at first execution",
    "e2e_first_pass": "End-to-end passed first",
    "final_e2e_ok": "Final end-to-end passed",
    "final_tests_green": "Final suite green",
    "clean_run": "Clean run",
}
# Row order of fig_equivalence: Table IV order, RQ1 rows then RQ4 rows (P35). Metrics not listed keep the frame order after these.
IEEE_TOST_ORDER = ("new_tests_per_intent", "line_rate", "story_line_rate", "max_coupling", "median_mi", "story_min_mi")
_IEEE_NEUTRAL_DARK = "#4D4D4D"   # reference lines (figure-style.md section 3)
_IEEE_BAND_GREY = "#DCDCDC"      # equivalence band


def _readable(metric: str) -> str:
    name = IEEE_METRIC_NAMES.get(metric) or IEEE_FLAG_NAMES.get(metric)
    if name is None:
        raise KeyError(f"no readable name for {metric!r}: extend the P31 mapping, never print the package id")
    return name


def _minus(s: str) -> str:
    return s.replace("-", "−")


def _measure_in(fig, texts: list[str], size: float, **kw) -> list[float]:
    """Rendered width in inches of each string at ``size`` pt (real font metrics, so a fallback face still fits)."""
    renderer = fig.canvas.get_renderer()
    out = []
    for s in texts:
        t = fig.text(0, 0, s, fontsize=size, **kw)
        out.append(t.get_window_extent(renderer).width / fig.dpi)
        t.remove()
    return out


def _wrap2(name: str, one_line: int) -> str:
    """One line when it fits ``one_line`` characters, otherwise the two-line split with the narrowest widest line."""
    if len(name) <= one_line:
        return name
    words = name.split()
    best = min(range(1, len(words)), key=lambda i: max(len(" ".join(words[:i])), len(" ".join(words[i:]))))
    return " ".join(words[:best]) + "\n" + " ".join(words[best:])


def _numbers_entries(ctx: FigureContext, select) -> dict:
    """Macro name -> full numbers.json entry (set S0) that ``select(name, entry)`` accepts. Empty when numbers.json is
    absent; the sidecar then simply carries no macro facts."""
    path = Path(ctx.out_dir) / "numbers.json"
    if not path.exists():
        return {}
    try:
        entries = json.loads(path.read_text()).get("numbers", {})
    except ValueError:
        return {}
    return {k: v for k, v in sorted(entries.items()) if v.get("set") == "S0" and select(k, v)}


def _numbers_for(ctx: FigureContext, select) -> dict:
    """Macro name -> value for the numbers.json entries (set S0) that ``select(name, entry)`` accepts."""
    return {k: v["value"] for k, v in _numbers_entries(ctx, select).items()}


def _macro_agreement(entries: dict, drawn: dict, tol: float = 1e-9) -> dict:
    """Cross-check the per-story macros (P44) against what a figure drew. ``drawn`` maps (story position, model, metric,
    stat) to the plotted value; macros of other cells are ignored. Returns how many macros were compared and which differ."""
    compared, mismatches = 0, []
    for name, v in entries.items():
        g = v.get("group") or {}
        key = (g.get("story_pos"), g.get("model"), v.get("metric"), v.get("stat"))
        if key in drawn:
            compared += 1
            if abs(float(v["value"]) - float(drawn[key])) > tol:
                mismatches.append(name)
    return {"compared": compared, "mismatches": mismatches}


def _agreement_caveat(check: dict) -> str:
    if not check["compared"]:
        return "no matching macros in numbers.json to cross-check the drawn values against"
    state = "all equal the drawn values" if not check["mismatches"] else f"DIFFER from the drawn values: {', '.join(check['mismatches'])}"
    return f"macros of numbers.json cross-checked against the drawn values: {check['compared']} compared, {state}"


def fig_forest_delta_ieee(ctx: FigureContext) -> dict:
    """The 12 tested comparisons (H1 to H8, then the four secondary metrics under a 'Secondary' label), pooled
    Cliff's delta with its 95 % interval, filled marker when Holm rejects within its family, delta and Holm p on
    the right. Exploratory rows, heterogeneity statistics and the per-story deltas (grey dots, dropped by P45) are
    not drawn."""
    eff = ctx.frame("effects")
    if eff.empty:
        raise ValueError("effects frame missing")
    rows, missing = [], []
    for group, fam, metrics in (("confirmatory", "primary", C.CONFIRMATORY_FAMILY), ("secondary", "secondary", C.SECONDARY_FAMILY)):
        for i, m in enumerate(metrics, 1):
            hit = eff[(eff["metric"] == m) & (eff["family"] == fam)].dropna(subset=["delta"])
            if hit.empty:
                missing.append(m)
                continue
            name = _readable(m)
            rows.append({"group": group, "h": f"H{i}" if group == "confirmatory" else "", "metric": m,
                         "label": f"H{i} {name}" if group == "confirmatory" else name, "r": hit.iloc[0]})
    n_c = sum(1 for x in rows if x["group"] == "confirmatory")
    # Vertical rhythm in row pitches. Page reduction (JP 2026-10-07): the row pitch is 0.12 in (8.6 pt, 1.2 times the
    # 7 pt row labels; it was 0.15 in), and the gap before the secondary rows and the margins are tighter, so the figure
    # is about 2.02 in high (it was 2.51 in, 80 %). Fonts, columns, markers, row order and labels are unchanged.
    sec_gap, sep_off, head_off, pad_top, pad_bottom = 1.25, 0.45, 0.35, 0.6, 0.6
    ys, y_sep, y_head = [], n_c - sep_off, n_c + head_off
    for k, x in enumerate(rows):
        ys.append(float(k) if x["group"] == "confirmatory" else n_c + sec_gap + (k - n_c))
    y_top, y_bottom = -pad_top, (ys[-1] if ys else 0.0) + pad_bottom

    W = T.IEEE_COLUMN_W
    lab, annot = T.IEEE_FONT_PT["label"], T.IEEE_FONT_PT["annot"]
    pitch_in, pad, g_lab, g_d, g_p = 0.12, 0.02, 0.06, 0.07, 0.10
    deltas = [_minus("0.00" if abs(x["r"]["delta"]) < 0.005 else f"{x['r']['delta']:+.2f}") for x in rows]
    ps = []
    for x in rows:
        p = x["r"].get("p_holm")
        ps.append("" if pd.isna(p) else ("<0.001" if p < 0.001 else f"{p:.3f}"))
    fig = plt.figure(figsize=(W, 3.0))
    w_lab = max(_measure_in(fig, [x["label"] for x in rows] + ["Secondary"], lab))
    w_d = max(_measure_in(fig, deltas + ["delta"], annot))
    w_p = max(_measure_in(fig, ps + ["Holm p"], annot))
    plot_w = W - pad - w_lab - g_lab - g_d - w_d - g_p - w_p - pad
    plot_h = (y_bottom - y_top) * pitch_in
    top_in, bottom_in = 0.13, 0.28
    H = top_in + plot_h + bottom_in
    fig.set_size_inches(W, H)
    ax = fig.add_axes([(pad + w_lab + g_lab) / W, bottom_in / H, plot_w / W, plot_h / H])
    ax.set_xlim(-1.0, 1.0)
    ax.set_ylim(y_bottom, y_top)
    ax.set_yticks([])
    ax.spines["left"].set_visible(False)
    ax.grid(axis="x", color=T.LIGHTER, linewidth=0.4)
    ax.grid(axis="y", visible=False)
    ax.set_xticks([-1.0, -0.5, 0.0, 0.5, 1.0])
    ax.set_xticklabels([_minus(t) for t in ("-1", "-0.5", "0", "0.5", "1")])
    ax.axvline(0, color=_IEEE_NEUTRAL_DARK, linewidth=0.6, linestyle=":", zorder=1)
    ax.set_xlabel("Cliff’s delta (Kimi minus Qwen)")
    yaxis = ax.get_yaxis_transform()
    x_lab, x_d, x_p = -(g_lab + w_lab) / plot_w, 1 + (g_d + w_d) / plot_w, 1 + (g_d + w_d + g_p + w_p) / plot_w
    for x, yi, dtxt, ptxt in zip(rows, ys, deltas, ps):
        r = x["r"]
        ax.plot([r["ci_low"], r["ci_high"]], [yi, yi], color="black", linewidth=0.8, zorder=2, solid_capstyle="butt")
        filled = bool(r.get("reject")) if pd.notna(r.get("reject")) else False
        ax.plot([r["delta"]], [yi], marker="D", markersize=3.3, markerfacecolor="black" if filled else "white",
                markeredgecolor="black", markeredgewidth=0.7, linestyle="none", zorder=3)
        ax.text(x_lab, yi, x["label"], transform=yaxis, ha="left", va="center", fontsize=lab)
        ax.text(x_d, yi, dtxt, transform=yaxis, ha="right", va="center", fontsize=annot)
        ax.text(x_p, yi, ptxt, transform=yaxis, ha="right", va="center", fontsize=annot)
    if any(x["group"] == "secondary" for x in rows):
        ax.text(x_lab, y_head, "Secondary", transform=yaxis, ha="left", va="center", fontsize=lab, fontstyle="italic")
        ax.plot([x_lab, x_p], [y_sep, y_sep], transform=yaxis, color="#BDBDBD", linewidth=0.4, clip_on=False, zorder=0)
    for text, xpos, ha in (("delta", x_d, "right"), ("Holm p", x_p, "right")):
        ax.annotate(text, xy=(xpos, 1.0), xycoords="axes fraction", xytext=(0, 1.5), textcoords="offset points",
                    ha=ha, va="bottom", fontsize=annot)
    ax.annotate("no effect", xy=(0.5, 1.0), xycoords="axes fraction", xytext=(0, 1.5), textcoords="offset points",
                ha="center", va="bottom", fontsize=annot, color=_IEEE_NEUTRAL_DARK)

    cols = ["metric", "delta", "ci_low", "ci_high", "p_used", "p_holm", "reject", "stories_kimi_higher", "stories_qwen_higher",
            "n_a", "n_b", "n_stories"]   # n_a = Kimi runs, n_b = Qwen runs, n_stories = strata (as in the effects frame)
    table = pd.DataFrame([{c: x["r"].get(c) for c in cols} for x in rows])
    data = {"rows": {**table.to_dict("list"), "label": [x["label"] for x in rows], "group": [x["group"] for x in rows],
                     "hypothesis": [x["h"] for x in rows]},
            "encoding": {"marker": "black diamond, filled when Holm rejects within its family, hollow otherwise",
                         "interval": "95% stratified bootstrap interval of the pooled Cliff's delta",
                         "xlim": [-1.0, 1.0],
                         "right_columns": ["delta", "Holm-adjusted p (within the family)"]}}
    metrics = {x["metric"] for x in rows}
    numbers = _numbers_for(ctx, lambda k, v: v.get("metric") in metrics and not v.get("group")
                           and v.get("stat") in {"cliffs_delta", "ci_lo", "ci_hi", "p_holm", "holm_reject"}
                           and (v.get("test") or {}).get("family") in ("primary", "secondary"))
    caveats = ["rows are the 12 tested comparisons only: H1 to H8 (Holm family of eight) and four secondary metrics (their own Holm family); exploratory rows are not drawn (P41)",
               "filled marker: Holm rejects within the metric's family at alpha 0.05; hollow: it does not",
               "axis label uses the short model names Kimi and Qwen (Kimi-K2.5, Qwen3.7-max)",
               "the figure draws no heterogeneity statistic and no subset marks (P37)",
               "the grey per-story dots of the first IEEE render are removed (P45): each row shows the pooled delta and its interval only"]
    if missing:
        caveats.append("rows without an effect in the analysis frame, not drawn: " + ", ".join(missing))
    return _save(ctx, fig, "fig_forest_delta", data=data, numbers=numbers, width_class="column", rq="all", kind="forest",
                 caveats=caveats, extra={"readable_names": {m: _readable(m) for m in sorted(metrics)}})


def _n_strata(used) -> int | None:
    """Number of stories behind a TOST interval, from the tost frame's ``strata_used`` (a list, or its CSV string)."""
    if isinstance(used, str):
        try:
            used = ast.literal_eval(used)
        except (ValueError, SyntaxError):
            return None
    return len(used) if isinstance(used, (list, tuple)) else None


def fig_equivalence_ieee(ctx: FigureContext) -> dict:
    """One row per TOST metric, each on its own axis in the metric's own unit: grey band = the equivalence band,
    black diamond and bar = the estimate and its 90 % interval (mean of per-story median differences)."""
    tost = ctx.frame("tost")
    if tost.empty:
        raise ValueError("tost frame missing")
    rank = {m: i for i, m in enumerate(IEEE_TOST_ORDER)}
    tost = (tost.assign(_row=tost["metric"].map(lambda m: rank.get(m, len(rank))))
            .sort_values("_row", kind="stable").drop(columns="_row").reset_index(drop=True))
    n = len(tost)
    W = T.IEEE_COLUMN_W
    lab, annot = T.IEEE_FONT_PT["label"], T.IEEE_FONT_PT["annot"]
    pad, g_lab, g_ann = 0.02, 0.06, 0.07
    row_h, pitch, gap_top = 0.14, 0.305, 0.03
    top_in, xlabel_in = 0.15, 0.27
    H = top_in + n * pitch + xlabel_in
    fig = plt.figure(figsize=(W, H))
    names = [_wrap2(_readable(m), 24) for m in tost["metric"]]
    verdicts = ["equivalent" if bool(e) else "not shown" for e in tost["equivalent"]]
    w_lab = max(_measure_in(fig, names, lab))
    w_ann = max(_measure_in(fig, ["equivalent", "not shown"], annot))
    plot_w = W - pad - w_lab - g_lab - g_ann - w_ann - pad
    left = pad + w_lab + g_lab
    data, axes = {}, []
    for i, (_, r) in enumerate(tost.iterrows()):
        zone_bottom = xlabel_in + (n - 1 - i) * pitch
        ax = fig.add_axes([left / W, (zone_bottom + pitch - gap_top - row_h) / H, plot_w / W, row_h / H])
        axes.append(ax)
        band = float(r["band"])
        span = max(2 * band, abs(r["ci_low"]) * 1.1, abs(r["ci_high"]) * 1.1)
        ax.set_xlim(-span, span)
        ax.set_ylim(-1, 1)
        ax.axvspan(-band, band, color=_IEEE_BAND_GREY, linewidth=0, zorder=0)
        for edge in (-band, band):
            ax.axvline(edge, color=_IEEE_NEUTRAL_DARK, linewidth=0.5, linestyle=":", zorder=1)
        ax.axvline(0, color=_IEEE_NEUTRAL_DARK, linewidth=0.5, linestyle=":", zorder=1)
        ax.plot([r["ci_low"], r["ci_high"]], [0, 0], color="black", linewidth=1.0, marker="|", markersize=4.0,
                markeredgewidth=0.8, solid_capstyle="butt", zorder=2)
        ax.plot([r["estimate"]], [0], marker="D", markersize=3.4, color="black", linestyle="none", zorder=3)
        ax.set_yticks([])
        ax.set_xticks([-band, 0.0, band])
        ax.set_xticklabels([_minus(f"{-band:.10g}"), "0", f"{band:.10g}"])
        ax.grid(False)
        for s in ("top", "right", "left"):
            ax.spines[s].set_visible(False)
        ax.text(-(g_lab) / plot_w, 0.5, names[i], transform=ax.transAxes, ha="right", va="center", fontsize=lab,
                linespacing=1.05)
        ax.text(1 + g_ann / plot_w, 0.5, verdicts[i], transform=ax.transAxes, ha="left", va="center", fontsize=annot)
        data[r["metric"]] = {"estimate": r["estimate"], "ci_low": r["ci_low"], "ci_high": r["ci_high"], "band": band,
                             "equivalent": bool(r["equivalent"]), "smallest_equivalent_band": r["smallest_equivalent_band"],
                             "label": _readable(r["metric"]), "n_stories": _n_strata(r.get("strata_used"))}
    axes[0].annotate("equivalence band", xy=(0.5, 1.0), xycoords="axes fraction", xytext=(0, 2.0),
                     textcoords="offset points", ha="center", va="bottom", fontsize=annot, color=_IEEE_NEUTRAL_DARK)
    fig.text((left + plot_w / 2) / W, 0.02 / H, "Kimi minus Qwen, in the unit of each metric\n"
             "(mean of per-story median differences, 90% interval)", ha="center", va="bottom", fontsize=lab, linespacing=1.05)
    metrics = set(tost["metric"])
    numbers = _numbers_for(ctx, lambda k, v: v.get("metric") in metrics and "Tost" in k
                           and v.get("stat") in {"tost_delta", "ci_lo", "ci_hi", "band", "decision", "smallest_band"})
    return _save(ctx, fig, "fig_equivalence", data=data, numbers=numbers, width_class="column", rq="RQ4", kind="tost",
                 caveats=["each row has its own x axis in the metric's own unit; ticks mark minus band, zero and plus band",
                          "axis label uses the short model names Kimi and Qwen (Kimi-K2.5, Qwen3.7-max)",
                          "'equivalent': the 90% interval lies inside the band (two one-sided tests); 'not shown': it does not"],
                 extra={"readable_names": {m: _readable(m) for m in sorted(metrics)}})


def fig_passk_ieee(ctx: FigureContext) -> dict:
    """pass@k (top) and pass^k (bottom) for the three first-attempt predicates, both models, 95 % story-bootstrap
    ribbons, k = 1 to 6, one shared legend. The final-state flags and the clean run are not drawn (the text reports them)."""
    pk = ctx.frame("passk")
    if pk.empty:
        raise ValueError("passk frame missing")
    preds = [p for p in ("build_first_pass", "green_first_try", "e2e_first_pass") if p in set(pk["predicate"])]
    height = 1.89   # page reduction (JP 2026-10-07): 80 % of the first IEEE height (2.35 in); panels, fonts and labels unchanged
    fig, axes = plt.subplots(2, len(preds), figsize=T.ieee_fig_size(height), sharex=True, sharey=True, layout="constrained")
    axes = np.atleast_2d(axes)
    data = {}
    for j, pred in enumerate(preds):
        for i, est in enumerate(("pass_at_k", "pass_hat_k")):
            ax = axes[i, j]
            for m in C.MODEL_ORDER:
                g = pk[(pk["predicate"] == pred) & (pk["estimator"] == est) & (pk["group"] == m)].sort_values("k")
                st = T.MODEL_STYLE[m]
                ax.fill_between(g["k"], g["ci_low"], g["ci_high"], color=st["color"], alpha=0.16, linewidth=0, zorder=1)
                ax.plot(g["k"], g["mean"], color=st["color"], marker=st["marker"], linestyle=st["linestyle"],
                        markersize=2.6, linewidth=0.9, zorder=3 if m == KIMI else 4)
                data[f"{pred}/{est}/{m}"] = g[[c for c in ("k", "mean", "ci_low", "ci_high", "n_strata") if c in g.columns]].to_dict("list")
            if i == 0:
                ax.set_title(_wrap2(_readable(pred), 14), fontsize=T.IEEE_FONT_PT["label"], linespacing=1.05)
            if j == 0:
                ax.set_ylabel("pass@k" if est == "pass_at_k" else "pass^k")
            ax.set_ylim(-0.03, 1.03)
            ax.set_yticks([0, 0.5, 1.0])
            ax.set_yticklabels(["0", "0.5", "1"])
            ax.set_xlim(0.7, 6.3)
            ax.set_xticks(range(1, 7))
    fig.supxlabel("Number of runs, k", fontsize=T.IEEE_FONT_PT["label"])
    handles = [Line2D([0], [0], marker=T.MODEL_STYLE[m]["marker"], color=T.MODEL_STYLE[m]["color"],
                      linestyle=T.MODEL_STYLE[m]["linestyle"], markersize=3.4, label=m) for m in C.MODEL_ORDER]
    fig.legend(handles=handles, loc="outside upper center", ncol=2, frameon=False)
    numbers = _numbers_for(ctx, lambda k, v: v.get("metric") in set(preds) and v.get("stat") in {"pass_at_k", "pass_hat_k"})
    return _save(ctx, fig, "fig_passk", data=data, numbers=numbers, width_class="column", rq="RQ5", kind="curves",
                 caveats=["mean over stories of the per-cell estimator from six runs; ribbons are story-bootstrap 95% intervals",
                          "three first-attempt predicates only: the final-state flags (near the ceiling) and the clean run (near the floor) are reported in the text",
                          "no panel letters: the top row is pass@k, the bottom row pass^k, columns are the predicates"],
                 extra={"readable_names": {p: _readable(p) for p in preds}})


# ---------------------------------------------------------------------------
# IEEE variants of P43 (JP, 2026-10-06): self-report (RQ6), first-execution red per story (RQ2), new tests per
# intent and layer balance per story (RQ1). Stories are drawn as the anonymous labels US1 to US10 only; the
# mapping is read from anonymous-stories.csv in the paper folder (the parent of the output folder).
# ---------------------------------------------------------------------------

IEEE_AXIS_NAMES = {
    "new_test_methods": "New test methods in the branch diff",
    "tg_test_methods": "Test methods in the stage file (agent-reported)",
    "story": "User story",
}


def _ieee_story_labels(ctx: FigureContext) -> tuple[dict[int, str], str]:
    """Chain position -> anonymous label (US1 to US10) and the source of the mapping. The mapping is read from
    ``anonymous-stories.csv`` (story_id, label) next to the output folder; chain position k must be USk."""
    path = Path(ctx.out_dir).resolve().parent / "anonymous-stories.csv"
    if path.exists():
        m = pd.read_csv(path)
        labels = {C.STORY_POS[s]: str(lab) for s, lab in zip(m["story_id"], m["label"]) if s in C.STORY_POS}
        wrong = {p: lab for p, lab in labels.items() if lab != f"US{p}"}
        if wrong or set(labels) != set(C.STORY_POS.values()):
            raise ValueError(f"{path.name}: chain position k must be labelled USk for all {len(C.STORY_POS)} stories; got {labels}")
        return labels, path.name
    return ({p: f"US{p}" for p in sorted(C.STORY_POS.values())},
            "built in (US plus chain position): anonymous-stories.csv not found next to the output folder")


def _check_no_story_ids(fig) -> None:
    """Stories appear only as US1 to US10 (P43): no drawn text may contain a story id of the package."""
    bad = [t for t in T.figure_texts(fig) if any(s in t for s in C.STORY_POS)]
    if bad:
        plt.close(fig)
        raise ValueError(f"text that contains a story id (use US1 to US10): {bad}")


def _ieee_axis_story_ticks(ax, positions: list[int], labels: dict[int, str]) -> None:
    ax.set_xlim(min(positions) - 0.6, max(positions) + 0.6)
    ax.set_xticks(positions)
    ax.set_xticklabels([labels[p] for p in positions])


def fig_self_report_ieee(ctx: FigureContext) -> dict:
    """RQ6, left panel of the report's Fig. 9 only: test methods in the stage file of the test generator (agent-reported)
    against the story-new test methods in the branch diff, one point per run, both axes in methods, the diagonal of
    agreement dotted. No jitter: marker faces are translucent, so coincident runs darken, and the Kimi circle is drawn
    larger than the Qwen square so a coincident pair stays visible. The test-cases panel is not drawn."""
    from matplotlib.colors import to_rgba
    r = ctx.runs
    xcol, ycol = "new_test_methods", "tg_test_methods"
    d = r.assign(_x=pd.to_numeric(r[xcol], errors="coerce"), _y=pd.to_numeric(r[ycol], errors="coerce"))
    dropped = int(d[["_x", "_y"]].isna().any(axis=1).sum())
    d = d.dropna(subset=["_x", "_y"])
    labels, label_src = _ieee_story_labels(ctx)
    lim = 5 * math.ceil((max(float(d["_x"].max()), float(d["_y"].max())) + 1) / 5)
    height = 2.08   # page reduction (JP 2026-10-07): 80 % of the first IEEE height (2.6 in); the equal-scale square axes shrink with it
    fig, ax = plt.subplots(figsize=T.ieee_fig_size(height), layout="constrained")
    ax.set_aspect("equal", adjustable="box")
    ax.set_xlim(0, lim)
    ax.set_ylim(0, lim)
    ax.set_xticks(range(0, lim, 10))
    ax.set_yticks(range(0, lim, 10))
    ax.grid(True, axis="both", color=T.LIGHTER, linewidth=0.4)
    ax.plot([0, lim], [0, lim], color=_IEEE_NEUTRAL_DARK, linewidth=0.7, linestyle=":", zorder=1)
    size = {KIMI: 4.6, QWEN: 3.4}
    face_alpha = 0.40
    data = {"points": {}}
    for m in C.MODEL_ORDER:
        g = d[d["model"] == m]
        st = T.MODEL_STYLE[m]
        ax.plot(g["_x"], g["_y"], linestyle="none", marker=st["marker"], markersize=size[m],
                markerfacecolor=to_rgba(st["color"], face_alpha), markeredgecolor=st["color"], markeredgewidth=0.5,
                zorder=3 if m == KIMI else 4)
        distinct = g.groupby(["_x", "_y"]).size().reset_index(name="count")
        data["points"][m] = {
            "x": [float(v) for v in g["_x"]], "y": [float(v) for v in g["_y"]],
            "story": [labels[int(p)] for p in g["story_pos"]], "iteration": [int(v) for v in g["iteration"]],
            "n": int(len(g)), "n_on_diagonal": int((g["_x"] == g["_y"]).sum()),
            "distinct": {"x": [float(v) for v in distinct["_x"]], "y": [float(v) for v in distinct["_y"]],
                         "count": [int(v) for v in distinct["count"]]}}
    x_label, y_label = IEEE_AXIS_NAMES[xcol], IEEE_AXIS_NAMES[ycol]
    est_axis_in = height - 0.45     # the axes are square; the label must not be longer than the axis
    if _measure_in(fig, [y_label], T.IEEE_FONT_PT["label"])[0] > est_axis_in:
        y_label = y_label.replace(" (agent-reported)", "\n(agent-reported)")
    ax.set_xlabel(x_label)
    ax.set_ylabel(y_label, linespacing=1.05)
    handles = [Line2D([0], [0], marker=T.MODEL_STYLE[m]["marker"], linestyle="none", markersize=size[m],
                      markerfacecolor=to_rgba(T.MODEL_STYLE[m]["color"], face_alpha),
                      markeredgecolor=T.MODEL_STYLE[m]["color"], markeredgewidth=0.5, label=m) for m in C.MODEL_ORDER]
    handles.append(Line2D([0], [0], color=_IEEE_NEUTRAL_DARK, linewidth=0.9, linestyle=":", label="Agreement"))
    ax.legend(handles=handles, loc="lower right", frameon=False, borderaxespad=0.3)
    _check_no_story_ids(fig)
    numbers = _numbers_for(ctx, lambda k, v: (v.get("metric") == "self_report_gap_methods_abs" and v.get("stat") in {"n", "max"}
                                              and "story" not in (v.get("group") or {}))
                           or (k.startswith("SelfReportGap") and k.endswith("ZeroK")))
    pairs = [(f"SelfReportGap{C.MODEL_SHORT[m]}ZeroK", data["points"][m]["n_on_diagonal"]) for m in C.MODEL_ORDER
             if f"SelfReportGap{C.MODEL_SHORT[m]}ZeroK" in numbers]
    check = {"compared": len(pairs), "mismatches": [k for k, v in pairs if float(numbers[k]) != float(v)]}
    caveats = [
        "left panel of the report's Fig. 9 only: both axes are test methods; the test-cases panel (cases after attribute expansion, a different unit) is not drawn",
        "the vertical axis is agent-reported: the stage file of the test generator, in words in the axis label; the paper marks the quantity with the dagger (P46) in the caption",
        "no jitter: overlapping runs are legible through translucent marker faces (opacity 0.4, so coincident runs darken) and marker sizes (Kimi circle larger than Qwen square)",
        f"axes run from 0 to {lim} on both sides, equal scale; the dotted diagonal is stage file = branch diff",
        f"n = {int(len(d))} runs ({', '.join(f'{m}: {data['points'][m]['n']}' for m in C.MODEL_ORDER)}); runs without both values dropped: {dropped}",
        f"stories are labelled US1 to US10 in the sidecar ({label_src}); no story id is drawn",
        "n_on_diagonal per model (data) counts the runs whose stage file equals the diff; zero-gap macros: " + _agreement_caveat(check),
    ]
    if "\n" in y_label:
        caveats.append("the vertical axis label breaks before '(agent-reported)' to fit the axis height")
    return _save(ctx, fig, "fig_self_report", data=data, numbers=numbers, width_class="column", rq="RQ6", kind="scatter",
                 caveats=caveats,
                 extra={"readable_names": {"new_test_methods": x_label, "tg_test_methods": IEEE_AXIS_NAMES[ycol],
                                           "self_report_gap_methods_abs": _readable("self_report_gap_methods_abs")},
                        "encoding": {"x": "story-new test methods in the branch diff (derived-from-git)",
                                     "y": "test methods the test generator reported in its stage file (self-reported)",
                                     "diagonal": "dotted line: equal counts", "jitter": None,
                                     "marker_face_opacity": face_alpha, "marker_size_pt": size,
                                     "story_labels": {str(p): lab for p, lab in sorted(labels.items())}},
                        "macro_check": check})


def fig_red_first_ieee(ctx: FigureContext) -> dict:
    """RQ2, top panel of the report's Fig. only: the share of runs whose first recorded test execution was not green
    (first-execution red), per story and model, with 95 % Wilson intervals; stories US1 to US10 in chain order, the two
    models side by side per story, one legend. The S4 shading, the compile-red notes and the new-fail-share panel are
    not drawn (P37, P43)."""
    r = ctx.runs
    labels, label_src = _ieee_story_labels(ctx)
    positions = sorted(int(p) for p in r["story_pos"].dropna().unique())
    fig, ax = plt.subplots(figsize=T.ieee_fig_size(1.7), layout="constrained")
    offs = {KIMI: -0.17, QWEN: 0.17}
    data = {"red_first": {}}
    for m in C.MODEL_ORDER:
        st = T.MODEL_STYLE[m]
        rows = []
        for pos in positions:
            v = r.loc[(r["model"] == m) & (r["story_pos"] == pos), "red_first"].dropna().astype(bool)
            p, lo, hi = wilson(int(v.sum()), int(len(v)))
            if not math.isnan(p):
                rows.append({"pos": pos, "k": int(v.sum()), "n": int(len(v)), "share": p, "lo": lo, "hi": hi})
        xs = np.array([x["pos"] + offs[m] for x in rows])
        ps, lo, hi = (np.array([x[c] for x in rows]) for c in ("share", "lo", "hi"))
        ax.errorbar(xs, ps, yerr=[np.clip(ps - lo, 0, None), np.clip(hi - ps, 0, None)], fmt=st["marker"], color=st["color"],
                    markersize=3.4, markeredgewidth=0, capsize=1.3, elinewidth=0.7, capthick=0.7,
                    zorder=3 if m == KIMI else 4)
        data["red_first"][m] = {"story": [labels[x["pos"]] for x in rows], "story_pos": [x["pos"] for x in rows],
                                "x": [float(v) for v in xs], "k": [x["k"] for x in rows], "n": [x["n"] for x in rows],
                                "share": [x["share"] for x in rows], "lo": [x["lo"] for x in rows], "hi": [x["hi"] for x in rows]}
    ax.set_ylim(-0.05, 1.05)
    ax.set_yticks([0, 0.5, 1.0])
    ax.set_yticklabels(["0", "0.5", "1"])
    _ieee_axis_story_ticks(ax, positions, labels)
    ax.set_xlabel(IEEE_AXIS_NAMES["story"])
    ax.set_ylabel(_readable("red_first") + "\n(share of runs)", linespacing=1.05)
    handles = [Line2D([0], [0], marker=T.MODEL_STYLE[m]["marker"], color=T.MODEL_STYLE[m]["color"], linestyle="none",
                      markersize=3.6, label=m) for m in C.MODEL_ORDER]
    fig.legend(handles=handles, loc="outside upper center", ncol=2, frameon=False)
    _check_no_story_ids(fig)
    entries = _numbers_entries(ctx, lambda k, v: v.get("metric") == "red_first" and v.get("stat") in {"count", "n", "proportion"}
                               and (v.get("group") or {}).get("model"))
    numbers = {k: v["value"] for k, v in entries.items()}
    drawn = {}
    for m in C.MODEL_ORDER:
        for pos, k_, n_ in zip(data["red_first"][m]["story_pos"], data["red_first"][m]["k"], data["red_first"][m]["n"]):
            drawn[(pos, m, "red_first", "count")], drawn[(pos, m, "red_first", "n")] = k_, n_
    check = _macro_agreement(entries, drawn)
    n_per = sorted({x for m in C.MODEL_ORDER for x in data["red_first"][m]["n"]})
    caveats = [
        "top panel of the report's figure only: the compile-red notes, the grey shading of the stories analysed in the artifact and the new-fail-share panel are not drawn (P37, P43)",
        "points are the share of runs with first-execution red per story and model; bars are 95% Wilson score intervals",
        f"n per point: {n_per} runs (each story and model has the same number of runs); n = {int(r['red_first'].notna().sum())} runs in all",
        "the two models are offset by plus and minus 0.17 story units so that their intervals do not overlap; this is a drawing offset, not data",
        "no lines join the points: stories are cases in chain order, not a series",
        f"stories are labelled US1 to US10 ({label_src}); no story id is drawn",
        "the vertical axis label breaks after 'First-execution red' to fit the panel height",
        _agreement_caveat(check),
    ]
    return _save(ctx, fig, "fig_red_first", data=data, numbers=numbers, width_class="column", rq="RQ2", kind="rates",
                 caveats=caveats,
                 extra={"readable_names": {"red_first": _readable("red_first")},
                        "encoding": {"point": "share of the runs of one story and model whose first recorded test execution was not green",
                                     "interval": "95% Wilson score interval", "x_offset_story_units": offs,
                                     "story_labels": {str(p): lab for p, lab in sorted(labels.items())}},
                        "macro_check": check})


def fig_fidelity_ieee(ctx: FigureContext) -> dict:
    """RQ1, the report's Fig. at one IEEE column: new tests per intent (top) and layer balance (bottom) per story, the
    median of each model as a line with markers (Kimi solid circles, Qwen dashed squares), the runs as light points
    (horizontal jitter, recorded in the sidecar), a dotted reference line at 1.0 in both panels, one legend."""
    r = ctx.runs
    labels, label_src = _ieee_story_labels(ctx)
    positions = sorted(int(p) for p in r["story_pos"].dropna().unique())
    metrics = ("new_tests_per_intent", "new_layer_balance")
    fig, axes = plt.subplots(2, 1, figsize=T.ieee_fig_size(2.2), sharex=True, layout="constrained")
    offs = {KIMI: -0.2, QWEN: 0.2}
    jitter_half = 0.07
    seeds = {}
    data = {}
    for pi, (ax, metric) in enumerate(zip(axes, metrics)):
        ax.axhline(1.0, color=_IEEE_NEUTRAL_DARK, linewidth=0.6, linestyle=":", zorder=1)
        lo_all, hi_all = 1.0, 1.0
        data[metric] = {}
        for mi, m in enumerate(C.MODEL_ORDER):
            st = T.MODEL_STYLE[m]
            g = r[r["model"] == m]
            v = pd.to_numeric(g[metric], errors="coerce")
            seeds[f"{metric}/{m}"] = ctx.seed + 100 * pi + 10 * mi
            x = g["story_pos"].astype(float) + offs[m] + _jitter(len(g), seeds[f"{metric}/{m}"], jitter_half)
            ax.plot(x, v, linestyle="none", marker=st["marker"], markersize=2.3, color=st["color"], alpha=0.35,
                    markeredgewidth=0, zorder=2)
            med = v.groupby(g["story_pos"]).median()
            cnt = v.groupby(g["story_pos"]).count()
            ax.plot(med.index.astype(float) + offs[m], med.values, color=st["color"], marker=st["marker"],
                    linestyle=st["linestyle"], markersize=3.4, linewidth=0.9, markeredgecolor="white",
                    markeredgewidth=0.4, zorder=3 if m == KIMI else 4)
            lo_all, hi_all = min(lo_all, float(v.min())), max(hi_all, float(v.max()))
            data[metric][m] = {"n": int(v.notna().sum()), "story": [labels[int(p)] for p in g["story_pos"]],
                               "story_pos": [int(p) for p in g["story_pos"]], "x": [float(a) for a in x],
                               "values": [None if pd.isna(a) else float(a) for a in v],
                               "median": {labels[int(p)]: float(a) for p, a in med.items()},
                               "n_per_story": {labels[int(p)]: int(a) for p, a in cnt.items()}}
        pad = 0.06 * (hi_all - lo_all)
        lo_lim, hi_lim = lo_all - pad, hi_all + pad
        ax.set_ylim(lo_lim, hi_lim)
        ticks = [t for t in plt.MaxNLocator(nbins=3, steps=[1, 2, 5, 10]).tick_values(lo_lim, hi_lim) if lo_lim <= t <= hi_lim]
        ax.set_yticks(ticks)
        ax.set_yticklabels([f"{t:.1f}" for t in ticks])
        ax.set_ylabel(_readable(metric))
    _ieee_axis_story_ticks(axes[-1], positions, labels)
    axes[-1].set_xlabel(IEEE_AXIS_NAMES["story"])
    handles = [Line2D([0], [0], marker=T.MODEL_STYLE[m]["marker"], color=T.MODEL_STYLE[m]["color"],
                      linestyle=T.MODEL_STYLE[m]["linestyle"], markersize=3.4, linewidth=0.9, label=m) for m in C.MODEL_ORDER]
    handles.append(Line2D([0], [0], color=_IEEE_NEUTRAL_DARK, linewidth=0.9, linestyle=":", label="Reference 1.0"))
    fig.legend(handles=handles, loc="outside upper center", ncol=3, frameon=False)
    _check_no_story_ids(fig)
    entries = _numbers_entries(ctx, lambda k, v: v.get("metric") in set(metrics) and v.get("stat") in {"median", "n"}
                               and (v.get("group") or {}).get("model"))
    numbers = {k: v["value"] for k, v in entries.items()}
    drawn = {}
    for metric in metrics:
        for m in C.MODEL_ORDER:
            for pos in positions:
                lab = labels[pos]
                if lab in data[metric][m]["median"]:
                    drawn[(pos, m, metric, "median")] = data[metric][m]["median"][lab]
                    drawn[(pos, m, metric, "n")] = data[metric][m]["n_per_story"][lab]
    check = _macro_agreement(entries, drawn)
    caveats = [
        "lines are the per-story medians of each model (six runs per story and model); light points are the individual runs",
        f"run points are jittered horizontally by a uniform draw of plus or minus {jitter_half} story units around the model's offset ({offs[KIMI]} for Kimi, +{offs[QWEN]} for Qwen); the medians are not jittered; seeds are in encoding",
        "dotted line: 1.0 in both panels (one story-new test method per confirmed intent; layer balance of one, its ceiling)",
        "vertical axes are not shared: each panel has the range of its own metric, with the reference line inside both",
        "layer balance is not a Table IV metric; it is defined in the text of RQ1",
        "no panel letters: each vertical axis label names its panel",
        f"stories are labelled US1 to US10 ({label_src}); no story id is drawn",
        _agreement_caveat(check),
    ]
    return _save(ctx, fig, "fig_fidelity", data=data, numbers=numbers, width_class="column", rq="RQ1", kind="strips",
                 caveats=caveats,
                 extra={"readable_names": {m: _readable(m) for m in metrics},
                        "encoding": {"median_line": "per-story median over the runs of a model, same x as the model's run points",
                                     "points": "individual runs, marker alpha 0.35",
                                     "x_offset_story_units": offs,
                                     "jitter": {"kind": "uniform", "half_width_story_units": jitter_half, "axis": "x",
                                                "seeds": seeds},
                                     "reference_line": 1.0,
                                     "story_labels": {str(p): lab for p, lab in sorted(labels.items())}},
                        "macro_check": check})


# ---------------------------------------------------------------------------
# fig_survival (item N1 of the co-author action items): time to green as Kaplan-Meier step curves. Descriptive only:
# no test, no interval, no new hypothesis. Panel (a) draws the paper's "Test executions to green" (attempts_to_green),
# panel (b) its "End-to-end attempts to pass" (e2e_attempts_to_pass_noinfra, Table IV; the infrastructure-inclusive
# e2e_attempts_to_pass is the S7 sensitivity). A run whose column is empty never succeeded: it is censored at its last
# counted execution (test executions; end-to-end executions where the API was up) and is not counted as a success.
# ---------------------------------------------------------------------------

SURVIVAL_AT = (1, 2, 3, 5)      # k at which the sidecar records the share still not green / not yet passed
SURVIVAL_CAP = {"green": 15}    # x-axis cap per panel (tail beyond it is drawn off-axis, never dropped)
_SURVIVAL_PANELS = (
    # key, runs.csv column, execution tree, API-up executions only, x label, y label, readable name
    ("green", "attempts_to_green", "test", False, "Test executions", "Not yet green\n(share of runs)", "Test executions to green"),
    ("e2e", "e2e_attempts_to_pass_noinfra", "e2e", True, "End-to-end attempts", "Not yet passed\n(share of runs)", "End-to-end attempts to pass"),
)


def _km_table(time: np.ndarray, event: np.ndarray) -> list[dict]:
    """Kaplan-Meier rows k = 1..max(time) of integer times. A run censored at c is at risk at every k <= c (its
    executions up to c did not succeed) and is a non-event at c; S(k) is right-continuous."""
    s, rows = 1.0, []
    for k in range(1, int(time.max()) + 1 if len(time) else 1):
        at_risk = int((time >= k).sum())
        d = int(((time == k) & event).sum())
        if at_risk:
            s *= 1.0 - d / at_risk
        rows.append({"k": k, "at_risk": at_risk, "events": d, "censored": int(((time == k) & ~event).sum()), "s": s})
    return rows


def fig_survival(ctx: FigureContext) -> dict:
    r, ex = ctx.runs, ctx.executions
    ieee = T.ieee_mode()
    lab, annot = float(plt.rcParams["axes.labelsize"]), float(plt.rcParams["xtick.labelsize"])
    labels, label_src = _ieee_story_labels(ctx)
    fig, axes = plt.subplots(1, 2, figsize=T.ieee_fig_size(2.0) if ieee else T.fig_size("column", height=2.0),
                             sharey=True, layout="constrained")
    size = {KIMI: 3.2, QWEN: 2.4}     # Kimi circle larger than Qwen square so a coincident step shows both
    data, numbers, caveats, censored_runs = {}, {}, [], []
    for ax, (key, col, tree, up_only, xlab, ylab, readable) in zip(axes, _SURVIVAL_PANELS):
        e = ex[ex["tree"] == tree]
        if up_only:   # the column counts only executions where the API started (dataset.py), so does the censor time
            e = e[e["backend_up"].map(lambda v: str(v).strip().lower() in ("true", "1"))]
        counted = e.groupby("run_id").size().reindex(r["run_id"]).fillna(0).to_numpy()
        value = pd.to_numeric(r[col], errors="coerce").astype(float).to_numpy()
        event = ~np.isnan(value)
        time = np.where(event, np.nan_to_num(value), counted).astype(int)
        cap = SURVIVAL_CAP.get(key)
        k_max = int(time.max())
        x_right = (cap if cap else k_max) + 0.6
        data[key] = {}
        ax.axhline(0.5, color=_IEEE_NEUTRAL_DARK, linewidth=0.6, linestyle=":", zorder=1)
        ax.text(1.0, 0.5, "median", transform=ax.get_yaxis_transform(), ha="right", va="bottom", fontsize=annot,
                color=_IEEE_NEUTRAL_DARK)
        for m in C.MODEL_ORDER:
            st, sel = T.MODEL_STYLE[m], (r["model"] == m).to_numpy()
            tt, ee = time[sel], event[sel]
            rows = _km_table(tt, ee)
            s_of = {x["k"]: x["s"] for x in rows}
            last = int(tt.max())
            reach = lambda k: s_of[k] if k <= last else (0.0 if s_of[last] == 0 else None)   # None: undefined, nobody at risk
            median = next((x["k"] for x in rows if x["s"] <= 0.5), None)
            n_cens = int((~ee).sum())
            end = min(last, cap) if cap else last
            xs = [0] + [x["k"] for x in rows if x["k"] <= end]
            ys = [1.0] + [x["s"] for x in rows if x["k"] <= end]
            if last > (cap or last) or s_of[last] == 0:   # curve continues past the axis edge, or is at zero for good
                xs.append(x_right)
                ys.append(ys[-1])
            ax.step(xs, ys, where="post", color=st["color"], linestyle=st["linestyle"], linewidth=0.9,
                    zorder=3 if m == KIMI else 4)
            ev = [x for x in rows if x["events"] and x["k"] <= end]
            ax.plot([x["k"] for x in ev], [x["s"] for x in ev], linestyle="none", marker=st["marker"], markersize=size[m],
                    color=st["color"], markeredgewidth=0, zorder=3 if m == KIMI else 4)
            ticks = [x for x in rows if x["censored"] and x["k"] <= end]
            ax.plot([x["k"] for x in ticks], [x["s"] for x in ticks], linestyle="none", marker="|", markersize=7.5,
                    color=st["color"], markeredgewidth=1.4, zorder=5)
            for i in np.flatnonzero(sel & ~event):
                censored_runs.append(f"{m} {labels[int(r['story_pos'].iloc[i])]} run {int(r['iteration'].iloc[i])} "
                                     f"(panel {key}: never succeeded, counted up to {int(time[i])} executions)")
            short = C.MODEL_SHORT[m]
            summary = {"n": int(len(tt)), "n_events": int(ee.sum()), "n_censored": n_cens,
                       "n_censored_at_zero": int(((~ee) & (tt == 0)).sum()), "n_at_risk_k1": rows[0]["at_risk"],
                       "events_at_k1": rows[0]["events"], "max_time": last,
                       "n_beyond_cap": int((tt > cap).sum()) if cap else 0,
                       "not_yet": {str(k): reach(k) for k in SURVIVAL_AT}, "median_k": median}
            data[key][m] = {"x": [x["k"] for x in rows], "s": [x["s"] for x in rows], "at_risk": [x["at_risk"] for x in rows],
                            "events": [x["events"] for x in rows], "censored": [x["censored"] for x in rows],
                            "summary": summary}
            stem = f"Survival{'Green' if key == 'green' else 'EndToEnd'}{short}"
            numbers.update({f"{stem}N": summary["n"], f"{stem}Events": summary["n_events"], f"{stem}Censored": n_cens,
                            f"{stem}MedianK": median, **{f"{stem}NotYetK{k}": v for k, v in summary["not_yet"].items()}})
        ax.set_xlim(0, x_right)
        ax.set_ylim(-0.03, 1.03)
        ax.set_yticks([0, 0.5, 1.0])
        ax.set_yticklabels(["0", "0.5", "1"])
        ticks_x = [1, 5, 10, 15] if cap else list(range(1, k_max + 1))
        ax.set_xticks(ticks_x)
        ax.set_xticks(range(1, int(x_right) + 1), minor=True)
        ax.tick_params(axis="x", which="minor", length=1.2, width=0.4)
        ax.set_xlabel(xlab)
        ax.set_ylabel(ylab, linespacing=1.05)
        ax.annotate("(a)" if key == "green" else "(b)", xy=(0, 1), xycoords="axes fraction", xytext=(0, 1.5),
                    textcoords="offset points", ha="left", va="bottom", fontsize=lab, fontweight="bold")
        data[key]["column"], data[key]["readable_name"] = col, readable
    handles = [Line2D([0], [0], marker=T.MODEL_STYLE[m]["marker"], color=T.MODEL_STYLE[m]["color"],
                      linestyle=T.MODEL_STYLE[m]["linestyle"], markersize=3.4, linewidth=0.9, label=m) for m in C.MODEL_ORDER]
    handles.append(Line2D([0], [0], marker="|", color=_IEEE_NEUTRAL_DARK, linestyle="none", markersize=7.5,
                          markeredgewidth=1.4, label="Never passed"))
    fig.legend(handles=handles, loc="outside upper center", ncol=3, frameon=False)
    _check_no_story_ids(fig)
    short = C.MODEL_SHORT
    pairs = []   # events at k = 1 and the number of known values against the macros of numbers.json
    for m in C.MODEL_ORDER:
        g, e2 = data["green"][m]["summary"], data["e2e"][m]["summary"]
        pairs += [(f"GreenFirstTry{short[m]}K", g["events_at_k1"]), (f"AttemptsToGreen{short[m]}N", g["n_events"]),
                  (f"EndToEndFirstPass{short[m]}K", e2["events_at_k1"]), (f"EndToEndAttemptsToPassNoinfra{short[m]}N", e2["n_events"])]
    ref = _numbers_for(ctx, lambda k, v: k in {p[0] for p in pairs})
    check = {"compared": sum(1 for k, _ in pairs if k in ref), "mismatches": [k for k, v in pairs if k in ref and float(ref[k]) != float(v)]}
    gk = data["green"]
    caveats = [
        "descriptive and exploratory: Kaplan-Meier step curves per model, no test, no interval, no new hypothesis; the curves are computed in figures.py from the run columns (no macro yet: n, never-passed runs, shares and medians are in the sidecar numbers)",
        "panel (a) column attempts_to_green (hypothesis H6, 'Test executions to green': index of the first green test execution); panel (b) column e2e_attempts_to_pass_noinfra (Table IV 'End-to-end attempts to pass': index among the end-to-end executions where the API started; e2e_attempts_to_pass, which also counts API-down executions, is the S7 sensitivity and is not drawn)",
        "all runs of S0 (120), no filter; each curve is the share of runs not yet green (a) or not yet passed (b) after each number of executions; 'median' (dotted line at 0.5) marks where the Kaplan-Meier curve reaches 0.5, median_k in data is the smallest number of executions after which that share is at most 0.5",
        "never-passed runs (censored in the Kaplan-Meier sense; legend 'Never passed'): a run whose column is empty never succeeded; it is counted up to its last execution (a: all test executions; b: end-to-end executions with the API up), marked by a tick, kept at risk up to that execution and never counted as a success; a run with 0 counted executions never enters the risk set and has no tick",
        f"never-passed runs: {', '.join(censored_runs) if censored_runs else 'none'}",
        f"panel (a) has {sum(gk[m]['summary']['n_censored'] for m in C.MODEL_ORDER)} never-passed runs: attempts_to_green is known for all 120 runs; a run with final_tests_green false (a red last test execution) still reached green earlier and is an event at its first green, not a never-passed run",
        f"panel (a) x axis is capped at {SURVIVAL_CAP['green']} test executions; runs needing more are not dropped, their curve continues beyond the edge ({', '.join(f'{m}: {gk[m]['summary']['n_beyond_cap']} runs, maximum {gk[m]['summary']['max_time']}' for m in C.MODEL_ORDER)})",
        "a curve whose last observation is a never-passed run ends at that run's last execution (nothing is at risk beyond); a curve that reaches 0 continues along 0 to the edge",
        "markers sit at the number of executions where the curve drops (Kimi circle larger than Qwen square so coincident steps stay visible); vertical axis shared, horizontal axes are not (their ranges differ)",
        f"stories are labelled US1 to US10 in the sidecar ({label_src}); no story id is drawn",
        "events at the first execution and known values against the K and N macros: " + _agreement_caveat(check),
    ]
    return _save(ctx, fig, "fig_survival", data=data, numbers=numbers, width_class="column", rq="N1", kind="survival",
                 filters={**ctx.filters, "set": "S0", "n_runs": int(len(r)), "models": list(C.MODEL_ORDER)}, caveats=caveats,
                 extra={"readable_names": {c: n for _, c, _, _, _, _, n in _SURVIVAL_PANELS},
                        "encoding": {"curve": "Kaplan-Meier survival curve per model, step function, right-continuous",
                                     "marker": "drop of the curve at a whole number of executions (at least one run succeeded there)",
                                     "tick": "never-passed run (censored: never succeeded), at its last counted execution",
                                     "dotted_line": "0.5, the median reference", "x_cap": SURVIVAL_CAP,
                                     "summary_k": list(SURVIVAL_AT)},
                        "macro_check": check})


FIGURE_REGISTRY = {
    "fig_chain": fig_chain, "fig_evidence": fig_evidence, "fig_retry_paths": fig_retry_paths,
    "fig_convergence": fig_convergence, "fig_forest_delta": fig_forest_delta, "fig_fidelity": fig_fidelity,
    "fig_red_first": fig_red_first, "fig_quality": fig_quality, "fig_equivalence": fig_equivalence,
    "fig_passk": fig_passk, "fig_rankings": fig_rankings, "fig_self_report": fig_self_report,
    "fig_behavioural": fig_behavioural, "fig_survival": fig_survival,
}


def make_figures(ctx: FigureContext, only: list[str] | None = None, progress=None) -> dict[str, dict]:
    T.paper_theme()
    ieee = T.ieee_mode()
    out = {}
    for slug, fn in FIGURE_REGISTRY.items():
        if only and slug not in only:
            continue
        if ieee and slug not in IEEE_ADAPTED:
            # TDD_PAPER_THEME=ieee adapts three figures only; never render another one with the IEEE theme at its
            # report size. A named request is an error, an unnamed run (no --only) just skips it.
            if only:
                out[slug] = {"figure": slug, "error": "no IEEE variant (TDD_PAPER_THEME=ieee adapts " + ", ".join(IEEE_ADAPTED) + ")"}
                if progress:
                    progress(slug, True)
            continue
        try:
            out[slug] = fn(ctx)
        except Exception as exc:  # keep going; report at the end
            out[slug] = {"figure": slug, "error": f"{type(exc).__name__}: {exc}"}
            plt.close("all")
        if progress:
            progress(slug, "error" in out[slug])
    return out
