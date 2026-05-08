# 09 — Prosumer Feature Roadmap

A prioritized list of features to evolve MoneyMap from a basic personal expense log into a tool a **prosumer** — a freelancer, contractor, sole trader, or finance-minded power user — can actually run their daily money tracking on, without it bloating into a full accounting suite.

> **Design rule for every item below:** if it isn't usable in under 10 seconds with one keyboard or one tap, it doesn't belong. The whole point is the app stays minimal. Anything that needs a wizard or a settings page nobody opens is a sign the feature is wrong.

The list has two parts:

1. **End-user features** (Areas/Users + Pages/) — what regular users live in.
2. **Admin features** (Areas/Admin/) — what the operator/admin needs to keep the app running and useful.

Within each part, items are tiered:

- **Must have** — the app feels incomplete without it for prosumer use.
- **Nice to have** — meaningful daily wins, but defer until the must-haves ship.
- **Exceptional** — differentiators; do these only after the foundation is rock solid, and only one or two so the app stays minimal.

---

## End-user features

### Must have

#### 1. Income / earnings (not just expenses)
A prosumer tracks **money in and out**. Today, the app only models negative cashflow.
- New aggregate `Income` (or a single signed `Transaction` aggregate — pick one and stick with it). Same shape: amount, date, category, note, userId.
- Default income categories: Salary, Freelance, Refund, Interest, Other.
- The `Index` becomes a **transactions** view with a +/- toggle.

> **Minimal-by-design hint:** prefer one `Transaction` table with a `Direction` enum over two parallel tables. Halves the migration churn and the UI duplication.

#### 2. Dashboard / "this month" summary on the home page
The current `Pages/Index.cshtml` is marketing copy. For a logged-in user it should be a single-screen snapshot:
- This month: total in, total out, net.
- Top 3 categories by spend.
- Last 5 transactions.
- One sparkline of the last 30 days net cashflow.

No drill-downs on the dashboard itself — it links into the existing list views. Keep it under one viewport.

#### 3. Date range + multi-category filter on the list
Currently `Index` filters by a single category and a search term. Prosumers think in **periods** (this week / this month / last quarter / custom range).
- Add `from`/`to` query params, default to current month.
- Multi-select categories (checkbox group) instead of a single dropdown.
- Persist the last filter in a cookie so refresh doesn't lose context.

#### 4. Currency support per user
Hard-coding `$` in the cshtml (`$@expense.Amount.ToString("F2")`) is fine for a demo, painful in production.
- Add `Currency` (3-letter ISO) to `ApplicationUser`. Default `USD`.
- Format with `CultureInfo` on render. Don't store currency per transaction yet — that's the multi-currency feature (exceptional tier).

#### 5. CSV export
A prosumer hands their accountant a spreadsheet. Without export, the app is a roach motel for data.
- One button on the transactions list: "Export CSV".
- Honors the active filter (date range + categories + search).
- Streamed response, not buffered — keep it fast on big lists.

#### 6. Recurring transactions
Rent, subscriptions, salary — entering these by hand every month is the #1 reason expense apps get abandoned.
- New aggregate `RecurringRule` (every {N} {day|week|month}, starting `DateOnly`, optional end date, template = amount/category/note).
- A background `IHostedService` (or, simpler, a "materialize on login" check) creates the missing transactions when the user logs in.
- Edit/skip a single occurrence without nuking the rule.

> **Minimal-by-design hint:** start with the on-login materialization. Defer the hosted service until you actually have prosumer users running automated reports nightly.

### Nice to have

#### 7. Tags (in addition to categories)
Categories are mutually exclusive (one per transaction), but real-world spend is multi-dimensional ("groceries" + "client-X" + "billable"). A simple `Tag` aggregate with a many-to-many to `Transaction` covers it.

Keep the UI **comma-separated tag input**; no fancy chip pickers. New tags are created on the fly.

#### 8. Budgets per category per month
- For each category, set a monthly cap.
- The transactions list shows a small "85% of grocery budget" pill at the top when filtered to that category.
- The dashboard surfaces categories that are over budget for the month.

