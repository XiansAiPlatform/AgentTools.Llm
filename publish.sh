#!/bin/bash

# Exit on error
set -e

# Check if version is provided
if [ -z "$1" ]; then
    echo "Error: Version parameter is required"
    echo "Usage: ./publish.sh <version>"
    echo "Example: ./publish.sh 1.0.0"
    exit 1
fi

VERSION=$1

# Check if NuGet API key is set
if [ -z "$NUGET_API_KEY" ]; then
    echo "Error: NUGET_API_KEY environment variable is not set"
    echo "Please set it with: export NUGET_API_KEY='your-api-key'"
    exit 1
fi

# Build the package
./build.sh

# Publish to NuGet
echo "Publishing version $VERSION to NuGet..."
dotnet nuget push ./artifacts/AgentTools.Llm.$VERSION.nupkg \
    --api-key $NUGET_API_KEY \
    --source https://api.nuget.org/v3/index.json

echo "Package published successfully!" 