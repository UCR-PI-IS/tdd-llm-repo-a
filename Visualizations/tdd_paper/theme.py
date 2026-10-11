"""Paper theme for figures: ACM acmart sigconf widths, Linux Libertine when TeX Live
provides it, an Okabe-Ito palette with fixed model encodings (colour, marker and line
style, never hue alone), and a ``savefig_with_sidecar`` helper that writes PDF + PNG +
a JSON sidecar holding every plotted number.

IEEE switch (SANER 2027 paper): when the environment variable ``TDD_PAPER_THEME`` is
``ieee`` (see ``ieee_mode``), ``paper_theme`` applies a Times face (TeX Gyre Termes) at the
IEEEtran column width, and ``savefig_with_sidecar`` audits the smallest font and records the
IEEE facts in the sidecar. Without the variable nothing in this module behaves differently:
the report's figures are rendered exactly as before.
"""
from __future__ import annotations

import json
import os
import shutil
import subprocess
from datetime import datetime, timezone
from pathlib import Path

import matplotlib

matplotlib.use("Agg")
import matplotlib.pyplot as plt  # noqa: E402
from matplotlib import font_manager  # noqa: E402

from . import config as C  # noqa: E402

COLUMN_W = C.COLUMN_WIDTH_IN
TEXT_W = C.TEXT_WIDTH_IN
GOLDEN = 0.618

# Okabe-Ito, colour-vision-deficiency safe.
OKABE_ITO = {"blue": "#0072B2", "orange": "#E69F00", "green": "#009E73", "sky": "#56B4E9",
             "vermilion": "#D55E00", "purple": "#CC79A7", "yellow": "#F0E442", "black": "#000000"}
MODEL_STYLE = {
    "Kimi-K2.5": {"color": OKABE_ITO["blue"], "marker": "o", "linestyle": "-", "hatch": ""},
    "Qwen3.7-max": {"color": OKABE_ITO["orange"], "marker": "s", "linestyle": "--", "hatch": "//"},
}
NEUTRAL = "#7f7f7f"
LIGHT = "#d9d9d9"
LIGHTER = "#f0f0f0"
STATUS = {"success": OKABE_ITO["green"], "failure": OKABE_ITO["vermilion"], "infra": NEUTRAL,
          "snapshot": OKABE_ITO["purple"], "red": OKABE_ITO["vermilion"], "green": OKABE_ITO["green"]}
BAND_GREYS = {"GREEN": "#ffffff", "YELLOW": "#efefef", "RED": "#dcdcdc"}

_FONT_FAMILY: str | None = None


def _kpsewhich(name: str) -> Path | None:
    exe = shutil.which("kpsewhich") or "/Library/TeX/texbin/kpsewhich"
    if not Path(exe).exists():
        return None
    proc = subprocess.run([exe, name], capture_output=True, text=True)
    out = proc.stdout.strip()
    return Path(out) if proc.returncode == 0 and out and Path(out).exists() else None


def register_fonts() -> str | None:
    """Register Linux Libertine O (acmart's text face) from TeX Live if present.
    Returns the family name to use, or None to fall back to the default serif list."""
    global _FONT_FAMILY
    if _FONT_FAMILY is not None:
        return _FONT_FAMILY or None
    found = []
    for name in ("LinLibertine_R.otf", "LinLibertine_RB.otf", "LinLibertine_RI.otf", "LinLibertine_RBI.otf"):
        path = _kpsewhich(name)
        if path:
            try:
                font_manager.fontManager.addfont(str(path))
                found.append(path)
            except Exception:  # pragma: no cover - font registration is best effort
                pass
    if found:
        try:
            _FONT_FAMILY = font_manager.FontProperties(fname=str(found[0])).get_name()
        except Exception:  # pragma: no cover
            _FONT_FAMILY = "Linux Libertine O"
    else:
        _FONT_FAMILY = ""
    return _FONT_FAMILY or None


