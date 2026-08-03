"""Ephemeral SQL Server lifecycle helpers for local end-to-end validation.

This module is the implementation behind the `docker-database.py` CLI and is
imported by `docker-e2e.py`. It creates a throwaway SQL Server container the
backend can connect to, applies schema/seed SQL, and tears everything down
again.

Every resource created here carries the `themepark-e2e=1` Docker label, so a
crashed or interrupted run can always be cleaned up afterwards with
`prune_resources()` (`docker-database.py prune`). Resources that do NOT carry
the label are never touched — a hand-started `themepark-sql` container used for
manual development is therefore safe from these helpers.
"""

import os
import platform
import shutil
import subprocess
import sys
import tempfile
import time
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import docker_utils as du

LABEL_KEY = "themepark-e2e"
LABEL = f"{LABEL_KEY}=1"

DEFAULT_CONTAINER = os.environ.get("THEMEPARK_SQL_CONTAINER", "themepark-sql-e2e")
DEFAULT_IMAGE = os.environ.get("THEMEPARK_SQL_IMAGE", "mcr.microsoft.com/mssql/server:2022-latest")
DEFAULT_DATABASE = os.environ.get("THEMEPARK_SQL_DATABASE", "ThemePark")
DEFAULT_USER = "sa"
DEFAULT_PASSWORD = os.environ.get("THEMEPARK_SQL_PASSWORD", "LocalDev!Pass123")
DEFAULT_READY_TIMEOUT = int(os.environ.get("THEMEPARK_SQL_READY_TIMEOUT", "300"))

# Ordered by preference: mssql-tools18 (SQL Server 2022 images), mssql-tools
# (older images), then go-sqlcmd if it is on PATH inside the container.
SQLCMD_CANDIDATES = (
    "/opt/mssql-tools18/bin/sqlcmd",
    "/opt/mssql-tools/bin/sqlcmd",
    "/usr/bin/sqlcmd",
)

BASE_SCHEMA_DIR = "UCR.ECCI.PI.ThemePark.Database/Tables"


class DatabaseError(RuntimeError):
    """Raised when the ephemeral database cannot be started or driven."""


def _noop(_msg: str) -> None:
    pass


def docker(args: list[str], timeout: int | None = 120) -> subprocess.CompletedProcess:
    """Run a docker command, capturing stdout/stderr. Never raises on non-zero."""
    return subprocess.run(
        ["docker", *args],
        capture_output=True,
        text=True,
        timeout=timeout,
    )


def docker_available() -> tuple[bool, str]:
    if shutil.which("docker") is None:
        return False, "The 'docker' CLI was not found on PATH."
    result = docker(["info", "--format", "{{.ServerVersion}}"], timeout=60)
    if result.returncode != 0:
        return False, f"The Docker daemon is not reachable: {result.stderr.strip()}"
    return True, result.stdout.strip()


def container_exists(name: str) -> bool:
    result = docker(["container", "inspect", name, "--format", "{{.Name}}"])
    return result.returncode == 0


def container_running(name: str) -> bool:
    result = docker(["container", "inspect", name, "--format", "{{.State.Running}}"])
    return result.returncode == 0 and result.stdout.strip() == "true"


def container_is_ours(name: str) -> bool:
    """True when the container carries our label (i.e. we may remove it)."""
    result = docker(
        ["container", "inspect", name, "--format", "{{index .Config.Labels \"" + LABEL_KEY + "\"}}"]
    )
    return result.returncode == 0 and result.stdout.strip() == "1"


def container_logs(name: str, tail: int = 200) -> str:
    result = docker(["logs", "--tail", str(tail), name])
    return (result.stdout or "") + (result.stderr or "")


def remove_container(name: str, log=_noop) -> bool:
    """Force-remove a container. Returns True when something was removed."""
    if not container_exists(name):
        return False
    result = docker(["rm", "-f", "-v", name], timeout=180)
    if result.returncode != 0:
        log(f"WARNING: could not remove container '{name}': {result.stderr.strip()}")
        return False
    log(f"Removed container '{name}'.")
    return True


