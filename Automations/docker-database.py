#!/usr/bin/env python3
"""Ephemeral local database for the Theme Park solution.

Starts a throwaway SQL Server container the backend can connect to, applies the
checked-in table scripts plus any seed SQL you hand it, and tears the whole
thing down again. Nothing survives a `down`.

Usage:
  ./docker-database.py up      [--name N] [--network N] [--port P | --no-port]
                               [--database D] [--no-base-schema] [--seed FILE ...]
  ./docker-database.py down    [--name N] [--network N] [--all]
  ./docker-database.py status
  ./docker-database.py exec    [--name N] (--file FILE | --sql "SELECT ...")
  ./docker-database.py prune

Examples:
  ./docker-database.py up --port 14330
  ./docker-database.py exec --file /tmp/seed-learningspace.sql
  ./docker-database.py down --all

Only resources labelled `themepark-e2e=1` — the ones this script creates — are
ever removed, so a hand-started `themepark-sql` container is left alone.
"""

import argparse
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import docker_database as db
import docker_utils as du


def cmd_up(args: argparse.Namespace) -> int:
    host_port = None if args.no_port else args.port
    try:
        info = db.start_database(
            container=args.name,
            network=args.network,
            image=args.image,
            database=args.database,
            password=args.password,
            host_port=host_port,
            ready_timeout=args.wait_timeout,
            log=print,
        )
    except db.DatabaseError as error:
        du.cprint(f"✗ {error}", "RED")
        return 2

    warnings: list[str] = []

    if not args.no_base_schema:
        schema_files = db.base_schema_files()
        if schema_files:
            print(f"Applying {len(schema_files)} base schema file(s) from {db.BASE_SCHEMA_DIR}/...")
            _, failures = db.apply_sql_files(
                args.name, schema_files, args.database, args.password, info["sqlcmd"], log=print
            )
            for failure in failures:
                warnings.append(f"base schema {failure['file']}: {failure['error'].splitlines()[-1]}")

    for seed in args.seed or []:
        path = Path(seed)
        if not path.is_file():
            du.cprint(f"✗ Seed file not found: {seed}", "RED")
            return 2
        print(f"Applying seed file {path}...")
        result = db.run_sql(
            args.name, db.read_sql_file(path), args.database, args.password, info["sqlcmd"]
        )
        if result.returncode != 0:
            du.cprint(f"✗ Seed failed: {(result.stderr or result.stdout).strip()}", "RED")
            return 1
        print(result.stdout.strip())

    du.cprint("✓ Ephemeral database is up", "GREEN")
    print(f"Container:  {info['container']}")
    print(f"Database:   {info['database']}")
    if info["network"]:
        print(f"Network:    {info['network']}")
        print(f"In-network connection string:")
        print(f"  {db.connection_string(info['container'], 1433, info['database'], info['password'])}")
    if host_port:
        print(f"Host port:  {host_port}")
        print("Host connection string:")
        print(f"  {db.connection_string('localhost', host_port, info['database'], info['password'])}")
    print()
    print(f"Seed more data:  python Automations/docker-database.py exec --file <file.sql>")
    print(f"Tear it down:    python Automations/docker-database.py down")

    for warning in warnings:
        du.cprint(f"! {warning}", "YELLOW")
    return 0


def cmd_down(args: argparse.Namespace) -> int:
    if args.all:
        removed = db.prune_resources(log=print)
        total = sum(len(v) for v in removed.values())
        du.cprint(f"✓ Removed {total} ephemeral resource(s)", "GREEN")
        return 0

    removed_container = db.remove_container(args.name, log=print)
    removed_network = db.remove_network(args.network, log=print) if args.network else False
    if not removed_container and not removed_network:
        print("Nothing to remove.")
    du.cprint("✓ Ephemeral database is down", "GREEN")
    return 0


