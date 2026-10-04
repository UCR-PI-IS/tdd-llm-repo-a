"""Git facts: resolver, baseline walk, counters (repo-gated) and the pure parser."""
import pytest

from tdd_paper import config as C
from tdd_paper import gitfacts as gf

from conftest import requires_repo


def test_count_test_markers_multiline_attributes():
    src = '''
[TestCase(1, 2,
    Description = "two-line attribute")]
[TestCase(3, 4)]
public void A_B_C(int a, int b) { Assert.That(a, Is.LessThan(b)); }
[Test] public void D() { }
[Test]
public async Task E()
{
    var x = 1;
    Assert.Pass();
}
[TestCaseSource(nameof(Cases))]
public void F(int v) => Assert.That(v, Is.Positive);
public static IEnumerable<int> Cases() { yield return 1; }
'''
    r = gf.count_test_markers(src)
    assert r["cases"] == 5 and r["methods"] == 4
    assert r["method_names"] == ["A_B_C", "D", "E", "F"]
    assert r["assertion_free"] == 1  # D has no assertion


@requires_repo
def test_run_refs_and_resolution():
    refs = gf.list_run_refs()
    assert len(refs) >= 260
    res = gf.resolve_cell_ref("CPD-LC-001-003", "kimik25", 1, refs)
    assert res["ref"] == "runs/wave-1/CPD-LC-001-003/Kimi-K2.5/1"
    assert res["overlap"] == res["n_wt"] > 0 and not res["ambiguous"]
    # split spelling: evidence under Kimi-K2-5 resolves to that branch, not the stale pointer
    res5 = gf.resolve_cell_ref("CPD-LC-001-005", "kimik25", 5, refs)
    assert res5["ref"].endswith("CPD-LC-001-005/Kimi-K2-5/5") and not res5["partial"]


@requires_repo
def test_baseline_walk_and_calibration():
    cell, methods, cases = C.CALIBRATION_CELL
    story, model, it = cell.split("/")
    f = gf.cell_facts(story, "kimik25", int(it))
    assert f["n_run_commits"] == 2
    link = next(l for l in C.STORY_CHAIN if l.story == story)
    assert f["baseline_sha"].startswith(link.baseline_sha)
    assert f["new_test_methods"] == methods and f["new_test_cases"] == cases
    assert (f["new_test_methods_Domain"], f["new_test_methods_Application"],
            f["new_test_methods_Infrastructure"], f["new_test_methods_Presentation"]) == (14, 4, 2, 5)
    assert f["identity_ambiguous"] is False and f["harness_pre_e2e"] is False


@requires_repo
def test_merged_run_baseline_is_not_its_tip():
    # fast-forwarded merged run: merge-base would be the tip, the walk gives the real baseline
    f = gf.cell_facts("PQL-AE-001-002", "kimik25", 3)
    assert f["baseline_sha"].startswith("2374db15d")
    assert f["identity_ambiguous"] is True  # agent files on that branch name qwen


@requires_repo
def test_story_chain_facts():
    rows = gf.story_chain_facts()
    assert [r["pos"] for r in rows] == list(range(1, 11))
    assert rows[0]["baseline_test_attrs"] == 0 and rows[-1]["baseline_test_attrs"] > 200
    assert all(r["baseline_prod_lines"] > 0 for r in rows)
