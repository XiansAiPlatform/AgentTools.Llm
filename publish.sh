#!/bin/bash

# Exit on error
set -e

# Check if version and api key are provided
if [ $# -ne 2 ]; then
    echo "Usage: $0 <version> <api_key>"
    echo "Example: $0 1.0.0 your-api-key-here"
    exit 1
fi

VERSION=$1
API_KEY=$2

# Build the package
./build.sh

# Publish to NuGet
echo "Publishing version $VERSION to NuGet..."
dotnet nuget push ./artifacts/AgentTools.Llm.$VERSION.nupkg \
    --api-key $API_KEY \
    --source https://api.nuget.org/v3/index.json

echo "Package published successfully!" 