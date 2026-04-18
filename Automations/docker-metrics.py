#!/usr/bin/env python3
"""Docker Metrics Script for Theme Park Solution.

Runs Microsoft Code Metrics via a cross-platform Roslyn-based calculator.
Results stored per user story with timestamped directories.
Usage: ./docker-metrics.py <STORY-ID>
Example: ./docker-metrics.py CPD-LC-001-001
"""

import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import docker_utils as du

# This inner script runs inside the Debian-based Docker container (has python3).
# It contains inline python3 for text summary and JSON generation.
METRICS_SCRIPT = r"""#!/bin/bash
set -e

echo "=== Starting Code Metrics Analysis ==="
echo "Working directory: $(pwd)"
echo ""

# Step 1: Prepare MetricsCalculator tool (builds on first use via dotnet run)
CALCULATOR="dotnet run --project tools/MetricsCalculator/MetricsCalculator.csproj -c Release --"
echo ">>> Restoring MetricsCalculator dependencies..."
dotnet restore tools/MetricsCalculator/MetricsCalculator.csproj --nologo -v q
echo ""

# Step 2: Restore the solution
echo ">>> Restoring solution dependencies..."
dotnet restore UCR.ECCI.PI.ThemePark.sln --nologo -v q
echo ""

# Step 3: Build all projects so Roslyn can load them
echo ">>> Building solution for metrics analysis..."
dotnet build UCR.ECCI.PI.ThemePark.sln -c Release --nologo -v q 2>&1 || true
echo ""

# Step 4: List of non-test production projects to analyze
PROJECTS=(
    "Backend.Domain/UCR.ECCI.PI.ThemePark.Backend.Domain.csproj"
    "Backend.Application/UCR.ECCI.PI.ThemePark.Backend.Application.csproj"
    "Backend.Infrastructure/UCR.ECCI.PI.ThemePark.Backend.Infrastructure.csproj"
    "Backend.Presentation/UCR.ECCI.PI.ThemePark.Backend.Presentation.csproj"
    "Backend.Api/UCR.ECCI.PI.ThemePark.Backend.Api.csproj"
    "Backend.DependencyInjection/UCR.ECCI.PI.ThemePark.Backend.DependencyInjection.csproj"
)

# Step 5: Run metrics for each project using MetricsCalculator
echo ">>> Running Code Metrics..."
METRICS_FAILED=0
for proj in "${PROJECTS[@]}"; do
    if [ -f "$proj" ]; then
        proj_name=$(basename "$proj" .csproj)
        output_file="/output/${proj_name}.Metrics.xml"
        echo "Analyzing: $proj"

        if $CALCULATOR "$proj" "$output_file" 2>&1; then
            echo ""
        else
            echo "  -> ERROR: Metrics generation failed for $proj_name"
            METRICS_FAILED=1
            echo ""
        fi
    else
        echo "Skipping (not found): $proj"
    fi
done

# Step 6: Generate human-readable summary from XML files
echo ">>> Generating metrics summary..."
SUMMARY_FILE="/output/metrics-summary.txt"

echo "================================================================" > "$SUMMARY_FILE"
echo "  CODE METRICS SUMMARY" >> "$SUMMARY_FILE"
echo "  Generated: $(date -u +"%Y-%m-%d %H:%M:%S UTC")" >> "$SUMMARY_FILE"
echo "================================================================" >> "$SUMMARY_FILE"
echo "" >> "$SUMMARY_FILE"

for xml_file in /output/*.Metrics.xml; do
    if [ -f "$xml_file" ]; then
        proj_name=$(basename "$xml_file" .Metrics.xml)
        echo "----------------------------------------------------------------" >> "$SUMMARY_FILE"
        echo "  Project: $proj_name" >> "$SUMMARY_FILE"
        echo "----------------------------------------------------------------" >> "$SUMMARY_FILE"

        echo "" >> "$SUMMARY_FILE"
        echo "  Types:" >> "$SUMMARY_FILE"
        grep -E '<NamedType ' "$xml_file" 2>/dev/null | while read -r line; do
            name=$(echo "$line" | grep -oP 'Name="[^"]*"' | head -1 | sed 's/Name="//;s/"//')
            echo "    $name" >> "$SUMMARY_FILE"
        done || true

        echo "" >> "$SUMMARY_FILE"
        echo "  Maintainability Index by type:" >> "$SUMMARY_FILE"
        python3 -c "
import xml.etree.ElementTree as ET
try:
    tree = ET.parse('$xml_file')
    root = tree.getroot()
    for nt in root.iter('NamedType'):
        name = nt.get('Name', 'unknown')
        for metric in nt.findall('./Metrics/Metric'):
            if metric.get('Name') == 'MaintainabilityIndex':
                val = metric.get('Value', '?')
                flag = ' [RED]' if int(val) < 10 else ' [YELLOW]' if int(val) < 20 else ''
                print(f'    {name}: MI={val}{flag}')
except Exception as e:
    print(f'    (parse error: {e})')
" >> "$SUMMARY_FILE" 2>&1 || echo "    (no data)" >> "$SUMMARY_FILE"

        echo "" >> "$SUMMARY_FILE"
        echo "  Cyclomatic Complexity by method (>10 shown):" >> "$SUMMARY_FILE"
        python3 -c "
import xml.etree.ElementTree as ET
try:
    tree = ET.parse('$xml_file')
    root = tree.getroot()
    found = False
    for method in root.iter('Method'):
        name = method.get('Name', 'unknown')
        for metric in method.findall('./Metrics/Metric'):
            if metric.get('Name') == 'CyclomaticComplexity':
                val = int(metric.get('Value', '0'))
                if val > 10:
                    flag = ' [RED]' if val > 25 else ' [YELLOW]'
                    print(f'    {name}: CC={val}{flag}')
                    found = True
    if not found:
        print('    (none above threshold)')
except Exception as e:
    print(f'    (parse error: {e})')
" >> "$SUMMARY_FILE" 2>&1 || echo "    (no data)" >> "$SUMMARY_FILE"

        echo "" >> "$SUMMARY_FILE"
        echo "  Class Coupling by type (>9 shown):" >> "$SUMMARY_FILE"
        python3 -c "
import xml.etree.ElementTree as ET
try:
    tree = ET.parse('$xml_file')
    root = tree.getroot()
    found = False
    for nt in root.iter('NamedType'):
        name = nt.get('Name', 'unknown')
        for metric in nt.findall('./Metrics/Metric'):
            if metric.get('Name') == 'ClassCoupling':
                val = int(metric.get('Value', '0'))
                if val > 9:
                    flag = ' [RED]' if val > 40 else ' [YELLOW]'
                    print(f'    {name}: Coupling={val}{flag}')
                    found = True
    if not found:
        print('    (none above threshold)')
except Exception as e:
    print(f'    (parse error: {e})')
" >> "$SUMMARY_FILE" 2>&1 || echo "    (no data)" >> "$SUMMARY_FILE"

        echo "" >> "$SUMMARY_FILE"
    fi
done

echo "================================================================" >> "$SUMMARY_FILE"
echo "  THRESHOLD REFERENCE (Microsoft Visual Studio / Industry)" >> "$SUMMARY_FILE"
echo "  Maintainability Index: 0-9 RED, 10-19 YELLOW, 20-100 GREEN" >> "$SUMMARY_FILE"
echo "  Cyclomatic Complexity: 1-10 GREEN, 11-25 YELLOW, >25 RED" >> "$SUMMARY_FILE"
echo "  Class Coupling: 0-9 GREEN, 10-40 YELLOW, >40 RED" >> "$SUMMARY_FILE"
echo "  Depth of Inheritance: 0-4 GREEN, 5 YELLOW, >=6 RED" >> "$SUMMARY_FILE"
echo "  Lines per Method: 5-20 GREEN, 21-50 YELLOW, >50 RED" >> "$SUMMARY_FILE"
echo "================================================================" >> "$SUMMARY_FILE"

# Step 7: Generate JSON summary
echo ">>> Generating metrics-summary.json..."
python3 -c "
import json, os, xml.etree.ElementTree as ET
from datetime import datetime, timezone

def flag_mi(v):
    return 'RED' if v < 10 else 'YELLOW' if v < 20 else 'GREEN'
def flag_cc(v):
    return 'RED' if v > 25 else 'YELLOW' if v > 10 else 'GREEN'
def flag_coupling(v):
    return 'RED' if v > 40 else 'YELLOW' if v > 9 else 'GREEN'
def flag_dit(v):
    return 'RED' if v >= 6 else 'YELLOW' if v >= 5 else 'GREEN'

projects = []
for xml_file in sorted([f for f in os.listdir('/output') if f.endswith('.Metrics.xml')]):
    proj_name = xml_file.replace('.Metrics.xml', '')
    tree = ET.parse(os.path.join('/output', xml_file))
    root = tree.getroot()
    types = []
    for nt in root.iter('NamedType'):
        name = nt.get('Name', 'unknown')
        metrics = {}
        for m in nt.findall('./Metrics/Metric'):
            metrics[m.get('Name')] = int(m.get('Value', '0'))
        methods = []
        for method in nt.findall('Method'):
            mm = {}
            for m in method.findall('./Metrics/Metric'):
                mm[m.get('Name')] = int(m.get('Value', '0'))
            methods.append({
                'name': method.get('Name', 'unknown'),
                'maintainabilityIndex': {'value': mm.get('MaintainabilityIndex', 0), 'flag': flag_mi(mm.get('MaintainabilityIndex', 0))},
                'cyclomaticComplexity': {'value': mm.get('CyclomaticComplexity', 0), 'flag': flag_cc(mm.get('CyclomaticComplexity', 0))},
                'sourceLines': mm.get('SourceLines', 0),
                'executableLines': mm.get('ExecutableLines', 0)
            })
        types.append({
            'name': name,
            'maintainabilityIndex': {'value': metrics.get('MaintainabilityIndex', 0), 'flag': flag_mi(metrics.get('MaintainabilityIndex', 0))},
            'cyclomaticComplexity': {'value': metrics.get('CyclomaticComplexity', 0), 'flag': flag_cc(metrics.get('CyclomaticComplexity', 0))},
            'classCoupling': {'value': metrics.get('ClassCoupling', 0), 'flag': flag_coupling(metrics.get('ClassCoupling', 0))},
            'depthOfInheritance': {'value': metrics.get('DepthOfInheritance', 0), 'flag': flag_dit(metrics.get('DepthOfInheritance', 0))},
            'sourceLines': metrics.get('SourceLines', 0),
            'executableLines': metrics.get('ExecutableLines', 0),
            'methods': methods
        })
    projects.append({'name': proj_name, 'types': types})

summary = {
    'status': 'success' if $METRICS_FAILED == 0 else 'failure',
    'timestamp': datetime.now(timezone.utc).strftime('%Y-%m-%dT%H:%M:%SZ'),
    'storyId': os.environ.get('STORY_ID', ''),
    'projects': projects
}
with open('/output/metrics-summary.json', 'w') as f:
    json.dump(summary, f, indent=2)
print('metrics-summary.json generated.')
" 2>&1 || echo "Warning: Failed to generate metrics-summary.json"

echo ""
cat "$SUMMARY_FILE"

if [ $METRICS_FAILED -eq 1 ]; then
    echo ""
    echo "WARNING: Some projects failed metrics generation"
fi

echo ""
echo "=== Code Metrics Analysis Complete ==="
"""


