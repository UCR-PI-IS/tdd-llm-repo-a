"""tdd_paper: pooled analysis, statistics, figures and tables for the wave-1 technical report.

Every number that reaches the report is produced here from tool-written evidence
(BuildResults/, TestResults/, MetricsResults/, E2EResults/) and from read-only git
facts about the run branches. Agent self-reports are loaded, tagged and never scored.

Typical use::

    from tdd_paper import build_runs, run_all, run_checks
    bundle = build_runs()                 # 120 runs, one row per cell
    result = run_all(bundle)              # contrasts, Holm, TOST, pass@k, rankings, sensitivity
    result.numbers.write("_build/paper")  # numbers.json + numbers.tex

or from the shell: ``.venv/bin/python -m tdd_paper all --out <dir>``.
"""
__version__ = "0.1.0"

from .analysis import run_all  # noqa: E402,F401
from .checks import run_checks  # noqa: E402,F401
from .composite import score_story  # noqa: E402,F401
from .dataset import build_runs, load_runs  # noqa: E402,F401
from .figures import make_figures  # noqa: E402,F401
from .paper_tables import make_tables  # noqa: E402,F401
