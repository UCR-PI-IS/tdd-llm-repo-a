"""Read-only git facts about the wave-1 run branches.

Every function here shells out to ``git`` in read-only mode (``for-each-ref``,
``log``, ``ls-tree``, ``show``, ``diff``, ``grep``, ``rev-parse``,
``merge-base``). Nothing fetches, checks out or writes to the repository.
Per-branch results are cached in ``Visualizations/_cache/gitfacts/<tip>.json``
because a branch tip determines everything derived from it.

Key rules (see the plan, section 1.4):

* The per-run baseline is the parent of commit A (``chore(run): ...``), found
  by walking back over the ``(run)`` commits. ``git merge-base`` against the
  trunk is wrong for the seven merged runs that were fast-forwarded.
* Story-new tests are counted from the diff between that baseline and the
  branch tip, as test *methods* (declarations carrying a test attribute) and as
  test *cases* (attribute count, which equals the TRX count after ``[TestCase]``
  expansion). The agent's ``testMethodsEmitted`` counts methods.
* The harness fingerprint hashes the agent prompt files with their ``model:``
  lines removed, so that the stratum reflects the prompts and not the model
  configured for the run.
"""
from __future__ import annotations

import hashlib
import json
import re
import subprocess
import sys
from dataclasses import asdict, dataclass
from pathlib import Path

from .config import (E2E_HARNESS_COMMIT, INCLUDED_ITERATIONS, LAYERS, MODEL_LABELS, REPO,
                     STORY_CHAIN, VIZ)

if str(VIZ) not in sys.path:
    sys.path.insert(0, str(VIZ))
import tdd_results as tdd  # noqa: E402

CACHE_DIR = VIZ / "_cache" / "gitfacts"
RUN_REF_RE = re.compile(r"^(?:origin/)?runs/wave-1/([^/]+)/([^/]+)/(\d+)$")
RUN_SUBJECT_RE = re.compile(r"^(feat|chore)\(run\)")
AGENT_FILES = ("orchestrator.md", "test-generator.md", "code-generator.md",
               "refactor-generator.md", "intent-generator.md", "data-injection.md")
IDENTITY_AGENT_FILES = ("orchestrator.md", "test-generator.md", "code-generator.md", "refactor-generator.md")
PROD_PROJECTS = ("Backend.Domain", "Backend.Application", "Backend.Infrastructure",
                 "Backend.Presentation", "Backend.DependencyInjection", "Backend.Api")
TEST_PROJECTS = tuple(f"Backend.{layer}.Tests.Unit" for layer in LAYERS)
LAYER_RE = re.compile(r"Backend\.(Domain|Application|Infrastructure|Presentation)\.Tests\.Unit")

# NUnit markers that make a method a test. ``TestCase`` is only matched when the
# next character closes the attribute or opens its argument list, so
# ``TestCaseSource`` is counted once, as itself.
_MARKER_RE = re.compile(r"(?<![\w.])(Test|TestCase|TestCaseSource|Theory)(?=\s*(?:\(|\]|,))")
_ATTR_LINE_RE = re.compile(r"^\s*\[")
_METHOD_RE = re.compile(
    r"^\s*(?:\[[^\]]*\]\s*)*(?:public|internal|protected|private)?\s*"
    r"(?:static\s+|async\s+|virtual\s+|override\s+|new\s+)*"
    r"(?:void|Task(?:<[^>]+>)?|ValueTask(?:<[^>]+>)?)\s+(\w+)\s*(?:<[^>]+>)?\s*\(")
_CLASS_RE = re.compile(r"\b(?:class|record)\s+(\w+)")
_ASSERT_RE = re.compile(r"\bAssert\b|\bAssume\b|\.Verify\(|\.VerifyNoOtherCalls\(|\bThrows|\bDoesNotThrow|"
                        r"\bCollectionAssert\b|\bStringAssert\b|\bFileAssert\b|\.Should\(")


class GitError(RuntimeError):
    pass


def git(*args: str, check: bool = True) -> str:
    proc = subprocess.run(["git", "-C", str(REPO), *args], capture_output=True, text=True)
    if check and proc.returncode != 0:
        raise GitError(f"git {' '.join(args)} failed: {proc.stderr.strip()}")
    return proc.stdout


