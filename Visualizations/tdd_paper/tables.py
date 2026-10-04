"""LaTeX tables and the numbers registry.

* ``to_booktabs`` writes a bare ``tabular`` body (``\\toprule``, ``\\midrule``,
  ``\\bottomrule``); writers own the float, caption and label.
* ``Numbers`` collects every quantity the report may quote, with provenance,
  and writes ``numbers.json`` plus ``numbers.tex`` (one ``\\newcommand`` per
  macro, prefix ``\\val``). Macro names are CamelCase letters only; digits are
  spelled out so that LaTeX accepts them.
"""
from __future__ import annotations

import json
import math
import re
from datetime import datetime, timezone
from pathlib import Path

import numpy as np
import pandas as pd

from . import config as C

_DIGITS = {"0": "Zero", "1": "One", "2": "Two", "3": "Three", "4": "Four", "5": "Five", "6": "Six",
           "7": "Seven", "8": "Eight", "9": "Nine"}
_MACRO_RE = re.compile(r"^[A-Za-z]+$")
# Tokens that read badly when digits are spelled out letter by letter.
_WORD_MAP = {"e2e": "EndToEnd", "a12": "Atwelve", "i2": "Isq", "q1": "QOne", "q3": "QThree"}


_NUMBER_WORDS = {10: "Ten", 11: "Eleven", 12: "Twelve", 13: "Thirteen", 14: "Fourteen", 15: "Fifteen", 16: "Sixteen",
                 17: "Seventeen", 18: "Eighteen", 19: "Nineteen", 20: "Twenty"}


def spell_digits(text: str) -> str:
    """Spell every digit run: 0 to 9 digit by digit, 10 to 20 as one word (S10 -> STen), larger runs digit by digit."""
    def repl(m: re.Match) -> str:
        n = int(m.group(0))
        if 10 <= n <= 20 and not m.group(0).startswith("0"):
            return _NUMBER_WORDS[n]
        return "".join(_DIGITS[ch] for ch in m.group(0))
    return re.sub(r"\d+", repl, str(text))


def macro_name(*parts: str) -> str:
    """CamelCase macro name from parts such as ("kimi", "failed_builds", "median", "S1").
    Digits are spelled out (LaTeX control sequences are letters only); ``e2e`` becomes ``EndToEnd``."""
    words: list[str] = []
    for part in parts:
        if part is None or part == "":
            continue
        for tok in re.split(r"[^0-9A-Za-z]+", str(part)):
            if tok:
                tok = _WORD_MAP.get(tok.lower(), spell_digits(tok))
                words.append(tok[0].upper() + tok[1:])
    name = "".join(words)
    if not _MACRO_RE.match(name):
        raise ValueError(f"macro name must be letters only: {name!r}")
    return name


def fmt_value(value, fmt: str = "{:.2f}") -> str:
    if value is None or (isinstance(value, float) and math.isnan(value)) or value is pd.NA:
        return "n/a"
    if isinstance(value, (bool, np.bool_)):
        return "yes" if value else "no"
    if isinstance(value, (np.integer,)):
        value = int(value)
    if isinstance(value, (np.floating,)):
        value = float(value)
    if fmt == "p":
        return fmt_p(value)
    if fmt == "int":
        return f"{int(round(value))}"
    if fmt == "pct":
        return f"{100 * value:.0f}\\%"
    if fmt == "pct1":
        return f"{100 * value:.1f}\\%"
    return fmt.format(value)


def fmt_p(p: float) -> str:
    if p is None or (isinstance(p, float) and math.isnan(p)):
        return "n/a"
    return "$<$0.001" if p < 0.001 else f"{p:.3f}"


def fmt_k_of_n(k: int, n: int) -> str:
    share = k / n if n else float("nan")
    return f"{k} of {n} ({share:.2f})" if n else "n/a"


def latex_escape(text) -> str:
    if text is None or (isinstance(text, float) and math.isnan(text)):
        return ""
    s = str(text)
    for a, b in (("\\", r"\textbackslash{}"), ("&", r"\&"), ("%", r"\%"), ("$", r"\$"), ("#", r"\#"),
                 ("_", r"\_"), ("{", r"\{"), ("}", r"\}"), ("~", r"\textasciitilde{}"), ("^", r"\textasciicircum{}")):
        s = s.replace(a, b)
    return s


