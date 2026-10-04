"""Command line interface: ``python -m tdd_paper <command> [options]``.

Commands: dataset | stats | figures | tables | numbers | all (alias build) | check | query.
"""
from __future__ import annotations

import argparse
import json
import sys
import time
from pathlib import Path

from . import __version__
from . import config as C

DEFAULT_OUT = C.VIZ / "_build" / "paper"


def _p(msg: str) -> None:
    print(msg, flush=True)


def _load_bundle(out_dir: Path) -> dict:
    from . import dataset as ds
    b = {"runs": ds.load_runs(out_dir)}
    for name in ("executions", "types", "cells", "stories", "excluded_refs"):
        try:
            b[name] = ds.load_table(out_dir, name)
        except FileNotFoundError:
            b[name] = None
    return b


def cmd_dataset(args) -> int:
    from . import dataset as ds
    t = time.time()
    bundle = ds.build_runs(stories=args.stories or None, with_git=not args.no_git, refresh_cache=args.refresh_cache,
                           progress=lambda s: _p(f"  built {s} ({time.time() - t:.0f}s)"))
    written = ds.write_dataset(bundle, args.out)
    _p(f"dataset: {written['runs']['rows']} runs -> {args.out}/data ({time.time() - t:.0f}s); {len(bundle['warnings'])} warnings")
    return 0


def _run_stats(args, bundle):
    from . import analysis as an
    meta = {"out_dir": str(args.out), "evidence_head": _git_head(), "harness_head": _git_head(C.TRUNK)}
    set_id = getattr(args, "set", None) or "S0"
    if set_id != "S0":
        subsets = an.filter_set(bundle["runs"], set_id)
        if len(subsets) != 1:
            raise SystemExit(f"--set {set_id} yields {len(subsets)} strata; run the full `stats` and read stats/sensitivity.csv")
        bundle = dict(bundle, runs=next(iter(subsets.values())))
        bundle["cells"] = __import__("tdd_paper.dataset", fromlist=["_cells"])._cells(bundle["runs"])
        meta["set"] = set_id
        _p(f"stats: restricted to set {set_id}: {len(bundle['runs'])} runs")
    t = time.time()
    res = an.run_all(bundle, n_boot=args.boot, n_perm=args.perm, seed=args.seed, with_stability=not args.no_stability,
                     meta=meta, progress=lambda s: _p(f"  stats: {s} ({time.time() - t:.0f}s)"))
    an.write_frames(res, args.out)
    res.numbers.write(args.out)
    _p(f"stats: {len(res.frames['effects'])} contrasts, {len(res.numbers.entries)} numbers ({time.time() - t:.0f}s)")
    return res


def cmd_stats(args) -> int:
    bundle = _load_bundle(args.out)
    _run_stats(args, bundle)
    _refresh_manifest(args.out)
    return 0


def _load_frames(out_dir: Path) -> dict:
    import pandas as pd
    frames = {}
    for p in sorted((Path(out_dir) / "stats").glob("*.csv")):
        try:
            frames[p.stem] = pd.read_csv(p)
        except pd.errors.EmptyDataError:
            frames[p.stem] = pd.DataFrame()
    return frames


def _run_figures(args, bundle, frames) -> dict:
    import hashlib
    from . import figures as F
    runs_csv = Path(args.out) / "data" / "runs.csv"
    sha = hashlib.sha256(runs_csv.read_bytes()).hexdigest() if runs_csv.exists() else None
    ctx = F.FigureContext(runs=bundle["runs"], executions=bundle["executions"], types=bundle["types"], cells=bundle["cells"],
                          stories=bundle["stories"], frames=frames, out_dir=Path(args.out), seed=args.seed, runs_sha256=sha)
    res = F.make_figures(ctx, only=args.only or None, progress=lambda slug, err: _p(f"  figure {slug}: {'ERROR' if err else 'ok'}"))
    errors = {k: v["error"] for k, v in res.items() if "error" in v}
    for k, e in errors.items():
        _p(f"  !! {k}: {e}")
    return res


def _refresh_manifest(out_dir: Path) -> None:
    """Keep manifest.json consistent after a partial re-render, so `check` can still
    tell hand edits from package output."""
    from . import checks as K
    mpath = Path(out_dir) / "manifest.json"
    if mpath.exists():
        prev = json.loads(mpath.read_text())
        extra = {k: v for k, v in prev.items() if k in ("seed", "n_boot", "n_perm", "figures_with_errors")}
        K.write_manifest(Path(out_dir), extra=extra)


def cmd_figures(args) -> int:
    bundle = _load_bundle(args.out)
    frames = _load_frames(args.out)
    res = _run_figures(args, bundle, frames)
    _refresh_manifest(args.out)
    return 1 if any("error" in v for v in res.values()) else 0


def cmd_tables(args) -> int:
    from . import paper_tables as PT
    bundle = _load_bundle(args.out)
    frames = _load_frames(args.out)
    written = PT.make_tables(bundle, frames, Path(args.out))
    _refresh_manifest(args.out)
    _p(f"tables: {len(written)} written -> {args.out}/tables")
    return 0