def git_ok(*args: str) -> bool:
    return subprocess.run(["git", "-C", str(REPO), *args], capture_output=True).returncode == 0


def head_sha(ref: str) -> str:
    return git("rev-parse", ref).strip()


# ---------------------------------------------------------------------------
# Run refs and cell resolution
# ---------------------------------------------------------------------------


@dataclass(frozen=True)
class RunRef:
    ref: str
    story: str
    model_dir: str
    model_key: str
    iteration: int
    sha: str
    remote: bool


def list_run_refs() -> list[RunRef]:
    """Every local and origin run branch, parsed; stale refs included."""
    out = git("for-each-ref", "--format=%(refname:short) %(objectname)",
              "refs/heads/runs/wave-1", "refs/remotes/origin/runs/wave-1")
    refs = []
    for line in out.splitlines():
        name, sha = line.split()
        m = RUN_REF_RE.match(name)
        if not m:
            continue
        story, model_dir, it = m.groups()
        refs.append(RunRef(name, story, model_dir, tdd.canonical_model(model_dir), int(it), sha,
                           name.startswith("origin/")))
    return refs


def working_tree_timestamps(story: str, model_key: str, iteration: int,
                            tree_name: str = "BuildResults") -> set[str]:
    """Timestamp folder names the working tree holds for a cell (any model spelling)."""
    out: set[str] = set()
    story_dir = REPO / tree_name / story
    if not story_dir.is_dir():
        return out
    for model_dir in story_dir.iterdir():
        if not model_dir.is_dir() or tdd.canonical_model(model_dir.name) != model_key:
            continue
        iter_dir = model_dir / str(iteration)
        if iter_dir.is_dir():
            out.update(p.name for p in iter_dir.iterdir() if p.is_dir() and tdd.parse_ts(p.name))
    return out


def _committed_timestamps(sha: str, story: str, model_key: str, iteration: int,
                          tree_name: str = "BuildResults") -> set[str]:
    out = git("ls-tree", "-r", "--name-only", sha, "--", f"{tree_name}/{story}", check=False)
    ts: set[str] = set()
    for line in out.splitlines():
        parts = line.split("/")
        if len(parts) >= 5 and tdd.canonical_model(parts[2]) == model_key and parts[3] == str(iteration):
            ts.add(parts[4])
    return ts


def resolve_cell_ref(story: str, model_key: str, iteration: int,
                     refs: list[RunRef] | None = None) -> dict:
    """Pick the run branch whose committed BuildResults match the working tree.

    Candidates are every ref with the same canonical cell id (local and origin).
    The one with the largest timestamp overlap wins; ties prefer local refs,
    then the lexicographically smaller name. ``partial`` flags an overlap that
    does not cover every working-tree execution.
    """
    refs = list_run_refs() if refs is None else refs
    wt = working_tree_timestamps(story, model_key, iteration)
    cands = [r for r in refs if r.story == story and r.model_key == model_key and r.iteration == iteration]
    scored = []
    for r in cands:
        overlap = len(wt & _committed_timestamps(r.sha, story, model_key, iteration))
        scored.append((overlap, not r.remote, r.ref, r))
    scored.sort(key=lambda t: (-t[0], -int(t[1]), t[2]))
    best = scored[0][3] if scored else None
    return {
        "ref": best.ref if best else None,
        "sha": best.sha if best else None,
        "overlap": scored[0][0] if scored else 0,
        "n_wt": len(wt),
        "n_candidates": len(cands),
        "partial": bool(best) and scored[0][0] < len(wt),
        "ambiguous": len(scored) > 1 and scored[0][0] == scored[1][0] and scored[0][0] > 0
        and scored[0][3].sha != scored[1][3].sha,
        "candidates": [{"ref": s[3].ref, "sha": s[3].sha, "overlap": s[0]} for s in scored],
    }


# ---------------------------------------------------------------------------
# Commits and baseline
# ---------------------------------------------------------------------------


