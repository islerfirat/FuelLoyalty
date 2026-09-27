using FuelLoyalty.Loyalty.Api.Common.Options;
using Microsoft.Extensions.Options;

namespace FuelLoyalty.Loyalty.Api.Features.ExpireAuthorizations
{
    /// <summary>
    /// Arka planda belirli aralıklarla ExpireAuthorizationsHandler'ı çalıştırır.
    /// Sadece zamanlamadan sorumludur, iş mantığı handler'dadır.
    /// </summary>
    public sealed class AuthorizationExpiryJob(
        IServiceScopeFactory scopeFactory,
        IOptions<AuthorizationOptions> options,
        ILogger<AuthorizationExpiryJob> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var interval = TimeSpan.FromSeconds(options.Value.ExpiryCheckSeconds);
            logger.LogInformation("Provizyon zaman aşımı görevi başladı. Kontrol aralığı: {Interval}", interval);

            using var timer = new PeriodicTimer(interval);

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    // Handler ve DbContext Scoped olduğu için her tur yeni bir scope açılır.
                    using var scope = scopeFactory.CreateScope();
                    var handler = scope.ServiceProvider.GetRequiredService<ExpireAuthorizationsHandler>();

                    await handler.HandleAsync(stoppingToken);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    logger.LogError(ex, "Provizyon zaman aşımı kontrolünde hata oluştu.");
                }
            }
        }
    }
}