def paper_theme(base: float = 8.0) -> dict:
    """Apply the paper rcParams. Returns the font family actually in use."""
    if ieee_mode():
        return _ieee_paper_theme()
    fam = register_fonts()
    serif = ([fam] if fam else []) + ["Libertinus Serif", "Times New Roman", "Times", "STIXGeneral", "DejaVu Serif"]
    plt.rcParams.update({
        "font.family": "serif", "font.serif": serif, "mathtext.fontset": "stix",
        "font.size": base, "axes.titlesize": base, "axes.labelsize": base,
        "xtick.labelsize": base - 1, "ytick.labelsize": base - 1, "legend.fontsize": base - 1,
        "legend.frameon": False, "legend.handlelength": 1.6,
        "axes.spines.top": False, "axes.spines.right": False, "axes.linewidth": 0.6,
        "axes.grid": True, "axes.grid.axis": "y", "grid.color": LIGHTER, "grid.linewidth": 0.5,
        "xtick.major.width": 0.6, "ytick.major.width": 0.6, "xtick.major.size": 2.5, "ytick.major.size": 2.5,
        "lines.linewidth": 1.1, "lines.markersize": 3.5, "patch.linewidth": 0.6,
        "figure.dpi": 110, "savefig.dpi": C.FIGURE_DPI, "savefig.bbox": "tight", "savefig.pad_inches": 0.02,
        "pdf.fonttype": 42, "ps.fonttype": 42, "figure.constrained_layout.use": True,
        "axes.prop_cycle": matplotlib.cycler(color=[OKABE_ITO[k] for k in ("blue", "orange", "green", "purple", "vermilion", "sky")]),
    })
    return {"font_family": fam or "fallback serif"}


# ---------------------------------------------------------------------------
# IEEE switch (SANER 2027 paper, IEEEtran conference, 10 pt Times)
# ---------------------------------------------------------------------------

IEEE_ENV = "TDD_PAPER_THEME"
IEEE_COLUMN_W = 3.486        # IEEEtran conference \columnwidth = 252.0 TeX pt = 3.4868 in (3.5 in nominal); a 3.5 in PDF is
                             # 0.94 pt too wide for the column (Overfull \hbox), so figures are 3.486 in and included unscaled
IEEE_MIN_FONT_PT = 6.5       # nothing smaller at final size
IEEE_FONT_PT = {"label": 7.0, "tick": 6.5, "legend": 6.5, "annot": 6.5}
_IEEE_FAMILY: str | None = None


def ieee_mode() -> bool:
    """True when ``TDD_PAPER_THEME=ieee``. Read at call time so one process can render both variants."""
    return os.environ.get(IEEE_ENV, "").strip().lower() == "ieee"


def ieee_fig_size(height: float) -> tuple[float, float]:
    return (IEEE_COLUMN_W, height)


def register_times() -> str | None:
    """Register TeX Gyre Termes (a Times clone, the face IEEEtran sets text in) from TeX Live if present.
    Returns the family name, or None when it is not installed (the serif fallback list then applies)."""
    found = []
    for name in ("texgyretermes-regular.otf", "texgyretermes-bold.otf", "texgyretermes-italic.otf",
                 "texgyretermes-bolditalic.otf"):
        path = _kpsewhich(name)
        if path:
            try:
                font_manager.fontManager.addfont(str(path))
                found.append(path)
            except Exception:  # pragma: no cover - font registration is best effort
                pass
    if not found:
        return None
    try:
        return font_manager.FontProperties(fname=str(found[0])).get_name()
    except Exception:  # pragma: no cover
        return "TeX Gyre Termes"


