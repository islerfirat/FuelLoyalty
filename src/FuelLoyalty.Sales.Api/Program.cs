using FuelLoyalty.ServiceDefaults;
using FuelLoyalty.Sales.Api;
using FuelLoyalty.Sales.Api.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddApiDefaults(typeof(Program).Assembly);
builder.Services.AddSalesServices(builder.Configuration);

var app = builder.Build();

await SalesDbInitializer.InitializeAsync(app.Services);

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseApiDefaults();

app.Run();