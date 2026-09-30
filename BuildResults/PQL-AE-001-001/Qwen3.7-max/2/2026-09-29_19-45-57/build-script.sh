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
