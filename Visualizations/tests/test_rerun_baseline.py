"""run_commits finds the true baseline of a cell re-run on its own branch (a superseded attempt reverted below the
live pair), and still stops at a trunk revert of the previous story. Uses throwaway repositories in tmp_path only."""
import subprocess

import pytest

from tdd_paper import gitfacts as G

A = "chore(run): build/test/e2e/metrics results for CPD-LC-001-009 wave-1 Qwen3.7-max iteration 1"
B = "feat(run): generated tests, implementation, and refactors for CPD-LC-001-009 wave-1 Qwen3.7-max iteration 1"


def _git(repo, *args):
    return subprocess.run(["git", "-C", str(repo), *args], capture_output=True, text=True, check=True).stdout.strip()


def _record(repo, subject, body=""):
    """Add one commit with this subject to the throwaway repository and return its sha."""
    (repo / "f.txt").write_text(subject + body)
    _git(repo, "add", "f.txt")
    message = subject + (f"\n\n{body}" if body else "")
    _git(repo, "-c", "user.email=t@example.org", "-c", "user.name=t", "commit", "-q", "-m", message)
    return _git(repo, "rev-parse", "HEAD")


@pytest.fixture
def repo(tmp_path, monkeypatch):
    _git(tmp_path, "init", "-q")
    monkeypatch.setattr(G, "REPO", tmp_path)
    return tmp_path


def test_rerun_on_its_own_branch_walks_past_the_superseded_attempt(repo):
    prev = _record(repo, "chore(run): build/test/e2e/metrics results for CPD-LC-001-003 wave-1 Qwen3.7-max iteration 4")
    base = _record(repo, f'Revert "chore(run): build/test/e2e/metrics results for CPD-LC-001-003 wave-1 Qwen3.7-max iteration 4"',
                   f"This reverts commit {prev}.")
    old_a = _record(repo, A)
    old_b = _record(repo, B)
    _record(repo, f'Revert "{B}"', f"This reverts commit {old_b}.")
    _record(repo, f'Revert "{A}"', f"This reverts commit {old_a}.")
    new_a = _record(repo, A)
    new_b = _record(repo, B.replace("Qwen3.7-max", "qwen3.7-max"))  # model label drift is tolerated
    rc = G.run_commits(new_b)
    assert rc["baseline_sha"] == base  # the trunk revert of the previous story ends the walk
    assert (rc["commit_a_sha"], rc["commit_b_sha"]) == (new_a, new_b)
    assert rc["n_run_commits"] == 2 and rc["n_reverted_run_commits"] == 2
    assert set(rc["superseded_run_shas"]) == {old_a, old_b}


def test_plain_run_is_unchanged_and_stops_at_a_trunk_revert(repo):
    prev = _record(repo, "chore(run): build/test/e2e/metrics results for CPD-LC-001-005 wave-1 Kimi-K2.5 iteration 6")
    base = _record(repo, 'Revert "chore(run): build/test/e2e/metrics results for CPD-LC-001-005 wave-1 Kimi-K2.5 iteration 6"',
                   f"This reverts commit {prev}.")
    a = _record(repo, A.replace("CPD-LC-001-009", "SPT-UM-001-003"))
    b = _record(repo, B.replace("CPD-LC-001-009", "SPT-UM-001-003"))
    rc = G.run_commits(b)
    assert rc["baseline_sha"] == base
    assert (rc["commit_a_sha"], rc["commit_b_sha"], rc["n_run_commits"]) == (a, b, 2)
    assert rc["n_reverted_run_commits"] == 0 and rc["superseded_run_shas"] == []


def test_revert_of_another_iteration_is_not_skipped(repo):
    _record(repo, "Remove results for the next iteration")
    other = A.replace("iteration 1", "iteration 2")
    other_a = _record(repo, other)
    rev = _record(repo, f'Revert "{other}"', f"This reverts commit {other_a}.")
    _record(repo, A)
    b = _record(repo, B)
    rc = G.run_commits(b)
    assert rc["baseline_sha"] == rev and rc["n_reverted_run_commits"] == 0
