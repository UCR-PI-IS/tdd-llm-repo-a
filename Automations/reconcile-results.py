#!/usr/bin/env python3
"""Reconcile missing/corrupt result files from raw data already in the repo.

Fills the gaps the Visualizations data layer (Visualizations/tdd_results.py)
needs, using ONLY existing raw sources — the files themselves and the per-run
git branches (runs/<wave>/<STORY>/<MODEL>/<ITERATION>). It never runs builds,
tests or Docker; it only reads and writes files.

Defect classes repaired:
  1. Stage/summary JSONs that are a valid JSON object followed by trailing
     garbage (leaked LLM tool-call artifacts) -> rewritten as the parsed object.
  2. Missing <cell>/test-generator/pipeline-stage-result.json whose content
     exists on the cell's run branch at the story-level stray path
     TestResults/<STORY>/test-generator/pipeline-stage-result.json
     -> recovered, model/iteration normalized from the branch name (only when
     the file says "unknown"), written to the canonical path.

Anything else (e.g. a cell whose final execution has no coverage) is REPORTED
but never fixed here.

Usage: ./Automations/reconcile-results.py [--apply] [--story S] [--model M]
                                          [--branch-glob GLOB] [--report PATH]
Dry-run by default; --apply writes. Idempotent: repaired/recovered files parse
cleanly on the next run and are skipped.
"""

from __future__ import annotations

import argparse
import json
import subprocess
import sys
from datetime import date
from pathlib import Path

WORKSPACE = Path(__file__).resolve().parent.parent
TREES = ("BuildResults", "TestResults", "MetricsResults")
STAGE_FILE = "pipeline-stage-result.json"
TOOL_NAME = "Automations/reconcile-results.py"

# Analytical inputs tdd_results.py reads; used for the audit matrix.
KEY_FILES = ("build-summary.json", "test-summary.json", "metrics-summary.json",
             STAGE_FILE)


def git(*args: str) -> str:
    res = subprocess.run(["git", *args], cwd=WORKSPACE, capture_output=True,
                         text=True)
    if res.returncode != 0:
        raise RuntimeError(f"git {' '.join(args)} failed: {res.stderr.strip()}")
    return res.stdout


def git_blob(ref: str, path: str) -> str | None:
    res = subprocess.run(["git", "cat-file", "-p", f"{ref}:{path}"],
                         cwd=WORKSPACE, capture_output=True, text=True)
    return res.stdout if res.returncode == 0 else None


# ---------------------------------------------------------------------------
# Discovery
# ---------------------------------------------------------------------------


def discover_cells(story_filter: str | None, model_filter: str | None):
    """(story, model, iteration) triples present in any result tree."""
    cells = set()
    for tree in TREES:
        base = WORKSPACE / tree
        if not base.is_dir():
            continue
        for story_dir in base.iterdir():
            if not story_dir.is_dir() or (story_filter and story_dir.name != story_filter):
                continue
            for model_dir in story_dir.iterdir():
                if not model_dir.is_dir() or model_dir.name == "test-generator":
                    continue
                if model_filter and model_dir.name != model_filter:
                    continue
                for iter_dir in model_dir.iterdir():
                    if iter_dir.is_dir() and iter_dir.name.isdigit():
                        cells.add((story_dir.name, model_dir.name, iter_dir.name))
    return sorted(cells)


def discover_run_branches(glob: str):
    """Map (story, model, iteration) -> branch, from runs/* branch names.

    Filtering happens here with fnmatch (where * crosses "/") rather than in
    git's for-each-ref pattern, whose wildmatch stops * at path separators.
    """
    from fnmatch import fnmatchcase
    out = git("for-each-ref", "--format=%(refname:short)", "refs/heads")
    mapping: dict[tuple[str, str, str], list[str]] = {}
    for branch in out.split():
        if not fnmatchcase(branch, glob):
            continue
        parts = branch.split("/")
        if len(parts) < 4:
            continue
        cell = tuple(parts[-3:])  # <STORY>/<MODEL>/<ITERATION>
        mapping.setdefault(cell, []).append(branch)
    return mapping


# ---------------------------------------------------------------------------
# Repairs
# ---------------------------------------------------------------------------


