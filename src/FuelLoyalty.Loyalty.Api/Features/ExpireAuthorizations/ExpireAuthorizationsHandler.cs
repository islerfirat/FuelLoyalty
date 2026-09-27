using FuelLoyalty.Loyalty.Api.Domain.Authorizations;
using FuelLoyalty.Loyalty.Api.Infrastructure.Persistence;
using FuelLoyalty.ServiceDefaults.Handlers;
using Microsoft.EntityFrameworkCore;

namespace FuelLoyalty.Loyalty.Api.Features.ExpireAuthorizations
{
    /// <summary>
    /// Süresi dolmuş ve hâlâ bekleyen provizyonları kapatır, blokelerini kaldırır.
    /// </summary>
    public sealed class ExpireAuthorizationsHandler(
        LoyaltyDbContext db,
        TimeProvider timeProvider,
        ILogger<ExpireAuthorizationsHandler> logger) : IHandler
    {
        private const int BatchSize = 100;

        /// <summary>Kapatılan provizyon sayısını döner.</summary>
        public async Task<int> HandleAsync(CancellationToken cancellationToken)
        {
            var now = timeProvider.GetUtcNow().UtcDateTime;

            var overdueIds = await db.Authorizations
                .Where(a => a.Status == AuthorizationStatus.Pending && a.ExpiresAt < now)
                .OrderBy(a => a.ExpiresAt)
                .Select(a => a.Id)
                .Take(BatchSize)
                .ToListAsync(cancellationToken);

            var expiredCount = 0;

            foreach (var authorizationId in overdueIds)
            {
                if (await TryExpireAsync(authorizationId, now, cancellationToken))
                    expiredCount++;

                // Her kayıttan sonra EF'in hafızasını temizle: bir çakışma sonraki kayıtları etkilemesin.
                db.ChangeTracker.Clear();
            }

            return expiredCount;
        }

        private async Task<bool> TryExpireAsync(Guid authorizationId, DateTime now, CancellationToken cancellationToken)
        {
            var authorization = await db.Authorizations
                .FirstOrDefaultAsync(a => a.Id == authorizationId, cancellationToken);

            if (authorization is null)
                return false;

            var card = await db.Cards
                .FirstOrDefaultAsync(c => c.Id == authorization.CardId, cancellationToken);

            if (card is null)
                return false;

            // Id'leri okuduğumuz an ile şu an arasında satış gelmiş olabilir: o zaman provizyon artık Pending değildir.
            var result = card.ExpireAuthorization(authorization, now);
            if (result.IsFailure)
                return false;

            try
            {
                await db.SaveChangesAsync(cancellationToken);

                logger.LogInformation(
                    "Provizyon süresi doldu, bloke kaldırıldı. Provizyon: {AuthorizationId}, Kart: {CardNumber}, Kaldırılan: {Value} {LimitType}",
                    authorization.Id, card.CardNumber, authorization.ReservedValue, authorization.LimitType);

                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                logger.LogWarning(
                    "Provizyon zaman aşımı çakıştı, sonraki turda tekrar denenecek. Provizyon: {AuthorizationId}",
                    authorization.Id);

                return false;
            }
        }
    }
}
