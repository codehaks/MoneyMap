# 03 — Data Model & Migrations

## Schema at a glance

PostgreSQL database, schema managed exclusively by EF Core migrations. There are two MoneyMap-specific tables on top of the standard ASP.NET Core Identity schema.

### `Expenses`
| Column | Type | Notes |
| ------ | ---- | ----- |
| `Id` | `int` PK | Identity, auto-increment. |
| `Amount` | `numeric(18,2)` | Precision configured in `ExpenseConfiguration`. |
| `Date` | `timestamp with time zone` | Stored UTC; PageModels call `Date.ToUniversalTime()` on save. |
| `Note` | `varchar(100)` `NOT NULL` | Length enforced at the database (and inside the entity). |
| `CategoryId` | `int` FK → `ExpenseCategories.Id` | `OnDelete: Restrict`. Eager-loaded via typed `Include(e => e.Category)`. |
| `UserId` | `text` `NOT NULL` | Identity user id — used for **all** access control filtering. Indexed; composite `(UserId, Date)` index also exists. |

### `ExpenseCategories`
| Column | Type | Notes |
| ------ | ---- | ----- |
| `Id` | `int` PK | Seeded values are 1–8. |
| `Name` | `string` | Unique by convention only — no DB unique index. |

Eight categories are seeded in `ExpenseCategoryConfiguration.HasData(...)`: Groceries, Rent, Utilities, Transportation, Entertainment, Health, Insurance, Other. Modifying that list **requires a new migration** — EF emits seed data through migrations, not at runtime.

### Identity tables
Standard ASP.NET Core Identity tables (`AspNetUsers`, `AspNetRoles`, `AspNetUserRoles`, `AspNetUserClaims`, etc.) created by `IdentityDbContext<ApplicationUser>`. `ApplicationUser` currently adds no extra columns; commented-out `FirstName`/`LastName` exist as placeholders.

## Conventions when extending the schema

1. **Add or modify a domain entity** under `src/MoneyMap/Core/DataModels/`.
2. **Expose it on the DbContext** if it is a new aggregate (`public DbSet<T> Foo { get; set; }`).
3. **Generate a migration** — names should be PascalCase verbs/short phrases (existing baseline is `Init`):
   ```bash
   dotnet ef migrations add AddBudgetTable \
     --project src/MoneyMap \
     --startup-project src/MoneyMap.Web
   ```
4. **Inspect the generated migration class** before applying. EF will sometimes emit destructive operations (column drops, type changes) that need manual review on a populated database.
5. **Apply** with `dotnet ef database update` (same `--project` / `--startup-project` flags).
6. **Commit both** the migration `.cs` file *and* the updated `MoneyMapDbContextModelSnapshot.cs`. A snapshot diff without the matching migration (or vice-versa) will break later migrations.

## Resetting a development database

```bash
# nukes data; safe only locally
dotnet ef database drop --project src/MoneyMap --startup-project src/MoneyMap.Web -f
dotnet ef database update --project src/MoneyMap --startup-project src/MoneyMap.Web
```

## Why migrations live in `MoneyMap`, not `MoneyMap.Web`

The DbContext is defined in `MoneyMap`, so EF's design-time tooling expects migration assemblies to live alongside it. The `--startup-project` flag points the tool at `MoneyMap.Web` only so it can read the connection string from `appsettings.json` and resolve the configured `DbContextOptions`. Don't move the migrations folder.

## Querying patterns to follow

- All queries that return a single user's data **must** filter by `UserId` inside the service. See `ExpenseService.FindByIdAsync`, `GetAllAsync`, `UpdateAsync`, `RemoveAsync`.
- Use typed `Include(e => e.Category)` when the page needs the category name; nothing in EF will lazy-load.
- Prefer `EF.Functions.Like` over `string.Contains` for case-insensitive search — `string.Contains(StringComparison.OrdinalIgnoreCase)` does not translate to SQL.
- All service methods are `async` and accept a `CancellationToken`. Pass the token from the page handler.
