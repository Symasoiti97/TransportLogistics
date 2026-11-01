---
paths:
  - "src/Locations/**"
---

# Locations: OSM preparation

- Scripts live in `src/Locations/Locations/SyncLocationsJob/OsmImport/Scripts`: `filter-osm.sh` → `import-to-postgres.sh` → `sync-locations.sh`. They need `osmium-tool` and `osm2pgsql`. Details: `OsmImport/README.md` in the same folder.
- Connection config: `OsmPostgres` (database `osm_locations`, port 5436, container `postgis-osm`, `infra` compose profile), `Neo4JSettings`.
- Root-level `import-osm.sh` and `*.osm.pbf` in `SyncLocationsJob/` are local untracked artifacts. Do not commit them.
