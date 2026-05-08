# 06 — Known Issues & Improvement Backlog

A candid list of rough edges in the current codebase, ordered roughly by impact. None of these block day-to-day development, but a maintainer should know they exist before designing changes.

## Security & configuration

1. **Committed development credentials.** `src/MoneyMap.Web/appsettings.json` ships with a real Postgres password. Move the connection string to user-secrets (`dotnet user-secrets`) for development and to environment variables / a vault for any deployed environment. Replace the committed value with a placeholder.
2. **No `IEmailSender`.** Identity is configured with `RequireConfirmedAccount = false` and the default no-op email sender, so password reset emails are never delivered. Either wire up a real `IEmailSender` or document that the feature is unsupported.
3. **No HTTPS redirection / HSTS.** `Program.cs` does not call `UseHttpsRedirection()` or `UseHsts()`. Add them before deployment behind a load balancer that does not already terminate TLS.
4. **No anti-forgery validation in custom POST handlers.** Razor Pages adds anti-forgery automatically for form posts, but if you introduce custom handlers (e.g. AJAX delete), `ValidateAntiForgeryToken` is your responsibility.
5. **No admin bootstrap.** Promoting the first admin currently requires raw SQL (see `04-authentication-and-authorization.md`). A one-time seed (env-var-gated) or an admin-bootstrap CLI would make onboarding less error-prone.

## Correctness

1. **`CalendarService.IsNewYear()` is a leftover.** It returns `DateTime.Now.Year < 2025`, which has been false since 2025. Either repurpose or delete.
2. **`UserService.GetRoles` does not null-check.** `_userManager.FindByIdAsync(userId)` can return `null`; the next call dereferences it. Fix before exposing this beyond the admin area.
3. **Decimal precision unset.** `Expense.Amount` is `decimal` with no `[Precision]` / `HasPrecision` configured. Npgsql defaults are workable but explicit precision (e.g. `(18, 2)`) is safer for money.
4. **`Date` validation is duplicated** between `Create.cshtml.cs` and (eventually) `Edit.cshtml.cs`. Centralize "not in future, not older than 1 year" in the service or a shared validator.
5. **`Note` length (≤100) is enforced only at the PageModel.** A direct service call (or a future API) could persist longer values. Add a `HasMaxLength(100)` in `OnModelCreating`.

## Architecture

1. **No tests.** The solution has no test project. A `tests/MoneyMap.Tests` project with `WebApplicationFactory<Program>` integration tests against a Testcontainers Postgres would catch most regressions cheaply. The README's `dotnet test` instruction is currently a no-op.
2. **Services are synchronous.** EF supports async I/O end-to-end; the current services block threads on every DB call. New services should be async-first, and existing ones should be migrated when touched.
3. **`CalendarService` async methods are fake.** They wrap sync calls in `Task.FromResult`. Either make them genuinely async (if they ever do I/O) or drop the async overloads.
4. **`ApplicationUser` is empty.** Profile fields (display name, currency, time zone) are likely needed soon. The commented-out `FirstName`/`LastName` are a hint.
5. **No domain validation layer.** Validation lives in PageModels and ad-hoc in `Create.cshtml.cs`. Consider FluentValidation or moving rules into the services.
6. **Admin pages mix concerns.** `Areas/Admin/Pages/Users/*` reads the full user list via `UserService` with no pagination. This will not scale; add paging + search before any non-trivial user count.

## Tooling & DevEx

1. **No CI configuration** in the repo — no `.github/workflows`, no Azure Pipelines. Add `dotnet build` + `dotnet ef migrations script` validation as a baseline.
2. **No EditorConfig / formatting baseline.** Whitespace and brace style drift across files. Add a root `.editorconfig` and run `dotnet format` once.
3. **No Dependabot / renovate.** Identity, EF Core, and Npgsql ship security fixes regularly.
4. **`libman.json`** governs client-side library restore but is undocumented in the README. Either commit the restored assets or call out the `libman restore` step.
5. **Single `Dockerfile`, no compose.** A `docker-compose.yml` with Postgres + the web app would shorten the onboarding loop documented in `01-getting-started.md`.

## Quick wins (good first contributions)

- Replace committed credentials with user-secrets references.
- Delete or fix `CalendarService.IsNewYear`.
- Add `[Precision(18, 2)]` to `Expense.Amount` and emit a migration.
- Null-check `UserService.GetRoles`.
- Add an `.editorconfig` and run `dotnet format`.
- Add a `tests/` project with one round-trip integration test for `ExpenseService`.