Don't try to model rolling budgets, weekly budgets, or envelopes in v1. Monthly cap, that's it.

#### 9. Receipt attachment
- One file (image/PDF) per transaction, stored on disk or S3-compatible storage.
- Thumbnail on the row, fullscreen viewer on click.
- Admin-configurable max size (default 5 MB), MIME allowlist.

Use `IFormFile` upload + filesystem path or a blob abstraction. Don't store the bytes in Postgres.

#### 10. Quick-add via natural language
A single input box on the dashboard: "12.50 lunch yesterday food".
- Server-side parser maps to `{ amount: 12.50, note: "lunch", date: yesterday, category: Food }`.
- Falls back to opening the full Create page if it can't parse.
- This is the highest-leverage UX win for daily prosumer use, and it is **less code than people think** — start with regex + a category lookup.

#### 11. Keyboard shortcuts
- `n` → new expense, `i` → new income, `/` → focus search, `g d` → go to dashboard, `g e` → expenses.
- One small JS file. No framework.

#### 12. Account / wallet split
A freelancer has multiple money pots: business checking, personal checking, cash, savings.
- New `Account` aggregate (name, currency, opening balance).
- Every transaction belongs to one account.
- The dashboard groups balance by account.

This is a "nice to have" because it adds one extra step to the create form. Only ship it if users ask.

### Exceptional

