#!/bin/bash
set -e

echo "=== Publishing Backend API ==="
echo "Project: Backend.Api/UCR.ECCI.PI.ThemePark.Backend.Api.csproj"
echo ""

dotnet restore "Backend.Api/UCR.ECCI.PI.ThemePark.Backend.Api.csproj"
dotnet publish "Backend.Api/UCR.ECCI.PI.ThemePark.Backend.Api.csproj" -c Release --no-restore -o /app-out
echo ""

for runtimeconfig in /app-out/*.runtimeconfig.json; do
    base=$(basename "$runtimeconfig" .runtimeconfig.json)
    echo "PUBLISHED_DLL=${base}.dll"
done

echo "=== Publish Complete ==="
