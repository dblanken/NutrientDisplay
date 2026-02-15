#!/bin/bash
# Package the mod for distribution

set -e

MOD_ID="nutrientdisplay"
VERSION=$(grep -oP '"version":\s*"\K[^"]+' modinfo.json)

# Build the project
dotnet build -c Release

# Create the mod zip
mkdir -p Releases
rm -f "Releases/${MOD_ID}_v${VERSION}.zip"
zip -j "Releases/${MOD_ID}_v${VERSION}.zip" modinfo.json bin/Release/NutrientDisplay.dll

echo "Created Releases/${MOD_ID}_v${VERSION}.zip"
