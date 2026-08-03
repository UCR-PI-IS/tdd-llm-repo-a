#!/usr/bin/env python3
"""End-to-end self-validation for the Theme Park backend.

Proves that the code actually runs against a real database, not just that it
compiles and its unit tests pass. One command does the whole round trip:

  1. start an ephemeral SQL Server container (nothing is reused, nothing persists)
  2. apply the checked-in table scripts and the story's sample data
  3. publish the backend and run it in a container wired to that database
  4. probe the HTTP endpoints the user story cares about
  5. stop the backend, delete the database container, network, and volume

Step 5 always runs — on success, on failure, on Ctrl-C, and on an unexpected
exception — so no container or process is ever left behind.

Usage: ./docker-e2e.py <STORY-ID> <MODEL> <ITERATION> [options]

Options:
  --seed FILE           sample-data SQL file to apply (repeatable)
  --seed-sql "SQL"      inline sample-data SQL (repeatable)
  --probe "GET /path [STATUS]"   HTTP check to run (repeatable, default status 200)
  --probes FILE         JSON file with richer probe definitions (see below)
  --ready-path PATH     readiness endpoint (default: /swagger/v1/swagger.json)
  --ready-timeout SEC   how long to wait for the backend to answer (default: 120)
  --db-timeout SEC      how long to wait for SQL Server (default: 300)
  --no-base-schema      skip UCR.ECCI.PI.ThemePark.Database/Tables/*.sql
  --require-green       refuse to run unless the latest build and test summaries for this
                        story/model/iteration are both green (use this in the TDD pipeline)
  --keep-up             DEBUG ONLY: leave containers running after the run

Probe JSON format:
  {"probes": [{"name": "list spaces", "method": "GET", "path": "/LearningSpaceList",
               "expectStatus": 200, "expectBodyContains": ["IF-0103"],
               "body": {...}, "headers": {"X-Foo": "bar"}}]}

Example:
  ./docker-e2e.py SQL-LS-001-007 Kimi-K2.5 1 \
      --seed /tmp/seed-learningspace.sql \
      --probe "GET /LearningSpaceList 200"

Output: E2EResults/<STORY-ID>/<MODEL>/<ITERATION>/<timestamp>/
  e2e.log, publish.log, api.log, e2e-summary.json, seeds/

Exit codes: 0 = validated, 1 = validation failure, 2 = environment problem.
"""

import argparse
import atexit
import json
import re
import signal
import sys
import time
import urllib.error
import urllib.request
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import docker_database as db
import docker_utils as du

IMAGE = "themepark-dotnet-sdk"
API_CONTAINER_PORT = 8080
DEFAULT_READY_PATH = "/swagger/v1/swagger.json"

PUBLISH_SCRIPT = """\
#!/bin/bash
set -e

echo "=== Publishing Backend API ==="
echo "Project: {csproj}"
echo ""

dotnet restore "{csproj}"
dotnet publish "{csproj}" -c Release --no-restore -o /app-out
echo ""

for runtimeconfig in /app-out/*.runtimeconfig.json; do
    base=$(basename "$runtimeconfig" .runtimeconfig.json)
    echo "PUBLISHED_DLL=${{base}}.dll"
done

echo "=== Publish Complete ==="
"""

ERROR_LINE_PATTERN = re.compile(
    r"(^fail:|^crit:|Unhandled exception|An unhandled exception|System\.[A-Za-z.]*Exception)",
    re.MULTILINE,
)


class RunLog:
    """Writes every line to stdout and to e2e.log."""

    def __init__(self, path: Path):
        self._file = path.open("w", encoding="utf-8")

    def __call__(self, message: str = "") -> None:
        text = str(message)
        print(text)
        self._file.write(text + "\n")
        self._file.flush()

    def close(self) -> None:
        self._file.close()


