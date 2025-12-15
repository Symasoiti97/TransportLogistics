#!/bin/bash

# Script to import filtered OSM data into PostgreSQL using osm2pgsql
# Usage: ./import-to-postgres.sh <filtered-osm-file> [database-options]

# Default PostgreSQL connection parameters
DB_HOST="${POSTGRES_HOST:-localhost}"
DB_PORT="${POSTGRES_PORT:-5436}"
DB_NAME="${POSTGRES_DB:-osm_locations}"
DB_USER="${POSTGRES_USER:-postgres}"
DB_PASS="${POSTGRES_PASSWORD:-postgres}"

# Check if osm2pgsql is installed
if ! command -v osm2pgsql &> /dev/null; then
    echo "Error: osm2pgsql is not installed"
    echo "Please install it using: brew install osm2pgsql (macOS) or apt-get install osm2pgsql (Ubuntu)"
    exit 1
fi

# Check arguments
if [ $# -lt 1 ]; then
    echo "Usage: $0 <filtered-osm-file>"
    echo "Example: $0 /path/to/filtered.osm.pbf"
    echo ""
    echo "Environment variables:"
    echo "  POSTGRES_HOST     PostgreSQL host (default: localhost)"
    echo "  POSTGRES_PORT     PostgreSQL port (default: 5436)"
    echo "  POSTGRES_DB       Database name (default: osm_locations)"
    echo "  POSTGRES_USER     Database user (default: postgres)"
    echo "  POSTGRES_PASSWORD Database password"
    exit 1
fi

INPUT_FILE="$1"
CONFIG_FILE="$(dirname "$0")/../Config/locations.lua"

# Check if input file exists
if [ ! -f "$INPUT_FILE" ]; then
    echo "Error: Input file '$INPUT_FILE' not found"
    exit 1
fi

# Check if config file exists
if [ ! -f "$CONFIG_FILE" ]; then
    echo "Error: Configuration file '$CONFIG_FILE' not found"
    exit 1
fi

# Build osm2pgsql connection string
CONNECTION_PARAMS="-H $DB_HOST -P $DB_PORT -U $DB_USER -d $DB_NAME"

# Add password if provided
if [ -n "$DB_PASS" ]; then
    export PGPASSWORD="$DB_PASS"
fi

echo "Starting OSM data import..."
echo "Input file: $INPUT_FILE"
echo "Database: $DB_NAME@$DB_HOST:$DB_PORT"
echo "Config: $CONFIG_FILE"

# Create PostGIS extension if not exists
echo "Ensuring PostGIS extension is enabled..."
psql $CONNECTION_PARAMS -c "CREATE EXTENSION IF NOT EXISTS postgis;" 2>/dev/null

# Import data using osm2pgsql with flex output
osm2pgsql \
  $CONNECTION_PARAMS \
  --create \
  --output=flex \
  --style="$CONFIG_FILE" \
  --slim \
  --drop \
  --cache=2000 \
  --number-processes=4 \
  --hstore-all \
  --extra-attributes \
  --verbose \
  "$INPUT_FILE"

if [ $? -eq 0 ]; then
    echo "Import completed successfully!"
    
    echo ""
    echo "Table statistics:"
    psql $CONNECTION_PARAMS -c "
        SELECT 
            table_name,
            pg_size_pretty(pg_total_relation_size(table_name::regclass)) as size
        FROM (VALUES 
            ('countries'),
            ('regions'),
            ('cities'),
            ('railways'),
            ('ports'),
            ('terminals'),
            ('warehouses')
        ) AS t(table_name)
        WHERE EXISTS (SELECT 1 FROM information_schema.tables WHERE table_schema = 'public' AND table_name = t.table_name)
        ORDER BY table_name;
    "
    
    echo ""
    echo "Record counts:"
    psql $CONNECTION_PARAMS -c "
        SELECT 'countries' as table_name, COUNT(*) as count FROM countries
        UNION ALL SELECT 'regions', COUNT(*) FROM regions
        UNION ALL SELECT 'cities', COUNT(*) FROM cities
        UNION ALL SELECT 'railways', COUNT(*) FROM railways
        UNION ALL SELECT 'ports', COUNT(*) FROM ports
        UNION ALL SELECT 'terminals', COUNT(*) FROM terminals
        UNION ALL SELECT 'warehouses', COUNT(*) FROM warehouses
        ORDER BY table_name;
    "
else
    echo "Error: Import failed"
    exit 1
fi

# Clean up password
unset PGPASSWORD