def _ieee_paper_theme() -> dict:
    """rcParams for the IEEE column: Times face, 7 pt labels, 6.5 pt ticks and legend, exact page size
    (no tight bounding box, so the PDF page is exactly the column wide), explicit layout per figure."""
    global _IEEE_FAMILY
    fam = register_times()
    serif = ([fam] if fam else []) + ["Nimbus Roman", "Times New Roman", "Times", "STIXGeneral", "DejaVu Serif"]
    lab, tick, leg = IEEE_FONT_PT["label"], IEEE_FONT_PT["tick"], IEEE_FONT_PT["legend"]
    plt.rcParams.update({
        "font.family": "serif", "font.serif": serif, "mathtext.fontset": "stix",
        "font.size": lab, "axes.titlesize": lab, "axes.labelsize": lab,
        "xtick.labelsize": tick, "ytick.labelsize": tick, "legend.fontsize": leg,
        "legend.frameon": False, "legend.handlelength": 1.8, "legend.handletextpad": 0.4,
        "legend.columnspacing": 1.2, "legend.borderaxespad": 0.2,
        "axes.spines.top": False, "axes.spines.right": False, "axes.linewidth": 0.5,
        "axes.grid": True, "axes.grid.axis": "y", "grid.color": LIGHTER, "grid.linewidth": 0.4,
        "axes.labelpad": 1.5, "axes.titlepad": 2.0, "axes.unicode_minus": True,
        "xtick.major.width": 0.5, "ytick.major.width": 0.5, "xtick.major.size": 2.0, "ytick.major.size": 2.0,
        "xtick.major.pad": 1.5, "ytick.major.pad": 1.5,
        "lines.linewidth": 0.9, "lines.markersize": 3.0, "patch.linewidth": 0.5,
        "figure.dpi": 110, "savefig.dpi": C.FIGURE_DPI, "savefig.bbox": "standard", "savefig.pad_inches": 0.0,
        "pdf.fonttype": 42, "ps.fonttype": 42, "figure.constrained_layout.use": False,
        "figure.constrained_layout.h_pad": 0.02, "figure.constrained_layout.w_pad": 0.02,
        "figure.constrained_layout.hspace": 0.02, "figure.constrained_layout.wspace": 0.02,
        "axes.prop_cycle": matplotlib.cycler(color=[OKABE_ITO[k] for k in ("blue", "orange", "green", "purple", "vermilion", "sky")]),
    })
    try:
        path = font_manager.findfont(font_manager.FontProperties(family="serif"), fallback_to_default=True)
        _IEEE_FAMILY = font_manager.FontProperties(fname=path).get_name()
    except Exception:  # pragma: no cover
        _IEEE_FAMILY = fam or "fallback serif"
    return {"font_family": _IEEE_FAMILY, "termes": bool(fam)}


def min_font_pt(fig) -> float:
    """Smallest effective font size (pt) of any visible text in the figure. Mathtext sub- and superscripts
    render at 70 % of the base size, so they are counted at that size."""
    from matplotlib.text import Text
    fig.canvas.draw()
    sizes = []
    for t in fig.findobj(Text):
        s = t.get_text().strip()
        if not s or not t.get_visible():
            continue
        size = float(t.get_fontsize())
        if "$" in s and ("^" in s or "_" in s):
            size *= 0.7
        sizes.append(size)
    return min(sizes) if sizes else float("nan")


def figure_texts(fig) -> list[str]:
    """Every visible, non-empty text string of the figure (labels, ticks, titles, legend, annotations), once each, in
    artist order. Recorded in the IEEE sidecar so the wording can be checked against the paper's metric table."""
    from matplotlib.text import Text
    fig.canvas.draw()
    out: list[str] = []
    for t in fig.findobj(Text):
        s = t.get_text().strip()
        if s and t.get_visible() and s not in out:
            out.append(s)
    return out


def fig_size(width: str = "column", aspect: float = GOLDEN, height: float | None = None) -> tuple[float, float]:
    w = COLUMN_W if width == "column" else TEXT_W
    return (w, height if height is not None else w * aspect)


def despine(ax, left: bool = False) -> None:
    ax.spines["top"].set_visible(False)
    ax.spines["right"].set_visible(False)
    if left:
        ax.spines["left"].set_visible(False)


def model_color(model: str) -> str:
    return MODEL_STYLE.get(model, {"color": NEUTRAL})["color"]


def threshold_bands(ax, metric: str, axis: str = "y", label: bool = True) -> None:
    """Grey bands for the harness thresholds (same semantics as tdd_results.add_threshold_bands)."""
    import tdd_results as tdd  # local import keeps theme importable without the repo
    bands = tdd.THRESHOLDS.get(metric)
    if not bands:
        return
    lo, hi = (ax.get_ylim() if axis == "y" else ax.get_xlim())
    span = ax.axhspan if axis == "y" else ax.axvspan
    for a, b, flag in bands:  # tdd_results.THRESHOLDS: [(start, end, flag), ...], None = unbounded
        a2 = lo if a is None else a
        b2 = hi if b is None else b
        if flag == "GREEN":
            continue
        span(a2, b2, facecolor=BAND_GREYS.get(flag, LIGHT), edgecolor="none", zorder=0)
        if label:
            mid = (a2 + b2) / 2
            if axis == "y":
                ax.text(1.0, mid, flag.lower(), transform=ax.get_yaxis_transform(), ha="right", va="center",
                        fontsize=plt.rcParams["font.size"] - 2, color=NEUTRAL)
            else:
                ax.text(mid, 1.0, flag.lower(), transform=ax.get_xaxis_transform(), ha="center", va="bottom",
                        fontsize=plt.rcParams["font.size"] - 2, color=NEUTRAL)