def parse_lenient(text: str):
    """(object, trailing) for a valid-prefix JSON; raises on hopeless input."""
    obj, end = json.JSONDecoder().raw_decode(text)
    return obj, text[end:].strip()


def provenance(actions: list[str], **extra) -> dict:
    return {"actions": actions, "date": date.today().isoformat(),
            "tool": TOOL_NAME, **extra}


def dump(obj: dict) -> str:
    return json.dumps(obj, indent=2, ensure_ascii=False) + "\n"


def repair_corrupt_jsons(stories: set[str], apply: bool, report: list[str]) -> int:
    """Defect class 1: valid JSON + trailing garbage, rewritten in place."""
    fixed = 0
    for tree in TREES:
        base = WORKSPACE / tree
        if not base.is_dir():
            continue
        for path in sorted(base.rglob("*.json")):
            rel = path.relative_to(WORKSPACE)
            if rel.parts[1] not in stories or ".tools" in rel.parts:
                continue
            text = path.read_text(encoding="utf-8")
            try:
                json.loads(text)
                continue  # healthy
            except json.JSONDecodeError:
                pass
            try:
                obj, trailing = parse_lenient(text)
            except json.JSONDecodeError as exc:
                report.append(f"UNRECOVERABLE json (left untouched): {rel} ({exc})")
                continue
            if not isinstance(obj, dict):
                report.append(f"UNRECOVERABLE json (non-object prefix): {rel}")
                continue
            obj["reconciliation"] = provenance(
                ["trailing-artifacts-stripped"],
                trailingGarbage=trailing[:60])
            if apply:
                path.write_text(dump(obj), encoding="utf-8")
            report.append(f"REPAIRED {rel} (stripped {len(trailing)} trailing chars: {trailing[:30]!r})")
            fixed += 1
    return fixed


def recover_stray_tg(cells, branches, apply: bool, report: list[str]) -> int:
    """Defect class 2: recover story-level stray tg results from run branches."""
    recovered = 0
    for story, model, iteration in cells:
        canonical = (WORKSPACE / "TestResults" / story / model / iteration /
                     "test-generator" / STAGE_FILE)
        rel = canonical.relative_to(WORKSPACE)
        cell_branches = branches.get((story, model, iteration), [])
        if canonical.exists():
            for br in cell_branches:  # surplus strays are reported, never used
                if git_blob(br, f"TestResults/{story}/test-generator/{STAGE_FILE}") is not None:
                    report.append(f"IGNORED stray tg on {br} ({rel} already present)")
            continue
        if not cell_branches:
            report.append(f"NO SOURCE for missing {rel}: no run branch matches the cell")
            continue
        if len(cell_branches) > 1:
            report.append(f"AMBIGUOUS branches for {story}/{model}/{iteration}: "
                          f"{cell_branches} — narrow with --branch-glob; skipped")
            continue
        branch = cell_branches[0]
        stray_path = f"TestResults/{story}/test-generator/{STAGE_FILE}"
        text = git_blob(branch, stray_path)
        source = stray_path
        if text is None:  # fall back: canonical path existed on branch but not in tree
            text = git_blob(branch, str(rel))
            source = str(rel)
        if text is None:
            report.append(f"NO SOURCE for missing {rel}: neither stray nor canonical on {branch}")
            continue
        try:
            obj, trailing = parse_lenient(text)
        except json.JSONDecodeError as exc:
            report.append(f"UNRECOVERABLE source for {rel} on {branch} ({exc})")
            continue
        actions = ["recovered-from-run-branch"]
        if trailing:
            actions.append("trailing-artifacts-stripped")
        if obj.get("stage") != "test-generation":
            report.append(f"CONFLICT for {rel}: source stage is {obj.get('stage')!r}; skipped")
            continue
        for field, value in (("model", model), ("iteration", iteration)):
            current = obj.get(field)
            if current in (None, "unknown"):
                if current == "unknown":
                    actions.append(f"{field}-normalized")
                obj[field] = value
            elif str(current) != value:
                report.append(f"CONFLICT for {rel}: source says {field}={current!r}, "
                              f"branch says {value!r}; skipped")
                break
        else:
            blob_sha = git("rev-parse", f"{branch}:{source}").strip()
            obj["reconciliation"] = provenance(actions,
                                               recoveredFrom=f"{branch}:{source}",
                                               blob=blob_sha)
            if apply:
                canonical.parent.mkdir(parents=True, exist_ok=True)
                canonical.write_text(dump(obj), encoding="utf-8")
            report.append(f"RECOVERED {rel} from {branch}:{source} ({', '.join(actions)})")
            recovered += 1
    return recovered