def create_network(name: str, log=_noop) -> bool:
    result = docker(["network", "inspect", name, "--format", "{{.Name}}"])
    if result.returncode == 0:
        return False
    created = docker(["network", "create", "--label", LABEL, name])
    if created.returncode != 0:
        raise DatabaseError(f"Could not create Docker network '{name}': {created.stderr.strip()}")
    log(f"Created Docker network '{name}'.")
    return True


def remove_network(name: str, log=_noop) -> bool:
    result = docker(["network", "inspect", name, "--format", "{{.Name}}"])
    if result.returncode != 0:
        return False
    removed = docker(["network", "rm", name])
    if removed.returncode != 0:
        log(f"WARNING: could not remove network '{name}': {removed.stderr.strip()}")
        return False
    log(f"Removed Docker network '{name}'.")
    return True


def create_volume(name: str, log=_noop) -> None:
    result = docker(["volume", "create", "--label", LABEL, name])
    if result.returncode != 0:
        raise DatabaseError(f"Could not create Docker volume '{name}': {result.stderr.strip()}")
    log(f"Created Docker volume '{name}'.")


def remove_volume(name: str, log=_noop) -> bool:
    result = docker(["volume", "inspect", name, "--format", "{{.Name}}"])
    if result.returncode != 0:
        return False
    removed = docker(["volume", "rm", "-f", name])
    if removed.returncode != 0:
        log(f"WARNING: could not remove volume '{name}': {removed.stderr.strip()}")
        return False
    log(f"Removed Docker volume '{name}'.")
    return True


def platform_args(image: str) -> list[str]:
    """SQL Server images are amd64-only; ask for emulation on arm64 hosts."""
    override = os.environ.get("THEMEPARK_SQL_PLATFORM")
    if override:
        return ["--platform", override]
    if "mssql/server" in image and platform.machine().lower() in ("arm64", "aarch64"):
        return ["--platform", "linux/amd64"]
    return []


def find_sqlcmd(container: str) -> str | None:
    for candidate in SQLCMD_CANDIDATES:
        result = docker(["exec", container, "test", "-x", candidate], timeout=60)
        if result.returncode == 0:
            return candidate
    return None


def start_database(
    container: str = DEFAULT_CONTAINER,
    network: str | None = None,
    image: str = DEFAULT_IMAGE,
    database: str = DEFAULT_DATABASE,
    password: str = DEFAULT_PASSWORD,
    host_port: int | None = None,
    ready_timeout: int = DEFAULT_READY_TIMEOUT,
    log=_noop,
) -> dict:
    """Start a fresh ephemeral SQL Server container and create the database.

    Any pre-existing container with the same name is removed first, but only if
    it carries our label — an unlabeled container of the same name means a human
    started it by hand, and we refuse rather than destroying their work.
    """
    ok, detail = docker_available()
    if not ok:
        raise DatabaseError(detail)

    if container_exists(container):
        if not container_is_ours(container):
            raise DatabaseError(
                f"A container named '{container}' already exists but was not created by this "
                f"automation (missing the '{LABEL_KEY}' label). Refusing to remove it. "
                f"Pass a different --name, or remove it yourself with: docker rm -f {container}"
            )
        log(f"Removing stale ephemeral container '{container}' from a previous run...")
        remove_container(container, log)

    if network:
        create_network(network, log)

    cmd = [
        "run", "-d",
        "--name", container,
        "--label", LABEL,
        *platform_args(image),
        "-e", "ACCEPT_EULA=Y",
        "-e", f"MSSQL_SA_PASSWORD={password}",
        "-e", f"SA_PASSWORD={password}",  # older image tags read this name
        "-e", "MSSQL_PID=Developer",
    ]
    if network:
        cmd.extend(["--network", network])
    if host_port:
        cmd.extend(["-p", f"{host_port}:1433"])
    cmd.append(image)

    log(f"Starting ephemeral database container '{container}' ({image})...")
    started = docker(cmd, timeout=600)
    if started.returncode != 0:
        raise DatabaseError(f"Could not start the database container: {started.stderr.strip()}")

    ready_seconds = wait_until_ready(container, password, ready_timeout, log)
    sqlcmd = find_sqlcmd(container)
    if not sqlcmd:
        raise DatabaseError(
            f"No sqlcmd binary found inside '{container}'. Tried: {', '.join(SQLCMD_CANDIDATES)}. "
            f"Set THEMEPARK_SQL_IMAGE to an image that ships the SQL Server command line tools."
        )

    ensure_database(container, database, password, log)

    info = {
        "container": container,
        "image": image,
        "network": network,
        "database": database,
        "user": DEFAULT_USER,
        "password": password,
        "sqlcmd": sqlcmd,
        "hostPort": host_port,
        "readySeconds": round(ready_seconds, 1),
    }
    log(f"Database '{database}' is ready after {info['readySeconds']}s.")
    return info


