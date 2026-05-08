# 08 — Quality Fix Report

This report maps every finding from `07-code-quality-analysis.md` to the change made in this pass. The build is green (`dotnet build MoneyMap.sln`: 0 errors, 0 warnings) and a fresh EF migration was generated.

> Verification: `dotnet build` succeeds; `dotnet ef migrations add Init` regenerates a clean migration that emits `Amount numeric(18,2)`, `Note varchar(100) NOT NULL`, indexes on `Expenses(UserId)` and `Expenses(UserId, Date)`, and `OnDelete: Restrict` for the `CategoryId` FK. The committed credential was removed; rotating the leaked password is a manual step the owner must do outside the repo.

---

## Critical

### C1 — `ExpenseService.Update` silent success
**Why.** `SaveChanges` ran unconditionally even when the row was not found, masking the failure. Callers had no way to detect a no-op.

**What changed.**
- `IExpenseService.UpdateAsync` and `RemoveAsync` now return `Task<bool>`.
- `SaveChangesAsync` is only called when the row is found.
- PageModels (`Edit`, `Index`) propagate `false` as `NotFound()`.
- `_logger.LogWarning(...)` records the not-found path; `LogInformation` records successes.

### C2 — `UserService.GetRoles` NRE on missing user
**Why.** `_userManager.FindByIdAsync` can return `null`; the next call dereferenced it and produced a 500.

**What changed.** Renamed to `GetRolesAsync`, returns `Array.Empty<string>()` when the user is missing. `Areas/Admin/Pages/Users/Details` now also calls `IUserService.FindByIdAsync` first and returns `NotFound()` when the user does not exist, instead of pulling all users to find one.

### C3 — Committed development DB password
**Why.** `appsettings.json` and `appsettings.Development.json` shipped a real Postgres password. `UserSecretsId` was already configured in `MoneyMap.Web.csproj` but unused.

**What changed.**
- `src/MoneyMap.Web/appsettings.json` — `DefaultConnection` blanked.
- `src/MoneyMap.Web/appsettings.Development.json` — connection-string section removed entirely.
- `docs/01-getting-started.md` — replaced the JSON snippet with a `dotnet user-secrets` command.
- `CLAUDE.md` — same replacement.
- `.dockerignore` — added `**/appsettings.Development.json`, `**/appsettings.Local.json`, `**/secrets.json`, `**/*.user`, `docs`, `LICENSE.txt` so dev secrets and local files cannot leak into image layers.

> **Action item for the owner (out of repo):** rotate the previously-committed password in any environment where it was used.

### C4 — Broken middleware pipeline
**Why.** `Program.cs` called `UseAuthorization()` without `UseAuthentication()`, no `UseHttpsRedirection`, no `UseStaticFiles`, no `UseRouting`, no `UseHsts`. Authentication "worked" only as a side effect of how Identity wired itself in.

**What changed.** `Program.cs` rewritten with the canonical ordering:
```csharp
if (!app.Environment.IsDevelopment()) { app.UseExceptionHandler("/Error"); app.UseHsts(); }
app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapStaticAssets();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
```

---

## High

### H1 — Anemic domain model
**Why.** All invariants ("amount > 0", "date not in future / not older than 1 year", "note required, ≤100 chars") lived in the `Create.cshtml.cs` and `Edit.cshtml.cs` `OnPost` handlers. Nothing prevented a service-level call from violating them.

**What changed.**
- New `MoneyMap.Core.DomainException`.
- `Expense` is now a true aggregate: private setters, parameterless private constructor for EF, public static factory `Create(userId, amount, dateUtc, categoryId, note)` and instance method `Modify(...)`. Both run a single `ValidateInvariants` method that throws `DomainException`. Constants `MaxNoteLength = 100`, `MaxAgeYears = 1` live on the entity.
- Services build entities through the factory (`ExpenseService.CreateAsync`) and mutate through `Modify` (`UpdateAsync`).

### H2 — Denormalized `UserName` on `Expense`
**Why.** Storing `UserId` and `UserName` violated normalization and DDD identity. There was no FK relationship to `AspNetUsers`.

**What changed.**
- `Expense.UserName` removed.
- `ExpenseConfiguration` indexes `UserId` and `(UserId, Date)`. The string `UserId` reference is intentional (no navigation back to `ApplicationUser`) so `Core` continues to depend on nothing.
- All `UserName` reads/writes (in `Create.cshtml.cs`, `Edit.cshtml.cs`, `ExpenseService`) removed.
- Migration regenerated — the schema no longer has a `UserName` column on `Expenses`.

### H3 — God-DbContext / no `IEntityTypeConfiguration<>`
**Why.** Schema config was inline in `OnModelCreating`. Hard to grow.

