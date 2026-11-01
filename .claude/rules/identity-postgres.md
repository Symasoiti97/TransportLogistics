---
paths:
  - "src/Socials/Identity/Infrastructure/DataAccess/Identity.Postgres/**"
  - "src/Socials/Identity/Startups/Identity.Api/**"
---

# Identity: миграции и подключение к БД

- `BloggingContextFactory` (`IdentityDbContext.cs`) захардкодил `Host=localhost;Port=5432`. Строка `ConnectionStrings__IdentityPostgres` на design-time команды `dotnet ef` не влияет — только на рантайм через DI.
- Локальный Postgres на `localhost:5432` — Docker-контейнер `postgres`, в нём базы других проектов (`ocr-db`, `shop`, `netsuite-refdata` и др.). Не выполнять `dotnet ef database update` без явного подтверждения пользователя: команда пишет в захардкоженную строку, то есть в этот общий контейнер.
- Миграции накатывать так: `dotnet ef migrations script --idempotent -o <путь>.sql` (скрипт генерируется без подключения к БД), затем применить SQL вручную через `docker exec -i postgres psql -U postgres -d <база> < <путь>.sql`, предварительно проверив, что это нужная база.
- Перед любой записью в БД проверить порт и имя сервера, куда она пойдёт.
- Новые миграции создавать через EF-тул: `dotnet ef migrations add <Имя>` из `Identity.Postgres` (нужен `--startup-project` на `Identity.Api`, если он не проходит без него). Команда не подключается к БД.
- Существующие миграции в `Migrations/` не удалять и не менять: применённые уже в базах.