def wait_until_ready(container: str, password: str, timeout: int, log=_noop) -> float:
    """Block until SQL Server answers a trivial query, or raise on timeout."""
    started = time.monotonic()
    deadline = started + timeout
    sqlcmd = None
    last_error = "not attempted"
    log(f"Waiting up to {timeout}s for SQL Server to accept connections...")

    while time.monotonic() < deadline:
        if not container_running(container):
            raise DatabaseError(
                f"The database container '{container}' exited while starting up.\n"
                f"--- container logs ---\n{container_logs(container)}"
            )
        sqlcmd = sqlcmd or find_sqlcmd(container)
        if sqlcmd:
            probe = docker(
                ["exec", container, sqlcmd, "-S", "localhost", "-U", DEFAULT_USER,
                 "-P", password, "-C", "-l", "5", "-b", "-Q", "SELECT 1"],
                timeout=60,
            )
            if probe.returncode == 0:
                return time.monotonic() - started
            last_error = (probe.stderr or probe.stdout or "").strip().splitlines()[-1:] or [""]
            last_error = last_error[0]
        time.sleep(3)

    raise DatabaseError(
        f"SQL Server in '{container}' was not ready within {timeout}s. Last error: {last_error}\n"
        f"--- container logs ---\n{container_logs(container)}"
    )


def run_sql(
    container: str,
    sql: str,
    database: str | None = None,
    password: str = DEFAULT_PASSWORD,
    sqlcmd: str | None = None,
    timeout: int = 300,
) -> subprocess.CompletedProcess:
    """Execute a SQL script inside the container.

    The script is copied into the container and run with `sqlcmd -i` so that
    multi-batch scripts (statements separated by `GO`) work as written.
    """
    sqlcmd = sqlcmd or find_sqlcmd(container)
    if not sqlcmd:
        raise DatabaseError(f"No sqlcmd binary found inside container '{container}'.")

    tmp = tempfile.NamedTemporaryFile("w", suffix=".sql", delete=False, encoding="utf-8", newline="\n")
    try:
        tmp.write(sql if sql.endswith("\n") else sql + "\n")
        tmp.close()
        # docker cp preserves the host mode, and SQL Server images run sqlcmd as the
        # unprivileged `mssql` user — the default 0600 temp file would be unreadable.
        os.chmod(tmp.name, 0o644)
        in_container = f"/tmp/{Path(tmp.name).name}"
        copied = docker(["cp", tmp.name, f"{container}:{in_container}"], timeout=120)
        if copied.returncode != 0:
            raise DatabaseError(f"Could not copy SQL into '{container}': {copied.stderr.strip()}")

        cmd = ["exec", container, sqlcmd, "-S", "localhost", "-U", DEFAULT_USER,
               "-P", password, "-C", "-b", "-i", in_container]
        if database:
            cmd.extend(["-d", database])
        result = docker(cmd, timeout=timeout)
        docker(["exec", container, "rm", "-f", in_container], timeout=60)
        return result
    finally:
        Path(tmp.name).unlink(missing_ok=True)


