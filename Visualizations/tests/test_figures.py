"""Every registered figure renders on a synthetic bundle and writes pdf, png and a sidecar."""
import json

import numpy as np
import pandas as pd
import pytest

from tdd_paper import config as C
from tdd_paper import figures as F


def _synthetic_bundle(seed=3):
    rng = np.random.default_rng(seed)
    rows = []
    for link in C.STORY_CHAIN:
        for model in C.MODEL_ORDER:
            for it in range(1, 7):
                k = model == "Kimi-K2.5"
                rows.append({
                    "run_id": f"{link.story}/{model}/{it}", "story": link.story, "story_pos": link.pos, "model": model,
                    "iteration": it, "n_intents": 20, "evidence_completeness": 1.0, "identity_ambiguous": False,
                    "compile_remove_added": 0, "exceptional_ending": "", "compile_red": False,
                    "build_failed_execs": rng.poisson(4 if k else 2), "build_errors_burned": rng.poisson(20 if k else 8),
                    "attempts_to_green": 1 + rng.poisson(1), "e2e_execs": 1 + rng.poisson(1 if k else 0.3),
                    "new_tests_per_intent": rng.normal(1.0 if k else 0.8, 0.15), "new_layer_balance": rng.uniform(0.5, 1),
                    "red_first": bool(rng.random() < 0.7), "first_run_new_fail_share": rng.uniform(0, 0.3),
                    "story_min_mi": rng.normal(55, 8), "story_max_cc": rng.normal(12, 3), "story_max_coupling": rng.normal(15, 4),
                    "story_min_mi_gain": rng.normal(0, 1), "e2e_attempts_to_pass_noinfra": 1 + rng.poisson(0.5),
                    "e2e_infra_failures": rng.poisson(0.2), "e2e_probe_depth": 1 + rng.poisson(1),
                    "tg_test_methods": 20 + rng.integers(-3, 4), "new_test_methods": 20 + rng.integers(-3, 4),
                    "new_test_cases": 26 + rng.integers(-3, 6),
                })
    runs = pd.DataFrame(rows)
    ex_rows = []
    for _, r in runs.iterrows():
        t = pd.Timestamp("2026-01-01 10:00:00")
        for i in range(3):
            ex_rows.append({"run_id": r["run_id"], "story": r["story"], "model": r["model"], "iteration": r["iteration"],
                            "tree": "build", "ts": t + pd.Timedelta(minutes=i), "attempt": i + 1,
                            "status": "failure" if i == 0 else "success", "total_errors": 5 if i == 0 else 0})
        ex_rows.append({"run_id": r["run_id"], "story": r["story"], "model": r["model"], "iteration": r["iteration"],
                        "tree": "test", "ts": t + pd.Timedelta(minutes=4), "attempt": 1, "status": "success", "failed": 0, "total": 10})
        ex_rows.append({"run_id": r["run_id"], "story": r["story"], "model": r["model"], "iteration": r["iteration"],
                        "tree": "e2e", "ts": t + pd.Timedelta(minutes=6), "attempt": 1, "status": "success", "probes_passed": 1,
                        "total_probes": 1, "backend_up": True})
        ex_rows.append({"run_id": r["run_id"], "story": r["story"], "model": r["model"], "iteration": r["iteration"],
                        "tree": "metrics", "ts": t + pd.Timedelta(minutes=5), "attempt": 1, "status": "snapshot"})
    executions = pd.DataFrame(ex_rows)
    stories = pd.DataFrame([{"story": l.story, "story_pos": l.pos, "n_intents": 20, "baseline_author_model": "Qwen3.7-max" if l.pos > 1 else "human",
                             "merged_run": f"{l.merged_model}/{l.merged_iteration}", "baseline_prod_lines": 400 * l.pos,
                             "baseline_test_attrs": 25 * (l.pos - 1), "effort_minutes": 500}
                            for l in C.STORY_CHAIN])
    metrics = ["build_failed_execs", "build_errors_burned", "attempts_to_green", "e2e_execs", "new_tests_per_intent", "red_first"]
    effects = pd.DataFrame([{"metric": m, "family": "primary", "delta": 0.3, "ci_low": 0.1, "ci_high": 0.5, "p_used": 0.01,
                             "p_holm": 0.04, "reject": True, "i2": 0.2, "stories_kimi_higher": 7, "stories_qwen_higher": 2,
                             "stories_tie": 1, "set": "S0"} for m in metrics])
    by_story = pd.DataFrame([{"metric": m, "set": "S0", "story": l.story, "delta": rng.uniform(-0.5, 0.9)}
                             for m in metrics for l in C.STORY_CHAIN])
    tost = pd.DataFrame([{"metric": m, "band": b, "estimate": 0.01, "ci_low": -0.1, "ci_high": 0.12, "equivalent": True,
                          "smallest_equivalent_band": 0.12, "set": "S0"} for m, b in C.TOST_BANDS.items()])
    passk = pd.DataFrame([{"group": g, "k": k, "estimator": e, "mean": 0.5, "ci_low": 0.3, "ci_high": 0.7, "predicate": p}
                          for g in C.MODEL_ORDER for k in range(1, 7) for e in ("pass_at_k", "pass_hat_k") for p in C.SUCCESS_PREDICATES])
    rank_rows = []
    for l in C.STORY_CHAIN:
        order = [(m, it) for m in C.MODEL_ORDER for it in range(1, 7)]
        rng.shuffle(order)
        for rank, (m, it) in enumerate(order, 1):
            rank_rows.append({"story": l.story, "rank": rank, "model": m, "iteration": it, "composite": 1 - rank / 13,
                              "pareto": rank <= 2, "top1_share": 1.0 if rank == 1 else 0.0})
    rankings = pd.DataFrame(rank_rows)
    frames = {"effects": effects, "effects_by_story": by_story, "tost": tost, "passk": passk, "rankings": rankings}
    return runs, executions, stories, frames


def test_every_registered_figure_renders(tmp_path):
    runs, executions, stories, frames = _synthetic_bundle()
    ctx = F.FigureContext(runs=runs, executions=executions, types=pd.DataFrame(), cells=pd.DataFrame(), stories=stories,
                          frames=frames, out_dir=tmp_path, seed=1)
    res = F.make_figures(ctx)
    errors = {k: v for k, v in res.items() if "error" in v}
    assert not errors, errors
    for slug in F.FIGURE_REGISTRY:
        for ext in ("pdf", "png", "json"):
            assert (tmp_path / "figures" / f"{slug}.{ext}").exists(), (slug, ext)
        side = json.loads((tmp_path / "figures" / f"{slug}.json").read_text())
        assert side["figure"] == slug and side["width_class"] in ("column", "text") and "data" in side