class Resources:
    """Everything this run created, plus the single idempotent teardown."""

    def __init__(self, slug: str):
        self.network = f"themepark-e2e-{slug}"
        self.sql_container = f"themepark-sql-e2e-{slug}"
        self.api_container = f"themepark-api-e2e-{slug}"
        self.app_volume = f"themepark-app-e2e-{slug}"
        self.cleaned = False
        self.report: dict = {
            "status": "not-run",
            "removedContainers": [],
            "removedNetworks": [],
            "removedVolumes": [],
            "leftovers": [],
        }

    def cleanup(self, log=print, keep_up: bool = False) -> dict:
        if self.cleaned:
            return self.report
        self.cleaned = True

        if keep_up:
            log("")
            du.cprint("! --keep-up: containers were LEFT RUNNING. Remove them with:", "YELLOW")
            log("    python Automations/docker-database.py prune")
            self.report["status"] = "skipped-by-flag"
            self.report["leftovers"] = [self.api_container, self.sql_container, self.network]
            return self.report

        log("")
        log(">>> Tearing down ephemeral environment...")
        for container in (self.api_container, self.sql_container):
            if db.remove_container(container, log):
                self.report["removedContainers"].append(container)
        if db.remove_network(self.network, log):
            self.report["removedNetworks"].append(self.network)
        if db.remove_volume(self.app_volume, log):
            self.report["removedVolumes"].append(self.app_volume)

        leftovers = [name for name in (self.api_container, self.sql_container)
                     if db.container_exists(name)]
        self.report["leftovers"] = leftovers
        self.report["status"] = "clean" if not leftovers else "leftovers"
        if leftovers:
            du.cprint(f"✗ Could not remove: {', '.join(leftovers)}", "RED")
            du.cprint("  Run: python Automations/docker-database.py prune", "RED")
        else:
            log("Teardown complete — no containers, networks, or volumes left behind.")
        return self.report


def parse_probe_shorthand(value: str) -> dict:
    parts = value.split()
    if len(parts) < 2:
        raise argparse.ArgumentTypeError(
            f"--probe expects \"METHOD /path [STATUS]\", got: {value!r}"
        )
    method, path = parts[0].upper(), parts[1]
    expect_status = int(parts[2]) if len(parts) > 2 else 200
    return {"name": f"{method} {path}", "method": method, "path": path,
            "expectStatus": expect_status, "expectBodyContains": []}


def load_probe_file(path: Path) -> tuple[list[dict], str | None]:
    data = json.loads(path.read_text(encoding="utf-8-sig"))
    if isinstance(data, list):
        return [normalize_probe(p) for p in data], None
    probes = [normalize_probe(p) for p in data.get("probes", [])]
    return probes, data.get("readyPath")


def normalize_probe(raw: dict) -> dict:
    contains = raw.get("expectBodyContains") or []
    if isinstance(contains, str):
        contains = [contains]
    method = str(raw.get("method", "GET")).upper()
    path = raw.get("path", "/")
    return {
        "name": raw.get("name") or f"{method} {path}",
        "method": method,
        "path": path,
        "expectStatus": int(raw.get("expectStatus", 200)),
        "expectBodyContains": list(contains),
        "body": raw.get("body"),
        "headers": raw.get("headers") or {},
    }


def http_request(base_url: str, probe: dict, timeout: int = 30) -> tuple[int | None, str, str | None]:
    """Return (status, body, error). status is None when no response arrived."""
    url = base_url.rstrip("/") + "/" + str(probe["path"]).lstrip("/")
    data = None
    headers = dict(probe.get("headers") or {})
    body = probe.get("body")
    if body is not None:
        if isinstance(body, (dict, list)):
            data = json.dumps(body).encode("utf-8")
            headers.setdefault("Content-Type", "application/json")
        else:
            data = str(body).encode("utf-8")
    request = urllib.request.Request(url, data=data, headers=headers, method=probe["method"])
    try:
        with urllib.request.urlopen(request, timeout=timeout) as response:
            return response.status, response.read().decode("utf-8", errors="replace"), None
    except urllib.error.HTTPError as error:
        return error.code, error.read().decode("utf-8", errors="replace"), None
    except Exception as error:  # connection refused, timeout, DNS, ...
        return None, "", f"{type(error).__name__}: {error}"


