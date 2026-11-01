---
name: osm-import
description: Import an OSM extract into local location data: filter with osmium, load into PostGIS, sync to Neo4j via SyncLocationsJob, and check row counts at each step. Use when the user asks to load, refresh or re-import OSM or location data. Triggers (RU): «импорт OSM», «загрузи локации», «обнови OSM», «osm import», «синхронизируй локации».
---

# osm-import

Rules: `.claude/rules/osm-locations.md`. Details: `src/Locations/Locations/SyncLocationsJob/OsmImport/README.md`.

## 0. Preconditions

- Tools: `osmium`, `osm2pgsql` (`/opt/homebrew/bin`). `psql` on the host is optional; without it, count rows in the container (step 4).
- PostGIS: `docker compose --profile infra up -d postgis-osm` (port 5436; amd64 emulation on Apple Silicon).
- Neo4j (step 3): `docker compose up -d neo4j` (port 7687).
- Ask before loading into anything but the local `postgis-osm` container: the import drops and recreates the osm2pgsql tables.

## 1. Filter

```bash
cd src/Locations/Locations/SyncLocationsJob/OsmImport/Scripts
./filter-osm.sh <input.osm.pbf> [output-filtered.osm.pbf]
```

Default output: `<input>-filtered.osm.pbf`. Planet ~70 GB → filtered ~5–10 GB.

## 2. Load into PostGIS

```bash
./import-to-postgres.sh <filtered.osm.pbf>
```

- Env: `POSTGRES_HOST` (localhost), `POSTGRES_PORT` (5436), `POSTGRES_DB` (osm_locations), `POSTGRES_USER` (postgres), `POSTGRES_PASSWORD` (postgres).
- Runs `osm2pgsql --create --drop --output=flex`. Tables: `countries`, `regions`, `cities`, `ports`, `railways`, `terminals`, `warehouses`.
- Known gap: exits 0 when `psql` is missing and skips the statistics. Do not trust the exit code; do step 4.

## 3. Sync to Neo4j

```bash
cd src/Locations/Locations/SyncLocationsJob
dotnet run --environment Development   # when appsettings.Development.json exists
```

- Long run: start in the background and read its log. Never start a second copy while one runs.
- Stop only on the user's request. Check with `pgrep -fl SyncLocationsJob`.
- Logs: `TL.Locations.Locations.SyncLocationsTool.Worker`. Trace: `python3 ${CLAUDE_PLUGIN_ROOT}/skills/tl-log-query-runner/scripts/seq_query.py --service sync-locations-job --since 2h`.

## 4. Verify counts

```bash
C=tl-postgis-osm-1
for t in countries regions cities ports railways terminals warehouses; do
  echo -n "$t: "; docker exec $C psql -U postgres -d osm_locations -tAc "select count(*) from $t"
done
docker exec tl-neo4j-1 cypher-shell -u neo4j -p password123 "MATCH (n) RETURN labels(n) AS label, count(*) AS cnt;"
```

- Sync is complete only when the Neo4j `Location` count matches the PostGIS total. If lower, report it as partial with both numbers.
- Last check (2026-11-01): PostGIS 9/405/6240/23/526/19/90; Neo4j `Location` 6381 after the early stop. Partial.
- Report numbers, not "sync done".

## Do not

- Do not commit `*.osm.pbf`, `import-osm.sh` or sync logs in `SyncLocationsJob/` (`osm-locations.md`).
- Do not load into a non-local database without asking.
