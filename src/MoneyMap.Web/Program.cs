using MoneyMap.Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddTransient<ICalendarService,CalendarService>();

var app = builder.Build();

app.MapRazorPages();

app.MapStaticAssets();

app.Run();