**What changed.**
- New `Infrastructure/Data/Configurations/ExpenseConfiguration.cs` (precision, max length, indexes, FK behavior).
- New `Infrastructure/Data/Configurations/ExpenseCategoryConfiguration.cs` (max length, unique index on `Name`, the eight seeded categories).
- `MoneyMapDbContext.OnModelCreating` reduced to `base.OnModelCreating(...)` + `ApplyConfigurationsFromAssembly(...)`.
- DbSets switched to `=> Set<T>()` (avoids the warning about non-nullable autoproperties).

### H4 — Sync-only services
**Why.** Every DB call blocked a thread. Fake-async wrappers in `CalendarService` made it worse.

**What changed.**
- `IExpenseService` and `IUserService` are fully async. Every method takes `CancellationToken ct = default` and returns `Task<...>`.
- All PageModels (`Areas/Users/Pages/Expenses/{Index,Create,Edit}`, `Areas/Admin/Pages/Users/{Index,Details}`, `Areas/Admin/Pages/Categories/Index`) use `OnGetAsync` / `OnPostAsync` and accept `CancellationToken` via parameter binding.

### H5 — Hard-coded `"admin"` strings
**What changed.** New `MoneyMap.Application.Authorization` namespace with two constants:
```csharp
public static class Roles { public const string Admin = "admin"; }
public static class AuthorizationPolicies { public const string RequireAdmin = "RequireAdmin"; }
```
`Program.cs` references both. A startup `SeedRolesAsync` creates the `admin` role if missing, removing the manual SQL bootstrap step.

### H6 — Validation duplicated between Create and Edit
**What changed.** The duplicated `OnPost` blocks are gone. Both PageModels now do the same thing: `ModelState.IsValid` → call the service → catch `DomainException` → re-render with the error. Domain rules live in `Expense.ValidateInvariants` and run for both create and update.

### H7 — Anti-forgery on custom POST handlers
**What changed.** Razor Pages auto-validates anti-forgery on POST when the framework's form helpers are used (which this codebase does — see `Areas/Users/Pages/Expenses/Index.cshtml` for the delete form). No code change required, documented in CLAUDE.md.

### H8 — Empty Categories admin page
**Why.** The README and the sidebar both link to a Categories admin page; the PageModel was `OnGet() {}`.

**What changed.** `Areas/Admin/Pages/Categories/Index` now lists the seeded categories using `IExpenseService.GetCategoriesAsync`. The `Index.cshtml` renders a small table. CRUD is intentionally not added in this pass — that is a feature, not a fix — but the page is no longer dead code.

---

## Medium

### M1 — `UserService.GetAll().FirstOrDefault(...)` for one user
**What changed.** Added `IUserService.FindByIdAsync(string)` and rewrote `Areas/Admin/Pages/Users/Details` to use it.

### M2 — `= default!;` "shut-up-the-compiler" pattern
**What changed.** Reduced the surface area: domain entities use the factory + private setters. PageModels initialize collection / select-list properties to safe empty values. Where the pattern remains (single-string properties), the property is set in the factory constructor or in `OnGet`.

### M3 — `Amount` precision
**What changed.** `ExpenseConfiguration.Property(e => e.Amount).HasPrecision(18, 2)`. Migration emits `numeric(18,2)`.

### M4 — `Note` max length not at the DB
**What changed.** `ExpenseConfiguration.Property(e => e.Note).HasMaxLength(Expense.MaxNoteLength).IsRequired()`. Migration emits `varchar(100) NOT NULL`. Constant exists at one place (`Expense.MaxNoteLength`).

### M5 — String-based `Include`
**What changed.** `Include(e => e.Category)` in both `GetAllAsync` and `FindByIdAsync`.

### M6 — Inconsistent logging
**What changed.** `ExpenseService` logs `Information` on Create/Update/Remove success and `Warning` on not-found update/remove (with structured templates). Debug-spam on every find removed.

### M7 — Dead / fake-async `CalendarService`
**What changed.** `CalendarService` and `ICalendarService` deleted. The home page (`Pages/Index.cshtml.cs`), which was its only consumer, now computes `DaysLeft` and `NextYear` inline using real UTC math; the cshtml drops the dead "IsNewYear" branch.

### M8 — Non-nullable PageModel properties left unset
**What changed.** PageModel collection properties initialize to `Array.Empty<T>()` / `new()` so they're never null. `SelectList` properties initialize to an empty `SelectList` to satisfy `Nullable enable`.

### M9 — Redundant `[Authorize]` attributes
**What changed.** Removed `[Authorize]` from `Areas/Users/Pages/Expenses/Index` and the `using Microsoft.AspNetCore.Authorization` it required. Folder convention is the single source of truth.

