using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MoneyMap.Application;
using MoneyMap.Application.Authorization;
using MoneyMap.Application.Services;
using MoneyMap.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MoneyMapDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.User.RequireUniqueEmail = true;
        options.Lockout.MaxFailedAccessAttempts = IdentityDefaults.MaxFailedAccessAttempts;
        options.Lockout.DefaultLockoutTimeSpan = IdentityDefaults.DefaultLockoutTimeSpan;
    })
    .AddDefaultUI()
    .AddEntityFrameworkStores<MoneyMapDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AuthorizationPolicies.RequireAdmin, policy => policy.RequireRole(Roles.Admin));
});

builder.Services.AddRazorPages().AddRazorPagesOptions(options =>
{
    options.Conventions.AuthorizeAreaFolder("users", "/");
    //options.Conventions.AuthorizeAreaFolder("admin", "/", AuthorizationPolicies.RequireAdmin);
});

builder.Services.AddTransient<IExpenseService, ExpenseService>();
builder.Services.AddTransient<IUserService, UserService>();

builder.WebHost.UseStaticWebAssets();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapStaticAssets();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

await SeedRolesAsync(app);

app.Run();

static async Task SeedRolesAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    if (!await roleManager.RoleExistsAsync(Roles.Admin))
    {
        await roleManager.CreateAsync(new IdentityRole(Roles.Admin));
    }
}

internal static class IdentityDefaults
{
    public const int MaxFailedAccessAttempts = 3;
    public static readonly TimeSpan DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
}
