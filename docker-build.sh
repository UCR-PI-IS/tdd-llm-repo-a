#!/bin/bash

# Docker Build Script for Theme Park Solution
# Runs build in an ephemeral Docker container with timestamped output directories
# Usage: ./docker-build.sh

set -e  # Exit on error

# Color codes for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Generate timestamp for this run
TIMESTAMP=$(date +"%Y-%m-%d_%H-%M-%S")
BUILD_OUTPUT_DIR="BuildResults/${TIMESTAMP}"

echo -e "${YELLOW}=== Docker Build Script ===${NC}"
echo "Timestamp: ${TIMESTAMP}"
echo "Output directory: ${BUILD_OUTPUT_DIR}"
echo ""

# Create output directory
mkdir -p "${BUILD_OUTPUT_DIR}"

# Get absolute path to workspace root
WORKSPACE_ROOT="$(cd "$(dirname "$0")" && pwd)"

# Docker image name
IMAGE_NAME="themepark-dotnet-sdk"

# Build Docker image (uses BuildKit caching)
echo -e "${YELLOW}Building Docker image...${NC}"
docker build -t "${IMAGE_NAME}" -f Dockerfile.build .

# Create a temporary script to run inside the container
cat > "${BUILD_OUTPUT_DIR}/build-script.sh" << 'EOF'
#!/bin/bash
set -e

echo "=== Starting Build Process ==="
echo "Working directory: $(pwd)"
echo ""

# Restore dependencies
echo ">>> Restoring dependencies..."
dotnet restore UCR.ECCI.PI.ThemePark.sln
echo ""

# Find and build all .csproj files (excluding .sqlproj)
echo ">>> Building .NET projects (excluding .sqlproj)..."
find . -name "*.csproj" -type f -not -path "./tools/*" | while read -r proj; do
    echo "Building: $proj"
    dotnet build "$proj" --no-restore -c Release
    echo ""
done

echo "=== Build Complete ==="
EOF

chmod +x "${BUILD_OUTPUT_DIR}/build-script.sh"

# Run build in Docker container with output capture
echo -e "${YELLOW}Running build in Docker container...${NC}"
echo ""

# Run the container and capture output to both stdout and log file
docker run --rm \
    -v "${WORKSPACE_ROOT}:/workspace" \
    -v "${WORKSPACE_ROOT}/${BUILD_OUTPUT_DIR}:/output" \
    -v "${HOME}/.nuget/packages:/root/.nuget/packages" \
    -w /workspace \
    "${IMAGE_NAME}" \
    bash /output/build-script.sh 2>&1 | tee "${BUILD_OUTPUT_DIR}/build.log"

# Capture exit code
BUILD_EXIT_CODE=${PIPESTATUS[0]}

# Print summary
echo ""
if [ $BUILD_EXIT_CODE -eq 0 ]; then
    echo -e "${GREEN}✓ Build succeeded${NC}"
    echo -e "${GREEN}Build results: ${BUILD_OUTPUT_DIR}/${NC}"
else
    echo -e "${RED}✗ Build failed (exit code: ${BUILD_EXIT_CODE})${NC}"
    echo -e "${RED}Build logs: ${BUILD_OUTPUT_DIR}/build.log${NC}"
fi

echo ""
echo "Output location: ${BUILD_OUTPUT_DIR}"

exit $BUILD_EXIT_CODE