def run_commits(sha: str, max_walk: int = 12) -> dict:
    """Walk back over ``(run)`` commits: tip = commit B (code), then commit A
    (results); the first non-run commit is the per-run baseline."""
    out = git("log", f"--format=%H%x1f%s%x1f%cI", f"-n{max_walk}", sha)
    commits = [line.split("\x1f") for line in out.splitlines() if line]
    run = []
    baseline = None
    for h, subject, when in commits:
        if RUN_SUBJECT_RE.match(subject):
            run.append({"sha": h, "subject": subject, "time": when})
        else:
            baseline = {"sha": h, "subject": subject, "time": when}
            break
    commit_b = next((c for c in run if c["subject"].startswith("feat(run)")), None)
    commit_a = next((c for c in run if c["subject"].startswith("chore(run)")), None)
    return {
        "tip_sha": sha,
        "n_run_commits": len(run),
        "run_commit_subjects": [c["subject"] for c in run],
        "commit_b_sha": commit_b["sha"] if commit_b else None,
        "commit_b_time": commit_b["time"] if commit_b else None,
        "commit_a_sha": commit_a["sha"] if commit_a else None,
        "commit_a_time": commit_a["time"] if commit_a else None,
        "baseline_sha": baseline["sha"] if baseline else None,
        "baseline_time": baseline["time"] if baseline else None,
        "baseline_subject": baseline["subject"] if baseline else None,
    }


# ---------------------------------------------------------------------------
# Source counting
# ---------------------------------------------------------------------------


def show(sha: str, path: str) -> str | None:
    proc = subprocess.run(["git", "-C", str(REPO), "show", f"{sha}:{path}"],
                          capture_output=True, text=True, errors="replace")
    return proc.stdout if proc.returncode == 0 else None


def count_test_markers(src: str) -> dict:
    """Count NUnit test methods and cases in one C# source text.

    * ``cases``   = number of test-marker attributes (``[Test]``, ``[TestCase(..)]``,
                    ``[TestCaseSource(..)]``, ``[Theory]``), i.e. executed cases
                    before data-source expansion.
    * ``methods`` = method declarations preceded by at least one such attribute.
    * ``method_names`` = those method names (duplicates kept in order).
    * ``assertion_free`` = counted methods whose body carries no assertion token.
    """
    lines = src.splitlines()
    cases = 0
    methods: list[str] = []
    assertion_free = 0
    pending = 0
    i = 0
    while i < len(lines):
        line = lines[i]
        stripped = line.strip()
        if _ATTR_LINE_RE.match(line):
            # An attribute may span several lines, e.g.
            # ``[TestCase(1, 2,\n    Description = "...")]``: consume until the
            # brackets balance, then count markers over the whole attribute text.
            attr_text = line
            depth = line.count("[") - line.count("]")
            j = i
            while depth > 0 and j + 1 < len(lines):
                j += 1
                attr_text += "\n" + lines[j]
                depth += lines[j].count("[") - lines[j].count("]")
            n = len(_MARKER_RE.findall(attr_text))
            cases += n
            pending += n
            tail = lines[j].split("]")[-1] if "]" in lines[j] else ""
            m = _METHOD_RE.match(tail)  # attribute and declaration on one line
            if m and pending:
                methods.append(m.group(1))
                assertion_free += int(not _has_assertion(lines, j))
                pending = 0
            i = j + 1
            continue
        if not stripped or stripped.startswith("//") or stripped.startswith("///") or stripped.startswith("#"):
            i += 1
            continue
        m = _METHOD_RE.match(line)
        if m and pending:
            methods.append(m.group(1))
            assertion_free += int(not _has_assertion(lines, i))
            pending = 0
        elif not m:
            pending = 0
        i += 1
    classes = _CLASS_RE.findall(src)
    return {"cases": cases, "methods": len(methods), "method_names": methods,
            "classes": classes, "assertion_free": assertion_free}


def _has_assertion(lines: list[str], start: int) -> bool:
    """Scan the method body starting at its declaration line (brace matching)."""
    depth = 0
    opened = False
    for j in range(start, min(len(lines), start + 400)):
        text = lines[j]
        if _ASSERT_RE.search(text):
            return True
        depth += text.count("{") - text.count("}")
        if "{" in text:
            opened = True
        if opened and depth <= 0:
            if "=>" in lines[start] and not opened:
                return bool(_ASSERT_RE.search(lines[start]))
            return False
    return False


