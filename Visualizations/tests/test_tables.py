"""Booktabs writer, macro names and the numbers registry round trip."""
import json

import pandas as pd
import pytest

from tdd_paper import tables as T


def test_macro_name_spells_digits_and_rejects_bad():
    assert T.macro_name("kimi", "failed_builds", "median", "S1") == "KimiFailedBuildsMedianSOne"
    assert T.macro_name("PassAt", "3") == "PassAtThree"
    with pytest.raises(ValueError):
        T.macro_name("")


def test_latex_escape_and_booktabs():
    df = pd.DataFrame({"a": ["x_1 & y", "z%"], "b": [1.234, float("nan")], "c": [0.5, 0.25]})
    body = T.to_booktabs(df, [("a", "Name", "str"), ("b", "Value", "{:.2f}"), ("c", "Share", "pct")])
    assert "\\toprule" in body and "\\bottomrule" in body
    assert "x\\_1 \\& y" in body and "z\\%" in body
    assert "n/a" in body and "50\\%" in body


def test_fmt_helpers():
    assert T.fmt_p(0.0004) == "$<$0.001"
    assert T.fmt_p(0.0234) == "0.023"
    assert T.fmt_k_of_n(43, 60) == "43 of 60 (0.72)"
    assert T.fmt_value(True) == "yes"


def test_numbers_registry_round_trip(tmp_path):
    nums = T.Numbers({"seed": 1})
    nums.add("KimiFailedBuildsMedian", 4.0, fmt="{:.1f}", unit="executions", metric="build_failed_execs", stat="median",
             provenance="tool-measured", n=60)
    nums.add("Tiny", 0.0004, fmt="p", provenance="statistic")
    with pytest.raises(ValueError):
        nums.add("KimiFailedBuildsMedian", 5.0)
    jpath, tpath = nums.write(tmp_path)
    tex = tpath.read_text()
    assert "\\newcommand{\\valKimiFailedBuildsMedian}{4.0}" in tex
    assert "\\newcommand{\\valTiny}{$<$0.001}" in tex
    payload = json.loads(jpath.read_text())
    assert payload["numbers"]["KimiFailedBuildsMedian"]["provenance"] == "tool-measured"
    # re-emit from JSON gives the same macro lines
    nums2 = T.Numbers(payload["meta"])
    nums2.entries = payload["numbers"]
    _, tpath2 = nums2.write(tmp_path / "again")
    assert [l for l in tpath2.read_text().splitlines() if l.startswith("\\newcommand")] == \
           [l for l in tex.splitlines() if l.startswith("\\newcommand")]



def test_compact_metric_catalog_holds_every_tested_metric_once(tmp_path):
    """Table 2 of the report (D16): every metric with a role appears exactly once, US spelling, fits by construction."""
    from tdd_paper import config as C
    from tdd_paper import paper_tables as T
    T.tab_metric_catalog(tmp_path)
    body = (tmp_path / "tables" / "tab_metric_catalog.tex").read_text()
    wanted = set(C.CONFIRMATORY_FAMILY) | set(C.SECONDARY_FAMILY) | set(C.TOST_BANDS) | set(C.SUCCESS_PREDICATES)
    for m in wanted:
        assert body.count("\\texttt{" + m.replace("_", "\\_") + "}") == 1, m
    rows = [l for l in body.splitlines() if l.startswith("\\texttt{")]
    assert len(rows) == len(wanted)
    assert "behavioural" not in body and "\\parbox[t]" in body
    roles = T.metric_roles()
    assert roles["new_tests_per_intent"][0] == "H1" and roles["e2e_execs"] == ["H8"]
    T.tab_metric_catalog_full(tmp_path)
    assert (tmp_path / "tables" / "tab_metric_catalog_full.csv").exists()
