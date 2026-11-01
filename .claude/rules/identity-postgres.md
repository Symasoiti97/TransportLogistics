---
paths:
  - "src/Socials/Identity/Infrastructure/DataAccess/Identity.Postgres/**"
  - "src/Socials/Identity/Startups/Identity.Api/**"
---

# Identity: migrations and DB access

- `BloggingContextFactory` (`IdentityDbContext.cs`) hardcodes `Host=localhost;Port=5432`. The `ConnectionStrings__IdentityPostgres` setting does not affect design-time `dotnet ef` commands, only runtime DI.
- The local Postgres on `localhost:5432` is the Docker container `postgres`. It hosts other projects' databases (`ocr-db`, `shop`, `netsuite-refdata`, etc.). Do not run `dotnet ef database update` without user confirmation and without checking the port and server first: the command writes to the hardcoded connection string, i.e. the shared container.
- Apply migrations with `dotnet ef migrations script --idempotent -o <path>.sql` (generates the script without connecting to the DB), then apply the SQL manually: `docker exec -i postgres psql -U postgres -d <db> < <path>.sql`. Verify the target database first.
- Create new migrations with the EF tool: `dotnet ef migrations add <Name>` from `Identity.Postgres` (pass `--startup-project Identity.Api` if it fails without it). This command does not connect to the DB.
- Do not delete or modify existing migrations in `Migrations/`: they are already applied to databases.