def diff_name_status(base: str, tip: str, *pathspecs: str) -> list[tuple[str, str]]:
    out = git("diff", "--name-status", "-M", base, tip, "--", *pathspecs)
    rows = []
    for line in out.splitlines():
        parts = line.split("\t")
        status = parts[0][0]
        path = parts[-1]
        rows.append((status, path))
    return rows


def test_method_delta(base: str, tip: str) -> dict:
    """Story-new test methods and cases (tip minus baseline) over the test projects."""
    files = [(s, p) for s, p in diff_name_status(base, tip, *TEST_PROJECTS) if p.endswith(".cs")]
    per_layer_m = {layer: 0 for layer in LAYERS}
    per_layer_c = {layer: 0 for layer in LAYERS}
    new_methods = cases = deleted = assertion_free = 0
    new_files = modified = 0
    classes: list[str] = []
    new_names: list[str] = []
    touched_files: list[str] = []
    per_file_methods: dict[str, int] = {}
    for status, path in files:
        m = LAYER_RE.search(path)
        layer = m.group(1) if m else None
        tip_src = None if status == "D" else show(tip, path)
        base_src = None if status == "A" else show(base, path)
        t = count_test_markers(tip_src) if tip_src is not None else {"cases": 0, "methods": 0, "method_names": [], "classes": [], "assertion_free": 0}
        b = count_test_markers(base_src) if base_src is not None else {"cases": 0, "methods": 0, "method_names": [], "classes": [], "assertion_free": 0}
        dm, dc = t["methods"] - b["methods"], t["cases"] - b["cases"]
        per_file_methods[path] = dm
        if status == "A":
            new_files += 1
        elif status == "D":
            deleted += b["methods"]
        else:
            modified += 1
        if dm > 0:
            new_methods += dm
            assertion_free += max(0, t["assertion_free"] - b["assertion_free"])
            if layer:
                per_layer_m[layer] += dm
        if dc > 0:
            cases += dc
            if layer:
                per_layer_c[layer] += dc
        if tip_src is not None:
            touched_files.append(path)
            classes.extend(c for c in t["classes"] if c not in classes)
            base_names = set(b["method_names"])
            new_names.extend(n for n in t["method_names"] if n not in base_names)
    return {
        "new_test_methods": new_methods, "new_test_cases": cases,
        **{f"new_test_methods_{layer}": per_layer_m[layer] for layer in LAYERS},
        **{f"new_test_cases_{layer}": per_layer_c[layer] for layer in LAYERS},
        "new_test_files": new_files, "modified_test_files": modified,
        "deleted_test_methods": deleted, "assertion_free_new_tests": assertion_free,
        "new_test_classes": classes, "new_test_method_names": new_names,
        "touched_test_files": touched_files, "per_file_method_delta": per_file_methods,
    }


def compile_remove_added(base: str, tip: str) -> dict:
    out = git("diff", "-U0", base, tip, "--", "*.csproj", check=False)
    files = re.findall(r'^\+.*<Compile\s+Remove="([^"]+)"', out, flags=re.MULTILINE)
    return {"compile_remove_added": len(files), "compile_remove_files": files}


def loc_churn(base: str, tip: str) -> dict:
    def numstat(*pathspecs: str) -> tuple[int, int]:
        out = git("diff", "--numstat", base, tip, "--", *pathspecs, check=False)
        added = deleted = 0
        for line in out.splitlines():
            a, d, path = line.split("\t", 2)
            if not path.endswith(".cs") or a == "-":
                continue
            added += int(a)
            deleted += int(d)
        return added, deleted
    prod_ps = [*PROD_PROJECTS, ":(exclude)*Tests.Unit*"]
    pa, pd_ = numstat(*prod_ps)
    ta, td = numstat(*TEST_PROJECTS)
    prod_files = [(s, p) for s, p in diff_name_status(base, tip, *prod_ps) if p.endswith(".cs")]
    return {
        "prod_loc_added": pa, "prod_loc_deleted": pd_,
        "prod_files_added": sum(1 for s, _ in prod_files if s == "A"),
        "prod_files_modified": sum(1 for s, _ in prod_files if s != "A"),
        "prod_files": [p for _, p in prod_files],
        "test_loc_added": ta, "test_loc_deleted": td,
    }