def ensure_database(container: str, database: str, password: str = DEFAULT_PASSWORD, log=_noop) -> None:
    sql = f"IF DB_ID('{database}') IS NULL CREATE DATABASE [{database}];"
    result = run_sql(container, sql, database=None, password=password)
    if result.returncode != 0:
        raise DatabaseError(
            f"Could not create database '{database}': {(result.stderr or result.stdout).strip()}"
        )
    log(f"Database '{database}' created (or already present).")


def read_sql_file(path: Path) -> str:
    # utf-8-sig strips the BOM that the .sqlproj table scripts carry.
    return path.read_text(encoding="utf-8-sig")


def base_schema_files(workspace_root: Path | None = None) -> list[Path]:
    root = workspace_root or du.get_workspace_root()
    schema_dir = root / BASE_SCHEMA_DIR
    if not schema_dir.is_dir():
        return []
    return sorted(schema_dir.glob("*.sql"))


def apply_sql_files(
    container: str,
    files: list[Path],
    database: str = DEFAULT_DATABASE,
    password: str = DEFAULT_PASSWORD,
    sqlcmd: str | None = None,
    log=_noop,
) -> tuple[list[str], list[dict]]:
    """Apply SQL files, retrying failures once after the others have run.

    The retry pass exists because the checked-in table scripts have no declared
    order and can reference each other via foreign keys. Returns
    (applied_relative_paths, failures).
    """
    applied: list[str] = []
    pending = list(files)

    for attempt in (1, 2):
        failures: list[dict] = []
        for path in pending:
            result = run_sql(
                container, read_sql_file(path), database=database, password=password, sqlcmd=sqlcmd
            )
            label = path.name
            if result.returncode == 0:
                applied.append(label)
                log(f"  applied {label}")
            else:
                message = (result.stderr or result.stdout or "").strip()
                failures.append({"file": label, "error": message[-800:]})
                if attempt == 2:
                    log(f"  FAILED {label}: {message.splitlines()[-1] if message else 'unknown error'}")
        pending = [p for p in pending if p.name in {f["file"] for f in failures}]
        if not pending:
            return applied, []

    return applied, failures


def connection_string(host: str, port: int, database: str, password: str) -> str:
    return (
        f"Server={host},{port};Database={database};User Id={DEFAULT_USER};"
        f"Password={password};TrustServerCertificate=True;Encrypt=True;"
    )


def labeled_resources() -> dict[str, list[str]]:
    def ids(kind: str) -> list[str]:
        result = docker([kind, "ls", "-q", "--filter", f"label={LABEL}"])
        if result.returncode != 0:
            return []
        return [line.strip() for line in result.stdout.splitlines() if line.strip()]

    containers = docker(["ps", "-a", "-q", "--filter", f"label={LABEL}"])
    return {
        "containers": [c.strip() for c in containers.stdout.splitlines() if c.strip()],
        "networks": ids("network"),
        "volumes": ids("volume"),
    }


def prune_resources(log=_noop) -> dict[str, list[str]]:
    """Remove every container, network, and volume this automation created."""
    removed: dict[str, list[str]] = {"containers": [], "networks": [], "volumes": []}
    found = labeled_resources()

    for cid in found["containers"]:
        name = docker(["container", "inspect", cid, "--format", "{{.Name}}"]).stdout.strip().lstrip("/")
        if docker(["rm", "-f", "-v", cid], timeout=180).returncode == 0:
            removed["containers"].append(name or cid)
            log(f"Removed container '{name or cid}'.")
    for nid in found["networks"]:
        name = docker(["network", "inspect", nid, "--format", "{{.Name}}"]).stdout.strip()
        if docker(["network", "rm", nid]).returncode == 0:
            removed["networks"].append(name or nid)
            log(f"Removed network '{name or nid}'.")
    for vid in found["volumes"]:
        if docker(["volume", "rm", "-f", vid]).returncode == 0:
            removed["volumes"].append(vid)
            log(f"Removed volume '{vid}'.")

    return removed
