"""Paper theme for figures: ACM acmart sigconf widths, Linux Libertine when TeX Live
provides it, an Okabe-Ito palette with fixed model encodings (colour, marker and line
style, never hue alone), and a ``savefig_with_sidecar`` helper that writes PDF + PNG +
a JSON sidecar holding every plotted number.
"""
from __future__ import annotations

import json
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
                         runs_sha256: str | None = None) -> dict:
    """Write fig_<slug>.pdf, .png and .json; return the sidecar dict."""
    out = Path(out_dir) / "figures"
    out.mkdir(parents=True, exist_ok=True)
    pdf = out / f"{slug}.pdf"
    png = out / f"{slug}.png"
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
    (out / f"{slug}.json").write_text(json.dumps(sidecar, indent=1))
    return sidecar