def _jsonable(obj):
    import numpy as np
    import pandas as pd
    if isinstance(obj, dict):
        return {str(k): _jsonable(v) for k, v in obj.items()}
    if isinstance(obj, (list, tuple)):
        return [_jsonable(v) for v in obj]
    if isinstance(obj, (np.integer,)):
        return int(obj)
    if isinstance(obj, (np.floating,)):
        return None if np.isnan(obj) else float(obj)
    if isinstance(obj, (np.bool_,)):
        return bool(obj)
    if isinstance(obj, np.ndarray):
        return _jsonable(obj.tolist())
    if isinstance(obj, (pd.Timestamp, datetime)):
        return obj.isoformat()
    if isinstance(obj, float) and obj != obj:
        return None
    if obj is pd.NA:
        return None
    return obj


def savefig_with_sidecar(fig, out_dir: Path, slug: str, *, data: dict, numbers: dict | None = None,
                         caveats: list[str] | None = None, width_class: str = "column", rq: str | None = None,
                         kind: str = "", filters: dict | None = None, seed: int | None = None,
                         runs_sha256: str | None = None, extra: dict | None = None) -> dict:
    """Write fig_<slug>.pdf, .png and .json; return the sidecar dict. ``extra`` is merged into the sidecar
    only under the IEEE switch (the report's sidecars keep their schema)."""
    out = Path(out_dir) / "figures"
    out.mkdir(parents=True, exist_ok=True)
    pdf = out / f"{slug}.pdf"
    png = out / f"{slug}.png"
    ieee = ieee_mode()
    smallest = None
    texts: list[str] = []
    if ieee:
        smallest = round(min_font_pt(fig), 2)
        if not smallest >= IEEE_MIN_FONT_PT:
            plt.close(fig)
            raise ValueError(f"{slug}: smallest text is {smallest} pt, below the {IEEE_MIN_FONT_PT} pt floor")
        texts = figure_texts(fig)
        leaks = [t for t in texts if "_" in t or "e2e" in t.lower()]
        if leaks:  # P31: readable names only, "End-to-end" never "e2e"; a package id would contain an underscore
            plt.close(fig)
            raise ValueError(f"{slug}: text that looks like a package id or says 'e2e': {leaks}")
    fig.savefig(pdf)
    fig.savefig(png, dpi=C.FIGURE_DPI)
    plt.close(fig)
    w, h = fig.get_size_inches()
    sidecar = {
        "figure": slug, "rq": rq, "kind": kind, "width_class": width_class,
        "size_in": [round(float(w), 3), round(float(h), 3)], "dpi": C.FIGURE_DPI,
        "generated_at": datetime.now(timezone.utc).isoformat(timespec="seconds"),
        "seed": seed, "runs_sha256": runs_sha256, "filters": filters or {},
        "files": [pdf.name, png.name, f"{slug}.json"],
        "font_family": _FONT_FAMILY or "fallback serif",
        "data": _jsonable(data), "numbers": _jsonable(numbers or {}), "caveats": caveats or [],
    }
    if ieee:
        sidecar["font_family"] = _IEEE_FAMILY or "fallback serif"
        sidecar["theme"] = "ieee"
        sidecar["column_width_in"] = IEEE_COLUMN_W
        sidecar["font_sizes_pt"] = dict(IEEE_FONT_PT)
        sidecar["min_font_pt"] = smallest
        sidecar["texts"] = texts
        if _IEEE_FAMILY != "TeX Gyre Termes":
            sidecar["caveats"] = list(sidecar["caveats"]) + [f"Times face fallback in use: {_IEEE_FAMILY}"]
        sidecar.update(_jsonable(extra or {}))
    (out / f"{slug}.json").write_text(json.dumps(sidecar, indent=1))
    return sidecar
