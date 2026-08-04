#!/usr/bin/env bash
#
# dev-database.sh
# ---------------
# Make sure the local development SQL Server container is running and reachable,
# creating it from scratch when it does not exist yet.
#
# This is what the "db: start SQL Server (Docker)" VS Code task runs. A plain
# `docker start themepark-sql` fails with "No such container" the first time, or
# any time the container has been removed (`docker rm`, `docker system prune`, a
# Docker Desktop reset). This script covers every state instead:
#
#   missing  -> create the container, wait for SQL Server, create the database
#   stopped  -> start the container, wait for SQL Server
#   running  -> verify it answers queries, then do nothing
#
# The container it manages is the long-lived, hand-run development database. It
# is NOT one of the throwaway containers created by docker-database.py /
# docker-e2e.py: those carry the `themepark-e2e=1` label and are pruned between
# runs, while this one carries `themepark-dev=1` and survives.
#
# Usage:
#   ./dev-database.sh                 # same as `up`
#   ./dev-database.sh up              # ensure the container + database are ready
#   ./dev-database.sh status          # report the current state, change nothing
#   ./dev-database.sh down            # stop and remove the container
#   ./dev-database.sh recreate        # remove it, then build a fresh one
#
# Options:
#   --port <n>        Host port to publish (default: 1433)
#   --timeout <sec>   How long to wait for SQL Server (default: 300)
#   --no-schema       Do not apply the table scripts to a newly created database
#   -h, --help        Show this help
#
# Environment overrides (defaults match Backend.Api/appsettings.Development.json):
#   SQL_CONTAINER   Docker container name   (default: themepark-sql)
#   SQL_IMAGE       SQL Server image        (default: mcr.microsoft.com/mssql/server:2022-latest)
#   SQL_DATABASE    Database to create      (default: ThemePark)
#   SQL_USER        SQL login               (default: sa)
#   SQL_PASSWORD    SQL password            (default: LocalDev!Pass123)
#   SQL_PORT        Published host port     (default: 1433)
#   SQL_PLATFORM    Docker platform         (default: linux/amd64 on Apple Silicon)
#   SQL_READY_TIMEOUT  Readiness timeout in seconds (default: 300)
#
set -euo pipefail

# ---------------------------------------------------------------------------
# Configuration
# ---------------------------------------------------------------------------
SQL_CONTAINER="${SQL_CONTAINER:-themepark-sql}"
SQL_IMAGE="${SQL_IMAGE:-mcr.microsoft.com/mssql/server:2022-latest}"
SQL_DATABASE="${SQL_DATABASE:-ThemePark}"
SQL_USER="${SQL_USER:-sa}"
SQL_PASSWORD="${SQL_PASSWORD:-LocalDev!Pass123}"
SQL_PORT="${SQL_PORT:-1433}"
SQL_READY_TIMEOUT="${SQL_READY_TIMEOUT:-300}"
SCHEMA_DIR="${SQL_SCHEMA_DIR:-UCR.ECCI.PI.ThemePark.Database/Tables}"

DEV_LABEL_KEY="themepark-dev"
E2E_LABEL_KEY="themepark-e2e"

WORKSPACE_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

# Ordered by preference: mssql-tools18 (SQL Server 2022 images), mssql-tools
# (older tags), then go-sqlcmd if it happens to be on PATH in the container.
SQLCMD_CANDIDATES=(
  /opt/mssql-tools18/bin/sqlcmd
  /opt/mssql-tools/bin/sqlcmd
  /usr/bin/sqlcmd
)
SQLCMD=""

APPLY_SCHEMA=1

# ---------------------------------------------------------------------------
# Output helpers
# ---------------------------------------------------------------------------
if [[ -t 1 ]]; then
  C_RED=$'\033[0;31m'; C_GREEN=$'\033[0;32m'; C_YELLOW=$'\033[1;33m'; C_OFF=$'\033[0m'
