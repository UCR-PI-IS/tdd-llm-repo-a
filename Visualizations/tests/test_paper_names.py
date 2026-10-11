"""Name-to-column table of the SANER paper: off by default, unique names, schema columns, Table IV and V agreement.

Table IV and Table V are read from the paper when it sits next to the repository (tdd-llm-docs/technical-report-saner-2027),
otherwise from the verbatim copies in tests/fixtures.
"""
import re
from pathlib import Path

import pandas as pd
import pytest

from tdd_paper import paper_names as PN
from tdd_paper import paper_tables as PT
from tdd_paper.schema import COLUMN_CATALOG

HERE = Path(__file__).resolve().parent
PAPER = HERE.parents[2] / "tdd-llm-docs" / "technical-report-saner-2027" / "sections"
_WORD = {1: "higher", -1: "lower", 0: "none"}


def _source(paper_file: str, fixture: str) -> str:
    p = PAPER / paper_file
    return (p if p.is_file() else HERE / "fixtures" / fixture).read_text()


def _table_iv() -> list[tuple[str, str, str, str, str]]:
    """(family, name, unit, better, provenance) per row of Table IV, in order."""
    rows, family = [], None
    for line in _source("02-6-metrics.tex", "saner_table_iv.tex").splitlines():
        m = re.search(r"\\multicolumn\{5\}\{@\{\}l\}\{\\textbf\{([^}]*)\}\}", line)
        if m:
            family = m.group(1)
            continue
        m = re.match(r"\\raggedright (.+?) & (.+?) & (.+?) & (.+?) & ", line)
        if m:
            rows.append((family, *(g.strip() for g in m.groups())))
    return rows


def _table_v() -> list[tuple[str, str, str, str]]:
    """(group, id, name, band) per row of Table V; band without \\dc{}, '--' for none."""
    rows, group = [], None
    for line in _source("02-7-statistics.tex", "saner_table_v.tex").splitlines():
        m = re.search(r"\\textbf\{([^}]*)\}", line)
        if m:
            group = m.group(1)
            continue
        m = re.match(r"^(H\d)?\s*& (.+?) & (.+?) \\\\", line)
        if m and group:
            band = re.sub(r"\\dc\{([^}]*)\}", r"\1", m.group(3)).strip()
            rows.append((group, m.group(1) or "", m.group(2).strip(), band))
    return rows


def test_off_by_default(monkeypatch, tmp_path):
    monkeypatch.delenv(PN.ENV, raising=False)
    written = PT.make_tables({}, {}, tmp_path)
    assert PN.SLUG not in written and not list((tmp_path / "tables").glob(f"{PN.SLUG}.*"))
    monkeypatch.setenv(PN.ENV, "1")
    written = PT.make_tables({}, {}, tmp_path)
    assert not written[PN.SLUG].startswith("ERROR"), written[PN.SLUG]
    df = pd.read_csv(tmp_path / "tables" / f"{PN.SLUG}.csv")
    assert len(df) == len(PN.TABLE_IV) + len(PN.DESCRIPTIVE) + 1 and (df["row_kind"] == "table-iv").sum() == 24


def test_names_unique_and_columns_in_schema():
    df = PN.name_map()
    m = df[df["row_kind"] != "note"]
    for col in ("readable_name", "package_column", "macro_stem"):
        assert m[col].is_unique, col
    for _, r in m.iterrows():
        spec = COLUMN_CATALOG[r["package_column"]]          # KeyError if the column is not in the schema
        assert r["provenance"] == spec.provenance.value
        assert r["better"] == _WORD[spec.direction], r["package_column"]


def test_missing_runs_column_is_refused(tmp_path):
    runs = pd.DataFrame(columns=[c for c in COLUMN_CATALOG if c != "story_max_cc"])
    with pytest.raises(ValueError, match="story_max_cc"):
        PN.tab_metric_names({"runs": runs}, tmp_path)


def test_rows_match_table_iv():
    expected = _table_iv()
    got = [(e.family, e.name, e.unit, e.better, COLUMN_CATALOG[e.column].provenance.value) for e in PN.TABLE_IV]
    assert len(expected) == 24 and got == expected


def test_roles_match_table_v():
    by_name = {e.name: e for e in PN.TABLE_IV}
    rows = _table_v()
    assert len(rows) == 17
    for group, hid, name, band in rows:
        e = by_name[name]
        r = PN.role(e.column, e.unit)
        if group == "Confirmatory":
            assert r.startswith(f"{hid} (confirmatory)"), (name, r)
        elif group == "Secondary":
            assert r == "secondary", (name, r)
        else:
            assert r.startswith("equivalence only"), (name, r)
        if band != "--":
            assert f"band {band}" in r, (name, r, band)
    in_v = {name for _, _, name, _ in rows}
    for e in PN.TABLE_IV:
        if e.name not in in_v:
            assert PN.role(e.column, e.unit) in ("reliability flag (pass@k, pass^k)", "descriptive"), e.name


def test_agrees_with_ieee_figure_names():
    from tdd_paper import figures as F
    ieee = {**F.IEEE_METRIC_NAMES, **F.IEEE_FLAG_NAMES}
    ours = {e.column: e.name for e in PN.TABLE_IV + PN.DESCRIPTIVE}
    assert {c: (ours[c], ieee[c]) for c in ours.keys() & ieee.keys() if ours[c] != ieee[c]} == {}
    assert ieee.keys() <= ours.keys()