# ---------------------------------------------------------------------------
# Audit matrix (report-only)
# ---------------------------------------------------------------------------


def audit(cells, report: list[str]) -> None:
    report.append("")
    report.append("| cell | build-sum | test-sum | trx | combined-cov | tg | cg | ref |")
    report.append("|---|---|---|---|---|---|---|---|")
    for story, model, iteration in cells:
        def cell_dir(tree):
            return WORKSPACE / tree / story / model / iteration

        def has(tree, pattern):
            base = cell_dir(tree)
            return base.is_dir() and any(base.rglob(pattern))

        def stage_ok(tree, want, exclude_tg=False):
            base = cell_dir(tree)
            if not base.is_dir():
                return False
            for p in base.rglob(STAGE_FILE):
                if exclude_tg and "test-generator" in p.parts:
                    continue
                try:
                    if json.loads(p.read_text(encoding="utf-8")).get("stage") == want:
                        return True
                except json.JSONDecodeError:
                    continue
            return False

        tick = lambda b: "Y" if b else "**–**"
        cov = has("TestResults", "Coverage/Combined/Cobertura.xml")
        report.append(
            f"| {story}/{model}/{iteration} "
            f"| {tick(has('BuildResults', 'build-summary.json'))} "
            f"| {tick(has('TestResults', 'test-summary.json'))} "
            f"| {tick(has('TestResults', '*.trx'))} "
            f"| {tick(cov)} "
            f"| {tick(stage_ok('TestResults', 'test-generation'))} "
            f"| {tick(stage_ok('TestResults', 'code-generation', exclude_tg=True))} "
            f"| {tick(stage_ok('MetricsResults', 'refactoring'))} |")


def main() -> int:
    ap = argparse.ArgumentParser(description=__doc__,
                                 formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("--apply", action="store_true", help="write changes (default: dry-run)")
    ap.add_argument("--story", help="limit to one story id")
    ap.add_argument("--model", help="limit to one model")
    ap.add_argument("--branch-glob", default="runs/*",
                    help="run-branch pattern (default: runs/*)")
    ap.add_argument("--report", default="Visualizations/_reconciliation-report.md",
                    help="report path relative to repo root")
    args = ap.parse_args()

    cells = discover_cells(args.story, args.model)
    if not cells:
        print("No cells discovered — nothing to do.")
        return 0
    stories = {c[0] for c in cells}
    branches = discover_run_branches(args.branch_glob)

    mode = "APPLY" if args.apply else "DRY-RUN"
    report: list[str] = [f"# Reconciliation report — {mode} {date.today().isoformat()}",
                         "",
                         f"cells: {len(cells)}  stories: {sorted(stories)}  "
                         f"run branches matched: {sum(1 for c in cells if c in branches)}",
                         ""]

    fixed = repair_corrupt_jsons(stories, args.apply, report)
    recovered = recover_stray_tg(cells, branches, args.apply, report)

    report.append("")
    report.append(f"**{mode}: {fixed} corrupt JSON(s) repaired, "
                  f"{recovered} test-generation result(s) recovered.**")
    report.append("")
    report.append("## Post-run audit (working tree as of this run)")
    audit(cells, report)
    report.append("")
    report.append("Coverage gaps (combined-cov column) are informational only: "
                  "regenerating coverage requires re-running tests, which is "
                  "out of scope for this script by design.")

    text = "\n".join(report) + "\n"
    print(text)
    if args.apply:
        report_path = WORKSPACE / args.report
        report_path.parent.mkdir(parents=True, exist_ok=True)
        report_path.write_text(text, encoding="utf-8")
        print(f"report written to {report_path.relative_to(WORKSPACE)}")
    return 0


if __name__ == "__main__":
    sys.exit(main())