else
  C_RED=""; C_GREEN=""; C_YELLOW=""; C_OFF=""
fi

info() { echo "${C_YELLOW}>>${C_OFF} $*"; }
ok()   { echo "${C_GREEN}✓${C_OFF} $*"; }
warn() { echo "${C_YELLOW}WARNING:${C_OFF} $*" >&2; }
err()  { echo "${C_RED}✗${C_OFF} $*" >&2; }
die()  { err "$*"; exit 1; }

usage() {
  grep '^#' "$0" | sed 's/^# \{0,1\}//' | sed '1d'
  exit "${1:-1}"
}

# ---------------------------------------------------------------------------
# Argument parsing
# ---------------------------------------------------------------------------
COMMAND="up"
if [[ $# -gt 0 && "$1" != -* ]]; then
  COMMAND="$1"
  shift
fi

while [[ $# -gt 0 ]]; do
  case "$1" in
    --port)
      [[ -n "${2:-}" ]] || die "--port requires a value."
      SQL_PORT="$2"; shift 2 ;;
    --timeout)
      [[ -n "${2:-}" ]] || die "--timeout requires a value."
      SQL_READY_TIMEOUT="$2"; shift 2 ;;
    --no-schema)
      APPLY_SCHEMA=0; shift ;;
    -h|--help)
      usage 0 ;;
    *)
      err "Unknown argument '$1'."
      usage 1 ;;
  esac
done

case "$COMMAND" in
  up|status|down|recreate) ;;
  -h|--help) usage 0 ;;
  *) err "Unknown command '$COMMAND'."; usage 1 ;;
esac

# ---------------------------------------------------------------------------
# Docker plumbing
# ---------------------------------------------------------------------------
require_docker() {
  command -v docker >/dev/null 2>&1 \
    || die "The 'docker' CLI was not found on PATH. Install Docker Desktop and try again."
  docker info --format '{{.ServerVersion}}' >/dev/null 2>&1 \
    || die "The Docker daemon is not reachable. Start Docker Desktop and try again."
}

container_exists() {
  docker container inspect "$SQL_CONTAINER" >/dev/null 2>&1
}

container_running() {
  [[ "$(docker container inspect "$SQL_CONTAINER" --format '{{.State.Running}}' 2>/dev/null)" == "true" ]]
}

container_label() { # container_label <label-key>
  docker container inspect "$SQL_CONTAINER" \
    --format "{{index .Config.Labels \"$1\"}}" 2>/dev/null || true
}

container_published_port() {
  docker container inspect "$SQL_CONTAINER" \
    --format '{{range $p := index .NetworkSettings.Ports "1433/tcp"}}{{$p.HostPort}}{{end}}' 2>/dev/null || true
}

docker_platform_args() {
  if [[ -n "${SQL_PLATFORM:-}" ]]; then
    echo "--platform $SQL_PLATFORM"
    return
  fi
  # SQL Server images are amd64-only; ask for emulation on Apple Silicon.
  case "$(uname -m)" in
    arm64|aarch64) echo "--platform linux/amd64" ;;
    *) echo "" ;;
  esac
}

port_taken_by_other() {
  # Prints the name of a *different* container already publishing SQL_PORT.
  # The leading colon keeps ":1433->" from matching ":11433->".
  docker ps --format '{{.Names}}\t{{.Ports}}' 2>/dev/null \
    | grep -F ":${SQL_PORT}->" \
    | cut -f1 \
    | grep -vx "$SQL_CONTAINER" \
    | head -n 1 || true
}

find_sqlcmd() {
  local candidate
  for candidate in "${SQLCMD_CANDIDATES[@]}"; do
    if docker exec "$SQL_CONTAINER" test -x "$candidate" >/dev/null 2>&1; then
      SQLCMD="$candidate"
      return 0
    fi
  done
  return 1
}

