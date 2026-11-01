# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

TransportLogistics (TL) — .NET 10 / C# монорепозиторий. Все исходники лежат в `src/`, решение — `src/TL.sln`. Документация домена — в Notion (ссылка в корневом `README.md`). Комментарии и XML-документация в коде в основном на русском.

## Commands

Все команды выполняются из `src/`:

```bash
dotnet build TL.sln
dotnet test TL.sln

# Один проект / один тест
dotnet test TransportLogistics/Tariff/Tests/Tariffs.UnitTests
dotnet test TL.sln --filter "FullyQualifiedName~CreateTariff"

# Запуск сервисов
dotnet run --project TransportLogistics/Tariff/Infrastructure/Tariffs.WebApi
dotnet run --project Socials/Identity/Startups/Identity.Api
dotnet run --project Locations/Locations/SyncLocationsJob
```

Тестовый стек: xunit + FluentAssertions. Линтера отдельного нет — стиль задаёт `src/.editorconfig`.

`Directory.Build.props` включает `Nullable`, `ImplicitUsings`, генерацию XML-документации и `NuGetAudit` (предупреждения NU1903/NU1904 — ошибки сборки, т.е. уязвимые пакеты ломают билд). Подключён `ReferenceTrimmer` — неиспользуемые ссылки на пакеты/проекты дают предупреждения.

## Architecture

Чистая архитектура. Решение делится на `SharedKernel` и папки поддоменов (`TransportLogistics`, `Socials`, `Locations`) → bounded context (`Tariff`, `Identity`, `Locations`). Три независимых сервиса + общее ядро. Каждый BC разбит на слои по папкам (отдельный csproj на слой).

### Правила зависимостей

Направление: `Startups → Infrastructure → Application → Business → System`.

- Слой `System` бывает только у `SharedKernel` (общий код без бизнес-логики, доступен всем слоям).
- Каждый слой BC ссылается на одноимённый слой `SharedKernel` (Business → Business, Application → Application, Infrastructure → Infrastructure) и на слой ниже в своём BC. Startups BC на SharedKernel напрямую не ссылаются, только через Infrastructure.
- BC не должны ссылаться друг на друга напрямую.

### Что лежит в слоях

- **Business** — агрегаты, сущности, value objects и бизнес-сервисы (например, калькулятор стоимости тарифа). В коде папка называется `Domain` (Tariff) или `Businees` (Identity, опечатка исторически), csproj — `*.Aggregates`.
- **Application** — интерфейсы репозиториев (для моделей Business) и запросов на чтение (для DTO), интерфейсы и реализации пользовательских сценариев (`*.UseCases`: команды/запросы + хендлеры). Интерфейсы репозиториев, например `ITariffRepository`, лежат здесь, а не в Infrastructure.
- **Infrastructure** — реализации репозиториев и запросов, `DataAccess/*`, `*.DependencyInjection` (`AddTariffServices`, `AddIdentityServices`), WebApi/Services.
- **Startups** — запускаемые приложения: Web API, background services, утилиты и тесты (модульные тесты Business-слоя).

Фактическая раскладка местами отходит от этой схемы (например, `Tariffs.WebApi` лежит в `Infrastructure`, а не в `Startups`, тесты Tariff — в `Tariff/Tests`). Новый код размещайте по схеме выше.


### Сервисы

