"""Per-story macros of the SANER paper (P44): off by default, grammar, values, collisions."""
import pandas as pd
import pytest

from tdd_paper import config as C
from tdd_paper import story_macros as SM
from tdd_paper.tables import Numbers


def _runs() -> pd.DataFrame:
    rows = []
    for story in C.STORY_ORDER[:2]:
        for model in C.MODEL_ORDER:
            for it in (1, 2, 3):
                row = {"story": story, "model": model, "iteration": it,
                       "compile_remove_added": 2 if it == 1 else 0, "exceptional_ending": "x" if it == 3 else None}
                for m in SM.MEDIAN_METRICS:
                    row[m] = float(it)
                for m in SM.FLAG_METRICS:
                    row[m] = it != 2
                rows.append(row)
    df = pd.DataFrame(rows)
    df.loc[0, "e2e_attempts_to_pass_noinfra"] = None
    for m in SM.FLAG_METRICS:
        df[m] = df[m].astype("boolean")
    return df


def test_off_by_default(monkeypatch):
    monkeypatch.delenv(SM.ENV, raising=False)
    nums = Numbers()
    nums.add("NCells", 120, fmt="int")
    assert SM.register_if_enabled(nums, _runs()) == 0
    assert list(nums.entries) == ["NCells"] and "story_macros" not in nums.meta


def test_names_values_and_idempotence(monkeypatch):
    monkeypatch.setenv(SM.ENV, "1")
    nums = Numbers()
    nums.add("StoryOneIntents", 5, fmt="int")
    n = SM.register_if_enabled(nums, _runs())
    per_group = 2 * len(SM.MEDIAN_METRICS) + 2 * len(SM.FLAG_METRICS) + 4
    assert n == 2 * 2 * per_group
    e = nums.entries
    assert e["StoryOneBuildFailedExecsKimiMedian"]["text"] == "2.0"           # Int64 metric: pooled format {:.1f}
    assert e["StoryTwoLineRateQwenMedian"]["text"] == "2.00"                   # float metric: {:.2f}
    assert e["StoryOneEndToEndAttemptsToPassNoinfraKimiN"]["value"] == 2      # one unknown value
    assert e["StoryOneEndToEndAttemptsToPassNoinfraKimiMedian"]["value"] == 2.5
    assert e["StoryOneRedFirstKimiK"]["value"] == 2 and e["StoryOneRedFirstKimiN"]["value"] == 3
    assert e["StoryOneCompileRemoveQwenK"]["value"] == 1 and e["StoryTwoExceptionalEndingKimiK"]["value"] == 1
    assert e["StoryOneRedFirstKimiK"]["kind"] == SM.KIND and e["StoryOneRedFirstKimiK"]["set"] == "S0"
    assert e["StoryOneRedFirstKimiK"]["provenance"] == "tool-measured"
    assert e["StoryOneIntents"]["value"] == 5 and "kind" not in e["StoryOneIntents"]
    assert SM.register_if_enabled(nums, _runs()) == n and len(nums.entries) == n + 1


def test_collision_raises(monkeypatch):
    monkeypatch.setenv(SM.ENV, "1")
    nums = Numbers()
    nums.add("StoryOneRedFirstKimiK", 99, fmt="int")
    with pytest.raises(ValueError):
        SM.register_story_macros(nums, _runs())
