using FuelLoyalty.ServiceDefaults.Health;
using FuelLoyalty.ServiceDefaults.Messaging;
using FuelLoyalty.Sales.Api.Infrastructure.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace FuelLoyalty.Sales.Api
{
    /// <summary>
    /// Sales servisine özel kayıtlar: veritabanı, mesajlaşma, Outbox, health check.
    /// Bu servis Loyalty'yi doğrudan çağırmaz; iki servis sadece RabbitMQ üzerinden haberleşir.
    /// </summary>
    public static class SaleServiceExtensions
    {
        public static IServiceCollection AddSalesServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("SalesDb")
                ?? throw new InvalidOperationException("'SalesDb' bağlantı cümlesi bulunamadı.");

            services.AddDbContext<SalesDbContext>(options => options.UseNpgsql(connectionString));

            services.AddHealthChecks()
                .AddDbContextCheck<SalesDbContext>(name: "sales-db", tags: [HealthCheckTags.Ready]);

            services.AddRabbitMqMessaging(configuration, bus =>
            {
                bus.AddEntityFrameworkOutbox<SalesDbContext>(outbox =>
                {
                    outbox.UsePostgres();
                    outbox.UseBusOutbox();
                    outbox.QueryDelay = TimeSpan.FromSeconds(1);
                });
            });

            return services;
        }
    }
}
