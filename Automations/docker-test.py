#!/usr/bin/env python3
"""Docker Test Script for Theme Park Solution.

Runs tests with coverage in an ephemeral Docker container with timestamped output directories.
Usage: ./docker-test.py
"""

import json
import re
import sys
import xml.etree.ElementTree as ET
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import docker_utils as du

TEST_SCRIPT = """\
#!/bin/bash
set -e

echo "=== Starting Test Process ==="
echo "Working directory: $(pwd)"
echo ""

# Restore dependencies
echo ">>> Restoring dependencies..."
dotnet restore UCR.ECCI.PI.ThemePark.sln
echo ""

# Find and build test projects
echo ">>> Building test projects..."
find . -name "*.csproj" -type f | grep -i "Backend.*Tests.Unit" | while read -r proj; do
    echo "Building test project: $proj"
    dotnet build "$proj" -c Release --no-restore
    echo ""
done

# Create directories for test results
mkdir -p /output/Coverage
mkdir -p /output/TestResults

# Run tests with coverage
echo ">>> Running Backend Unit Tests with Coverage..."
find . -name "*.csproj" -type f | grep -i "Backend.*Tests.Unit" | while read -r proj; do
    project_name=$(basename "$proj" .csproj)
    project_results_dir="/output/TestResults/${project_name}"
    mkdir -p "$project_results_dir"

    echo "Running tests for: $proj"
    dotnet test "$proj" \\
        -c Release \\
        --no-build \\
        --logger "trx;LogFileName=${project_name}.trx" \\
        --results-directory "$project_results_dir" \\
        --collect "XPlat Code Coverage"
    echo ""
done

# Install ReportGenerator
echo ">>> Installing ReportGenerator..."
mkdir -p /output/.tools
dotnet tool install --tool-path /output/.tools dotnet-reportgenerator-globaltool || true
echo ""

# Generate Coverage Reports
echo ">>> Generating Coverage Reports..."
coverage_files=$(find /output/TestResults -name "coverage.cobertura.xml" -type f 2>/dev/null || true)

if [ -n "$coverage_files" ]; then
    for cov_file in $coverage_files; do
        project_name=$(basename $(dirname $(dirname "$cov_file")))
        report_dir="/output/Coverage/${project_name}"
        mkdir -p "$report_dir"

        echo "Generating coverage report for: $project_name"
        /output/.tools/reportgenerator \\
            -reports:"$cov_file" \\
            -targetdir:"$report_dir" \\
            -reporttypes:"Html;Cobertura"
        echo ""
    done

    # Generate combined report
    echo "Generating combined coverage report..."
    /output/.tools/reportgenerator \\
        -reports:"/output/TestResults/**/coverage.cobertura.xml" \\
        -targetdir:"/output/Coverage/Combined" \\
        -reporttypes:"Html;Cobertura"
else
    echo "No coverage files found."
fi

echo "=== Test Complete ==="
"""

TRX_NS = {"t": "http://microsoft.com/schemas/VisualStudio/TeamTest/2010"}


def parse_test_results(output_dir: Path, exit_code: int, timestamp: str) -> dict:
    trx_base = output_dir / "TestResults"
    projects = []
    total_tests = 0
    total_passed = 0
    total_failed = 0
    total_skipped = 0

    if trx_base.is_dir():
        for proj_dir in sorted(trx_base.iterdir()):
            if not proj_dir.is_dir():
                continue
            for trx_file in proj_dir.glob("*.trx"):
                try:
                    tree = ET.parse(trx_file)
                    root = tree.getroot()
                    counters = root.find(".//t:ResultSummary/t:Counters", TRX_NS)
                    if counters is not None:
                        t = int(counters.get("total", "0"))
                        p = int(counters.get("passed", "0"))
                        f = int(counters.get("failed", "0"))
                        s = t - p - f
                        projects.append({
                            "name": proj_dir.name,
                            "total": t, "passed": p, "failed": f, "skipped": s,
                        })
                        total_tests += t
                        total_passed += p
                        total_failed += f
                        total_skipped += s
                except ET.ParseError:
                    pass

    # Fallback: parse log if no TRX data
    if not projects:
        log_path = output_dir / "test.log"
        if log_path.exists():
            log = log_path.read_text()
            for pattern in [r"Passed!\s+-\s+Failed:\s+(\d+),\s+Passed:\s+(\d+),\s+Skipped:\s+(\d+),\s+Total:\s+(\d+)",
                            r"Failed!\s+-\s+Failed:\s+(\d+),\s+Passed:\s+(\d+),\s+Skipped:\s+(\d+),\s+Total:\s+(\d+)"]:
                for m in re.finditer(pattern, log):
                    f, p, s, t = int(m.group(1)), int(m.group(2)), int(m.group(3)), int(m.group(4))
                    total_failed += f
                    total_passed += p
                    total_skipped += s
                    total_tests += t

    return {
        "status": "failure" if exit_code != 0 or total_failed > 0 else "success",
        "timestamp": timestamp,
        "projects": projects,
        "totalTests": total_tests,
        "totalPassed": total_passed,
        "totalFailed": total_failed,
        "totalSkipped": total_skipped,
    }


def main():
    timestamp = du.generate_timestamp()
    output_dir = Path("TestResults") / timestamp
    output_dir.mkdir(parents=True, exist_ok=True)
    ws = du.get_workspace_root()

    du.print_banner("Docker Test Script", timestamp, str(output_dir))
    du.build_docker_image("themepark-dotnet-sdk", "Automations/Dockerfile.build")

    du.write_inner_script(output_dir, "test-script.sh", TEST_SCRIPT)

    du.cprint("Running tests in Docker container...", "YELLOW")
    print()

    exit_code = du.run_docker_container(
        "themepark-dotnet-sdk", ws, output_dir,
        "/output/test-script.sh", "test.log",
    )

    summary = parse_test_results(ws / output_dir, exit_code, timestamp)
    (ws / output_dir / "test-summary.json").write_text(json.dumps(summary, indent=2))

    du.print_result(exit_code == 0, str(output_dir), "test.log")
    print(f"TRX files: {output_dir}/TestResults/")
    print(f"Coverage reports: {output_dir}/Coverage/")
    sys.exit(exit_code)


if __name__ == "__main__":
    main()