def main():
    if len(sys.argv) < 2:
        du.cprint("Error: STORY-ID is required", "RED")
        print("Usage: ./docker-metrics.py <STORY-ID>")
        print("Example: ./docker-metrics.py CPD-LC-001-001")
        sys.exit(1)

    story_id = sys.argv[1]
    timestamp = du.generate_timestamp()
    output_dir = Path("MetricsResults") / story_id / timestamp
    output_dir.mkdir(parents=True, exist_ok=True)
    ws = du.get_workspace_root()

    du.print_banner("Docker Metrics Script", timestamp, str(output_dir))
    print(f"Story ID: {story_id}")
    print()

    du.build_docker_image("themepark-dotnet-metrics", "Automations/Dockerfile.metrics")

    du.write_inner_script(output_dir, "metrics-script.sh", METRICS_SCRIPT)

    du.cprint("Running code metrics in Docker container...", "YELLOW")
    print()

    exit_code = du.run_docker_container(
        "themepark-dotnet-metrics", ws, output_dir,
        "/output/metrics-script.sh", "metrics.log",
        env_vars={"STORY_ID": story_id},
    )

    du.print_result(exit_code == 0, str(output_dir), "metrics.log")
    print(f"Metrics XML files: {output_dir}/*.Metrics.xml")
    print(f"Summary: {output_dir}/metrics-summary.txt")
    print(f"JSON: {output_dir}/metrics-summary.json")
    sys.exit(exit_code)


if __name__ == "__main__":
    main()
