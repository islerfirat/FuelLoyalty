using FuelLoyalty.Loyalty.Api;
using FuelLoyalty.Loyalty.Api.Infrastructure.Persistence;
using FuelLoyalty.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddApiDefaults(typeof(Program).Assembly);
builder.Services.AddLoyaltyServices(builder.Configuration);

var app = builder.Build();

await LoyaltyDbInitializer.InitializeAsync(app.Services);

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseApiDefaults();

app.Run();