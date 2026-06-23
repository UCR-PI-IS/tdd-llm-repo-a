# TDD-LLM Experiment — Introduction

## What this project is

This repository is an **experimentation harness for Test-Driven Development (TDD) driven by Large Language Models**. It uses [OpenCode](https://opencode.ai) and a set of purpose-built OpenCode agents to run the full **test → implement → refactor** cycle automatically from a written **user story**, then captures build, test, and code-metrics evidence for every run.

The repo plays two roles at once:

- **System Under Test (SUT)** — a Clean Architecture / DDD .NET solution (`Backend.*` projects and `Frontend.Blazor.*`).
- **Experimental harness** — OpenCode agents in `.opencode/agents/` drive the TDD cycle, and Python scripts in `Automations/` run every build/test/metrics command inside Docker so results are reproducible across machines.

The goal is to produce **empirical, comparable evidence** of how different LLMs perform at orchestrator-driven TDD on the same user stories under identical, fixed conditions.

---

## What the agents do

The pipeline is five OpenCode agents under `.opencode/agents/`. They run with fixed hyper-parameters (`temperature: 1.0`, `top_p: 0.1`); the driving model is swapped per experiment.

| Agent | Role | Runs when |
|---|---|---|
| `intent-generator` | Reads a user story and proposes the **minimum test intents** for 100% decision coverage across DDD layers (Domain → Application → Infrastructure → Presentation). Interactive `YES / NO / UNKNOWN` confirmation loop; writes `UserIntents/<STORY-ID>.json`. | **Once, manually, before runs.** Not part of the automated loop. |
| `orchestrator` | Entry point for an automated run. Takes `(wave, story-id, model, iteration)`, verifies confirmed intents exist, creates an isolated run branch, then drives the three generators in sequence and commits the results. | Per run (the thing you launch). |
| `test-generator` | Emits NUnit test classes from the confirmed intents into `Backend.*.Tests.Unit/`. Does not build or commit. | Step 1 of each run. |
| `code-generator` | Writes the **minimum production code** to make the generated tests pass; builds and tests via Docker, retrying on failure. | Step 2 of each run. |
| `refactor-generator` | Runs Microsoft Code Metrics, then loops targeted refactorings against quality thresholds (Maintainability Index, Cyclomatic Complexity, Class Coupling, Depth of Inheritance, Lines/Method). Every refactor must keep build + tests green. | Step 3 of each run. |

Each stage writes a canonical `pipeline-stage-result.json` — that file (not raw logs) is the source of truth for analysis.

> A separate `data-injection` agent exists to seed the local SQL database from the live backend schema. It is a utility, **not** part of the TDD experiment loop.

---

## Prerequisites (software)

Before running any experiment you need:

1. **Docker** — running locally. All build / test / metrics execution happens inside containers; raw `dotnet` calls are forbidden by the agents.
2. **OpenCode CLI** — with this repository opened as the project.
3. **A reachable model endpoint** — configured in `opencode.json`. The current providers are Azure AI Foundry (`azure-foundry-base-models/*`) and OpenRouter; valid API keys must be in place.
4. **Python 3** — available as `python`, `python3`, or `py` (the agents try each in order). Used to launch the `Automations/*.py` scripts.
5. **Git** — runs are isolated on dedicated branches cut from the wave baseline.
6. *(Analysis only)* **Jupyter** + the packages in `Visualizations/requirements.txt`, to open `Visualizations/tdd_cycle_dashboard.ipynb`.

---

## This batch: 18 runs

This round executes **18 runs total** for a single user story, to compare three models head-to-head:

```
6 runs/model × 3 models × 1 user story = 18 runs
```

- **User story:** one story from `UserStories/` (e.g. `CPD-LC-001-001`), with its confirmed intents already in `UserIntents/<STORY-ID>.json`.
- **Models (3 selected):** swapped per run via the `<MODEL>` argument — e.g. `Kimi-K2.5`, `DeepSeek-V4-Pro`, and `grok-4-1-fast-reasoning` (the model set configured in `opencode.json`).
- **Iterations:** 6 per model (`1`–`6`). Each iteration is **fully independent** — no agent reads artifacts from another iteration. The iteration number only partitions output; it is not a feedback channel. This independence is what lets the 6 runs per (story, model) cell be treated as independent samples.

Every run produces its own git branch off the wave baseline:

```
runs/wave-<WAVE>/<STORY-ID>/<MODEL>/<ITERATION>
```

and carries its own `BuildResults/`, `TestResults/`, `MetricsResults/`, and three `pipeline-stage-result.json` files.

---

## How to run an experiment

> Confirmed intents for the existing stories are already committed under `UserIntents/`, so the intent-generation step is **not** needed for this batch. Only run `@intent-generator <STORY-ID>` if you add a new story or need to re-confirm intents.

For each of the 18 `(model, iteration)` combinations:

1. **Make sure Docker is running** and the model endpoint is reachable.
2. **Launch the orchestrator** in OpenCode and provide the four inputs when prompted:
   ```
   @orchestrator
   # wave=1, story=CPD-LC-001-001, model=Kimi-K2.5, iteration=1
   ```
   The orchestrator will:
   - verify `UserStories/<STORY-ID>.md` and confirmed `UserIntents/<STORY-ID>.json` exist,
   - create branch `runs/wave-1/<STORY-ID>/<MODEL>/<ITERATION>` off `experiments/wave-1` (fails fast if it already exists — runs are never reused),
   - run `test-generator → code-generator → refactor-generator`,
   - commit two commits to the run branch: **(A)** result artifacts, then **(B)** generated tests + source.
3. **Repeat** for iterations `1`–`6`, then switch `<MODEL>` and repeat — until all 18 runs are complete.
4. **Analyze** with `Visualizations/tdd_cycle_dashboard.ipynb`, pointing it at the `pipeline-stage-result.json` files across the run branches.

To debug a single stage, invoke `@test-generator`, `@code-generator`, or `@refactor-generator` directly — but always pass `<STORY-ID>`, `<MODEL>`, and `<ITERATION>` so artifacts land in the right folder.

---

## Where to look next

- **Full harness reference:** `Experimentation/Readme.md` (wave model, on-disk layout, automation scripts, metric thresholds).
- **Agent definitions:** `.opencode/agents/`.
- **Model / provider config:** `opencode.json`.
- **Stories & intents under test:** `UserStories/` and `UserIntents/`.