def wait_for_backend(base_url: str, ready_path: str, container: str, timeout: int, log) -> float:
    started = time.monotonic()
    deadline = started + timeout
    last = "no attempt yet"
    log(f"Waiting up to {timeout}s for the backend at {base_url}{ready_path} ...")

    while time.monotonic() < deadline:
        if not db.container_running(container):
            raise RuntimeError(
                f"The backend container exited during startup.\n"
                f"--- backend logs ---\n{db.container_logs(container)}"
            )
        status, _, error = http_request(base_url, {"method": "GET", "path": ready_path}, timeout=10)
        if status is not None and status < 500:
            return time.monotonic() - started
        last = error or f"HTTP {status}"
        time.sleep(2)

    raise RuntimeError(
        f"The backend did not become ready within {timeout}s (last: {last}).\n"
        f"--- backend logs ---\n{db.container_logs(container)}"
    )


def latest_summary(workspace: Path, kind: str, story_id: str, model: str,
                   iteration: str, filename: str) -> dict | None:
    """Newest parsable summary of the given kind for this story/model/iteration."""
    base = workspace / kind / story_id / model / iteration
    if not base.is_dir():
        return None
    for directory in sorted((d for d in base.iterdir() if d.is_dir()), reverse=True):
        candidate = directory / filename
        if candidate.is_file():
            try:
                return json.loads(candidate.read_text())
            except json.JSONDecodeError:
                continue
    return None


def green_gate_failures(workspace: Path, story_id: str, model: str, iteration: str) -> list[str]:
    """Reasons this iteration is not ready for end-to-end validation."""
    reasons: list[str] = []

    build = latest_summary(workspace, "BuildResults", story_id, model, iteration, "build-summary.json")
    if build is None:
        reasons.append(
            f"No build-summary.json under BuildResults/{story_id}/{model}/{iteration}/ — "
            f"run docker-build.py first."
        )
    elif build.get("status") != "success" or build.get("totalErrors", 0) > 0:
        reasons.append(
            f"The latest build is not green (status={build.get('status')}, "
            f"totalErrors={build.get('totalErrors')})."
        )

    tests = latest_summary(workspace, "TestResults", story_id, model, iteration, "test-summary.json")
    if tests is None:
        reasons.append(
            f"No test-summary.json under TestResults/{story_id}/{model}/{iteration}/ — "
            f"run docker-test.py first."
        )
    elif tests.get("status") != "success" or tests.get("totalFailed", 0) > 0:
        reasons.append(
            f"The latest test run is not green (status={tests.get('status')}, "
            f"totalFailed={tests.get('totalFailed')})."
        )

    return reasons


def find_api_csproj(workspace_root: Path) -> Path:
    candidates = sorted((workspace_root / "Backend.Api").glob("*.csproj"))
    if not candidates:
        raise RuntimeError("No .csproj found in Backend.Api/ — cannot run the backend.")
    return candidates[0]


def resolve_host_port(container: str) -> int:
    result = db.docker(["port", container, f"{API_CONTAINER_PORT}/tcp"])
    if result.returncode != 0 or not result.stdout.strip():
        raise RuntimeError(f"Could not resolve the published port of '{container}'.")
    first = result.stdout.strip().splitlines()[0]
    return int(first.rsplit(":", 1)[1])


def build_parser() -> argparse.ArgumentParser:
    parser = argparse.ArgumentParser(
        description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter
    )
    parser.add_argument("story_id")
    parser.add_argument("model")
    parser.add_argument("iteration")
    parser.add_argument("--seed", action="append", default=[], help="sample-data SQL file (repeatable)")
    parser.add_argument("--seed-sql", action="append", default=[], help="inline sample-data SQL (repeatable)")
    parser.add_argument("--probe", action="append", default=[], help='"METHOD /path [STATUS]" (repeatable)')
    parser.add_argument("--probes", help="JSON file with probe definitions")
    parser.add_argument("--ready-path", default=None, help=f"readiness path (default: {DEFAULT_READY_PATH})")
    parser.add_argument("--ready-timeout", type=int, default=120)
    parser.add_argument("--db-timeout", type=int, default=db.DEFAULT_READY_TIMEOUT)
    parser.add_argument("--no-base-schema", action="store_true")
    parser.add_argument("--require-green", action="store_true",
                        help="refuse to run unless the latest build and test summaries for this "
                             "story/model/iteration are both green")
    parser.add_argument("--keep-up", action="store_true",
                        help="DEBUG ONLY: leave the environment running (never use in the TDD pipeline)")
    return parser


