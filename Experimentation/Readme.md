# Experimentation Setup

This folder is the entry point for researchers running TDD-LLM experiments on this repository. It documents the wave model, the agent pipeline that produces each run, the automation scripts that execute build / test / metrics inside Docker, and the on-disk layout that stores empirical evidence per user story, model, and iteration.

The repo doubles as both the System Under Test (a Clean Architecture / DDD .NET solution — `Backend.*` projects plus `Frontend.Blazor.*`) and the experimental harness. The OpenCode agents in `.opencode/agents/` drive the TDD cycle; the Python scripts in `Automations/` execute every build/test/metrics command inside Docker so results are reproducible across hosts.

---

## Concepts

### Experimentation Wave

An **experimentation wave** is a versioned iteration of the research effort. Each wave fixes a focus, a set of models, a number of runs, and a procedure for selecting candidate solutions. Waves are tracked as git branches (`experiments/wave-<N>`) and serve as the baseline that every run branch is cut from.

Examples of waves:
- **Wave 0** — proof of concept
- **Wave 1** — unit testing focus (current)
- **Wave 2** — LLM as a judge (planned)

### Run

A **run** is a single end-to-end execution of the TDD pipeline for one `(user story, model, iteration)` triple. Each run produces:
- A dedicated git branch off the wave baseline
- Generated tests and production code
- Build / test / metrics artifacts under timestamped folders
- Two commits on the run branch (result artifacts, then source/test changes)

### Iteration Independence

Every `<STORY-ID>/<MODEL>/<ITERATION>` combination is fully independent. Neither the orchestrator nor any subagent may read artifacts from another story/model/iteration folder. The iteration counter is a label for output partitioning, **not** a feedback channel between runs — this is what allows the 40 runs per (story, model) cell to be treated as independent samples for the statistical analysis.

---

## Waves

### Wave 1 (current)

#### Objective

Generate empirical evidence of orchestrator-driven TDD runs focused on **unit testing**, across configurable open-weight thinking models with fixed hyper-parameters. The current model set includes **Kimi K2.5**, **DeepSeek**, and **Qwen**. All results are versioned on the branch `origin/experiments/wave-1`.

#### Procedure

For each user story, run the full pipeline **40 times per model** to provide enough samples for normality analysis and statistical relevance testing. With 10 user stories and 3 models that yields:

```
40 runs × 3 models × 10 stories = 1200 runs
```

Each run is stored on its own branch, auto-created from `experiments/wave-1` with the convention:

```
runs/wave-<WAVE>/<STORY-ID>/<MODEL>/<ITERATION>
```

For each user story, after the 3×40 runs complete, the **best candidate** across all models is merged back into `origin/experiments/wave-1`, so the next user story starts from the updated baseline. Only one candidate is merged, but **every candidate stays in the repo** (on its run branch) as empirical evidence.

##### Flow

The diagram below shows the lifecycle of a single user story within the wave. The same loop repeats sequentially for stories B, C, ... J, each starting from the baseline updated by the previous story.

```
                       UserStories/<STORY-A>.md
                       UserIntents/<STORY-A>.json (confirmed offline)
                                  |
                                  v
                    +-----------------------------+
                    |   experiments/wave-1        |  <-- baseline branch
                    |   (updated after each story)|
                    +--------------+--------------+
                                   |
        +---------------------------+---------------------------+
        | branch each run off the baseline (no cross-talk)      |
        v                          v                            v
+--------------+           +--------------+              +--------------+
|  Kimi-K2.5   |           |   DeepSeek   |              |     Qwen     |
| iter 1..40   |           | iter 1..40   |              | iter 1..40   |
+--------------+           +--------------+              +--------------+
        \                          |                            /
         \                         |                           /
          \                        v                          /
           \         per iteration: orchestrator runs        /
            \       test-gen -> code-gen -> refactor        /
             \      (Docker build/test/metrics)            /
              \    commits A (artifacts) + B (sources)    /
               \           on run branch                 /
                \                                       /
                 \                                     /
                  v                                   v
            120 run branches for STORY-A (3 models x 40 iters)
                              |
                              v
                +-----------------------------+
                | select best candidate       |
                | (analysis over JSON results)|
                +--------------+--------------+
                               |
                               v
              merge best candidate -> experiments/wave-1
                               |
                               v
                  proceed to STORY-B (new baseline)
```

Key properties visible in the flow:
- The baseline (`experiments/wave-1`) advances only between stories, never between iterations.
- Run branches are leaves — they are never merged into each other; only the chosen best per story merges upward.
- Each run branch carries its own `BuildResults/`, `TestResults/`, `MetricsResults/`, and the three `pipeline-stage-result.json` files needed for analysis.

