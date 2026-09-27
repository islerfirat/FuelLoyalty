using FuelLoyalty.Portal.Services;
using FuelLoyalty.Portal.Services.Cards;
using FuelLoyalty.Portal.Services.Sales;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace FuelLoyalty.Portal
{
    /// <summary>
    /// Portal servislerinin kaydı: gateway ayarları, API istemcileri, zaman sağlayıcı, Data Protection.
    /// </summary>
    public static class PortalServiceExtensions
    {
        public static IServiceCollection AddPortalServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<GatewayOptions>()
                .Bind(configuration.GetSection(GatewayOptions.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddHttpClient<ICardApiClient, CardApiClient>(ConfigureGatewayClient);
            services.AddHttpClient<ISalesApiClient, SalesApiClient>(ConfigureGatewayClient);

            // "Bugün" bilgisi DateTime.Now yerine buradan alınır (test edilebilirlik için).
            services.TryAddSingleton(TimeProvider.System);

            AddPersistentDataProtection(services, configuration);

            return services;
        }

        /// <summary>Tüm istemciler aynı gateway adresini ve zaman aşımını kullanır.</summary>
        private static void ConfigureGatewayClient(IServiceProvider serviceProvider, HttpClient client)
        {
            var options = serviceProvider.GetRequiredService<IOptions<GatewayOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
        }

        /// <summary>
        /// Çerez şifreleme anahtarlarını kalıcı bir klasörde saklar.
        /// Klasör ayarlanmamışsa (Visual Studio'da çalışırken) varsayılan davranış kullanılır.
        /// </summary>
        private static void AddPersistentDataProtection(IServiceCollection services, IConfiguration configuration)
        {
            var keysPath = configuration["DataProtection:KeysPath"];
            if (string.IsNullOrWhiteSpace(keysPath))
                return;

            services.AddDataProtection()
                .SetApplicationName("FuelLoyalty.Portal")
                .PersistKeysToFileSystem(new DirectoryInfo(keysPath));
        }
    }
}