def main() -> None:
    args = build_parser().parse_args()

    story_id = du.sanitize_path_component(args.story_id)
    model = du.sanitize_path_component(args.model)
    iteration = du.sanitize_path_component(args.iteration)
    timestamp = du.generate_timestamp()

    workspace = du.get_workspace_root()
    output_dir = Path("E2EResults") / story_id / model / iteration / timestamp
    (workspace / output_dir).mkdir(parents=True, exist_ok=True)

    slug = re.sub(r"[^a-z0-9]+", "-", f"{story_id}-{model}-{iteration}".lower()).strip("-")[:40]
    slug = f"{slug}-{timestamp.replace('_', '').replace('-', '')[-6:]}"
    resources = Resources(slug)

    log = RunLog(workspace / output_dir / "e2e.log")
    du.print_banner("Docker End-to-End Validation", timestamp, str(output_dir))
    log(f"Story ID: {story_id}")
    log(f"Model: {model}")
    log(f"Iteration: {iteration}")
    log("")

    summary = {
        "stage": "e2e-validation",
        "status": "failure",
        "timestamp": timestamp,
        "storyId": story_id,
        "model": model,
        "iteration": iteration,
        "database": {},
        "backend": {},
        "probes": [],
        "totalProbes": 0,
        "probesPassed": 0,
        "probesFailed": 0,
        "cleanup": {},
        "warnings": [],
        "errors": [],
    }

    # Teardown must survive every exit path: normal return, exception, SIGINT,
    # SIGTERM, and even os._exit-adjacent interpreter shutdown.
    atexit.register(lambda: resources.cleanup(log=print, keep_up=args.keep_up))
    signal.signal(signal.SIGINT, lambda *_: sys.exit(130))
    signal.signal(signal.SIGTERM, lambda *_: sys.exit(143))

    exit_code = 1
    try:
        exit_code = run_validation(args, workspace, output_dir, resources, summary, log)
    except KeyboardInterrupt:
        summary["errors"].append("Interrupted by user.")
        du.cprint("Interrupted — cleaning up.", "YELLOW")
        exit_code = 130
    except SystemExit as error:
        summary["errors"].append(f"Terminated with signal exit {error.code}.")
        exit_code = int(error.code or 1)
    except db.DatabaseError as error:
        summary["errors"].append(str(error))
        log(f"✗ Database error: {error}")
        exit_code = 2
    except Exception as error:  # noqa: BLE001 - the summary must record anything that goes wrong
        summary["errors"].append(f"{type(error).__name__}: {error}")
        log(f"✗ {type(error).__name__}: {error}")
        exit_code = 1
    finally:
        summary["cleanup"] = resources.cleanup(log=log, keep_up=args.keep_up)
        if summary["cleanup"]["status"] == "leftovers":
            summary["errors"].append(
                f"Teardown left resources behind: {', '.join(summary['cleanup']['leftovers'])}"
            )
            exit_code = exit_code or 1
        summary["status"] = "success" if exit_code == 0 else "failure"
        (workspace / output_dir / "e2e-summary.json").write_text(json.dumps(summary, indent=2))
        du.print_result(exit_code == 0, str(output_dir), "e2e.log")
        log.close()

    sys.exit(exit_code)


