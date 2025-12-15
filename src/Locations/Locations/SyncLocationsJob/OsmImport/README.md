# OSM Import Process

This directory contains scripts and configurations for importing OpenStreetMap (OSM) location data into the database
using a hybrid approach with osmium and osm2pgsql.

## Prerequisites

Before running the import process, you need to install the following tools:

### macOS

```bash
brew install osmium-tool
brew install osm2pgsql
```

### Ubuntu/Debian

```bash
apt-get install osmium-tool
apt-get install osm2pgsql postgis
```

## Directory Structure

```
OsmImport/
├── Scripts/
│   ├── filter-osm.sh          # Filters OSM data using osmium
│   ├── import-to-postgres.sh  # Imports filtered data to PostgreSQL
│   └── sync-locations.sh      # Full synchronization workflow
├── Config/
│   └── locations.lua          # osm2pgsql configuration for location mapping
└── README.md                  # This file
```

## Import Process Overview

The import process consists of three main steps:

1. **Filter OSM Data**: Use osmium to extract only the location types we need
2. **Import to PostgreSQL**: Use osm2pgsql to import filtered data with custom schema
3. **Sync to Neo4j**: Run the C# SyncLocationsJob to copy data from PostgreSQL to Neo4j

## Location Types Imported

The following location types are extracted from OSM:

- **Countries**: Administrative boundaries with `admin_level=2`
- **Regions**: Administrative boundaries with `admin_level=3-8`
- **Cities**: Places tagged as city, town, village, or hamlet
- **Ports**: Harbours and seaports
- **Railway Stations**: Train stations and halts (excluding subway)
- **Terminals**: Ferry terminals and building terminals
- **Warehouses**: Buildings tagged as warehouses

## Usage

### Full Synchronization

```bash
./Scripts/sync-locations.sh /path/to/planet-latest.osm.pbf
```

This will:

1. Filter the OSM dump
2. Import to PostgreSQL with optimized table structure
3. Run the C# sync job to copy to Neo4j

### Individual Steps

#### 1. Filter OSM Data

```bash
./Scripts/filter-osm.sh /path/to/planet-latest.osm.pbf [output-file]
```

This reduces the file size from ~70GB to ~5-10GB by extracting only relevant data.

#### 2. Import to PostgreSQL

```bash
# Set environment variables for database connection
export POSTGRES_HOST=localhost
export POSTGRES_PORT=5436
export POSTGRES_DB=osm_locations
export POSTGRES_USER=postgres
export POSTGRES_PASSWORD=your_password

./Scripts/import-to-postgres.sh /path/to/filtered.osm.pbf
```

#### 3. Run C# Sync Job

```bash
cd ../..
dotnet run
```

## Database Schema

The import creates dedicated tables for each location type:

- `countries` - Country-level administrative boundaries
- `regions` - Regional administrative boundaries
- `cities` - Cities, towns, villages, and hamlets
- `railways` - Railway stations
- `ports` - Ports and harbours
- `terminals` - Various types of terminals
- `warehouses` - Warehouse buildings

Each table includes:

- OSM ID and type (node/way/relation)
- Multilingual names (name, name:en, name:ru)
- Geometry data (PostGIS)
- Additional tags as JSONB

## Filtering Details

The osmium filter extracts:

### Nodes (n/)

- `place=city,town,village,hamlet`
- `railway=station,halt`
- `harbour=yes`
- `seamark:type=harbour`
- `building=warehouse,terminal`
- `amenity=ferry_terminal`
- `industrial=warehouse`
- `landuse=port`

### Ways (w/)

- Same tags as nodes (for polygonal features)

### Relations (r/)

- `boundary=administrative` with various admin_levels
- `place=city,town,village,hamlet`
- `landuse=port,harbour`

## Configuration

### Lua Configuration

The `locations.lua` file defines:

- Table schemas for each location type
- Mapping logic from OSM tags to database columns
- Filtering rules during import

### Environment Variables

- `POSTGRES_HOST` - PostgreSQL host (default: localhost)
- `POSTGRES_PORT` - PostgreSQL port (default: 5436)
- `POSTGRES_DB` - Database name (default: osm_locations)
- `POSTGRES_USER` - Database user (default: postgres)
- `POSTGRES_PASSWORD` - Database password

## Performance Tips

1. **Disk Space**: Ensure you have enough space for:
    - Original OSM dump (~70GB)
    - Filtered file (~5-10GB)
    - PostgreSQL data

2. **Memory**: osm2pgsql uses cache for better performance. Default is 2GB, can be adjusted in `import-to-postgres.sh`

3. **Processing**: Uses 4 parallel processes by default, adjust based on your CPU

## Troubleshooting

### Out of Memory

- Reduce cache size in `import-to-postgres.sh`
- Filter by region first using `osmium extract`

### Import Fails

- Check PostgreSQL logs
- Ensure PostGIS extension is installed
- Verify database permissions

### Missing Locations

- Check filter criteria in `filter-osm.sh`
- Verify OSM tags in source data
- Review Lua configuration mappings
