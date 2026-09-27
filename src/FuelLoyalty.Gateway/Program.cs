using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// YARP: yönlendirme kurallarý ve saðlýk yoklamasý appsettings.json'daki "ReverseProxy" bölümünden okunur.
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Ýstek sýnýrlama: deðerler appsettings'teki "RateLimit" bölümünden okunur.
var rateLimit = builder.Configuration.GetSection("RateLimit");

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddFixedWindowLimiter("pump-policy", limiter =>
    {
        limiter.PermitLimit = rateLimit.GetValue("PermitLimit", 10);
        limiter.Window = TimeSpan.FromSeconds(rateLimit.GetValue("WindowSeconds", 10));
        limiter.QueueLimit = 0;
    });
});

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRateLimiter();

app.MapHealthChecks("/health/live");
app.MapReverseProxy();

app.Run();