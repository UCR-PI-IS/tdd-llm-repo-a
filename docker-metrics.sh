#!/bin/bash

# Docker Metrics Script for Theme Park Solution
# Runs Microsoft Code Metrics via a cross-platform Roslyn-based calculator
# Results stored per user story with timestamped directories
# Usage: ./docker-metrics.sh <STORY-ID>
# Example: ./docker-metrics.sh CPD-LC-001-001

set -e  # Exit on error

# Color codes for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Validate input
if [ -z "$1" ]; then
    echo -e "${RED}Error: STORY-ID is required${NC}"
    echo "Usage: ./docker-metrics.sh <STORY-ID>"
    echo "Example: ./docker-metrics.sh CPD-LC-001-001"
    exit 1
fi

STORY_ID="$1"
TIMESTAMP=$(date +"%Y-%m-%d_%H-%M-%S")
METRICS_OUTPUT_DIR="MetricsResults/${STORY_ID}/${TIMESTAMP}"

echo -e "${YELLOW}=== Docker Metrics Script ===${NC}"
echo "Story ID: ${STORY_ID}"
echo "Timestamp: ${TIMESTAMP}"
echo "Output directory: ${METRICS_OUTPUT_DIR}"
echo ""

# Create output directory
mkdir -p "${METRICS_OUTPUT_DIR}"

# Get absolute path to workspace root
WORKSPACE_ROOT="$(cd "$(dirname "$0")" && pwd)"

# Docker image name (uses full Debian SDK for Roslyn workspace compatibility)
IMAGE_NAME="themepark-dotnet-metrics"

# Build Docker image
echo -e "${YELLOW}Building Docker metrics image...${NC}"
docker build -t "${IMAGE_NAME}" -f Dockerfile.metrics .

# Create the metrics script to run inside the container
cat > "${METRICS_OUTPUT_DIR}/metrics-script.sh" << 'METRICS_EOF'
#!/bin/bash
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

        # Extract NamedType metrics (class-level)
        echo "" >> "$SUMMARY_FILE"
        echo "  Types:" >> "$SUMMARY_FILE"
        # Find NamedType elements and extract their metrics
        grep -E '<NamedType ' "$xml_file" 2>/dev/null | while read -r line; do
            name=$(echo "$line" | grep -oP 'Name="[^"]*"' | head -1 | sed 's/Name="//;s/"//')
            echo "    $name" >> "$SUMMARY_FILE"
        done || true

        # Extract all Metric elements with their context
        echo "" >> "$SUMMARY_FILE"
        echo "  Maintainability Index by type:" >> "$SUMMARY_FILE"
        # Parse XML for NamedType > Metrics > Metric where Name=MaintainabilityIndex
        python3 -c "
import xml.etree.ElementTree as ET
import sys
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

echo ""
cat "$SUMMARY_FILE"

if [ $METRICS_FAILED -eq 1 ]; then
    echo ""
    echo "WARNING: Some projects failed metrics generation"
fi

echo ""
echo "=== Code Metrics Analysis Complete ==="
METRICS_EOF

chmod +x "${METRICS_OUTPUT_DIR}/metrics-script.sh"

# Run metrics in Docker container with output capture
echo -e "${YELLOW}Running code metrics in Docker container...${NC}"
echo ""

docker run --rm \
    -v "${WORKSPACE_ROOT}:/workspace" \
    -v "${WORKSPACE_ROOT}/${METRICS_OUTPUT_DIR}:/output" \
    -v "${HOME}/.nuget/packages:/root/.nuget/packages" \
    -w /workspace \
    "${IMAGE_NAME}" \
    bash /output/metrics-script.sh 2>&1 | tee "${METRICS_OUTPUT_DIR}/metrics.log"

# Capture exit code
METRICS_EXIT_CODE=${PIPESTATUS[0]}

# Print summary
echo ""
if [ $METRICS_EXIT_CODE -eq 0 ]; then
    echo -e "${GREEN}✓ Metrics analysis succeeded${NC}"
    echo -e "${GREEN}Results: ${METRICS_OUTPUT_DIR}/${NC}"
else
    echo -e "${RED}✗ Metrics analysis failed (exit code: ${METRICS_EXIT_CODE})${NC}"
    echo -e "${RED}Logs: ${METRICS_OUTPUT_DIR}/metrics.log${NC}"
fi

echo ""
echo "Output location: ${METRICS_OUTPUT_DIR}"
echo "Metrics XML files: ${METRICS_OUTPUT_DIR}/*.Metrics.xml"
echo "Summary: ${METRICS_OUTPUT_DIR}/metrics-summary.txt"

exit $METRICS_EXIT_CODE
