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

## Pooled paper build (`tdd_paper`)

The notebooks above analyse one story at a time. The `tdd_paper` package pools the ten wave-1 stories
(120 runs) and produces every number, table and figure of the technical report from tool-written
evidence plus read-only git facts about the run branches. Agent self-reports are loaded, tagged
`self-reported` and never scored.

The venv's launcher scripts point at an old path on this machine, so always call the interpreter
directly:

```bash
cd Visualizations
.venv/bin/python -m pytest tests -q                       # unit, regression and repo-gated tests
.venv/bin/python -m tdd_paper check                       # integrity gate (120 cells, flags, composite regression)
.venv/bin/python -m tdd_paper all --out _build/paper      # dataset -> stats -> figures -> tables -> numbers -> manifest
.venv/bin/python -m tdd_paper all --out "../../tdd-llm-docs/technical-report/generated"   # the report's inputs
.venv/bin/python -m tdd_paper query attempts_to_green --out _build/paper                  # look up macros
.venv/bin/python -m papermill 50_pooled_analysis.ipynb _executed/50.ipynb -p OUT_DIR _build/paper
```

Outputs under `--out`: `data/runs.csv` (one row per run, every column catalogued in
`tdd_paper/schema.py` with its provenance), `data/executions.csv`, `data/types.csv`, `data/cells.csv`,
`data/stories.csv`, `data/excluded_refs.csv`; `stats/*.csv` (descriptives, effects, effects by story, TOST,
pass@k, rankings, rank stability, sensitivity, robustness); `figures/fig_<slug>.{pdf,png,json}` (sidecar
JSON holds every plotted number); `tables/tab_<slug>.tex` (bare booktabs bodies); `numbers.json` and
`numbers.tex` (`\val<Macro>` for every quotable number, with provenance); `manifest.json` (hashes of all
outputs, branch heads, seed).

Conventions that the report relies on:

* identity comes from the folder path and is canonicalised across stories (`Kimi-K2-5`, `Kimi-k2.5`,
  `qwen3.7-max` fold into the two display labels);
* the per-run baseline is the parent of the results commit (`chore(run)`), never `git merge-base`
  with the trunk (merged runs were fast-forwarded);
* story-new tests are counted from the branch diff as methods (comparable to `testMethodsEmitted`)
  and as cases (comparable to TRX after `[TestCase]` expansion);
* `red_first` means "first recorded test execution not green"; the harness has no controlled red phase;
* the primary confirmatory family is the artifact's eight contrasts (Holm); the secondary family has
  its own Holm correction; everything else is exploratory;
* sensitivity sets S0 to S10 are defined in `tdd_paper/config.py`.
