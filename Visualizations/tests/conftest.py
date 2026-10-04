"""Shared fixtures. Repo-gated tests skip when the evidence trees are absent."""
from pathlib import Path

import pytest

VIZ = Path(__file__).resolve().parent.parent
REPO = VIZ.parent

requires_repo = pytest.mark.skipif(
    not (REPO / "BuildResults").is_dir() or not (REPO / ".git").exists(),
    reason="evidence trees / git history not available",
)


@pytest.fixture(scope="session")
def repo_root() -> Path:
    return REPO


@pytest.fixture(scope="session")
def viz_root() -> Path:
    return VIZ

import sys  # noqa: E402

if str(VIZ) not in sys.path:  # make `import tdd_results` and `import tdd_paper` work from any cwd
    sys.path.insert(0, str(VIZ))