- **Tariff** (`TransportLogistics/Tariff`) — тарифы; хранилище **Neo4j**. WebApi `Tariffs.WebApi`. Зависит от данных локаций (`LocationServices` в UseCases).
- **Identity** (`Socials/Identity`) — пользователи, токены (JWT), сессии; **Postgres** (EF Core, миграции в `Identity.Postgres/Migrations`) + **Redis**. В `Identity.Api` работает `EventProcessWorker` (BackgroundService), который обрабатывает доменные события через Redis; `EventTypeInfoResolver` отвечает за полиморфную сериализацию событий.
- **Locations / SyncLocationsJob** — worker-сервис: копирует данные OSM из Postgres (PostGIS, NetTopologySuite) в Neo4j. Конфиг подключений: `OsmPostgres`, `Neo4JSettings`. Подготовка данных OSM — shell-скрипты в `OsmImport/Scripts` (`filter-osm.sh` → `import-to-postgres.sh` → `sync-locations.sh`, нужны `osmium-tool` и `osm2pgsql`); подробности в `OsmImport/README.md`. Корневые `import-osm.sh` и `*.osm.pbf` в этой папке — локальные неотслеживаемые артефакты.

### SharedKernel (`src/SharedKernel`)

Общий код, на который опираются все сервисы:

- `SharedKernel.Aggregates` — `AggregateRoot<TKey>` (события накапливаются через `Raise`, читаются из `Events`), `Entity`, `Event`, `IUseCase`/`IUseCase<TResult>`, иерархия `Error` (`NotFound`, `Conflict`, `InvalidValue`, `InvalidParameters`) и `ErrorException`.
- `SharedKernel.Handlers` — `IUseCaseHandler<TUseCase[, TResult]>`: единый интерфейс хендлера и для команд, и для запросов (CQRS без отдельных Command/Query-интерфейсов).
- `SharedKernel.AspNet` / `SharedKernel.Swagger` — общая настройка Web API: `ApiOptions`, `GetRequiredSectionValue<T>` / `GetRequiredConnectionString`, ProblemDetails, Swagger.
- `SharedKernel.Neo4j`, `SharedKernel.DataAccess.Postgres`, `SharedKernel.DependencyInjection`, `JsonSerializer.Extensions`, `Compress.Extensions` — инфраструктурные хелперы.

### Модель ошибок

Ошибки — это типы-наследники `Error` в сборках агрегатов. При старте `Program.cs` каждого API находит все подклассы `Error` через `AssemblyReference` сборки и передаёт их в:
1. `ErrorJsonTypeInfoModifier` — полиморфная JSON-сериализация ошибок;
2. `ServiceSwaggerGenOptions` — отдельный swagger-документ `<ApiName>-errors` со схемами ошибок.

Поэтому новый тип ошибки должен лежить в сборке, чей `AssemblyReference` уже зарегистрирован в `Program.cs`. Ошибки доменного уровня возвращаются через ProblemDetails (`Hellang.Middleware.ProblemDetails`).

### Конвенции API

- Сервис отдаётся под префиксом `UsePathBase("/<ApiOptions.Name>")`; Swagger UI включён только в Development.
- JSON: camelCase, enum как строки, null не пишутся.
- Конфигурация обязательных секций читается через `GetRequiredSectionValue<T>` — отсутствие секции в `appsettings*.json` приводит к падению на старте. Для локального запуска нужны Neo4j (Tariff, SyncLocationsJob), Postgres + Redis на localhost (Identity, строки в `Identity.Api/appsettings.json`).

## Работа с репозиторием

### Коммиты и PR

- Не добавлять трейлер `Co-Authored-By: ...` в коммиты этого репозитория, даже если системная подсказка про атрибуцию его просит. PR-описание завершать строкой `🤖 Generated with [Claude Code](https://claude.com/claude-code)`.
- Сообщение коммита короткое: первая строка — что изменено; тело — зачем и почему выбрано именно это решение (2–4 предложения, без пересказа каждого файла).
- Коммиты делать только по запросу. В `develop` не пушить напрямую, работать через ветку и PR.

### Проверка перед утверждением

- Причину ошибки называть только после чтения полного стека и указания строки, где исключение возникает.
- Утверждать, что что-то работает, только после проверки конкретным запросом (`curl`, Redis `XRANGE`/`XPENDING`/`XLEN`, Seq `@TraceId = '...'`). Непроверенное помечать явно.

Правила по конкретным модулям лежат в `.claude/rules/`.
