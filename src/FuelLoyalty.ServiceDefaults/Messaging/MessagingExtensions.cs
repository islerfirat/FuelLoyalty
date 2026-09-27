using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FuelLoyalty.ServiceDefaults.Messaging
{
    public static class MessagingExtensions
    {
        /// <summary>
        /// MassTransit'i RabbitMQ ile yapılandırır.
        /// Servise özel ayarlar (consumer, outbox) configure parametresiyle eklenir.
        /// </summary>
        public static IServiceCollection AddRabbitMqMessaging(
            this IServiceCollection services,
            IConfiguration configuration,
            Action<IBusRegistrationConfigurator>? configure = null)
        {
            services.AddOptions<RabbitMqOptions>()
                .Bind(configuration.GetSection(RabbitMqOptions.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddMassTransit(bus =>
            {
                // Servise özel kayıtlar: Loyalty consumer ekler, Sale outbox ekler.
                configure?.Invoke(bus);

                bus.UsingRabbitMq((context, cfg) =>
                {
                    var options = context.GetRequiredService<IOptions<RabbitMqOptions>>().Value;

                    cfg.Host(options.Host, options.VirtualHost, host =>
                    {
                        host.Username(options.Username);
                        host.Password(options.Password);
                    });

                    cfg.UseMessageRetry(retry =>
                        retry.Interval(options.RetryCount, TimeSpan.FromSeconds(options.RetryIntervalSeconds)));

                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
}
