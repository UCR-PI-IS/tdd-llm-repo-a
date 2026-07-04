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
| `00_overview_scorecard.ipynb` | **Which model wins overall?** Effort (retries), pipeline stage outcomes, refactoring success, and a KPI scorecard heatmap across all dimensions. Start here. |
| `10_build.ipynb` | How reliably does each model produce a compiling build, how many errors does it fight through, and where does it fail? |
| `20_tests_coverage.ipynb` | Pass rates, convergence to green, test methods vs story intents, line/branch coverage, test durations. |
| `30_code_metrics.ipynb` | Maintainability, complexity, coupling, inheritance and size of the produced code, against the documented GREEN/YELLOW/RED thresholds. |

Each notebook has a papermill-tagged `parameters` cell at the top
(`STORY_ID = "CPD-LC-001-001"`) — change it to analyze another story. Models,
iterations and timestamps are discovered from the filesystem, so new waves,
stories or models appear automatically on re-run.

## Data conventions (implemented in `tdd_results.py`)

- Result trees follow `<Tree>/<STORY>/<MODEL>/<ITERATION>/<TIMESTAMP>/`.
  Each (model, iteration) cell holds many retry executions.
- **Final run** = the execution with the max timestamp in a cell — the outcome
  the agent left behind. **All attempts** = every retry (an effort signal).
- Build/test/metrics timestamps don't align across trees, so joins are on
  `(story, model, iteration)`.
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
for nb in 00_overview_scorecard 10_build 20_tests_coverage 30_code_metrics; do
  .venv/bin/papermill "$nb.ipynb" "_executed/$nb.ipynb" -p STORY_ID CPD-LC-001-001 -k tdd-viz
done
```

Papermill exits non-zero if any cell fails; each notebook ends with a
sanity-assert cell (finals uniqueness, value ranges, iteration filter), so a
green headless run means the data layer and charts are healthy.