def cmd_numbers(args) -> int:
    # numbers are produced by `stats`; this re-emits numbers.tex from numbers.json
    from .tables import Numbers
    jpath = Path(args.out) / "numbers.json"
    payload = json.loads(jpath.read_text())
    nums = Numbers(payload.get("meta", {}))
    nums.entries = payload["numbers"]
    nums.write(args.out)
    _refresh_manifest(args.out)
    _p(f"numbers: {len(nums.entries)} macros re-emitted")
    return 0


def cmd_all(args) -> int:
    from . import checks as K
    rc = cmd_dataset(args)
    if rc:
        return rc
    bundle = _load_bundle(args.out)
    res = _run_stats(args, bundle)
    frames = {k: v for k, v in res.frames.items()}
    figs = _run_figures(args, bundle, frames)
    from . import paper_tables as PT
    PT.make_tables(bundle, frames, Path(args.out))
    K.write_manifest(Path(args.out), extra={"seed": args.seed, "n_boot": args.boot, "n_perm": args.perm,
                                            "figures_with_errors": [k for k, v in figs.items() if "error" in v]})
    _p(f"all: done -> {args.out} (manifest written)")
    return 1 if any("error" in v for v in figs.values()) else 0


def cmd_check(args) -> int:
    from . import checks as K
    bundle = None
    if (Path(args.out) / "data" / "runs.csv").exists() and not args.rebuild:
        bundle = _load_bundle(args.out)
    else:
        from . import dataset as ds
        bundle = ds.build_runs(progress=lambda s: _p(f"  built {s}"))
    checks = K.run_checks(bundle, out_dir=Path(args.out) if (Path(args.out) / "manifest.json").exists() else None)
    _p(K.report(checks))
    (Path(args.out)).mkdir(parents=True, exist_ok=True)
    (Path(args.out) / "check-report.json").write_text(json.dumps([c.to_dict() for c in checks], indent=1))
    return 1 if any(not c.ok and c.required for c in checks) else 0


def cmd_query(args) -> int:
    jpath = Path(args.out) / "numbers.json"
    payload = json.loads(jpath.read_text())
    q = args.term.lower()
    hits = {k: v for k, v in payload["numbers"].items() if q in k.lower() or q in str(v.get("metric", "")).lower()}
    for k, v in list(hits.items())[: args.limit]:
        _p(f"\\{C.MACRO_PREFIX}{k} = {v['text']}  [{v['provenance']} | {v['metric']} {v['stat']} | n={v['n']} | set {v['set']}]")
    _p(f"{len(hits)} macros match {args.term!r}")
    return 0


def _git_head(ref: str = "HEAD") -> str | None:
    from . import gitfacts as gf
    try:
        return gf.git("rev-parse", ref).strip()
    except Exception:
        return None


def build_parser() -> argparse.ArgumentParser:
    p = argparse.ArgumentParser(prog="tdd_paper", description=__doc__)
    p.add_argument("--version", action="version", version=__version__)
    sub = p.add_subparsers(dest="command", required=True)

    def common(sp):
        sp.add_argument("--out", type=Path, default=DEFAULT_OUT, help="output directory (default Visualizations/_build/paper)")
        sp.add_argument("--seed", type=int, default=C.SEED)
        sp.add_argument("--boot", type=int, default=C.N_BOOT, help="bootstrap replicates")
        sp.add_argument("--perm", type=int, default=C.N_PERM, help="permutations for the stratified test")
        sp.add_argument("--no-stability", action="store_true", help="skip the rank-stability bootstrap")
        sp.add_argument("--stories", nargs="*", help="restrict the dataset to these story ids")
        sp.add_argument("--no-git", action="store_true", help="skip git-derived facts")
        sp.add_argument("--refresh-cache", action="store_true", help="ignore the gitfacts cache")
        sp.add_argument("--only", nargs="*", help="figure slugs to (re)render")
        sp.add_argument("--rebuild", action="store_true", help="check: rebuild the dataset instead of loading it")
        sp.add_argument("--set", default="S0", help="stats: restrict to one sensitivity set (single-stratum sets only)")

    for name, fn in (("dataset", cmd_dataset), ("stats", cmd_stats), ("figures", cmd_figures), ("tables", cmd_tables),
                     ("numbers", cmd_numbers), ("all", cmd_all), ("build", cmd_all), ("check", cmd_check)):
        sp = sub.add_parser(name)
        common(sp)
        sp.set_defaults(func=fn)
    q = sub.add_parser("query", help="look up macros in numbers.json")
    common(q)
    q.add_argument("term")
    q.add_argument("--limit", type=int, default=40)
    q.set_defaults(func=cmd_query)
    return p


def main(argv: list[str] | None = None) -> int:
    args = build_parser().parse_args(argv)
    return args.func(args)
