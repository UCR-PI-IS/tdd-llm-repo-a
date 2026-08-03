# Docker Build, Test & End-to-End Automation

This directory contains Docker-based automation scripts to run builds, tests, and full end-to-end validation in isolated, ephemeral containers. These scripts are designed for agent automation with full output capture and historical run preservation.

| Script | Purpose |
|--------|---------|
| `docker-build.py` | Compile every project |
| `docker-test.py` | Run unit tests with coverage |
| `docker-metrics.py` | Run Microsoft Code Metrics |
| `docker-database.py` | Start/stop a throwaway SQL Server the solution can connect to |
| `docker-e2e.py` | Seed a throwaway database, run the backend against it, probe the endpoints, tear it all down |
| `insert-sample-data.sh` | Execute caller-supplied SQL inside a running SQL container |

## Prerequisites

- **Docker** installed and running
- **Bash** shell (Linux, macOS, or Windows WSL/Git Bash)

## Quick Start

```bash
# Make scripts executable
chmod +x docker-build.sh docker-test.sh

# Run build
./docker-build.sh

# Run tests
./docker-test.sh

# Run both in parallel
./docker-build.sh & ./docker-test.sh &
```

## Scripts

### `docker-build.sh`

Builds all .NET projects in an ephemeral Docker container.

**What it does:**
1. Creates timestamped output directory: `BuildResults/YYYY-MM-DD_HH-MM-SS/`
2. Builds Docker image (with caching for speed)
3. Restores NuGet dependencies
4. Builds all `.csproj` files (excludes `.sqlproj`)
5. Streams output to stdout/stderr and captures to `build.log`
6. Returns exit code (0 = success, non-zero = failure)

**Output structure:**
```
BuildResults/
├── 2026-04-06_14-30-15/
│   ├── build.log           # Complete build output
│   └── build-script.sh     # Generated build script
└── 2026-04-06_15-45-22/    # Next run
    ├── build.log
    └── build-script.sh
```

### `docker-database.py`

Runs a throwaway SQL Server container the solution can connect to. Nothing is reused between runs and nothing survives a `down`.

```bash
python Automations/docker-database.py up --port 14330            # start + create the ThemePark DB
python Automations/docker-database.py up --seed /tmp/seed.sql    # ...and apply sample data
python Automations/docker-database.py exec --file /tmp/more.sql  # run more SQL
python Automations/docker-database.py exec --sql "SELECT 1"
python Automations/docker-database.py status                     # what is currently alive
python Automations/docker-database.py down                       # remove it
python Automations/docker-database.py prune                      # remove leftovers from a crashed run
```

**What `up` does:**
1. Starts `mcr.microsoft.com/mssql/server:2022-latest` as `themepark-sql-e2e` (adding `--platform linux/amd64` automatically on Apple Silicon)
2. Waits until SQL Server accepts connections, then creates the `ThemePark` database
3. Applies the table scripts in `UCR.ECCI.PI.ThemePark.Database/Tables/` (skip with `--no-base-schema`)
4. Applies every `--seed` file, in order
5. Prints the connection string to drop into `ConnectionStrings__DefaultConnection`

**Safety:** every container, network, and volume it creates carries the `themepark-e2e=1` Docker label, and only labelled resources are ever removed. A hand-started `themepark-sql` container is never touched — if you point `--name` at an unlabelled container, the script refuses rather than deleting it.

**Environment overrides:** `THEMEPARK_SQL_IMAGE`, `THEMEPARK_SQL_PASSWORD`, `THEMEPARK_SQL_DATABASE`, `THEMEPARK_SQL_CONTAINER`, `THEMEPARK_SQL_READY_TIMEOUT`, `THEMEPARK_SQL_PLATFORM`.

The existing `insert-sample-data.sh` can target the ephemeral container instead of the manual one:

```bash
SQL_CONTAINER=themepark-sql-e2e ./Automations/insert-sample-data.sh --file /tmp/seed.sql
```

### `docker-e2e.py`

Proves the solution actually runs, not just that it compiles and its unit tests pass. One command does the full round trip and always cleans up.

```bash
python Automations/docker-e2e.py <STORY-ID> <MODEL> <ITERATION> \
    --require-green \
    --seed /tmp/seed-learningspace.sql \
    --probe "GET /LearningSpaceList 200"
```

**`--require-green`** makes the script read the newest `build-summary.json` and `test-summary.json` for the same `<STORY-ID>/<MODEL>/<ITERATION>` and refuse to start anything (exit 2) unless both are green. Use it in the TDD pipeline so end-to-end validation can only ever run on code that compiles and passes its tests.

**What it does:**
1. Starts an ephemeral SQL Server container on a private Docker network
2. Applies the base table scripts plus the sample data you pass in (`--seed FILE`, `--seed-sql "..."`)
3. Publishes `Backend.Api` into a Docker volume and starts it in a container wired to that database
4. Waits for readiness (`--ready-path`, default `/swagger/v1/swagger.json`), then runs the HTTP probes
5. Stops the backend and deletes the container, the database container, the network, and the volume

