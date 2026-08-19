# Visualizations

Seaborn/Jupyter analysis of the TDD-LLM experiment results: compares the models
that ran a user story across **build results**, **test results** and **code
metrics** so you can weigh them side by side and decide.

## Setup (self-contained — everything installs into this folder)

```zsh
cd Visualizations
uv venv .venv --python 3.12
uv pip install --python .venv/bin/python -r requirements.txt
.venv/bin/python -m ipykernel install --user --name tdd-viz --display-name "Python (tdd-viz)"
```

Then launch JupyterLab and open any notebook (they are pinned to the
`Python (tdd-viz)` kernel):

```zsh
.venv/bin/jupyter lab
```

## Notebooks

| Notebook | Question it answers |
|---|---|
| `00_overview_scorecard.ipynb` | **Which model wins overall?** Data integrity gate, effort (retries), pipeline stage outcomes, a KPI scorecard heatmap across all dimensions, and a best-run ranking of every (model, iteration) from best to worst. Start here. |
| `10_build.ipynb` | How reliably does each model produce a compiling build, what does convergence cost, and where does it fail? |
| `20_tests_coverage.ipynb` | Pass rates, convergence to green, per-layer intent coverage, line/branch coverage, test durations. |
| `30_code_metrics.ipynb` | Maintainability, complexity, coupling, inheritance and size of the produced code, against the documented GREEN/YELLOW/RED thresholds, plus measured refactoring movement. |
| `40_e2e_validation.ipynb` | Does the story actually **work** — against a real SQL Server, a published API and HTTP probes? How many attempts did that take, and how often was the harness itself the failure? |

Each notebook has a papermill-tagged `parameters` cell at the top
(`STORY_ID = "CPD-LC-001-001"`) — change it to analyze another story. Models,
iterations and timestamps are discovered from the filesystem, so new waves,
stories or models appear automatically on re-run.

## Metric conventions (implemented in `tdd_results.py`)

Four rules decide what gets measured. They exist because result formats drift
between waves and agents do not report themselves consistently.

1. **Identity is canonical, never literal.** A run is addressed by
   `(story, model, iteration)`, but the model's spelling drifts: `Kimi-K2.5`
   under `BuildResults/`, `Kimi-k2.5` under `E2EResults/`, and something
   different again in the `"model"` field agents write inside their JSON.
   `canonical_model()` folds case and punctuation, `resolve_model()` maps any
   spelling to one display label, and **identity always comes from the path**
   — harness-created — never from the payload. `audit_identity(story)` lists
   every disagreement; anything with `reconciles = False` is genuinely
   misfiled and needs a human.
2. **Measure the path, not the endpoint.** The pipeline does not stop until
   the build compiles and the tests are green, so final build status, final
   error count and final pass rate are constant across runs by construction.
   The metrics that discriminate are convergence costs — `build_failed_execs`,
   `build_errors_burned`, `attempts_to_green`, `e2e_attempts_to_pass`.
3. **Tool-measured beats self-reported.** Anything scored comes from a build
   log, a VSTest `.trx`, a Cobertura report, a metrics snapshot or an e2e
   probe result. Agent-authored stage fields are still loaded, but tagged
   `self-reported` in `METRIC_CATALOG` and kept out of the composite — a
   claimed `allGreenAchieved` is checked against measured movement, not
   trusted.
4. **Absent is not zero.** `evidence(story)` reports which of the four result
   trees produced an artifact per run. Missing evidence is shown beside the
   scores so "the agent produced nothing" never looks like "the loader failed
   to join".

### Guard rails — run these on every new story

- `discrimination(summ)` flags any metric that is constant or all-NA across
  the cohort. A metric that cannot separate two runs is a defect (a broken
  join, or a quantity the pipeline saturates), not a data point.
- `audit_identity(story)` surfaces path/payload disagreements.
- Every notebook opens with a **Data integrity gate** cell running both, and
  ends with sanity assertions that fail loudly if identity drift ever leaks
  into a chart. A blank chart is the failure mode these prevent: `seaborn`
  given a `hue_order` that matches no row draws empty bars without raising.

### Layout

- Result trees follow `<Tree>/<STORY>/<MODEL>/<ITERATION>/<TIMESTAMP>/` for
  `BuildResults/`, `TestResults/`, `MetricsResults/` and `E2EResults/`.
  Each (model, iteration) cell holds many retry executions.
- **Final run** = the execution with the max timestamp in a cell — the outcome
  the agent left behind. **All attempts** = every retry (an effort signal).
- Build/test/metrics/e2e timestamps don't align across trees, so joins are on
  `(story, model, iteration)`.
- Field reads go through `pick()`, which takes an alias list
  (`totalProbes|probesTotal`), so a rename in a future wave degrades to a
  logged warning in `load_warnings` rather than a silently wrong number.
- Every loader returns an empty frame **with the full column set** when no
  data matches, and `iteration_summary` keeps all columns present as NaN. A
  notebook run against a story whose results have not landed yet renders empty
  charts instead of raising, and names the stories that do have results.
- The setup cell calls `reset_caches()` before loading. The model-label cache
  and `load_warnings` are module-level, so re-running cells in a live kernel
  after dropping in new results would otherwise reuse stale labels and report
  the same warning once per re-run.
- `INCLUDED_ITERATIONS` in `tdd_results.py` limits analysis to iterations 1–6
  (iterations 7–8 exist for DeepSeek but produced no canonical results).
  Set it to `None` to include everything.
- One fixed model→color mapping (`model_palette`) is used in every chart;
  GREEN/YELLOW/RED status colors are reserved for flags/outcomes and never
  reused for models. Palette is colorblind-validated.

## Headless execution / regression check

```zsh
cd Visualizations
mkdir -p _executed
for nb in 00_overview_scorecard 10_build 20_tests_coverage 30_code_metrics 40_e2e_validation; do
  .venv/bin/papermill "$nb.ipynb" "_executed/$nb.ipynb" -p STORY_ID CPD-LC-001-001 -k tdd-viz
done
```

Papermill exits non-zero if any cell fails; each notebook ends with a
sanity-assert cell (finals uniqueness, value ranges, iteration filter), so a
green headless run means the data layer and charts are healthy.