#### 13. Dual-entry for transfers
Once accounts (#12) exist, a "transfer between accounts" should not appear as a phantom expense + phantom income. Build a real transfer concept (one entity, two ledger lines, balanced). This is the only "real accounting" concept worth importing — without it, the totals lie.

#### 14. Bank import (OFX / CSV)
- Drag-and-drop a CSV from your bank. The app shows a preview, lets the user map columns, and creates draft transactions.
- "Match to existing recurring rule" auto-categorizes.
- **No** Plaid / TrueLayer / live bank links in v1 — the moment you take credentials, you're a regulated entity. CSV is the prosumer sweet spot.

#### 15. Insights & alerts
- Weekly email: "you spent 32% more on Food this week vs your 4-week average."
- Anomaly detection on a single category (z-score on the last N weeks).
- Crucially: **opt-in**, and the email links straight to the relevant filtered list.

#### 16. PWA + offline create
- Manifest + service worker.
- "Add expense" works offline; queued and posted when connectivity returns.
- For mobile-first prosumer use, this is the quality bar.

#### 17. Multi-currency
- Per-transaction currency, FX rate captured at entry time.
- Reports normalize to the user's home currency.
- Defer until a user explicitly asks. It's a tax for everyone if applied universally.

---

## Admin panel features

The current `Areas/Admin` has Users (list + details) and a placeholder Categories page. For an admin running a real prosumer SaaS, that's not enough. Below is what a prosumer-grade admin needs, with the same tiering.

### Must have

#### A1. Category management (real CRUD)
The Categories page lists rows but has no add/edit/delete. Either build the CRUD or remove the link from the sidebar.
- Create / rename / soft-delete categories.
- Soft-delete: a deleted category remains visible on existing transactions but is not available to new ones. Don't hard-delete and orphan rows.
- Surface usage count next to each category ("237 transactions") so admins know what they're about to deprecate.

#### A2. Role assignment from the UI
There is no UI to grant the `admin` role today — the docs require raw SQL. For a 1-to-3-admin prosumer team this is the single most-needed admin feature.
- On `Users/Details`, add a role picker (multi-select) gated to admins.
- Audit log entry for every grant/revoke.

#### A3. User lockout / disable
- "Disable account" toggle on `Users/Details` that sets `LockoutEnd = DateTimeOffset.MaxValue`.
- "Force password reset" button.
- Visible reason field stored in audit log.

#### A4. Audit log
- One append-only table: `actor_user_id`, `action`, `target_type`, `target_id`, `before_json`, `after_json`, `at_utc`.
- Written by an EF Core `SaveChanges` interceptor, not by every service.
- Admin page shows a paginated, filterable feed.

This is must-have because once you have role assignment (A2) and account disable (A3), you legally and operationally need to know who did what.

#### A5. Server health page
- App version / git SHA at the top.
- DB connectivity check.
- Last migration applied.
- Pending background jobs (when #6 / #15 ship).
- Outbox of failed emails (when an `IEmailSender` ships).

A simple read-only `/admin/health` page; no fancy graphs.

### Nice to have

#### A6. User search and pagination
`Users/Index` does `_userManager.Users.ToListAsync()` — fine for ten users, broken for ten thousand. Add a search box (email / username) and `Skip`/`Take` paging. Same on the audit log.

#### A7. Per-user activity dashboard
On `Users/Details`, show a mini summary:
- Date joined, last login.
- Number of transactions, sum in/out (for support purposes).
- Last 10 actions from the audit log.

Read-only; do **not** let the admin browse the user's transactions in detail unless there's a clearly justified support flow — that's a privacy minefield.

#### A8. Feature flags
A handful of boolean toggles that an admin can flip without redeploying: enable receipt uploads, enable bank import, enable email digests. Read once at startup and via `IOptionsMonitor` for hot-reload. A two-column page with a checkbox each.

#### A9. Backup / restore
- "Download a backup" button → produces a SQL dump or pg_dump-compatible archive.
- "Restore" is read-only documentation linking to the runbook; don't let the admin click-to-restore — that's how data losses happen.

#### A10. Email templates editor
Once an `IEmailSender` is wired up (improvement #2 in `06-known-issues-and-improvement-backlog.md`), the password reset / lockout / digest emails should be editable as Markdown templates with named placeholders.

### Exceptional

#### A11. Tenant / organization mode
For a true SaaS step-up: an `Organization` aggregate, users belong to one or more orgs, transactions belong to an org rather than a user. Roles become `OwnerOf(orgId)`, `MemberOf(orgId)`. The admin panel grows an Orgs section.

This is exceptional because it's a one-way door — once you ship multi-tenant, every model gains an `OrganizationId` and every query gains a tenancy filter. Don't do it unless you've validated a market for team prosumer use.

#### A12. Anonymized usage analytics dashboard
- DAU / WAU / MAU.
- Funnel: register → first expense → 10th expense → 30-day retention.
- Top categories across the user base (anonymized, aggregated).
- Crash counts.

Self-host the metrics (Postgres + a chart) before reaching for a third-party. Privacy is a feature in the prosumer segment.

#### A13. Impersonate user (with audit trail)
For support: "log in as this user" — every action while impersonating is recorded with the original admin's id in the audit log, the user is emailed the next morning. Implement only after A4 (audit log). High-value for support, high-risk if half-built.

---

## Suggested build order

The minimum that turns MoneyMap into a usable prosumer tool:

1. **Income (#1)** — without it, everything else feels half a tool.
2. **Dashboard (#2)** + **date-range filter (#3)** — go together; same UI surface.
3. **Category CRUD (A1)** + **role assignment (A2)** — admin debt is the cheapest debt to clear.
4. **CSV export (#5)**.
5. **Currency per user (#4)**.
6. **Recurring rules (#6)** — biggest retention lever.
7. **Audit log (A4)** — must precede any further admin power features.
8. **Budgets (#8)** + **tags (#7)** — the second wave of daily-use polish.
9. Pick **one** exceptional item to differentiate (NL quick-add #10 or bank CSV import #14 are the highest leverage). Resist doing more than one until it's used in anger.

## What to deliberately NOT build

A short stop-list, because feature creep is the real enemy of a minimal app:

- Full double-entry accounting / chart of accounts. (Use Beancount or QuickBooks if you need this.)
- Investment / asset / portfolio tracking.
- Loan amortization schedules.
- Tax calculations or filing.
- Live bank credential connections.
- Mobile native apps. (Ship the PWA instead — #16.)
- Social / sharing / "challenge a friend" features.
- AI-everything. The natural-language quick-add (#10) is fine; an "AI advisor" is not a product, it's a footgun.

The strength of MoneyMap is that it does one thing in 10 seconds. Every feature above earns its place by either (a) collapsing manual work for an existing flow, or (b) making the data already in the app actionable. If a proposed feature does neither, it doesn't ship.
