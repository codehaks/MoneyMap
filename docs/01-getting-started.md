# 01 — Getting Started

Read this first if you have never built or run MoneyMap locally. It is the shortest path from a fresh clone to a running app.

## Prerequisites

- **.NET 9.0 SDK** — the solution targets `net9.0`. Older SDKs will fail to restore.
- **PostgreSQL 13+** — the only supported database (Npgsql provider). SQL Server / SQLite are *not* configured.
- **`dotnet-ef` global tool** for migrations:
  ```bash
  dotnet tool install --global dotnet-ef
  ```
- An IDE that understands Razor Pages (Visual Studio 2022, Rider, or VS Code with the C# Dev Kit).

## Repository layout (top level)

| Path | Purpose |
| ---- | ------- |
| `MoneyMap.sln` | Solution wiring `MoneyMap` and `MoneyMap.Web`. |
| `src/MoneyMap/` | Class library: domain, services, DbContext, migrations. |
| `src/MoneyMap.Web/` | ASP.NET Core 9 Razor Pages host (composition root). |
| `src/MoneyMap.Web/data.sql` | Optional seed SQL for ad-hoc data. |
| `Dockerfile` | Multi-stage build for the Web project. |
| `docs/` | Developer documentation (this folder). |

## Configure the database

Edit `src/MoneyMap.Web/appsettings.Development.json` (preferred) or override with the env var
`ConnectionStrings__DefaultConnection`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=moneymapdb1;Username=postgres;Password=changeme"
  }
}
```

The committed `appsettings.json` contains a development-only password. **Do not deploy with that file unchanged.** See `04-authentication-and-authorization.md` and `06-known-issues-and-improvement-backlog.md`.

## First-run sequence

```bash
# from repo root
dotnet restore
dotnet ef database update --project src/MoneyMap --startup-project src/MoneyMap.Web
dotnet run --project src/MoneyMap.Web
```

The migration step creates the Identity tables, `Expenses`, `ExpenseCategories`, and seeds the eight default categories (Groceries, Rent, Utilities, Transportation, Entertainment, Health, Insurance, Other).

By default Kestrel binds to `https://localhost:5001` / `http://localhost:5000`. Register a new user from the UI; the first user has no special role — see `04-authentication-and-authorization.md` for promoting a user to `admin`.

## Useful commands

```bash
# build everything
dotnet build MoneyMap.sln

# run the web app with file watcher
dotnet watch --project src/MoneyMap.Web

# publish a release build
dotnet publish src/MoneyMap.Web -c Release -o ./publish

# build container image
docker build -t moneymap .
```

There is currently **no test project** in the solution. `dotnet test` is a no-op until one is added (tracked in `06-known-issues-and-improvement-backlog.md`).