Step 5 runs on every exit path — success, failure, `Ctrl-C`, `SIGTERM`, or an unexpected exception — so no container or process is left behind. `--keep-up` opts out for manual debugging and is the only way to leave anything running.

**Probes.** Simple form: `--probe "METHOD /path [STATUS]"` (repeatable, status defaults to 200). Richer assertions via `--probes <file.json>`:

```json
{"probes": [{"name": "list includes seeded space", "method": "GET", "path": "/LearningSpaceList",
             "expectStatus": 200, "expectBodyContains": ["IF-0103"]}]}
```

**Output structure:**
```
E2EResults/SQL-LS-001-007/Kimi-K2.5/1/2026-07-27_22-08-30/
├── e2e.log            # full run output
├── publish.log        # dotnet publish output
├── api.log            # everything the running backend logged
├── seeds/             # the sample data that was applied
├── publish-script.sh  # generated publish script
└── e2e-summary.json   # structured summary (probes, cleanup, warnings, errors)
```

**Exit codes:** `0` validated, `1` validation failure (probe/seed/startup), `2` environment problem (Docker unreachable).

**Failure signals to read from `e2e-summary.json`:** `backend.publishStatus`, `backend.started`, `backend.errorLines` (unhandled exceptions logged while serving), `probes[].actualStatus`, `probes[].missingContent`, and `cleanup.status` (must be `clean`).

### `docker-test.sh`

Runs unit tests with code coverage in an ephemeral Docker container.

**What it does:**
1. Creates timestamped output directory: `TestResults/YYYY-MM-DD_HH-MM-SS/`
2. Builds Docker image (with caching for speed)
3. Restores NuGet dependencies
4. Builds all Backend test projects
5. Runs tests with XPlat Code Coverage
6. Generates coverage reports (HTML and Cobertura)
7. Streams output to stdout/stderr and captures to `test.log`
8. Returns exit code (0 = success, non-zero = failure)

**Output structure:**
```
TestResults/
├── 2026-04-06_14-30-15/
│   ├── test.log               # Complete test output
│   ├── test-script.sh         # Generated test script
│   ├── TestResults/           # Test results by project
│   │   ├── ProjectName1/
│   │   │   ├── ProjectName1.trx
│   │   │   └── coverage.cobertura.xml
│   │   └── ProjectName2/
│   │       ├── ProjectName2.trx
│   │       └── coverage.cobertura.xml
│   ├── Coverage/              # Coverage reports
│   │   ├── ProjectName1/
│   │   │   └── index.html     # HTML coverage report
│   │   ├── ProjectName2/
│   │   │   └── index.html
│   │   └── Combined/
│   │       └── index.html     # Combined coverage report
│   └── .tools/                # ReportGenerator tool
└── 2026-04-06_15-45-22/       # Next run
    └── ...
```

## Docker Image

**Base Image:** `mcr.microsoft.com/dotnet/sdk:8.0-alpine`

**Size:** ~200-250MB (compressed) vs ~700MB for standard Debian-based image

**Features:**
- .NET 8.0 SDK
- Alpine Linux (minimal footprint)
- Bash shell
- Non-interactive environment variables set

## Agent Integration

### Parsing Output

Both scripts print structured status messages:

```bash
# Success case
✓ Build succeeded
Build results: BuildResults/2026-04-06_14-30-15/

# Failure case
✗ Build failed (exit code: 1)
Build logs: BuildResults/2026-04-06_14-30-15/build.log
```

### Exit Codes

- `0` = Success
- Non-zero = Failure (propagated from `dotnet` commands)

### Detecting Output Location

Parse the last lines of script output:

```bash
Output location: BuildResults/2026-04-06_14-30-15
```

Or capture the directory path:

```bash
# For build
BUILD_DIR=$(./docker-build.sh 2>&1 | grep "^Output location:" | cut -d' ' -f3)

# For tests
TEST_DIR=$(./docker-test.sh 2>&1 | grep "^Output location:" | cut -d' ' -f3)
```

### Analyzing Logs

```bash
# View complete build log
cat BuildResults/2026-04-06_14-30-15/build.log

# Search for errors in build
grep -i "error" BuildResults/2026-04-06_14-30-15/build.log

# View test results summary
cat TestResults/2026-04-06_14-30-15/test.log

# List failed tests
grep -i "failed" TestResults/2026-04-06_14-30-15/test.log

# Open coverage report
open TestResults/2026-04-06_14-30-15/Coverage/Combined/index.html
```

## Parallel Execution

Scripts are designed to run in parallel safely:

```bash
# Run both simultaneously
./docker-build.sh & BUILD_PID=$!
./docker-test.sh & TEST_PID=$!

# Wait for both to complete
wait $BUILD_PID
BUILD_CODE=$?
wait $TEST_PID
TEST_CODE=$?

# Check results
if [ $BUILD_CODE -eq 0 ] && [ $TEST_CODE -eq 0 ]; then
    echo "Both succeeded"
else
    echo "One or more failed: build=$BUILD_CODE, test=$TEST_CODE"
fi
```

