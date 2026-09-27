using Microsoft.EntityFrameworkCore;

namespace FuelLoyalty.Sales.Api.Infrastructure.Persistence
{
    /// <summary>
    /// Uygulama açılırken bekleyen migration'ları uygular.
    /// </summary>
    public static class SalesDbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider services, CancellationToken cancellationToken = default)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<SalesDbContext>();

            await db.Database.MigrateAsync(cancellationToken);
        }
    }
}
