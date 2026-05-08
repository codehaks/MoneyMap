# 07 — Code Quality Analysis

A review of the existing codebase against **.NET / ASP.NET Core best practices**, **Clean Code**, and **Domain-Driven Design (DDD)** principles. Findings are grouped by severity. Each item gives the offender, why it matters, and a concrete fix.

> Severity scale
> - **Critical** — security, data-integrity, or correctness bug; fix before next deploy.
> - **High** — likely to cause incidents, scaling problems, or recurring bugs.
> - **Medium** — quality / maintainability issues; pay down before the codebase grows.
> - **Low** — style, polish, minor smells.

---

## Critical

### C1. Concurrent disposal risk: `ExpenseService.Update` saves even when the row was not found
`Application/Services/ExpenseService.cs:69-82`

```csharp
public void Update(string userId, Expense expense)
{
    var oldExpense = _db.Expenses.FirstOrDefault(e => e.Id == expense.Id && e.UserId == userId);
    if (oldExpense != null)
    {
        oldExpense.Amount = expense.Amount;
        // ...
    }
    _db.SaveChanges();   // runs unconditionally
}
```

If the expense is not found (deleted by the user in another tab, or `userId` mismatch), the method silently succeeds. The caller has no way to know the update was a no-op, and the DbContext may still flush other tracked changes.

**Fix:** return `bool`/`Result<T>` from the service, return `404` from the page when false, and move `SaveChanges` inside the `if` branch (or throw a domain exception).

### C2. `UserService.GetRoles` dereferences a possibly-null user
`Application/Services/UserService.cs:25-29`

```csharp
var user = await _userManager.FindByIdAsync(userId);
return await _userManager.GetRolesAsync(user);   // NRE if user == null
```

Even though the project has `<Nullable>enable</Nullable>`, the compiler warning is being ignored. A bad `userId` (admin opens a stale link to a deleted user) produces a 500.

**Fix:** null-check and return empty list, or throw `KeyNotFoundException` and 404 in the page.

### C3. Committed development database password
`src/MoneyMap.Web/appsettings.json:10`

```json
"DefaultConnection": "Host=localhost;Port=5432;Database=moneymapdb1;Username=postgres;Password=2385"
```

Real credentials in `appsettings.json` is a hard rule violation even when "only" a dev secret. The `UserSecretsId` is already configured in `MoneyMap.Web.csproj` (line 7) but not used.

**Fix:** move to `dotnet user-secrets`, leave a placeholder (or empty string) in the committed file, document the bootstrap step in `docs/01-getting-started.md`. Rotate the leaked password.

### C4. No HTTPS / HSTS / authentication middleware
`src/MoneyMap.Web/Program.cs:54-61`

```csharp
var app = builder.Build();
app.UseAuthorization();
app.MapRazorPages();
app.MapStaticAssets();
app.Run();
```

Missing: `UseHttpsRedirection`, `UseHsts`, `UseStaticFiles`, **`UseAuthentication`** (`UseAuthorization` without `UseAuthentication` will not populate `User`). It "works" today only because Identity middleware is registered through `AddDefaultUI` and the cookie auth handler runs anyway via the authorization filter — but this is not a guaranteed behavior, and any future change to the pipeline will break login silently.

**Fix:** explicit pipeline:
```csharp
app.UseHsts();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();   // <-- missing today
app.UseAuthorization();
app.MapRazorPages();
```

---

## High

### H1. Domain entities live in `Core/` but contain only data
`Core/DataModels/Expense.cs`, `ExpenseCategory.cs`

DDD-wise these are **anemic models**: pure DTOs with public setters. All behavior (validation of `Amount > 0`, "date not in future", "date not older than 1 year") lives in PageModel `OnPost` handlers. That makes the rules unenforceable from any non-Razor entry point and forces duplication between `Create` and `Edit`.

**Fix:** start small — make setters `private`, expose constructors / factory methods (`Expense.Create(userId, userName, amount, date, categoryId, note)`), and put the validation rules where they belong:
```csharp
public static Expense Create(string userId, string userName, decimal amount,
                             DateTime dateUtc, int categoryId, string note)
{
    if (amount <= 0) throw new DomainException("Amount must be greater than zero.");
    if (dateUtc.Date > DateTime.UtcNow.Date) throw new DomainException("Date cannot be in the future.");
    if (dateUtc < DateTime.UtcNow.AddYears(-1)) throw new DomainException("Date is too old.");
    // ...
}
```
Even without going full DDD, this kills the duplication and protects the invariants.

