---
paths:
  - "src/Locations/**"
---

# Locations: подготовка OSM

- Скрипты в `src/Locations/Locations/SyncLocationsJob/OsmImport/Scripts`: `filter-osm.sh` → `import-to-postgres.sh` → `sync-locations.sh`. Нужны `osmium-tool` и `osm2pgsql`. Подробности: `OsmImport/README.md` в той же папке.
- Конфиг подключений: `OsmPostgres` (база `osm_locations`, порт 5436, контейнер `postgis-osm`, профиль `infra`), `Neo4JSettings`.
- Корневые `import-osm.sh` и `*.osm.pbf` в `SyncLocationsJob/` — локальные неотслеживаемые артефакты, не коммитить.
