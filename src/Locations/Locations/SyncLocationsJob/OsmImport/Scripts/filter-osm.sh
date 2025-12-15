#!/bin/bash

# Script to filter OSM data using osmium-tool
# Usage: ./filter-osm.sh <input-file> [output-file]

# Check if osmium is installed
if ! command -v osmium &> /dev/null; then
    echo "Error: osmium-tool is not installed"
    echo "Please install it using: brew install osmium-tool (macOS) or apt-get install osmium-tool (Ubuntu)"
    exit 1
fi

# Check arguments
if [ $# -lt 1 ]; then
    echo "Usage: $0 <input-osm-file> [output-file]"
    echo "Example: $0 /path/to/planet-latest.osm.pbf"
    exit 1
fi

INPUT_FILE="$1"
# Determine output filename based on input format
if [[ "$INPUT_FILE" == *.osm.bz2 ]]; then
    OUTPUT_FILE="${2:-${INPUT_FILE%.osm.bz2}-filtered.osm.pbf}"
elif [[ "$INPUT_FILE" == *.osm.pbf ]]; then
    OUTPUT_FILE="${2:-${INPUT_FILE%.osm.pbf}-filtered.osm.pbf}"
else
    OUTPUT_FILE="${2:-${INPUT_FILE%.osm}-filtered.osm.pbf}"
fi

# Check if input file exists
if [ ! -f "$INPUT_FILE" ]; then
    echo "Error: Input file '$INPUT_FILE' not found"
    exit 1
fi

echo "Starting OSM data filtering..."
echo "Input: $INPUT_FILE"
echo "Output: $OUTPUT_FILE"

# Filter OSM data for location types we need
osmium tags-filter "$INPUT_FILE" \
  --overwrite \
  --progress \
  --output="$OUTPUT_FILE" \
  n/place=city,town,village,hamlet \
  n/railway=station,halt \
  n/harbour=yes \
  n/seamark:type=harbour \
  n/building=warehouse,terminal \
  n/amenity=ferry_terminal \
  n/industrial=warehouse \
  n/landuse=port \
  w/place=city,town,village,hamlet \
  w/railway=station,halt \
  w/harbour=yes \
  w/seamark:type=harbour \
  w/building=warehouse,terminal \
  w/amenity=ferry_terminal \
  w/industrial=warehouse \
  w/landuse=port,harbour \
  r/place=city,town,village,hamlet \
  r/boundary=administrative \
  r/admin_level=2,3,4,5,6,7,8 \
  r/landuse=port,harbour

if [ $? -eq 0 ]; then
    echo "Filtering completed successfully!"
    echo "Output file: $OUTPUT_FILE"
    
    # Show file sizes for comparison
    INPUT_SIZE=$(du -h "$INPUT_FILE" | cut -f1)
    OUTPUT_SIZE=$(du -h "$OUTPUT_FILE" | cut -f1)
    echo "Size reduction: $INPUT_SIZE -> $OUTPUT_SIZE"
else
    echo "Error: Filtering failed"
    exit 1
fi