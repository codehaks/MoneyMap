# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

All commands run from the repo root unless noted.

```bash
# Restore / build / run
dotnet restore
dotnet build MoneyMap.sln
dotnet run --project src/MoneyMap.Web

# Publish (Release)
dotnet publish src/MoneyMap.Web -c Release -o ./publish

# Docker
docker build -t moneymap .
```

EF Core migrations target the `MoneyMap` class library project but use `MoneyMap.Web` as the startup project (where the DbContext is wired up and the connection string lives):

```bash
dotnet ef migrations add <Name> --project src/MoneyMap --startup-project src/MoneyMap.Web
dotnet ef database update --project src/MoneyMap --startup-project src/MoneyMap.Web
dotnet ef migrations remove --project src/MoneyMap --startup-project src/MoneyMap.Web
```

There is no test project in the solution; `dotnet test` will be a no-op until one is added.

## Architecture

Two C# projects under `src/`:

- **`MoneyMap`** — class library, layered into `Core/` (domain entities), `Application/` (service interfaces + implementations: `ExpenseService`, `CalendarService`, `UserService`), `Infrastructure/Data/` (`MoneyMapDbContext`, `ApplicationUser`), and `Migrations/`.
- **`MoneyMap.Web`** — ASP.NET Core 9.0 Razor Pages host. `Program.cs` is the only composition root: it registers the Npgsql `MoneyMapDbContext`, configures Identity (`ApplicationUser` + `IdentityRole`, lockout after 3 failed attempts), defines the `RequireAdminRole` policy, registers the three application services as **transient**, and applies area-folder authorization conventions.

### Authorization model (set in `Program.cs`)

Authorization is applied by area folder convention, not per-page attributes:

- `Areas/Users/**` requires authentication.
- `Areas/Admin/**` requires the `admin` role (`RequireAdminRole` policy).
- `Areas/Identity/**` and root `Pages/` are public.

When adding new pages, place them in the correct area to inherit the right policy — don't add `[Authorize]` attributes ad hoc.

### Data model

`MoneyMapDbContext` extends `IdentityDbContext<ApplicationUser>` and owns `Expenses` and `ExpenseCategories`. Eight default categories (Groceries, Rent, Utilities, Transportation, Entertainment, Health, Insurance, Other) are seeded via `HasData` in `OnModelCreating` — changes to that seed list require a new migration. `Expense` carries both `UserId` **and** denormalized `UserName` fields for user association; keep both in sync when creating expenses.

### Database

PostgreSQL via Npgsql. Connection string `DefaultConnection` is read from `appsettings.json` (or the `ConnectionStrings__DefaultConnection` env var). `src/MoneyMap.Web/data.sql` exists for seeding.

## Conventions

- Adding a new feature typically touches: a `Core/DataModels` entity → `MoneyMapDbContext` DbSet → migration → `Application` service interface + impl → DI registration in `Program.cs` → Razor Pages under the appropriate `Areas/*/Pages` folder.
- Services are registered transient; don't introduce singleton state in them without coordination with the DbContext lifetime.
