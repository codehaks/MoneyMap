# 05 — Adding Features & Code Conventions

This is the playbook for extending MoneyMap. Follow it and your change will fit the existing patterns; deviate and you will create odd one-off code paths.

## The standard recipe

A feature that adds a new user-owned entity (say, "Budget") touches every layer:

1. **Domain entity** — `src/MoneyMap/Core/DataModels/Budget.cs`
   - POCO with public auto-properties, no EF attributes.
   - Include `UserId` (string) and any denormalized display fields needed.

2. **DbSet + IEntityTypeConfiguration** — add `public DbSet<Budget> Budgets => Set<Budget>();` to `MoneyMapDbContext`, and a new class `Infrastructure/Data/Configurations/BudgetConfiguration.cs : IEntityTypeConfiguration<Budget>` for indexes, lengths, precision, and any seed data. `ApplyConfigurationsFromAssembly` picks it up automatically.

3. **Migration** — see `03-data-model-and-migrations.md`. Always inspect the generated migration before applying.

4. **Service interface** — `src/MoneyMap/Application/IBudgetService.cs`
   - Methods for any user-scoped operation take `string userId` first.
   - Keep return types simple (`IList<T>`, `T?`) — the existing services are not async, see note below.

5. **Service implementation** — `src/MoneyMap/Application/Services/BudgetService.cs`
   - Inject `MoneyMapDbContext` and `ILogger<BudgetService>`.
   - All methods are `async` and accept `CancellationToken ct = default`.
   - Filter every query by `UserId`. Never call `_db.Budgets.ToListAsync()` without a `Where(b => b.UserId == userId)`.

6. **DI registration** — `src/MoneyMap.Web/Program.cs`
   - `builder.Services.AddTransient<IBudgetService, BudgetService>();`
   - Match the lifetime of the existing services unless you have a reason not to.

7. **Razor Pages** — under the correct area (`Areas/Users/Pages/Budgets/` for end users, `Areas/Admin/Pages/Budgets/` for admin tooling).
   - Folder choice is the auth choice — see `04-authentication-and-authorization.md`.
   - In each PageModel, read the user id from `User.FindFirstValue(ClaimTypes.NameIdentifier)!` and pass it to the service.
   - Use `[BindProperties]` at the class level and `[BindNever]` for fields that are server-supplied (e.g. `SelectList`s).

8. **Validation** — apply data annotations on the PageModel properties, *not* on the domain entity. Domain entities stay free of MVC concerns.

9. **Manual smoke test** — register a user, exercise the new pages, then sign in as an admin and confirm admin-only paths still work.

## Page model conventions in this codebase

Look at `Areas/Users/Pages/Expenses/Create.cshtml.cs` as the canonical example:

- Class-level `[BindProperties]` rather than per-property `[BindProperty]`.
- `SelectList`s for dropdowns are populated in `OnGet` (and re-populated on every failed `OnPost` — model state is stateless).
- Cross-field validation (e.g. "date not in the future") is added with `ModelState.AddModelError` *before* `ModelState.IsValid` is checked.
- `Date` fields are converted to UTC (`ToUniversalTime()`) before persistence.
- After a successful POST, `RedirectToPage("Index")` — never re-render the same page on success (avoids form re-submission on refresh).

## Style and structure

- **Namespaces** mirror folder structure (`MoneyMap.Application.Services`, `MoneyMap.Web.Areas.Users.Pages.Expenses`). File-scoped namespaces are used in newer files.
- **Nullability:** the projects do not currently have `<Nullable>enable</Nullable>` set globally. Properties that are required-but-EF-managed use `= default!;` to silence init warnings. Match what surrounding files do; don't enable nullable annotations file-by-file.
- **Async I/O everywhere.** All service methods are `async Task<T>` and accept `CancellationToken`. Page handlers are `OnGetAsync` / `OnPostAsync`. Do not introduce sync wrappers around async work.
- **Logging:** inject `ILogger<T>` and use structured templates with named placeholders (`"... {ExpenseId} for user {UserId}"`). Avoid string interpolation in log messages.
- **Magic strings:** avoid hardcoding role names outside of `Program.cs`. If you find yourself typing `"admin"` again, introduce a constant.

## What to NOT do

- Don't add EF attributes (`[Required]`, `[Column]`) to domain entities — keep them clean.
- Don't bypass the service layer from Razor PageModels (no direct `MoneyMapDbContext` injection into pages).
- Don't trust `userId` from form posts; always read from `User` claims.
- Don't introduce a second composition root or a new DI container.
- Don't add a new database provider; the schema and EF model assume PostgreSQL.

## When in doubt

`Expenses` is the worked example for *every* concern: per-user filtering, validation, date handling, dropdowns, list/search, edit, delete. Mirror it.