#### Analysis

Raw data analysis lives under `Visualizations/` (currently `tdd_cycle_dashboard.ipynb` with `requirements.txt`). The notebook reads the structured JSON artifacts produced by the docker scripts (`build-summary.json`, `test-summary.json`, `metrics-summary.json`, and the `pipeline-stage-result.json` files emitted by each agent). The visualization layer will be expanded over the wave.

The user stories under analysis in Wave 1 are listed in `UserStories/` and their confirmed test intents in `UserIntents/`.

---

## OpenCode Agent Pipeline

The pipeline is implemented as five OpenCode agents under `.opencode/agents/`. They run on `azure-foundry-base-models/Kimi-K2.5` by default (configurable per run via the `<MODEL>` argument) with `temperature: 1.0` and `top_p: 0.1`.

### `intent-generator` (interactive, prerequisite)

Analyzes a user story and proposes the minimum set of test intents needed for 100% decision coverage across DDD layers (Domain → Application → Infrastructure → Presentation). Runs an interactive **YES / NO / UNKNOWN** confirmation loop with the researcher and writes the result to `UserIntents/<STORY-ID>.json`. This step **must be completed manually before the orchestrator runs** — the orchestrator does not invoke it.

### `orchestrator` (primary, autonomous)

Entry point for an automated run. Given `(wave, story-id, model, iteration)` it:

1. Verifies `UserStories/<STORY-ID>.md` and `UserIntents/<STORY-ID>.json` exist and that at least one intent has `"status": "confirmed"`.
2. Creates the run branch `runs/wave-<WAVE>/<STORY-ID>/<MODEL>/<ITERATION>` off `experiments/wave-1` (fails fast if it already exists — runs are never reused).
3. Invokes `test-generator` → `code-generator` → `refactor-generator` in sequence, reading each stage's `pipeline-stage-result.json` to decide whether to continue.
4. Commits results to the run branch in **two separate commits, in this order**:
   - **Commit A** (`chore(run): ...`) — only the directories produced by the docker scripts (`BuildResults/...`, `TestResults/...`, `MetricsResults/...`).
   - **Commit B** (`feat(run): ...`) — generated tests, implementation, and refactor edits.
5. Produces a final report combining the three stage JSONs deterministically (no log re-parsing).

The orchestrator never modifies files directly and never pushes/merges — that is the merge-of-best-candidate step done outside the pipeline.

### `test-generator`

Reads confirmed intents from `UserIntents/<STORY-ID>.json` and emits NUnit test classes into `Backend.*.Tests.Unit/`, one test class per production class, AAA-structured, with at most one logical assertion per test (or `Assert.Multiple` for multi-property checks). Does **not** build, test, or commit. Emits `TestResults/<STORY-ID>/<MODEL>/<ITERATION>/test-generator/pipeline-stage-result.json` for the orchestrator.

### `code-generator`

Implements the minimum production code required to make the generated tests pass. Operates per DDD layer (`Backend.Domain/`, `Backend.Application/`, `Backend.Infrastructure/`, `Backend.Presentation/`, plus `Backend.DependencyInjection/`) and validates with `docker-build.py` and `docker-test.py` autonomously, retrying up to 5 times on build/test failure (always reusing the same `<MODEL>` and `<ITERATION>` so artifacts stay grouped). Writes `pipeline-stage-result.json` next to the latest test-run output.

### `refactor-generator`

Closes the TDD cycle. Runs Microsoft Code Metrics via `docker-metrics.py`, performs a static-smell pass (block duplication, dead members, unused overloads, repeated guards), then loops up to 10 iterations of targeted refactorings against these thresholds:

| Metric | Green | Yellow | Red |
|---|---|---|---|
| Maintainability Index | 20–100 | 10–19 | 0–9 |
| Cyclomatic Complexity | 1–10 | 11–25 | > 25 |
| Class Coupling | 0–9 | 10–40 | > 40 |
| Depth of Inheritance | 0–4 | 5 | ≥ 6 |
| Lines per Method | 5–20 | 21–50 | > 50 |

Every refactor must keep build + tests green; failed refactors are reverted. Emits the final `pipeline-stage-result.json` with baseline vs. final metrics and any remaining violations.

---

## Automations

All build, test, restore, and metrics operations **must** go through the Python launchers in `Automations/`. Direct `dotnet build` / `dotnet test` / `dotnet msbuild` calls are forbidden by every agent — Docker is the only execution boundary, which is what makes runs comparable across machines.

