"""Loader fixes in tdd_results: stage-file identity from the path relative to the tree root,
and non-execution folders (latest/, code-generator/, refactor-generator/) skipped."""
import json
from pathlib import Path

import tdd_results as tdd


def _write(path: Path, payload: dict) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(payload))


def _fake_tree(root: Path) -> None:
    story = "FAKE-ST-001-001"
    test = root / "TestResults" / story
    metrics = root / "MetricsResults" / story
    build = root / "BuildResults" / story
    # canonical test execution + canonical stage file
    _write(test / "ModelA" / "1" / "2026-01-01_10-00-00" / "test-summary.json",
           {"status": "success", "timestamp": "2026-01-01_10-00-00", "projects": [], "totalTests": 3,
            "totalPassed": 3, "totalFailed": 0, "totalSkipped": 0})
    _write(test / "ModelA" / "1" / "test-generator" / "pipeline-stage-result.json",
           {"stage": "test-generation", "status": "success", "iteration": 1, "model": "whatever",
            "metrics": {"intentsConfirmed": 2, "testMethodsEmitted": 3}})
    # misplaced stage file one level too deep (the SPT-UM-001-003 pattern)
    _write(test / "ModelA" / "2" / "20260923T000000Z" / "test-generator" / "pipeline-stage-result.json",
           {"stage": "test-generation", "status": "success", "iteration": 2, "metrics": {"testMethodsEmitted": 5}})
    _write(test / "ModelA" / "2" / "2026-01-02_10-00-00" / "test-summary.json",
           {"status": "success", "projects": [], "totalTests": 1, "totalPassed": 1, "totalFailed": 0, "totalSkipped": 0})
    # stray code-generator folder holding a stage file
    _write(test / "ModelA" / "2" / "code-generator" / "pipeline-stage-result.json",
           {"stage": "code-generation", "status": "success", "iteration": 2, "metrics": {}})
    # metrics: a timestamped snapshot and a duplicate 'latest' folder
    snap = {"status": "success", "timestamp": "2026-01-01T10:05:00Z", "projects": []}
    _write(metrics / "ModelA" / "1" / "2026-01-01_10-05-00" / "metrics-summary.json", snap)
    _write(metrics / "ModelA" / "1" / "latest" / "metrics-summary.json", snap)
    _write(metrics / "ModelA" / "1" / "2026-01-01_10-05-00" / "pipeline-stage-result.json",
           {"stage": "refactoring", "status": "success", "iteration": 1,
            "metrics": {"loopIterationsPerformed": 1, "maxLoopIterations": 10, "allGreenAchieved": True,
                        "remainingViolations": [{"type": "X", "metric": "cc", "value": 30, "flag": "RED"}]}})
    _write(build / "ModelA" / "1" / "2026-01-01_09-59-00" / "build-summary.json",
           {"status": "success", "projects": [], "totalErrors": 0, "totalWarnings": 0})


def test_stage_identity_and_non_execution_folders(tmp_path, monkeypatch):
    _fake_tree(tmp_path)
    monkeypatch.setattr(tdd, "REPO_ROOT", tmp_path)
    monkeypatch.setattr(tdd, "BUILD_TREE", tmp_path / "BuildResults")
    monkeypatch.setattr(tdd, "TEST_TREE", tmp_path / "TestResults")
    monkeypatch.setattr(tdd, "METRICS_TREE", tmp_path / "MetricsResults")
    monkeypatch.setattr(tdd, "E2E_TREE", tmp_path / "E2EResults")
    monkeypatch.setattr(tdd, "TREES", {"build": tmp_path / "BuildResults", "test": tmp_path / "TestResults",
                                       "metrics": tmp_path / "MetricsResults", "e2e": tmp_path / "E2EResults"})
    tdd.reset_caches()
    story = "FAKE-ST-001-001"
    stages = tdd.load_stage_results(story)
    assert sorted(stages["model"].unique()) == ["ModelA"], stages["model"].unique()
    deep = stages[(stages["iteration"] == 2) & (stages["stage"] == "test-generation")]
    assert len(deep) == 1 and int(deep["tg_test_methods"].iloc[0]) == 5
    assert any("non-canonical depth" in w for w in tdd.load_warnings)
    # the stray folders are not executions and raise no 'unparseable timestamp' warning
    assert not any("unparseable" in w for w in tdd.load_warnings), tdd.load_warnings
    cells = tdd.discover_cells(story)
    assert set(cells["iteration"]) == {1, 2}
    viol = tdd.load_remaining_violations(story)
    assert list(viol["model"].unique()) == ["ModelA"] and len(viol) == 1
    # 'latest' is skipped: exactly one metrics snapshot for iteration 1
    mt = tdd.load_metrics_types(story)
    assert mt.empty or mt[mt["iteration"] == 1]["ts"].nunique() <= 1