def run_validation(args, workspace: Path, output_dir: Path, resources: Resources,
                   summary: dict, log: RunLog) -> int:
    if args.require_green:
        reasons = green_gate_failures(
            workspace, summary["storyId"], summary["model"], summary["iteration"]
        )
        if reasons:
            log("✗ Refusing to run: the code is not green yet.")
            for reason in reasons:
                log(f"    {reason}")
            summary["errors"].extend(reasons)
            return 2
        log("Green gate passed: the latest build and test runs for this iteration are both green.")

    available, detail = db.docker_available()
    if not available:
        raise db.DatabaseError(detail)
    log(f"Docker server: {detail}")

    # --- 1. Ephemeral database -------------------------------------------------
    log("")
    log(">>> [1/5] Starting the ephemeral database...")
    info = db.start_database(
        container=resources.sql_container,
        network=resources.network,
        ready_timeout=args.db_timeout,
        log=log,
    )
    summary["database"] = {
        "container": info["container"],
        "image": info["image"],
        "network": info["network"],
        "database": info["database"],
        "readySeconds": info["readySeconds"],
        "baseSchemaApplied": [],
        "seedsApplied": [],
    }

    # --- 2. Schema + story sample data ----------------------------------------
    log("")
    log(">>> [2/5] Applying schema and sample data...")
    if not args.no_base_schema:
        schema_files = db.base_schema_files(workspace)
        if schema_files:
            applied, failures = db.apply_sql_files(
                resources.sql_container, schema_files, info["database"],
                info["password"], info["sqlcmd"], log=log,
            )
            summary["database"]["baseSchemaApplied"] = applied
            for failure in failures:
                summary["warnings"].append(f"base schema {failure['file']}: {failure['error']}")
        else:
            log(f"  no base schema files in {db.BASE_SCHEMA_DIR}/")

    seeds_dir = workspace / output_dir / "seeds"
    seed_sources: list[tuple[str, str]] = []
    for seed_path in args.seed:
        path = Path(seed_path)
        if not path.is_file():
            raise RuntimeError(f"Seed file not found: {seed_path}")
        seed_sources.append((path.name, db.read_sql_file(path)))
    for index, inline in enumerate(args.seed_sql, start=1):
        seed_sources.append((f"inline-{index}.sql", inline))

    if not seed_sources:
        message = ("No sample data supplied (--seed/--seed-sql). The backend was validated against "
                   "an empty database, which cannot prove the story's read paths work.")
        log(f"  ! {message}")
        summary["warnings"].append(message)
    else:
        seeds_dir.mkdir(parents=True, exist_ok=True)
        for name, sql in seed_sources:
            (seeds_dir / name).write_text(sql, encoding="utf-8")
            result = db.run_sql(
                resources.sql_container, sql, info["database"], info["password"], info["sqlcmd"]
            )
            if result.returncode != 0:
                message = (result.stderr or result.stdout or "").strip()
                log(f"  FAILED {name}: {message}")
                summary["errors"].append(f"seed {name}: {message[-800:]}")
                return 1
            summary["database"]["seedsApplied"].append(name)
            log(f"  seeded {name}")
            if result.stdout.strip():
                log("    " + result.stdout.strip().replace("\n", "\n    "))

    # --- 3. Publish the backend ------------------------------------------------
    log("")
    log(">>> [3/5] Publishing the backend...")
    csproj = find_api_csproj(workspace)
    relative_csproj = csproj.relative_to(workspace).as_posix()
    db.create_volume(resources.app_volume, log)
    du.write_inner_script(
        workspace / output_dir, "publish-script.sh", PUBLISH_SCRIPT.format(csproj=relative_csproj)
    )
    publish_code = du.run_docker_container(
        IMAGE, workspace, output_dir, "/output/publish-script.sh", "publish.log",
        extra_volumes=[f"{resources.app_volume}:/app-out"],
    )
    publish_log = (workspace / output_dir / "publish.log").read_text(errors="replace")
    summary["backend"] = {
        "container": resources.api_container,
        "project": relative_csproj,
        "publishStatus": "success" if publish_code == 0 else "failure",
        "started": False,
        "url": None,
        "readySeconds": None,
        "errorLines": [],
    }
    if publish_code != 0:
        summary["errors"].append(
            f"dotnet publish failed (exit {publish_code}); see {output_dir}/publish.log"
        )
        log(f"✗ Publish failed — see {output_dir}/publish.log")
        return 1

    dll_matches = re.findall(r"^PUBLISHED_DLL=(.+)$", publish_log, re.MULTILINE)
    if not dll_matches:
        raise RuntimeError("Publish succeeded but no runnable assembly was found in the output.")
    dll = dll_matches[0].strip()
    log(f"Published assembly: {dll}")

    # --- 4. Run the backend and probe it ---------------------------------------
    log("")
    log(">>> [4/5] Starting the backend against the ephemeral database...")
    connection = db.connection_string(
        resources.sql_container, 1433, info["database"], info["password"]
    )
    run_cmd = [
        "run", "-d",
        "--name", resources.api_container,
        "--label", db.LABEL,
        "--network", resources.network,
        "-p", f"0:{API_CONTAINER_PORT}",
        "-v", f"{resources.app_volume}:/app:ro",
        "-e", "ASPNETCORE_ENVIRONMENT=Development",
        "-e", f"ASPNETCORE_URLS=http://0.0.0.0:{API_CONTAINER_PORT}",
        "-e", f"ConnectionStrings__DefaultConnection={connection}",
        "-w", "/app",
        IMAGE, "dotnet", f"/app/{dll}",
    ]
    started = db.docker(run_cmd, timeout=300)
    if started.returncode != 0:
        raise RuntimeError(f"Could not start the backend container: {started.stderr.strip()}")

    host_port = resolve_host_port(resources.api_container)
    base_url = f"http://127.0.0.1:{host_port}"
    summary["backend"]["started"] = True
    summary["backend"]["url"] = base_url
    log(f"Backend container '{resources.api_container}' listening on {base_url}")

    probes: list[dict] = [parse_probe_shorthand(p) for p in args.probe]
    ready_path = args.ready_path
    if args.probes:
        file_probes, file_ready = load_probe_file(Path(args.probes))
        probes.extend(file_probes)
        ready_path = ready_path or file_ready
    ready_path = ready_path or DEFAULT_READY_PATH

    ready_seconds = wait_for_backend(
        base_url, ready_path, resources.api_container, args.ready_timeout, log
    )
    summary["backend"]["readySeconds"] = round(ready_seconds, 1)
    log(f"Backend is answering requests after {summary['backend']['readySeconds']}s.")

    log("")
    log(">>> [5/5] Running endpoint probes...")
    if not probes:
        message = ("No probes supplied (--probe/--probes). Only backend startup was verified, "
                   "not the story's endpoints.")
        log(f"  ! {message}")
        summary["warnings"].append(message)

    for probe in probes:
        status, body, error = http_request(base_url, probe)
        missing = [needle for needle in probe["expectBodyContains"] if needle not in body]
        passed = error is None and status == probe["expectStatus"] and not missing
        record = {
            "name": probe["name"],
            "method": probe["method"],
            "path": probe["path"],
            "expectedStatus": probe["expectStatus"],
            "actualStatus": status,
            "passed": passed,
            "missingContent": missing,
            "error": error,
            "bodySnippet": body[:500],
        }
        summary["probes"].append(record)
        if passed:
            log(f"  ✓ {probe['name']} → {status}")
        else:
            reason = error or (f"expected {probe['expectStatus']}, got {status}"
                               if status != probe["expectStatus"]
                               else f"body missing {missing}")
            log(f"  ✗ {probe['name']} → {reason}")
            log(f"    body: {body[:300]}")

    summary["totalProbes"] = len(summary["probes"])
    summary["probesPassed"] = sum(1 for p in summary["probes"] if p["passed"])
    summary["probesFailed"] = summary["totalProbes"] - summary["probesPassed"]

    api_logs = db.container_logs(resources.api_container, tail=500)
    (workspace / output_dir / "api.log").write_text(api_logs, encoding="utf-8")
    error_lines = [line.strip() for line in api_logs.splitlines() if ERROR_LINE_PATTERN.search(line)]
    summary["backend"]["errorLines"] = error_lines[:50]
    if error_lines:
        log("")
        log(f"! The backend logged {len(error_lines)} error line(s) — see {output_dir}/api.log")
        for line in error_lines[:5]:
            log(f"    {line}")

    if not db.container_running(resources.api_container):
        summary["errors"].append("The backend container exited before the run finished.")
        return 1
    if summary["probesFailed"] > 0:
        summary["errors"].append(f"{summary['probesFailed']} probe(s) failed.")
        return 1
    # A probe that deliberately asks for a 4xx/5xx makes server-side error logging
    # expected, so it is only a warning there; otherwise it is a real defect.
    if error_lines:
        if any(probe["expectStatus"] >= 400 for probe in probes):
            summary["warnings"].append(
                f"The backend logged {len(error_lines)} error line(s); a probe expected an error "
                f"response, so this was not treated as a failure. See api.log."
            )
        else:
            summary["errors"].append(
                f"The backend logged {len(error_lines)} error line(s) while serving probes."
            )
            return 1
    return 0


if __name__ == "__main__":
    main()
