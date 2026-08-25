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

# Run tests with coverage. A failing project must not abort the script
# (set -e): coverage reports are generated for red runs too, and the failure
# is re-raised via the final exit so callers still see a non-zero code.
# Process substitution keeps the loop out of a pipeline subshell so TEST_EXIT
# survives it.
echo ">>> Running Backend Unit Tests with Coverage..."
TEST_EXIT=0
while read -r proj; do
    project_name=$(basename "$proj" .csproj)
    project_results_dir="/output/TestResults/${project_name}"
    mkdir -p "$project_results_dir"

    echo "Running tests for: $proj"
    dotnet test "$proj" \
        -c Release \
        --no-build \
        --logger "trx;LogFileName=${project_name}.trx" \
        --results-directory "$project_results_dir" \
        --collect "XPlat Code Coverage" || TEST_EXIT=$?
    echo ""
done < <(find . -name "*.csproj" -type f | grep -i "Backend.*Tests.Unit")

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
exit $TEST_EXIT
