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

The connection string is **not** committed. Set it via user-secrets:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" \
  "Host=localhost;Port=5432;Database=moneymapdb1;Username=postgres;Password=..." \
  --project src/MoneyMap.Web
```

There is no test project in the solution; `dotnet test` will be a no-op until one is added.

## Architecture

Two C# projects under `src/`:

- **`MoneyMap`** — class library, layered into `Core/` (domain entities + `DomainException`), `Application/` (service interfaces + implementations: `ExpenseService`, `UserService`; `Authorization/Roles` and `AuthorizationPolicies` constants), `Infrastructure/Data/` (`MoneyMapDbContext`, `ApplicationUser`, `Configurations/IEntityTypeConfiguration<>` classes), and `Migrations/`.
- **`MoneyMap.Web`** — ASP.NET Core 9.0 Razor Pages host. `Program.cs` is the only composition root: it registers the Npgsql `MoneyMapDbContext`, configures Identity (`ApplicationUser` + `IdentityRole`, lockout after 3 failed attempts for 10 minutes), defines the `RequireAdmin` policy, registers application services as **transient**, applies area-folder authorization conventions, and seeds the `admin` role on startup.

### Domain model

`Expense` is a small aggregate with private setters and a static factory: `Expense.Create(userId, amount, dateUtc, categoryId, note)`. Invariants — amount > 0, date not in future, date not older than `MaxAgeYears`, `Note` non-empty and ≤ `MaxNoteLength` (100) — are enforced inside the entity and throw `DomainException`. Mutation goes through `Modify(...)`. **Do not bypass the factory** — services and PageModels should always go through it so the rules can't be skipped.

`ExpenseCategory` is a simple aggregate (no behavior). Categories are seeded by `ExpenseCategoryConfiguration.HasData(...)`.

### Authorization model (set in `Program.cs`)

Authorization is applied by area folder convention, not per-page attributes:

- `Areas/Users/**` requires authentication.
- `Areas/Admin/**` requires the `admin` role (policy `AuthorizationPolicies.RequireAdmin`).
- `Areas/Identity/**` and root `Pages/` are public.

When adding new pages, place them in the correct area to inherit the right policy — don't add `[Authorize]` attributes ad hoc. Reference `Roles.Admin` and `AuthorizationPolicies.RequireAdmin` constants from `MoneyMap.Application.Authorization` rather than hardcoding strings.

### Per-row authorization

`IExpenseService` methods take `userId` first and filter every query by `UserId`. PageModels read it from claims:

```csharp
var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
```

Never trust a `userId` from a form post.

### Database

PostgreSQL via Npgsql. Schema is configured through `IEntityTypeConfiguration<>` classes under `Infrastructure/Data/Configurations/` and applied by `ApplyConfigurationsFromAssembly` in `OnModelCreating`. `Expense.Amount` is `decimal(18,2)`; `Expense.Note` is `varchar(100)` and required. There is an index on `Expenses(UserId)` and a composite index on `Expenses(UserId, Date)`.

## Conventions

- Adding a feature typically touches: `Core/DataModels` entity → `Configurations/IEntityTypeConfiguration<T>` → migration → `Application` service interface + impl → DI registration in `Program.cs` → Razor Pages under the appropriate `Areas/*/Pages` folder.
- Services are registered **transient**; the `DbContext` is scoped (default `AddDbContext`).
- All service methods that touch I/O are `async` and accept a `CancellationToken`. Match this style.
- Page handlers use `OnGetAsync` / `OnPostAsync` and pass `HttpContext.RequestAborted` (or accept `CancellationToken` via parameter binding).
- Use `Roles.Admin` / `AuthorizationPolicies.RequireAdmin` constants — no hardcoded role strings outside `Authorization/Roles.cs`.
- Use typed `Include(e => e.Foo)`, never the string overload.
