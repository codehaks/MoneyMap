# 04 — Authentication & Authorization

Authentication and authorization are configured **once**, in `src/MoneyMap.Web/Program.cs`. There are no `[Authorize]` attributes scattered through the codebase; access control is enforced by:

1. **ASP.NET Core Identity** for sign-in / sign-out / user management.
2. **Folder-level Razor Pages conventions** for which areas require authentication.
3. **A single named policy** (`RequireAdminRole`) for admin pages.
4. **Service-layer `userId` filtering** for per-row authorization on expenses.

If you add a new page or service, you must understand all four.

## Identity configuration

```csharp
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;     // no email confirmation
    options.User.RequireUniqueEmail = true;
    options.Lockout.MaxFailedAccessAttempts = 3;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
})
.AddDefaultUI()
.AddEntityFrameworkStores<MoneyMapDbContext>()
.AddDefaultTokenProviders();
```

Notable defaults left implicit:
- **Password rules** are ASP.NET Core defaults (≥6 chars, upper/lower/digit/non-alphanumeric).
- **No external providers** (Google, Microsoft, …) configured.
- **No email sender** registered — the default `IEmailSender` is a no-op, so password-reset and confirmation links are written to logs, not delivered.
- **Default cookie auth** is used; tokens / JWT are not configured.

The Identity UI lives under `Areas/Identity/` and is provided entirely by `AddDefaultUI()`. To customize a page, scaffold it with `dotnet aspnet-codegenerator identity` rather than editing the runtime assembly.

## Folder-level authorization conventions

```csharp
builder.Services.AddRazorPages().AddRazorPagesOptions(options =>
{
    options.Conventions.AuthorizeAreaFolder("users", "/");
    options.Conventions.AuthorizeAreaFolder("admin", "/", "RequireAdminRole");
});
```

| Path under `Areas/` | Auth requirement |
| ------------------- | ---------------- |
| `Users/**` | Any authenticated user. |
| `Admin/**` | Authenticated **and** in role `admin`. |
| `Identity/**` | Public (login/register/etc.). |
| `Pages/` (root) | Public. |

**When you add a page, choose the area first.** That decision is the auth decision. Do not paper over a wrong placement with `[Authorize]` attributes — it makes the policy harder to audit.

## The `RequireAdminRole` policy

```csharp
options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("admin"));
```

The role name is lowercase `"admin"`. Identity role names are case-sensitive in lookups by default; create the role with the exact same casing.

## Promoting a user to admin

There is no UI for role assignment. To create the first admin:

```sql
-- after the user has registered
INSERT INTO "AspNetRoles" ("Id", "Name", "NormalizedName", "ConcurrencyStamp")
VALUES (gen_random_uuid()::text, 'admin', 'ADMIN', gen_random_uuid()::text)
ON CONFLICT DO NOTHING;

INSERT INTO "AspNetUserRoles" ("UserId", "RoleId")
SELECT u."Id", r."Id"
  FROM "AspNetUsers" u, "AspNetRoles" r
 WHERE u."Email" = 'you@example.com' AND r."Name" = 'admin';
```

The user must sign out and sign back in for the new role claim to appear in their cookie.

A cleaner approach (a seeded admin or an admin-bootstrap page) is on the backlog — see `06-known-issues-and-improvement-backlog.md`.

## Per-row authorization (the second line of defense)

Folder conventions only check that *someone* is logged in. They do **not** prevent a logged-in user from reading or modifying another user's expenses by guessing IDs. That filtering happens in the service layer:

```csharp
// ExpenseService.FindById
return _db.Expenses.FirstOrDefault(e => e.Id == id && e.UserId == userId);
```

Every `IExpenseService` method takes a `userId` and uses it inside the LINQ query. PageModels read the current user id from claims:

```csharp
var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
```

**Rules for new code:**
- Any new service method that touches user-owned data takes `userId` as the first argument.
- Inside the implementation, every query is filtered by `UserId`.
- Never trust a `userId` from a form post — always read it from `User` claims in the PageModel and pass it down.
- Never expose the raw `Expense` entity to admins without an explicit, separate code path (no admin-side endpoint exists today).

## Public vs admin user data

`UserService` calls `UserManager.Users` directly, returning all users. Only `Areas/Admin/Pages/Users/*` consumes it, and that area is gated by `RequireAdminRole`. If you reuse `UserService.GetAll()` from a non-admin page, you have created a privacy bug — wrap it or add a more specific method instead.