### H2. `Expense` carries a denormalized `UserName`
`Core/DataModels/Expense.cs:14-15`

```csharp
public string UserId { get; set; } = default!;
public string UserName { get; set; } = default!;
```

Storing both `UserId` (FK) and a string `UserName` violates normalization and DDD's notion of identity. If a user changes their email/username, every old expense still shows the stale value. There is also no FK relationship configured to `AspNetUsers`, so EF will not cascade or constrain.

**Fix:** drop `UserName`, configure a real navigation `public ApplicationUser User { get; set; }` with `HasOne/WithMany` in `OnModelCreating`, and project the username in queries when displaying.

### H3. `MoneyMapDbContext` mixes Identity store and domain — no aggregate boundaries
`Infrastructure/Data/MoneyMapDbContext.cs`

The same DbContext serves Identity + business tables, and has no `IModelConfiguration` / `IEntityTypeConfiguration<>` classes. As schema grows, `OnModelCreating` will become a god-method.

**Fix:** introduce `IEntityTypeConfiguration<Expense>`, `…<ExpenseCategory>` and call `modelBuilder.ApplyConfigurationsFromAssembly(typeof(MoneyMapDbContext).Assembly);` Long-term, consider separating the read model (Identity) from the write model with a second DbContext.

### H4. Synchronous EF I/O on every request
`Application/Services/ExpenseService.cs` (all methods), all PageModels except `Details`

`ToList()`, `FirstOrDefault()`, `SaveChanges()` block the request thread. ASP.NET Core's threadpool exhaustion failure mode is "everything gets slow", which is hard to diagnose in production.

**Fix:** convert to `ToListAsync`, `FirstOrDefaultAsync`, `SaveChangesAsync` and propagate `async Task` up through interfaces and PageModels.

### H5. Hard-coded role string `"admin"`
`src/MoneyMap.Web/Program.cs:37` and the SQL bootstrap in `docs/04`

A single typo (`"Admin"` vs `"admin"`) silently disables admin access. There is no constants class, no enum, and no startup check that the role exists.

**Fix:**
```csharp
public static class Roles { public const string Admin = "admin"; }
```
…and add a `RoleManager`-based seeder that creates the role if missing on startup.

### H6. Cross-field validation duplicated between `Create` and `Edit`
`Areas/Users/Pages/Expenses/Create.cshtml.cs:42-68`, `Edit.cshtml.cs:61-87`

Identical "too old / future date" blocks, identical re-population of `CategorySelectList`, identical model-state branching. ~25 lines duplicated verbatim.

**Fix:** move to a `ExpenseValidator` (or fold into the domain factory in **H1**). Replace the duplicated `CategorySelectList` re-population with a small `private void PopulateCategories()` helper.

### H7. No anti-forgery token check on custom POST handlers
`Areas/Users/Pages/Expenses/Index.cshtml.cs:46` (`OnPostDelete`)

Razor Pages adds anti-forgery automatically for forms, but if anyone introduces an AJAX delete or skips the form helper, the handler will accept any cross-site POST. Add the check now, before someone forgets.