# ---------------------------------------------------------------------------
# Identity and harness
# ---------------------------------------------------------------------------


def _model_key_from_id(model_id: str | None) -> str | None:
    if not model_id:
        return None
    low = model_id.lower()
    for needle, key in (("kimi", "kimik25"), ("qwen", "qwen37max"), ("deepseek", "deepseekv4pro"),
                        ("grok-4.3", "grok43"), ("grok-4.5", "grok45"), ("grok", "grok")):
        if needle in low:
            return key
    return tdd.canonical_model(model_id)


def declared_models(sha: str) -> dict:
    """``model:`` line of each agent file on the branch tip."""
    per_file: dict[str, str | None] = {}
    for name in IDENTITY_AGENT_FILES:
        src = show(sha, f".opencode/agents/{name}") or ""
        m = re.search(r"^model:\s*(.+?)\s*$", src, flags=re.MULTILINE)
        per_file[name] = m.group(1) if m else None
    keys = {_model_key_from_id(v) for v in per_file.values() if v}
    declared = next(iter(keys)) if len(keys) == 1 else (sorted(k for k in keys if k)[0] if keys else None)
    return {"identity_per_file": per_file, "identity_declared_model": declared,
            "identity_agents_agree": len(keys) <= 1}


def harness_fingerprint(sha: str) -> dict:
    """Fingerprint of the harness at a commit: agent prompts with ``model:`` lines
    removed plus the Automations tree hash."""
    h = hashlib.sha1()
    for name in AGENT_FILES:
        src = show(sha, f".opencode/agents/{name}") or ""
        src = re.sub(r"^model:.*$", "model: <stripped>", src, flags=re.MULTILINE)
        h.update(name.encode())
        h.update(src.encode("utf-8", "replace"))
    agents_tree = git("rev-parse", f"{sha}:.opencode/agents", check=False).strip() or None
    autom_tree = git("rev-parse", f"{sha}:Automations", check=False).strip() or None
    h.update((autom_tree or "").encode())
    return {"harness_fingerprint": h.hexdigest()[:12], "harness_agents_tree": agents_tree,
            "harness_automations_tree": autom_tree}


def is_pre_e2e(sha: str) -> bool:
    return not git_ok("merge-base", "--is-ancestor", E2E_HARNESS_COMMIT, sha)


def inherited_size(sha: str) -> dict:
    """Size of the code base at a commit: test attributes, test files, production files and lines."""
    attrs = 0
    test_files = 0
    out = git("grep", "-c", "-E", r"^[[:space:]]*\[(Test|TestCase|TestCaseSource|Theory)([^A-Za-z]|$)",
              sha, "--", *TEST_PROJECTS, check=False)
    for line in out.splitlines():
        _, path, n = line.rsplit(":", 2)
        if path.endswith(".cs"):
            attrs += int(n)
    test_files = sum(1 for p in git("ls-tree", "-r", "--name-only", sha, "--", *TEST_PROJECTS, check=False).splitlines()
                     if p.endswith(".cs"))
    prod_lines = prod_files = 0
    out = git("grep", "-c", "", sha, "--", *PROD_PROJECTS, check=False)
    for line in out.splitlines():
        _, path, n = line.rsplit(":", 2)
        if path.endswith(".cs") and "Tests.Unit" not in path and "/obj/" not in path and "/bin/" not in path:
            prod_lines += int(n)
            prod_files += 1
    return {"test_attrs": attrs, "test_files": test_files, "prod_files": prod_files, "prod_lines": prod_lines}


# ---------------------------------------------------------------------------
# Per-cell facts with cache
# ---------------------------------------------------------------------------


def _cache_path(sha: str) -> Path:
    return CACHE_DIR / f"{sha}.json"


