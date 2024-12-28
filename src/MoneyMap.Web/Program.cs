using Microsoft.EntityFrameworkCore;
using MoneyMap.Application;
using MoneyMap.Application.Services;
using MoneyMap.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MoneyMapDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});



builder.Services.AddRazorPages();

builder.Services.AddTransient<IExpenseService, ExpenseService>();
builder.Services.AddTransient<ICalendarService, CalendarService>();

var app = builder.Build();

app.MapRazorPages();

app.MapStaticAssets();

app.Run();