def cmd_status(_args: argparse.Namespace) -> int:
    ok, detail = db.docker_available()
    if not ok:
        du.cprint(f"✗ {detail}", "RED")
        return 2
    print(f"Docker server: {detail}")

    found = db.labeled_resources()
    if not any(found.values()):
        print("No ephemeral Theme Park resources are running.")
        return 0

    for cid in found["containers"]:
        info = db.docker(
            ["container", "inspect", cid, "--format", "{{.Name}}\t{{.Config.Image}}\t{{.State.Status}}"]
        ).stdout.strip()
        print(f"container  {info}")
    for nid in found["networks"]:
        name = db.docker(["network", "inspect", nid, "--format", "{{.Name}}"]).stdout.strip()
        print(f"network    {name}")
    for vid in found["volumes"]:
        print(f"volume     {vid}")
    return 0


def cmd_exec(args: argparse.Namespace) -> int:
    if not db.container_running(args.name):
        du.cprint(
            f"✗ Container '{args.name}' is not running. Start it with: "
            f"python Automations/docker-database.py up",
            "RED",
        )
        return 2

    if args.file:
        path = Path(args.file)
        if not path.is_file():
            du.cprint(f"✗ File not found: {args.file}", "RED")
            return 2
        sql = db.read_sql_file(path)
    else:
        sql = args.sql

    result = db.run_sql(args.name, sql, args.database, args.password)
    if result.stdout:
        print(result.stdout.rstrip())
    if result.returncode != 0:
        du.cprint(f"✗ SQL failed: {(result.stderr or result.stdout).strip()}", "RED")
        return 1
    du.cprint("✓ SQL executed", "GREEN")
    return 0


def cmd_prune(_args: argparse.Namespace) -> int:
    removed = db.prune_resources(log=print)
    total = sum(len(v) for v in removed.values())
    du.cprint(f"✓ Pruned {total} ephemeral resource(s)", "GREEN")
    return 0


def build_parser() -> argparse.ArgumentParser:
    parser = argparse.ArgumentParser(
        description=__doc__,
        formatter_class=argparse.RawDescriptionHelpFormatter,
    )
    sub = parser.add_subparsers(dest="command", required=True)

    def add_common(p: argparse.ArgumentParser) -> None:
        p.add_argument("--name", default=db.DEFAULT_CONTAINER, help="container name")
        p.add_argument("--database", default=db.DEFAULT_DATABASE, help="database name")
        p.add_argument("--password", default=db.DEFAULT_PASSWORD, help="sa password")

    up = sub.add_parser("up", help="start a fresh ephemeral database")
    add_common(up)
    up.add_argument("--network", default=None, help="attach to this Docker network (created if absent)")
    up.add_argument("--port", type=int, default=14330, help="host port to publish (default: 14330)")
    up.add_argument("--no-port", action="store_true", help="do not publish a host port")
    up.add_argument("--image", default=db.DEFAULT_IMAGE, help="SQL Server image")
    up.add_argument("--wait-timeout", type=int, default=db.DEFAULT_READY_TIMEOUT,
                    help="seconds to wait for SQL Server to accept connections")
    up.add_argument("--no-base-schema", action="store_true",
                    help=f"skip the table scripts in {db.BASE_SCHEMA_DIR}/")
    up.add_argument("--seed", action="append", help="seed SQL file to apply (repeatable)")
    up.set_defaults(func=cmd_up)

    down = sub.add_parser("down", help="remove the ephemeral database")
    add_common(down)
    down.add_argument("--network", default=None, help="also remove this Docker network")
    down.add_argument("--all", action="store_true", help="remove every labelled ephemeral resource")
    down.set_defaults(func=cmd_down)

    status = sub.add_parser("status", help="list ephemeral resources currently alive")
    status.set_defaults(func=cmd_status)

    execute = sub.add_parser("exec", help="run SQL against the ephemeral database")
    add_common(execute)
    group = execute.add_mutually_exclusive_group(required=True)
    group.add_argument("--file", help="path to a .sql file")
    group.add_argument("--sql", help="inline SQL string")
    execute.set_defaults(func=cmd_exec)

    prune = sub.add_parser("prune", help="remove leftovers from crashed runs")
    prune.set_defaults(func=cmd_prune)

    return parser


def main() -> None:
    args = build_parser().parse_args()
    try:
        sys.exit(args.func(args))
    except db.DatabaseError as error:
        du.cprint(f"✗ {error}", "RED")
        sys.exit(2)
    except KeyboardInterrupt:
        du.cprint("Interrupted.", "YELLOW")
        sys.exit(130)


if __name__ == "__main__":
    main()