def cell_facts(story: str, model_key: str, iteration: int, *, refs: list[RunRef] | None = None,
               refresh: bool = False) -> dict:
    """All git-derived facts for one cell (cached by branch tip)."""
    res = resolve_cell_ref(story, model_key, iteration, refs)
    facts: dict = {"story": story, "model_key": model_key, "iteration": iteration,
                   "ref": res["ref"], "tip_sha": res["sha"], "ref_overlap": res["overlap"],
                   "ref_n_wt": res["n_wt"], "ref_n_candidates": res["n_candidates"],
                   "ref_partial": res["partial"], "ref_ambiguous": res["ambiguous"],
                   "ref_candidates": res["candidates"]}
    if res["sha"] is None:
        return facts
    cp = _cache_path(res["sha"])
    if cp.exists() and not refresh:
        cached = json.loads(cp.read_text())
        cached.update({k: facts[k] for k in ("ref", "ref_overlap", "ref_n_wt", "ref_n_candidates",
                                                "ref_partial", "ref_ambiguous", "ref_candidates")})
        return cached
    facts.update(run_commits(res["sha"]))
    base = facts["baseline_sha"]
    facts.update(declared_models(res["sha"]))
    facts["identity_ambiguous"] = bool(facts["identity_declared_model"]) and facts["identity_declared_model"] != model_key
    facts["harness_pre_e2e"] = is_pre_e2e(res["sha"])
    if base:
        facts.update(harness_fingerprint(base))
        facts.update(test_method_delta(base, res["sha"]))
        facts.update(compile_remove_added(base, res["sha"]))
        facts.update(loc_churn(base, res["sha"]))
    CACHE_DIR.mkdir(parents=True, exist_ok=True)
    cp.write_text(json.dumps(facts, indent=1, sort_keys=True))
    return facts


def all_cell_facts(cells, *, refresh: bool = False, progress=None) -> dict[str, dict]:
    """``cells`` = iterable of (story, model_key, iteration). Returns run_id -> facts."""
    refs = list_run_refs()
    out: dict[str, dict] = {}
    for n, (story, model_key, iteration) in enumerate(cells, 1):
        label = MODEL_LABELS.get(model_key, model_key)
        run_id = f"{story}/{label}/{iteration}"
        out[run_id] = cell_facts(story, model_key, iteration, refs=refs, refresh=refresh)
        if progress:
            progress(n, run_id)
    return out


def story_chain_facts() -> list[dict]:
    """Baseline commit facts for every story in the chain."""
    rows = []
    for link in STORY_CHAIN:
        sha = head_sha(link.baseline_sha)
        when = git("log", "-1", "--format=%cI", sha).strip()
        row = {**asdict(link), "baseline_full_sha": sha, "baseline_date": when,
               **{f"baseline_{k}": v for k, v in inherited_size(sha).items()},
               **harness_fingerprint(sha)}
        rows.append(row)
    return rows


def excluded_refs(cells_present: set[tuple[str, str, int]], stories: set[str] | None = None,
                  used_refs: set[str] | None = None) -> list[dict]:
    """Run branches outside the study with a reason each (restricted to ``stories`` when given).

    A remote twin of a used local ref (same name under ``origin/``, same commit) is
    not listed; any other unused branch of a present cell is an ``unused-duplicate-ref``
    (for example the pre-e2e Kimi batch of story 1 or a stale pointer).
    """
    refs = list_run_refs()
    sha_by_ref = {r.ref: r.sha for r in refs}
    rows = []
    for r in refs:
        if stories is not None and r.story not in stories:
            continue
        reason = None
        if r.model_key not in MODEL_LABELS:
            reason = "model-not-in-study"
        elif r.iteration not in INCLUDED_ITERATIONS:
            reason = "iteration-out-of-range"
        elif (r.story, r.model_key, r.iteration) not in cells_present:
            reason = "no-evidence-in-working-tree"
        elif used_refs and r.ref not in used_refs:
            local_twin = r.ref[len("origin/"):] if r.remote else None
            if local_twin and local_twin in used_refs and sha_by_ref.get(local_twin) == r.sha:
                continue
            reason = "unused-duplicate-ref"
        if reason:
            rc = run_commits(r.sha, max_walk=4)
            rows.append({"ref": r.ref, "story": r.story, "model_dir": r.model_dir, "iteration": r.iteration,
                         "sha": r.sha, "reason": reason, "n_run_commits": rc["n_run_commits"],
                         "pre_e2e": is_pre_e2e(r.sha)})
    return rows
