#!/bin/bash

# Docker Test Script for Theme Park Solution
# Runs tests with coverage in an ephemeral Docker container with timestamped output directories
# Usage: ./docker-test.sh

set -e  # Exit on error

# Color codes for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Generate timestamp for this run
TIMESTAMP=$(date +"%Y-%m-%d_%H-%M-%S")
TEST_OUTPUT_DIR="TestResults/${TIMESTAMP}"

echo -e "${YELLOW}=== Docker Test Script ===${NC}"
echo "Timestamp: ${TIMESTAMP}"
echo "Output directory: ${TEST_OUTPUT_DIR}"
echo ""

# Create output directory
mkdir -p "${TEST_OUTPUT_DIR}"

# Get absolute path to workspace root
WORKSPACE_ROOT="$(cd "$(dirname "$0")" && pwd)"

# Docker image name
IMAGE_NAME="themepark-dotnet-sdk"

# Build Docker image (uses BuildKit caching)
echo -e "${YELLOW}Building Docker image...${NC}"
docker build -t "${IMAGE_NAME}" -f Dockerfile.build .

# Create a temporary script to run inside the container
cat > "${TEST_OUTPUT_DIR}/test-script.sh" << 'EOF'
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
    dotnet test "$proj" \
        -c Release \
        --no-build \
        --logger "trx;LogFileName=${project_name}.trx" \
        --results-directory "$project_results_dir" \
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
        /output/.tools/reportgenerator \
            -reports:"$cov_file" \
            -targetdir:"$report_dir" \
            -reporttypes:"Html;Cobertura"
        echo ""
    done
    
    # Generate combined report
    echo "Generating combined coverage report..."
    /output/.tools/reportgenerator \
        -reports:"/output/TestResults/**/coverage.cobertura.xml" \
        -targetdir:"/output/Coverage/Combined" \
        -reporttypes:"Html;Cobertura"
else
    echo "No coverage files found."
fi

echo "=== Test Complete ==="
EOF

chmod +x "${TEST_OUTPUT_DIR}/test-script.sh"

# Run tests in Docker container with output capture
echo -e "${YELLOW}Running tests in Docker container...${NC}"
echo ""

# Run the container and capture output to both stdout and log file
docker run --rm \
    -v "${WORKSPACE_ROOT}:/workspace" \
    -v "${WORKSPACE_ROOT}/${TEST_OUTPUT_DIR}:/output" \
    -v "${HOME}/.nuget/packages:/root/.nuget/packages" \
    -w /workspace \
    "${IMAGE_NAME}" \
    bash /output/test-script.sh 2>&1 | tee "${TEST_OUTPUT_DIR}/test.log"

# Capture exit code
TEST_EXIT_CODE=${PIPESTATUS[0]}

# Print summary
echo ""
if [ $TEST_EXIT_CODE -eq 0 ]; then
    echo -e "${GREEN}✓ Tests succeeded${NC}"
    echo -e "${GREEN}Test results: ${TEST_OUTPUT_DIR}/${NC}"
    echo -e "${GREEN}Coverage reports: ${TEST_OUTPUT_DIR}/Coverage/${NC}"
else
    echo -e "${RED}✗ Tests failed (exit code: ${TEST_EXIT_CODE})${NC}"
    echo -e "${RED}Test logs: ${TEST_OUTPUT_DIR}/test.log${NC}"
fi

echo ""
echo "Output location: ${TEST_OUTPUT_DIR}"
echo "TRX files: ${TEST_OUTPUT_DIR}/TestResults/"
echo "Coverage reports: ${TEST_OUTPUT_DIR}/Coverage/"

exit $TEST_EXIT_CODE