def to_booktabs(df: pd.DataFrame, columns: list[tuple[str, str, str]], *, align: str | None = None,
                escape: bool = True, midrule_every: int | None = None, group_col: str | None = None) -> str:
    """``columns`` = [(column, header, fmt)] where fmt is a format string, "int", "p", "pct",
    "pct1", "raw" (already LaTeX) or "str". Returns the tabular environment body."""
    align = align or ("l" + "r" * (len(columns) - 1))
    lines = [f"\\begin{{tabular}}{{{align}}}", "\\toprule",
             " & ".join(latex_escape(h) if escape else h for _, h, _ in columns) + " \\\\", "\\midrule"]
    last_group = None
    for i, (_, row) in enumerate(df.iterrows()):
        if group_col is not None and row[group_col] != last_group and last_group is not None:
            lines.append("\\addlinespace")
        last_group = row[group_col] if group_col is not None else None
        cells = []
        for col, _, fmt in columns:
            v = row[col] if col in row.index else ""
            if fmt == "raw":
                cells.append("" if v is None or (isinstance(v, float) and math.isnan(v)) else str(v))
            elif fmt == "str":
                cells.append(latex_escape(v) if escape else str(v))
            else:
                cells.append(fmt_value(v, fmt))
        lines.append(" & ".join(cells) + " \\\\")
        if midrule_every and (i + 1) % midrule_every == 0 and i + 1 < len(df):
            lines.append("\\midrule")
    lines += ["\\bottomrule", "\\end{tabular}"]
    return "\n".join(lines) + "\n"


def write_table(out_dir: Path, slug: str, body: str, *, meta: dict | None = None) -> Path:
    out = Path(out_dir) / "tables"
    out.mkdir(parents=True, exist_ok=True)
    path = out / f"{slug}.tex"
    header = (f"% {slug}: generated by tdd_paper {C.__name__.split('.')[0]} on "
              f"{datetime.now(timezone.utc).isoformat(timespec='seconds')}; do not edit by hand.\n")
    path.write_text(header + body)
    (out / f"{slug}.json").write_text(json.dumps({"table": slug, **(meta or {})}, indent=1, default=str))
    return path


class Numbers:
    """Registry of every number the report may cite."""

    def __init__(self, meta: dict | None = None):
        self.meta = meta or {}
        self.entries: dict[str, dict] = {}

    def add(self, macro: str, value, *, fmt: str = "{:.2f}", unit: str = "", family: str = "", metric: str = "",
            stat: str = "", group: dict | None = None, provenance: str = "tool-measured",
            source_columns=(), n=None, set_id: str = "S0", ci: dict | None = None, test: dict | None = None,
            code_ref: str = "", notes: str = "", example_run: str | None = None) -> str:
        if not _MACRO_RE.match(macro):
            macro = macro_name(macro)
        if macro in self.entries and self.entries[macro]["value"] != _plain(value):
            raise ValueError(f"macro {macro} registered twice with different values")
        self.entries[macro] = {
            "value": _plain(value), "text": fmt_value(value, fmt), "fmt": fmt, "unit": unit,
            "family": family, "metric": metric, "stat": stat, "group": group or {},
            "provenance": provenance, "source_columns": list(source_columns),
            "n": None if n is None else int(n), "set": set_id, "ci": ci, "test": test,
            "code_ref": code_ref, "notes": notes, "example_run": example_run,
        }
        return macro

    def to_dict(self) -> dict:
        return {"meta": {**self.meta, "generated_at": datetime.now(timezone.utc).isoformat(timespec="seconds"),
                         "n_numbers": len(self.entries), "macro_prefix": C.MACRO_PREFIX},
                "numbers": dict(sorted(self.entries.items()))}

    def write(self, out_dir: Path) -> tuple[Path, Path]:
        out = Path(out_dir)
        out.mkdir(parents=True, exist_ok=True)
        jpath = out / "numbers.json"
        tpath = out / "numbers.tex"
        payload = self.to_dict()
        jpath.write_text(json.dumps(payload, indent=1, default=str))
        lines = [f"% numbers.tex generated by tdd_paper on {payload['meta']['generated_at']}; do not edit by hand.",
                 f"% {len(self.entries)} macros; every value carries a provenance tag in numbers.json.",
                 "\\providecommand{\\valmissing}{\\textcolor{red}{[number missing]}}"]
        for macro, e in sorted(self.entries.items()):
            comment = f"% {e['provenance']} | {e['metric']} {e['stat']} | n={e['n']} | set {e['set']}"
            lines.append(f"\\newcommand{{\\{C.MACRO_PREFIX}{macro}}}{{{e['text']}}} {comment}")
        tpath.write_text("\n".join(lines) + "\n")
        return jpath, tpath


def _plain(value):
    if isinstance(value, (np.integer,)):
        return int(value)
    if isinstance(value, (np.floating,)):
        return None if np.isnan(value) else float(value)
    if isinstance(value, (np.bool_,)):
        return bool(value)
    if value is pd.NA:
        return None
    if isinstance(value, float) and math.isnan(value):
        return None
    return value