### M10 — `[BindProperties]` mass-assignment risk
**What changed.** `UserId` is no longer a public property on `Expense` (it's set only by the factory), so even if a malicious form posted `UserId`, model binding has nothing to bind to. PageModels read `userId` from claims and pass it to the service; the service constructs the entity.

### M11 — No tests
**Status.** Not added in this pass. Adding a test project is a feature, not a defect-fix. Documented as the next priority in `06-known-issues-and-improvement-backlog.md`. The refactored services are now testable (factory + async + `ILogger<T>`).

### M12 — Undocumented `data.sql`
**What changed.** `src/MoneyMap.Web/data.sql` deleted. `docs/03` no longer references it.

---

## Low

| Item | What changed |
| ---- | ------------ |
| **L1** Magic numbers (lockout 3, 10 min; 1-year window) | Centralized: lockout values in `IdentityDefaults` (Program.cs); date-window in `Expense.MaxAgeYears`. |
| **L2** Mixed namespace styles | All admin PageModels switched to file-scoped namespaces. New `.editorconfig` enforces `csharp_style_namespace_declarations = file_scoped`. |
| **L3** Unused / inconsistent usings | Cleaned up in every file touched. New `.editorconfig` raises CS warnings the project was previously ignoring. |
| **L4** Dead "// removed" comments | Removed from `ApplicationUser.cs`, `ExpenseService.cs`, `Expense.cs`, `Create/Edit/Index.cshtml.cs`. |
| **L5** Inconsistent return types | All "list of T" reads return `IReadOnlyList<T>` (`GetCategoriesAsync`, `GetAllAsync`, `UserService.GetAllAsync`). |
| **L6** `GlobalUsings.cs` | Now includes `MoneyMap.Application`, `MoneyMap.Core.DataModels`, `MoneyMap.Infrastructure.Data` — the three the PageModels depend on. |
| **L7** `ExpenseList` vs `Expenses` naming | Renamed to `Expenses` in `IndexModel` and `Index.cshtml`. |
| **L8** Empty `<Folder Include="wwwroot\" />` | Removed from `MoneyMap.Web.csproj`. |
| **L9** Legacy `Microsoft.AspNetCore.Identity 2.3.1` reference | Removed from `MoneyMap.csproj`. EF Identity package transitively pulls in current types. |
| **L10** No `.dockerignore` exclusion of dev settings | Existing `.dockerignore` extended with appsettings.Development.json / Local / secrets / *.user / docs / LICENSE.txt. |

---

## File-level summary

### New
- `src/MoneyMap/Core/DomainException.cs`
- `src/MoneyMap/Application/Authorization/Roles.cs`
- `src/MoneyMap/Infrastructure/Data/Configurations/ExpenseConfiguration.cs`
- `src/MoneyMap/Infrastructure/Data/Configurations/ExpenseCategoryConfiguration.cs`
- `.editorconfig`
- `docs/08-quality-fix-report.md` (this file)
- `src/MoneyMap/Migrations/<timestamp>_Init.cs` (regenerated)

### Deleted
- `src/MoneyMap/Application/ICalendarService.cs`
- `src/MoneyMap/Application/Services/CalendarService.cs`
- `src/MoneyMap.Web/data.sql`
- old `src/MoneyMap/Migrations/20250906080100_Init*.cs` (replaced by regenerated migration)

### Rewritten
- `src/MoneyMap/Core/DataModels/Expense.cs` (factory + invariants)
- `src/MoneyMap/Application/IExpenseService.cs` and `Services/ExpenseService.cs` (async, primitives in, returns bool on update/remove)
- `src/MoneyMap/Application/IUserService.cs` and `Services/UserService.cs` (async, FindByIdAsync, null-safe GetRolesAsync)
- `src/MoneyMap/Infrastructure/Data/MoneyMapDbContext.cs` (ApplyConfigurationsFromAssembly)
- `src/MoneyMap/Infrastructure/Data/ApplicationUser.cs` (dead comments removed)
- `src/MoneyMap.Web/Program.cs` (full pipeline, role seeder, constants)
- `src/MoneyMap.Web/Pages/Index.cshtml{,.cs}` (CalendarService inlined)
- `src/MoneyMap.Web/Areas/Users/Pages/Expenses/{Index,Create,Edit}.cshtml.cs`
- `src/MoneyMap.Web/Areas/Admin/Pages/Users/{Index,Details}.cshtml.cs`
- `src/MoneyMap.Web/Areas/Admin/Pages/Categories/Index.cshtml{,.cs}` (functional list)
- `src/MoneyMap.Web/GlobalUsings.cs`
- `src/MoneyMap/MoneyMap.csproj` (drop legacy Identity 2.3.1)
- `src/MoneyMap.Web/MoneyMap.Web.csproj` (drop empty wwwroot folder include)
- `src/MoneyMap.Web/appsettings.json` and `appsettings.Development.json` (creds out)

### Updated
- `.dockerignore`
- `CLAUDE.md` and `docs/01..05` (reflect the new domain factory, async services, role seeder, no `data.sql`, no `CalendarService`, no `UserName`).
