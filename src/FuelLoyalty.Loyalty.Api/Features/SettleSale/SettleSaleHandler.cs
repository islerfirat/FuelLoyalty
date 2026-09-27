using FuelLoyalty.Contracts;
using FuelLoyalty.Loyalty.Api.Domain.Authorizations;
using FuelLoyalty.Loyalty.Api.Domain.Cards;
using FuelLoyalty.Loyalty.Api.Infrastructure.Persistence;
using FuelLoyalty.ServiceDefaults.Handlers;
using FuelLoyalty.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace FuelLoyalty.Loyalty.Api.Features.SettleSale
{
    /// <summary>
    /// Tamamlanan satışı karta işler: blokeyi kaldırır, kullanımı bakiyeden düşer.
    /// Aynı satış iki kez gelirse ikincisi "zaten tamamlanmış" hatasıyla atlanır (idempotency).
    /// </summary>
    public sealed class SettleSaleHandler(
        LoyaltyDbContext db,
        TimeProvider timeProvider,
        ILogger<SettleSaleHandler> logger) : IHandler
    {
        public async Task<Result> HandleAsync(SaleCompleted sale, CancellationToken cancellationToken)
        {
            var authorization = await db.Authorizations
                .FirstOrDefaultAsync(a => a.Id == sale.AuthorizationId, cancellationToken);

            if (authorization is null)
                return AuthorizationErrors.NotFound(sale.AuthorizationId);

            var card = await db.Cards
                .FirstOrDefaultAsync(c => c.Id == authorization.CardId, cancellationToken);

            if (card is null)
                return CardErrors.NotFound(authorization.CardNumber);

            var now = timeProvider.GetUtcNow().UtcDateTime;

            var result = card.SettleAuthorization(authorization, sale.Liters, sale.Amount, now);
            if (result.IsFailure)
                return result;

            if (authorization.UsedValue > authorization.ReservedValue)
            {
                logger.LogWarning(
                    "Kullanım bloke miktarını aştı. Provizyon: {AuthorizationId}, Bloke: {Reserved}, Kullanılan: {Used}",
                    authorization.Id, authorization.ReservedValue, authorization.UsedValue);
            }

            // Kart aynı anda başka bir işlemle güncellenirse DbUpdateConcurrencyException fırlar
            // ve MassTransit mesajı tekrar dener. Bu yüzden burada yakalanmaz.
            await db.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                "Satış işlendi. Kart: {CardNumber}, Düşülen: {Used} {LimitType}, Yeni bakiye: {Balance}",
                card.CardNumber, authorization.UsedValue, card.LimitType, card.Balance);

            return Result.Success();
        }
    }
}
