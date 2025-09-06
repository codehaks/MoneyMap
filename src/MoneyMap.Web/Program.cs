using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MoneyMap.Application;
using MoneyMap.Application.Services;
using MoneyMap.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MoneyMapDbContext>(options =>
{
    
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Add Identity
builder.Services.AddIdentity<ApplicationUser,IdentityRole>
    (options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.User.RequireUniqueEmail = true;
        options.Lockout.MaxFailedAccessAttempts = 3;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);

    })
    .AddDefaultUI()
    .AddEntityFrameworkStores<MoneyMapDbContext>()
    .AddDefaultTokenProviders();


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy =>
    {
        policy.RequireRole("admin");
    });
});

builder.Services.AddRazorPages().AddRazorPagesOptions(options =>
{
    options.Conventions.AuthorizeAreaFolder("users", "/");
    options.Conventions.AuthorizeAreaFolder("admin", "/", "RequireAdminRole");
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IExpenseService, ExpenseService>();
builder.Services.AddTransient<ICalendarService, CalendarService>();
builder.Services.AddTransient<IUserService, UserService>();

builder.WebHost.UseStaticWebAssets();


var app = builder.Build();

app.UseAuthorization();

app.MapRazorPages();
app.MapStaticAssets();

app.Run();