# ---------------------------------------------------------------------------
# SQL helpers
# ---------------------------------------------------------------------------
sql_query() { # sql_query <sql> [database]
  local sql="$1" db="${2:-}"
  local args=(-S localhost -U "$SQL_USER" -P "$SQL_PASSWORD" -C -b -h -1 -W)
  if [[ -n "$db" ]]; then
    args+=(-d "$db")
  fi
  docker exec "$SQL_CONTAINER" "$SQLCMD" "${args[@]}" -Q "$sql"
}

apply_sql_file() { # apply_sql_file <host-path>
  local path="$1" name tmp in_container rc=0
  name="$(basename "$path")"
  tmp="$(mktemp "${TMPDIR:-/tmp}/themepark-schema.XXXXXX")"
  # The checked-in table scripts are UTF-8 with a BOM, which some sqlcmd builds
  # report as a syntax error on the first statement. Strip it before copying.
  sed $'1s/^\xEF\xBB\xBF//' "$path" > "$tmp"
  # sqlcmd runs as the unprivileged `mssql` user inside the image, and docker cp
  # preserves the host mode, so the default 0600 temp file would be unreadable.
  chmod 644 "$tmp"
  in_container="/tmp/themepark-$name"

  docker cp "$tmp" "$SQL_CONTAINER:$in_container" >/dev/null 2>&1 || rc=$?
  rm -f "$tmp"
  if [[ $rc -ne 0 ]]; then
    echo "could not copy $name into the container" >&2
    return $rc
  fi

  docker exec "$SQL_CONTAINER" "$SQLCMD" \
    -S localhost -U "$SQL_USER" -P "$SQL_PASSWORD" -C -b \
    -d "$SQL_DATABASE" -i "$in_container" || rc=$?
  docker exec "$SQL_CONTAINER" rm -f "$in_container" >/dev/null 2>&1 || true
  return $rc
}

wait_until_ready() {
  local elapsed=0 interval=3 next_notice=15
  info "Waiting up to ${SQL_READY_TIMEOUT}s for SQL Server to accept connections..."
  while [[ $elapsed -lt $SQL_READY_TIMEOUT ]]; do
    if ! container_running; then
      err "Container '$SQL_CONTAINER' stopped while starting up. Last log lines:"
      docker logs --tail 40 "$SQL_CONTAINER" >&2 || true
      return 1
    fi
    if [[ -z "$SQLCMD" ]]; then
      find_sqlcmd || true
    fi
    if [[ -n "$SQLCMD" ]] \
      && docker exec "$SQL_CONTAINER" "$SQLCMD" \
           -S localhost -U "$SQL_USER" -P "$SQL_PASSWORD" -C -l 5 -b \
           -Q "SELECT 1" >/dev/null 2>&1; then
      ok "SQL Server is accepting connections (${elapsed}s)."
      return 0
    fi
    sleep "$interval"
    elapsed=$((elapsed + interval))
    if [[ $elapsed -ge $next_notice ]]; then
      info "  ...still starting (${elapsed}s elapsed)"
      next_notice=$((next_notice + 15))
    fi
  done

  err "SQL Server in '$SQL_CONTAINER' was not ready within ${SQL_READY_TIMEOUT}s."
  if [[ -z "$SQLCMD" ]]; then
    err "No sqlcmd binary was found inside the container (tried: ${SQLCMD_CANDIDATES[*]})."
    err "Set SQL_IMAGE to an image that ships the SQL Server command line tools."
  else
    err "If the password was changed after the container was created, the existing"
    err "data directory still uses the old one — rebuild with: $0 recreate"
  fi
  err "Last log lines:"
  docker logs --tail 40 "$SQL_CONTAINER" >&2 || true
  return 1
}

database_exists() {
  local out
  out="$(sql_query "SET NOCOUNT ON; SELECT CASE WHEN DB_ID('$SQL_DATABASE') IS NULL THEN 0 ELSE 1 END" 2>/dev/null \
        | tr -d '[:space:]')"
  [[ "$out" == "1" ]]
}

