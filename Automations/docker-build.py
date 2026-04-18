#!/usr/bin/env python3
"""Docker Build Script for Theme Park Solution.

Runs build in an ephemeral Docker container with timestamped output directories.
Usage: ./docker-build.py
"""

import json
import os
import re
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import docker_utils as du

BUILD_SCRIPT = """\
#!/bin/bash
set -e

echo "=== Starting Build Process ==="
echo "Working directory: $(pwd)"
echo ""

# Restore dependencies
echo ">>> Restoring dependencies..."
dotnet restore UCR.ECCI.PI.ThemePark.sln
echo ""

# Find and build all .csproj files (excluding .sqlproj and tools)
echo ">>> Building .NET projects (excluding .sqlproj)..."
find . -name "*.csproj" -type f -not -path "./tools/*" | while read -r proj; do
    echo "Building: $proj"
    dotnet build "$proj" --no-restore -c Release
    echo ""
done

echo "=== Build Complete ==="
"""


def parse_build_log(log_path: Path, exit_code: int, timestamp: str) -> dict:
    log = log_path.read_text()
    sections = re.split(r"Building: (\./[^\n]+)", log)
    projects = []
    total_warnings = 0
    total_errors = 0

    i = 1
    while i < len(sections) - 1:
        proj_path = sections[i].strip()
        proj_output = sections[i + 1]
        proj_name = os.path.basename(proj_path).replace(".csproj", "")

        warnings = len(re.findall(r"warning [A-Z]{2,}\d+", proj_output))
        errors = len(re.findall(r"error [A-Z]{2,}\d+", proj_output))
        error_msgs = [m.strip() for m in re.findall(r": error ([^\[]+)", proj_output)]

        status = "failure" if errors > 0 else "success"
        total_warnings += warnings
        total_errors += errors

        projects.append({
            "name": proj_name,
            "status": status,
            "warnings": warnings,
            "errors": errors,
            "errorMessages": error_msgs,
        })
        i += 2

    return {
        "status": "failure" if exit_code != 0 or total_errors > 0 else "success",
        "timestamp": timestamp,
        "projects": projects,
        "totalWarnings": total_warnings,
        "totalErrors": total_errors,
    }


def main():
    timestamp = du.generate_timestamp()
    output_dir = Path("BuildResults") / timestamp
    output_dir.mkdir(parents=True, exist_ok=True)
    ws = du.get_workspace_root()

    du.print_banner("Docker Build Script", timestamp, str(output_dir))
    du.build_docker_image("themepark-dotnet-sdk", "Automations/Dockerfile.build")

    du.write_inner_script(output_dir, "build-script.sh", BUILD_SCRIPT)

    du.cprint("Running build in Docker container...", "YELLOW")
    print()

    exit_code = du.run_docker_container(
        "themepark-dotnet-sdk", ws, output_dir,
        "/output/build-script.sh", "build.log",
    )

    summary = parse_build_log(ws / output_dir / "build.log", exit_code, timestamp)
    (ws / output_dir / "build-summary.json").write_text(json.dumps(summary, indent=2))

    du.print_result(exit_code == 0, str(output_dir), "build.log")
    sys.exit(exit_code)


if __name__ == "__main__":
    main()
