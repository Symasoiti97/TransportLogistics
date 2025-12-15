#!/bin/bash

# Full cycle script for syncing OSM locations to database
# Usage: ./sync-locations.sh <osm-dump-path>

set -e  # Exit on error

# Get the directory of this script
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# Check arguments
if [ $# -lt 1 ]; then
    echo "Usage: $0 <osm-dump-path>"
    echo "Example: $0 /path/to/planet-latest.osm.pbf"
    echo ""
    echo "This script will:"
    echo "  1. Filter the OSM dump using osmium"
    echo "  2. Import filtered data to PostgreSQL"
    echo "  3. Run the C# SyncLocationsJob to sync to Neo4j"
    exit 1
fi

OSM_DUMP="$1"

# Check if OSM dump exists
if [ ! -f "$OSM_DUMP" ]; then
    echo "Error: OSM dump file '$OSM_DUMP' not found"
    exit 1
fi

# Timestamp for logging
TIMESTAMP=$(date +"%Y-%m-%d %H:%M:%S")
echo "[$TIMESTAMP] Starting OSM locations sync..."

# Step 1: Filter OSM data
echo ""
echo "Step 1/3: Filtering OSM data..."
FILTERED_FILE="${OSM_DUMP%.osm.pbf}-filtered-$(date +%Y%m%d).osm.pbf"
"$SCRIPT_DIR/filter-osm.sh" "$OSM_DUMP" "$FILTERED_FILE"

if [ $? -ne 0 ]; then
    echo "Error: Filtering failed"
    exit 1
fi

# Step 2: Import to PostgreSQL
echo ""
echo "Step 2/3: Importing to PostgreSQL..."
"$SCRIPT_DIR/import-to-postgres.sh" "$FILTERED_FILE"

if [ $? -ne 0 ]; then
    echo "Error: PostgreSQL import failed"
    exit 1
fi

# Step 3: Run C# sync job
echo ""
echo "Step 3/3: Running C# SyncLocationsJob..."
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")/../.."

# Check if we're in development environment
if [ -f "$PROJECT_ROOT/appsettings.Development.json" ]; then
    echo "Running in Development environment..."
    cd "$PROJECT_ROOT" && dotnet run --environment Development
else
    echo "Running in Production environment..."
    cd "$PROJECT_ROOT" && dotnet run
fi

if [ $? -ne 0 ]; then
    echo "Error: C# sync job failed"
    exit 1
fi

# Success
TIMESTAMP=$(date +"%Y-%m-%d %H:%M:%S")
echo ""
echo "[$TIMESTAMP] OSM locations sync completed successfully!"

# Optional: Clean up filtered file to save space
read -p "Delete filtered OSM file to save space? (y/N) " -n 1 -r
echo
if [[ $REPLY =~ ^[Yy]$ ]]; then
    rm -f "$FILTERED_FILE"
    echo "Filtered file deleted."
fi