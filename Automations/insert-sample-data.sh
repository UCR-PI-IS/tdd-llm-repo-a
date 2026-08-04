#!/usr/bin/env bash
#
# insert-sample-data.sh
# ---------------------
# Generic SQL executor for the ThemePark database running inside the local
# Docker SQL Server container. It does NOT know about any specific entity or
# schema: the caller supplies the SQL to run (typically a CREATE TABLE + a set
# of INSERT statements). This lets the data-injection agent build the schema
# and sample rows *dynamically* from the backend entity implementation and then
# hand the generated SQL to this script for execution.
#
# Usage:
#   ./insert-sample-data.sh --file <path-to.sql>     # run SQL from a file
#   ./insert-sample-data.sh --sql  "<sql string>"    # run an inline SQL string
#   ./insert-sample-data.sh < seed.sql               # run SQL from stdin
#   cat seed.sql | ./insert-sample-data.sh           # run SQL piped via stdin
#
# Environment overrides (sensible defaults for the local dev setup):
#   SQL_CONTAINER   Docker container name   (default: themepark-sql)
#   SQL_DATABASE    Target database         (default: ThemePark)
#   SQL_USER        SQL login               (default: sa)
#   SQL_PASSWORD    SQL password            (default: LocalDev!Pass123)
#
set -euo pipefail

# ---------------------------------------------------------------------------
# Configuration
# ---------------------------------------------------------------------------
SQL_CONTAINER="${SQL_CONTAINER:-themepark-sql}"
SQL_DATABASE="${SQL_DATABASE:-ThemePark}"
SQL_USER="${SQL_USER:-sa}"
SQL_PASSWORD="${SQL_PASSWORD:-LocalDev!Pass123}"
SQLCMD="/opt/mssql-tools18/bin/sqlcmd"

usage() {
  grep '^#' "$0" | sed 's/^# \{0,1\}//' | sed '1d'
  exit "${1:-1}"
}

# ---------------------------------------------------------------------------
# Resolve the SQL to run (from --file, --sql, or stdin)
# ---------------------------------------------------------------------------
SQL=""
case "${1:-}" in
  -h|--help)
    usage 0
    ;;
  --file)
    [[ -n "${2:-}" ]] || { echo "ERROR: --file requires a path." >&2; exit 1; }
    [[ -f "$2" ]]     || { echo "ERROR: file not found: $2" >&2; exit 1; }
    SQL="$(cat "$2")"
    ;;
  --sql)
    [[ -n "${2:-}" ]] || { echo "ERROR: --sql requires a SQL string." >&2; exit 1; }
    SQL="$2"
    ;;
  "")
    # No args: read SQL from stdin (pipe or here-doc).
    if [[ -t 0 ]]; then
      echo "ERROR: no SQL provided. Pass --file <path>, --sql \"<sql>\", or pipe via stdin." >&2
      usage 1
    fi
    SQL="$(cat)"
    ;;
  *)
    echo "ERROR: unknown argument '$1'." >&2
    usage 1
    ;;
esac

if [[ -z "${SQL// /}" ]]; then
  echo "ERROR: the supplied SQL is empty." >&2
  exit 1
fi

# ---------------------------------------------------------------------------
# Pre-flight: make sure the container is up
# ---------------------------------------------------------------------------
if ! docker ps --format '{{.Names}}' | grep -qx "$SQL_CONTAINER"; then
  echo "ERROR: Docker container '$SQL_CONTAINER' is not running." >&2
  echo "       Start it with: ./Automations/dev-database.sh up" >&2
  exit 1
fi

# ---------------------------------------------------------------------------
# Execute the SQL inside the container
# ---------------------------------------------------------------------------
echo ">> Executing SQL against db='$SQL_DATABASE' (container='$SQL_CONTAINER')..."
docker exec -i "$SQL_CONTAINER" "$SQLCMD" \
  -S localhost -U "$SQL_USER" -P "$SQL_PASSWORD" -No \
  -d "$SQL_DATABASE" -b -Q "$SQL"
echo ">> Done."
