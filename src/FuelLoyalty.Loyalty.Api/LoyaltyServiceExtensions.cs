using FuelLoyalty.Loyalty.Api.Common.Options;
using FuelLoyalty.Loyalty.Api.Features.ExpireAuthorizations;
using FuelLoyalty.Loyalty.Api.Features.SettleSale;
using FuelLoyalty.Loyalty.Api.Infrastructure.Persistence;
using FuelLoyalty.ServiceDefaults.Health;
using FuelLoyalty.ServiceDefaults.Messaging;
using Microsoft.EntityFrameworkCore;

namespace FuelLoyalty.Loyalty.Api
{
    /// <summary>
    /// Loyalty servisine özel kayıtlar: veritabanı, ayarlar, arka plan görevi, mesajlaşma, health check.
    /// </summary>
    public static class LoyaltyServiceExtensions
    {
        public static IServiceCollection AddLoyaltyServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("LoyaltyDb")
                ?? throw new InvalidOperationException("'LoyaltyDb' bağlantı cümlesi bulunamadı.");

            services.AddDbContext<LoyaltyDbContext>(options => options.UseNpgsql(connectionString));

            services.AddHealthChecks()
                .AddDbContextCheck<LoyaltyDbContext>(name: "loyalty-db", tags: [HealthCheckTags.Ready]);

            services.AddOptions<AuthorizationOptions>()
                .Bind(configuration.GetSection(AuthorizationOptions.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddHostedService<AuthorizationExpiryJob>();

            services.AddRabbitMqMessaging(configuration, bus =>
                bus.AddConsumer<SaleCompletedConsumer>());

            return services;
        }
    }
}