| Script | Purpose | Output root |
|---|---|---|
| `Automations/docker-build.py <STORY-ID> <MODEL> <ITERATION>` | Restore + build the full .NET solution inside `themepark-dotnet-sdk` container. Emits `build.log` + `build-summary.json`. | `BuildResults/<STORY-ID>/<MODEL>/<ITERATION>/<timestamp>/` |
| `Automations/docker-test.py <STORY-ID> <MODEL> <ITERATION>` | Build test projects + run NUnit with coverage (coverlet). Emits `test.log`, `test-summary.json`, TRX files, and Cobertura/HTML coverage. | `TestResults/<STORY-ID>/<MODEL>/<ITERATION>/<timestamp>/` |
| `Automations/docker-metrics.py <STORY-ID> <MODEL> <ITERATION>` | Run the Roslyn-based metrics calculator (`tools/MetricsCalculator/`) over production projects. Emits `metrics-summary.json`, `metrics-summary.txt`, and `*.Metrics.xml`. | `MetricsResults/<STORY-ID>/<MODEL>/<ITERATION>/<timestamp>/` |

Supporting files:

- `Automations/Dockerfile.build` — image used by build/test runs (.NET SDK).
- `Automations/Dockerfile.metrics` — image used by the metrics calculator.
- `Automations/docker_utils.py` — shared helpers (timestamping, path-component sanitization, docker invocation).

### Cross-platform invocation

Always invoke the scripts via a Python launcher rather than the Unix shebang form so the same command works on macOS, Linux, and Windows. The agents are pre-approved for every supported launcher; try them in this order:

1. `python Automations/docker-build.py <STORY-ID> <MODEL> <ITERATION>`
2. `python3 Automations/docker-build.py <STORY-ID> <MODEL> <ITERATION>`
3. `py Automations/docker-build.py <STORY-ID> <MODEL> <ITERATION>` (Windows `py.exe` launcher)

Use forward slashes in paths even on Windows. `<MODEL>` and `<ITERATION>` are sanitized to `[A-Za-z0-9._-]` on disk — keep this in mind when reconciling branch names with folder names.

---

## On-disk Layout per Run

Once the pipeline completes for `(<STORY-ID>, <MODEL>, <ITERATION>)`, the run branch contains:

```
BuildResults/<STORY-ID>/<MODEL>/<ITERATION>/<timestamp>/
   build.log
   build-summary.json

TestResults/<STORY-ID>/<MODEL>/<ITERATION>/
   test-generator/pipeline-stage-result.json        # written by test-generator
   <timestamp>/
       test.log
       test-summary.json
       pipeline-stage-result.json                   # written by code-generator
       TestResults/<project>/*.trx
       Coverage/Combined/Cobertura.xml + HTML report

MetricsResults/<STORY-ID>/<MODEL>/<ITERATION>/<timestamp>/
   metrics-summary.json
   metrics-summary.txt
   *.Metrics.xml
   pipeline-stage-result.json                       # written by refactor-generator (latest folder)

Backend.*.Tests.Unit/                                # generated test classes
Backend.Domain/, Backend.Application/, ...           # generated/modified production code
```

The three `pipeline-stage-result.json` files are the **canonical source of truth** for analysis — they normalize what each agent did into a stable schema (`status`, `filesCreated`, `filesModified`, `metrics`, `warnings`, `notes`). Prefer them over re-parsing logs.

---

## How to Run an Experiment (Researcher Quick Start)

Prerequisites: Docker running locally; OpenCode CLI with this repo opened; a model endpoint reachable as `azure-foundry-base-models/<model>`. Confirmed intents for the Wave 1 stories are already committed under `UserIntents/` — no intent generation step is needed before running the pipeline.

1. **Pick a story** from `UserStories/` (e.g., `CPD-LC-001-001`) and verify `UserIntents/<STORY-ID>.json` exists with confirmed entries.
2. **Run the pipeline** for one iteration:
   ```
   @orchestrator
   # When prompted, provide: wave=1, story=CPD-LC-001-001, model=Kimi-K2.5, iteration=1
   ```
   The orchestrator creates `runs/wave-1/CPD-LC-001-001/Kimi-K2.5/1`, runs the three subagents, and commits the two artifact/source commits to that branch.
3. **Repeat** for iterations 2..40, then switch model, then switch story. (Automating this outer loop is a planned addition.)
4. **Analyze** with the notebook under `Visualizations/`, pointing it at the `pipeline-stage-result.json` files in the run branches.

To debug an individual stage in isolation, you can invoke `@test-generator`, `@code-generator`, or `@refactor-generator` directly — but supply `<STORY-ID>`, `<MODEL>`, and `<ITERATION>` so their artifacts land in the right folder. Stages do **not** read artifacts from other iterations.
