using FuelLoyalty.Contracts;
using FuelLoyalty.Loyalty.Api.Domain.Cards;
using Microsoft.EntityFrameworkCore;

namespace FuelLoyalty.Loyalty.Api.Infrastructure.Persistence
{
    /// <summary>
    /// Uygulama açılırken migration'ları uygular ve veritabanı boşsa test kartlarını ekler.
    /// </summary>
    public static class LoyaltyDbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider services, CancellationToken cancellationToken = default)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<LoyaltyDbContext>();

            await db.Database.MigrateAsync(cancellationToken);

            if (await db.Cards.AnyAsync(cancellationToken))
                return;

            var passiveCard = Card.Create("1000200030004002", "35 PAS 789", FuelType.Benzin, LimitType.Amount, 1000m).Value;
            passiveCard.Deactivate();

            db.Cards.AddRange(
                Card.Create("1000200030004000", "34 ABC 123", FuelType.Benzin, LimitType.Amount, 5000m).Value,
                Card.Create("1000200030004001", "06 XYZ 456", FuelType.Motorin, LimitType.Liters, 200m).Value,
                passiveCard,
                Card.Create("1000200030004003", "16 LPG 000", FuelType.LPG, LimitType.Amount, 0m).Value);

            await db.SaveChangesAsync(cancellationToken);
        }
    }
}
