using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MoneyMap.Application;
using MoneyMap.Application.Services;
using MoneyMap.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MoneyMapDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Add Identity
builder.Services.AddDefaultIdentity<ApplicationUser>
    (options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.User.RequireUniqueEmail = true;
        options.Lockout.MaxFailedAccessAttempts = 3;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);

    }).AddEntityFrameworkStores<MoneyMapDbContext>();

builder.Services.AddRazorPages();

builder.Services.AddTransient<IExpenseService, ExpenseService>();
builder.Services.AddTransient<ICalendarService, CalendarService>();

var app = builder.Build();

app.UseAuthorization();

app.MapRazorPages();
app.MapStaticAssets();

app.Run();
