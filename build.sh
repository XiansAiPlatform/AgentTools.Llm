#!/bin/bash

# Exit on error
set -e

# Configuration
CONFIGURATION="Release"
VERSION="1.0.0"

echo "Building AgentTools.Llm..."

# Clean previous builds
dotnet clean

# Restore dependencies
dotnet restore

# Build the solution
dotnet build -c $CONFIGURATION

# Run tests
dotnet test --no-build -c $CONFIGURATION

# Create NuGet package
dotnet pack AgentTools.Llm/AgentTools.Llm.csproj -c $CONFIGURATION -o ./artifacts

echo "Build completed successfully!"
echo "Package location: ./artifacts/AgentTools.Llm.$VERSION.nupkg" 