create_database() {
  info "Creating database '$SQL_DATABASE'..."
  if ! sql_query "IF DB_ID('$SQL_DATABASE') IS NULL CREATE DATABASE [$SQL_DATABASE];" >/dev/null; then
    die "Could not create database '$SQL_DATABASE'."
  fi
  ok "Database '$SQL_DATABASE' created."
}

apply_base_schema() {
  local dir="$WORKSPACE_ROOT/$SCHEMA_DIR"
  if [[ ! -d "$dir" ]]; then
    info "No schema directory at $SCHEMA_DIR; skipping table scripts."
    return 0
  fi

  local files=() f
  for f in "$dir"/*.sql; do
    [[ -f "$f" ]] && files+=("$f")
  done
  if [[ ${#files[@]} -eq 0 ]]; then
    info "No table scripts in $SCHEMA_DIR; skipping."
    return 0
  fi

  info "Applying ${#files[@]} table script(s) from $SCHEMA_DIR..."
  local pending=("${files[@]}")
  local attempt out rc
  # Two passes: the table scripts have no declared order and reference each
  # other through foreign keys, so a failure can simply mean "not yet".
  for attempt in 1 2; do
    local failed=()
    for f in "${pending[@]}"; do
      rc=0
      out="$(apply_sql_file "$f" 2>&1)" || rc=$?
      if [[ $rc -eq 0 ]]; then
        echo "  applied $(basename "$f")"
      else
        failed+=("$f")
        if [[ $attempt -eq 2 ]]; then
          warn "could not apply $(basename "$f"): $(echo "$out" | grep -v '^$' | tail -n 1)"
        fi
      fi
    done
    if [[ ${#failed[@]} -eq 0 ]]; then
      ok "Schema applied."
      return 0
    fi
    pending=("${failed[@]}")
  done

  # A partial schema still lets the backend start, so this is a warning and not
  # a hard failure: the data-injection step creates what it needs anyway.
  warn "${#pending[@]} table script(s) could not be applied. The database is up but incomplete."
  return 0
}

# ---------------------------------------------------------------------------
# Container lifecycle
# ---------------------------------------------------------------------------
create_container() {
  local conflict
  conflict="$(port_taken_by_other)"
  if [[ -n "$conflict" ]]; then
    die "Host port $SQL_PORT is already published by container '$conflict'. Stop it, or pass --port <n> (and update Backend.Api/appsettings.Development.json)."
  fi

  if ! docker image inspect "$SQL_IMAGE" >/dev/null 2>&1; then
    info "Pulling $SQL_IMAGE (first run only, this takes a few minutes)..."
    docker pull $(docker_platform_args) "$SQL_IMAGE" \
      || die "Could not pull $SQL_IMAGE. Check your network connection."
  fi

  info "Creating container '$SQL_CONTAINER' from $SQL_IMAGE on port $SQL_PORT..."
  # shellcheck disable=SC2046  # platform args are intentionally word-split
  docker run -d \
    --name "$SQL_CONTAINER" \
    --label "$DEV_LABEL_KEY=1" \
    --restart unless-stopped \
    $(docker_platform_args) \
    -p "$SQL_PORT:1433" \
    -e "ACCEPT_EULA=Y" \
    -e "MSSQL_SA_PASSWORD=$SQL_PASSWORD" \
    -e "SA_PASSWORD=$SQL_PASSWORD" \
    -e "MSSQL_PID=Developer" \
    "$SQL_IMAGE" >/dev/null \
    || die "Could not create container '$SQL_CONTAINER'. See the Docker error above."
  ok "Container '$SQL_CONTAINER' created."
}

start_container() {
  info "Starting existing container '$SQL_CONTAINER'..."
  docker start "$SQL_CONTAINER" >/dev/null \
    || die "Could not start '$SQL_CONTAINER'. Rebuild it with: $0 recreate"
  ok "Container '$SQL_CONTAINER' started."
}

remove_container() {
  if ! container_exists; then
    info "No container named '$SQL_CONTAINER' to remove."
    return 0
  fi
  if [[ "$(container_label "$E2E_LABEL_KEY")" == "1" ]]; then
    die "Container '$SQL_CONTAINER' is labelled '$E2E_LABEL_KEY=1', so it belongs to an end-to-end run. Refusing to remove it. Use: python Automations/docker-database.py down"
  fi
  info "Removing container '$SQL_CONTAINER'..."
  docker rm -f -v "$SQL_CONTAINER" >/dev/null \
    || die "Could not remove container '$SQL_CONTAINER'."
  ok "Container '$SQL_CONTAINER' removed."
}

print_connection_string() {
  echo
  echo "Connection string (Backend.Api/appsettings.Development.json):"
  echo "  Server=localhost,$SQL_PORT;Database=$SQL_DATABASE;User Id=$SQL_USER;Password=$SQL_PASSWORD;TrustServerCertificate=True;Encrypt=True;"
}

cmd_up() {
  require_docker

  local created=0
  if ! container_exists; then
    create_container
    created=1
  elif ! container_running; then
    start_container
  else
    info "Container '$SQL_CONTAINER' is already running."
  fi

  local mapped
  mapped="$(container_published_port)"
  if [[ -n "$mapped" && "$mapped" != "$SQL_PORT" ]]; then
    warn "'$SQL_CONTAINER' publishes host port $mapped, not $SQL_PORT. The backend expects $SQL_PORT — rebuild with: $0 recreate --port $SQL_PORT"
  fi

  wait_until_ready || die "The database container is not usable. See the output above."

  if database_exists; then
    ok "Database '$SQL_DATABASE' is present."
  else
    create_database
    if [[ $APPLY_SCHEMA -eq 1 ]]; then
      apply_base_schema
    else
      info "Skipping table scripts (--no-schema)."
    fi
    if [[ $created -eq 0 ]]; then
      info "The database was missing from an existing container, so it starts out empty."
    fi
    # Only Course/Semester are checked in as table scripts; the tables the
    # endpoints actually read are created by the data-injection step.
    info "This is an empty database. Load entity tables and sample rows with:"
    info "  ./Automations/insert-sample-data.sh --file <seed.sql>"
  fi

  ok "SQL Server is ready on localhost:$SQL_PORT (container '$SQL_CONTAINER')."
  print_connection_string
}

cmd_status() {
  require_docker

  if ! container_exists; then
    echo "Container '$SQL_CONTAINER': missing (run '$0 up' to create it)"
    return 0
  fi

  local state mapped
  state="$(docker container inspect "$SQL_CONTAINER" --format '{{.State.Status}}' 2>/dev/null)"
  mapped="$(container_published_port)"
  echo "Container '$SQL_CONTAINER': $state"
  echo "  image:      $(docker container inspect "$SQL_CONTAINER" --format '{{.Config.Image}}')"
  echo "  host port:  ${mapped:-<not published>}"

  if ! container_running; then
    echo "  database:   unknown (container not running)"
    return 0
  fi
  if ! find_sqlcmd; then
    echo "  database:   unknown (no sqlcmd in the container)"
    return 0
  fi
  if sql_query "SELECT 1" >/dev/null 2>&1; then
    if database_exists; then
      echo "  database:   '$SQL_DATABASE' present"
    else
      echo "  database:   '$SQL_DATABASE' MISSING (run '$0 up')"
    fi
  else
    echo "  database:   not accepting connections yet"
  fi
}

cmd_down() {
  require_docker
  remove_container
}

cmd_recreate() {
  require_docker
  remove_container
  cmd_up
}

case "$COMMAND" in
  up)       cmd_up ;;
  status)   cmd_status ;;
  down)     cmd_down ;;
  recreate) cmd_recreate ;;
esac
