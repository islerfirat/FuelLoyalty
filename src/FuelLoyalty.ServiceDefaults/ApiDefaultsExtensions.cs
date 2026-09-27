using FluentValidation;
using FuelLoyalty.ServiceDefaults.Endpoints;
using FuelLoyalty.ServiceDefaults.ErrorHandling;
using FuelLoyalty.ServiceDefaults.Handlers;
using FuelLoyalty.ServiceDefaults.Health;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;
using System.Text.Json.Serialization;

namespace FuelLoyalty.ServiceDefaults
{
    /// <summary>
    /// Her API servisinin ortak kurulumu. Program.cs'te iki satırla kullanılır:
    /// <c>builder.Services.AddApiDefaults(typeof(Program).Assembly);</c> ve <c>app.UseApiDefaults();</c>
    /// </summary>
    public static class ApiDefaultsExtensions
    {
        public static IServiceCollection AddApiDefaults(this IServiceCollection services, Assembly featureAssembly)
        {
            // Standart hata cevapları
            services.AddProblemDetails();
            services.AddExceptionHandler<GlobalExceptionHandler>();

            // Enum'lar JSON'da yazı olarak: "Benzin", "Amount"
            services.ConfigureHttpJsonOptions(options =>
                options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            // Özellik klasörlerindeki doğrulayıcılar, endpoint'ler ve handler'lar
            services.AddValidatorsFromAssembly(featureAssembly, includeInternalTypes: true);
            services.AddEndpoints(featureAssembly);
            services.AddHandlers(featureAssembly);

            // Zaman bilgisi DateTime.UtcNow yerine buradan alınır (test edilebilirlik için)
            services.TryAddSingleton(TimeProvider.System);

            // Health check altyapısı (servisler kendi kontrollerini ekler)
            services.AddHealthChecks();

            return services;
        }

        public static WebApplication UseApiDefaults(this WebApplication app)
        {
            app.UseExceptionHandler();
            app.UseStatusCodePages();

            MapHealthEndpoints(app);
            app.MapEndpoints();

            return app;
        }

        private static void MapHealthEndpoints(WebApplication app)
        {
            // Uygulama ayakta mı? Hiçbir kontrol çalıştırılmaz.
            app.MapHealthChecks("/health/live", new HealthCheckOptions
            {
                Predicate = _ => false,
                ResponseWriter = HealthCheckResponseWriter.WriteAsync
            });

            // İstek kabul etmeye hazır mı? Sadece zorunlu bağımlılıklar kontrol edilir.
            app.MapHealthChecks("/health/ready", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains(HealthCheckTags.Ready),
                ResponseWriter = HealthCheckResponseWriter.WriteAsync
            });

            // Tüm bağlantıların durumu (teşhis amaçlı).
            app.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = HealthCheckResponseWriter.WriteAsync
            });
        }
    }
}