Each run gets its own timestamp, preventing conflicts.

## Performance Optimizations

### NuGet Package Caching

Both scripts mount `~/.nuget/packages` to avoid re-downloading packages:

```bash
-v "${HOME}/.nuget/packages:/root/.nuget/packages"
```

This reduces restore time from ~30s to ~5s on subsequent runs.

### Docker Image Caching

The Docker image is built with BuildKit caching enabled. After the first build, subsequent builds are nearly instantaneous.

### Volume Mounts vs COPY

Source code is mounted as a volume (not copied), making iteration instant:

```bash
-v "${WORKSPACE_ROOT}:/workspace:ro"  # Read-only mount
```

## Ephemeral Containers

Build, test, and metrics containers are removed after execution with the `--rm` flag:

```bash
docker run --rm ...
```

The database and backend containers created by `docker-database.py` and `docker-e2e.py` are long-lived by nature, so they are labelled `themepark-e2e=1` and removed explicitly during teardown. `docker-e2e.py` tears down on every exit path, including `Ctrl-C` and unexpected exceptions.

Verify nothing remains:

```bash
python Automations/docker-database.py status   # "No ephemeral Theme Park resources are running."
python Automations/docker-database.py prune    # remove leftovers from a crashed run
docker ps -a                                   # should show no themepark containers
```

## Troubleshooting

### Permission Issues

If you get permission denied errors on macOS/Linux:

```bash
chmod +x docker-build.sh docker-test.sh
```

### Docker Not Running

```bash
docker ps  # Check if Docker daemon is running
```

### Build Failures

Check the timestamped build log:

```bash
cat BuildResults/YYYY-MM-DD_HH-MM-SS/build.log
```

Common issues:
- Missing project references
- NuGet restore failures
- Compilation errors

### Test Failures

Check the timestamped test log:

```bash
cat TestResults/YYYY-MM-DD_HH-MM-SS/test.log
```

Common issues:
- Test assertion failures
- Missing test dependencies
- Configuration issues

### Disk Space

Timestamped directories accumulate over time. To clean up old runs:

```bash
# Keep only last 10 build runs
ls -t BuildResults/ | tail -n +11 | xargs -I {} rm -rf BuildResults/{}

# Keep only last 10 test runs
ls -t TestResults/ | tail -n +11 | xargs -I {} rm -rf TestResults/{}

# Or delete all runs older than 7 days
find BuildResults/ -maxdepth 1 -type d -mtime +7 -exec rm -rf {} \;
find TestResults/ -maxdepth 1 -type d -mtime +7 -exec rm -rf {} \;
```

### Docker Image Size

Check image size:

```bash
docker images themepark-dotnet-sdk
```

Should be ~200-250MB. If larger, rebuild:

```bash
docker rmi themepark-dotnet-sdk
./docker-build.sh  # Rebuilds image
```

## Cross-Platform Support

### Linux / macOS
Scripts work natively with bash.

### Windows
Use one of:
- **WSL (Windows Subsystem for Linux)** - Recommended
- **Git Bash** - Works with Docker Desktop
- **PowerShell versions** - Create equivalent `.ps1` scripts if needed

## Advanced Usage

### Custom Build Configuration

Edit `build-script.sh` template in `docker-build.sh` to:
- Change build configuration (Debug vs Release)
- Add custom build flags
- Filter which projects to build

### Custom Test Filters

Edit `test-script.sh` template in `docker-test.sh` to:
- Run specific test categories
- Change test frameworks
- Add custom test filters
- Adjust coverage settings

### CI/CD Integration

These scripts can be called from GitHub Actions:

```yaml
- name: Run Docker Build
  run: ./docker-build.sh

- name: Run Docker Tests
  run: ./docker-test.sh

- name: Upload Build Results
  uses: actions/upload-artifact@v3
  with:
    name: build-results
    path: BuildResults/**/build.log

- name: Upload Test Results
  uses: actions/upload-artifact@v3
  with:
    name: test-results
    path: TestResults/**/
```

## Architecture Decisions

**Why Alpine?**
- Minimal size (~200MB vs ~700MB)
- Full .NET 8 SDK support
- Faster pulls and builds

**Why separate scripts?**
- True parallel execution
- Independent failure domains
- Clearer agent orchestration

**Why timestamped directories?**
- Preserve all runs for analysis
- No overwriting
- Chronological sorting
- Easy comparison across runs

**Why dual logging?**
- Real-time monitoring (stdout/stderr)
- Persistent analysis (log files)
- Agent-friendly output parsing

## Support

For issues specific to this Docker automation, check:
1. This README
2. Build/test logs in timestamped directories
3. Docker container logs: `docker logs <container_id>`
4. GitHub Actions workflows in `.github/workflows/` for reference

## License

Same as parent project.
