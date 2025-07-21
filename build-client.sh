#!/bin/bash
set -e

OPENAPI_FILE="./source/RegistryLookup.Backend/RegistryLookup.Backend.json"
OUTPUT_DIR="./source/RegistryLookup.Client"
NAMESPACE="RegistryLookup.Client"

# Optional: Pfad zur Kiota CLI prüfen
if ! command -v kiota &> /dev/null
then
    echo "❌ Kiota CLI nicht gefunden. Installiere mit:"
    echo "   dotnet tool install --global Microsoft.OpenApi.Kiota"
    exit 1
fi

# Client generieren
kiota generate \
  --language csharp \
  --class-name RestClient \
  --namespace-name "$NAMESPACE" \
  --openapi "$OPENAPI_FILE" \
  --output "$OUTPUT_DIR" \