**Fix:** annotate handlers with `[ValidateAntiForgeryToken]` (it's the default for Razor Pages forms but explicit is safer for handler-named POSTs).

### H8. `Areas/Admin/Pages/Categories/Index.cshtml.cs` is empty — feature is undocumented vapor
The README and architecture docs claim "Admin can manage categories" but the page model has no logic at all (just `OnGet()` with an empty body). Either the cshtml does the work (it doesn't, since no service is injected) or the feature is incomplete.

**Fix:** either implement category CRUD via a new `ICategoryService`, or remove the page so the feature surface matches reality.

---

## Medium

### M1. PageModels reach into services for unrelated lookups
`Areas/Admin/Pages/Users/Details.cshtml.cs:26`

```csharp
UserDetails = _userService.GetAll().FirstOrDefault(u => u.Id == id);
```

Pulling **all users** to find one is O(n) memory and round-trip cost on every Details hit. The service is missing a `FindById` method.

**Fix:** add `Task<ApplicationUser?> FindByIdAsync(string id)` to `IUserService` and call it.

### M2. Public mutable properties on entities and PageModels with `= default!;`
Multiple files (e.g. `Note { get; set; } = default!;`)

`= default!;` is a "tell the compiler to shut up" hack. It produces null reference exceptions at runtime when bind-skipped. Required-init properties (`required string Note { get; init; }`) or constructors solve this safely under nullable-enabled.

**Fix:** prefer `required` for entity properties and constructor injection of required PageModel fields.

### M3. `Expense.Amount` has no precision/scale set
`Core/DataModels/Expense.cs:6`

Money should be `decimal(18, 2)` (or whatever the business chooses). Npgsql defaults to `numeric` without precision.

**Fix:** `[Precision(18, 2)]` on the property or `entity.Property(e => e.Amount).HasPrecision(18, 2);` in a configuration; emit a migration.

### M4. `Expense.Note` length not enforced at the database
The `[StringLength(100)]` lives on the PageModel, so it's enforced at the HTTP boundary only. A direct service call could persist a 10MB note. Add `HasMaxLength(100)` and a migration.

### M5. `ExpenseService.GetAll` uses string-based `Include`
`ExpenseService.cs:41`

```csharp
_db.Expenses.Include("Category").Where(...)
```

String-based includes are not refactor-safe. EF can't catch a rename at compile time.

**Fix:** `Include(e => e.Category)`.

### M6. Logging is inconsistent
- `ExpenseService` logs `Debug` events for find operations.
- `Create`, `Update`, `Remove` log nothing — including failed updates that we just fixed (**C1**).
- `UserService`, `CalendarService` and every PageModel log nothing.

**Fix:** add a logging convention — log warnings for not-found / unauthorized branches, info for state changes (create/update/delete), and avoid debug-level chatter on every find.

### M7. `CalendarService` is dead / fake-async
`Application/Services/CalendarService.cs`

- `IsNewYear()` returns `DateTime.Now.Year < 2025` — has been false since Jan 2025.
- `*Async` overloads wrap synchronous logic in `Task.FromResult`, which is misleading and trains readers to ignore async signatures.
- The service is registered but **not consumed** anywhere in the codebase (grep `ICalendarService`).

**Fix:** delete the service and its DI registration if unused; if it is needed, give it a real responsibility and drop the fake async.

### M8. PageModel non-nullable properties initialized after construction
`Areas/Users/Pages/Expenses/Index.cshtml.cs:22`, `Areas/Admin/Pages/Users/Index.cshtml.cs:16`

```csharp
public IList<Expense> ExpenseList { get; set; }   // CS8618 expected
public List<ApplicationUser> Users { get; set; }
```

These are non-nullable but unset until `OnGet`. With `Nullable enable` these properties produce CS8618. The codebase appears to ignore the warnings.

**Fix:** initialize to `[]` / `new List<T>()` or annotate as nullable, and treat warnings as errors (`<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`).

### M9. `[Authorize]` attribute on `Areas/Users/Pages/Expenses/Index.cshtml.cs` and `Edit` is redundant
The folder convention (`AuthorizeAreaFolder("users", "/")`) already requires authentication. The mixed style (some files with `[Authorize]`, some without) makes auditing harder.

**Fix:** pick one mechanism. Since the folder convention is broader, remove the redundant attributes.

### M10. PageModel `[BindProperties]` exposes everything as bindable, including `Id`
`Areas/Users/Pages/Expenses/Index.cshtml.cs:43-44`, and Edit/Create

`[BindProperties]` opts every public setter into model binding, which is the well-known **mass assignment** smell. Pair this with denormalized fields like `UserId` on the entity (from H2) and a malicious form post becomes a vector.

**Fix:** prefer per-property `[BindProperty]` for fields you actually want bound. For Edit, never bind `UserId`/`UserName` — derive from claims server-side (currently done correctly, but only because the entity is rebuilt manually).

### M11. No unit / integration tests
The solution has zero test projects. Service logic, validation rules, and the auth conventions all lack regression coverage.

**Fix:** add `tests/MoneyMap.Tests` with `xUnit` + `WebApplicationFactory<Program>` + Testcontainers for Postgres. Start with `ExpenseService` (filter by `userId`, search, categoryId).

### M12. `data.sql` exists but is undocumented
`src/MoneyMap.Web/data.sql` has no callsite, no comment, no mention in the README. Future developers will not know whether to run it.

**Fix:** delete or document. If it's seed data, prefer EF `HasData` or a startup seeder.

---

## Low

### L1. Magic numbers
Lockout `MaxFailedAccessAttempts = 3`, lockout `10 minutes`, "older than `1 year`" date validation — all literals. Move to named constants or `IOptions<>` if they're ever likely to change.

### L2. Inconsistent namespace style
File-scoped namespaces in `MoneyMap.*` and most `MoneyMap.Web.Areas.Users`, but block-scoped in `Areas/Admin/Pages/Users/Index.cshtml.cs` and `Areas/Admin/Pages/Categories/Index.cshtml.cs`. Pick one (file-scoped is the modern default).

### L3. `using` directives include unused namespaces
`Application/Services/UserService.cs:3-7` imports `System`, `System.Collections.Generic`, `System.Linq`, `System.Text`, `System.Threading.Tasks` — all of which are already in the project's `<ImplicitUsings>` set. `Areas/Users/Pages/Expenses/Edit.cshtml.cs:1` imports `Microsoft.AspNetCore.Authorization` but never uses `[Authorize]`.

**Fix:** run `dotnet format` once and add an `.editorconfig`.

### L4. `// removed code` and TODO comments left as breadcrumbs
- `ApplicationUser.cs` keeps commented-out `FirstName`/`LastName`.
- `ExpenseService.cs:46` has a commented-out `string.Contains` line next to its replacement.
- `IndexModel.cs:50` has `// ExpenseList = _expenseService.GetAll();`.
- `Edit.cshtml.cs:97` and `Create.cshtml.cs:77` carry `//DateTime.UtcNow,` mid-expression.

**Fix:** delete. Source control is the history.

### L5. Inconsistent return types in services
`IUserService.GetRoles` returns `Task<IList<string>>`, but `IExpenseService.GetCategories` returns `List<ExpenseCategory>` (concrete). The standard convention is "expose the smallest interface that satisfies callers" — both should be `IReadOnlyList<T>` or `IList<T>`.

### L6. `GlobalUsings.cs` only globalises one namespace
`MoneyMap.Web/GlobalUsings.cs` lists `MoneyMap.Infrastructure.Data` but not `MoneyMap.Application` or `MoneyMap.Core.DataModels`, even though they're imported in nearly every PageModel. Either add them or remove the file and let `<ImplicitUsings>` plus per-file `using`s do the work consistently.

### L7. Naming inconsistencies
- `MoneyMapDbContext` exposes `Expenses` (plural) and `ExpenseCategories` (plural) — consistent. Good.
- `Areas/Users/Pages/Expenses/Index.cshtml.cs` calls the property `ExpenseList` instead of `Expenses` — inconsistent with the DbSet and creates two names for the same concept in the codebase.

### L8. Empty `wwwroot/` declared in csproj
`MoneyMap.Web.csproj:13-14` has an explicit `<Folder Include="wwwroot\" />` ItemGroup. This was scaffolded when the folder was empty; now that there are real assets it's noise.

### L9. `Identity` package version mismatch
`MoneyMap.csproj:10` references `Microsoft.AspNetCore.Identity 2.3.1` (the legacy 2.x package) while `Microsoft.AspNetCore.Identity.EntityFrameworkCore 9.0.8` is the current package. The 2.3.1 reference is almost certainly unnecessary — the EF package transitively pulls the right Identity types.

**Fix:** remove the legacy `2.3.1` reference and verify the build.

### L10. `data.sql` and the `Dockerfile` `COPY . .` pattern will bake everything into the image
The `Dockerfile` (per README example) copies the whole repo. If `data.sql`, `appsettings.json` with creds, or local secrets exist they end up in the layer history. Add a `.dockerignore`.

---

## Suggested order of attack

1. **C3** (rotate creds, move to user-secrets), **C4** (fix the pipeline), **C2** (null check), **C1** (Update silent failure) — these are short, mechanical fixes with outsized risk reduction.
2. **H1 + H6** together — moving validation into the domain entity removes the duplication in one shot and unblocks tests.
3. **M11** — add a test project before doing any of the larger refactors below.
4. **H4** — async migration; tedious but prerequisite for any real load.
5. **H2 + H3 + M3 + M4** — schema cleanup as a single migration ("Money domain hardening").
6. Sweep **M-** and **L-** items with `dotnet format` + a code-review-first PR.

## What to keep — strengths in the current code

- Clear layering with no circular references between `Core / Application / Infrastructure / Web`.
- Authorization is centralized in `Program.cs` rather than scattered across attributes — easy to audit.
- Per-row authorization in `ExpenseService` (`UserId` filter on every method) is the right pattern.
- DI is small and explicit; no service locator anti-pattern.
- Folder structure mirrors namespaces consistently.

These patterns are worth preserving as the rest of the code is hardened.
