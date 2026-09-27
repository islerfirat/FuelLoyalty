using FuelLoyalty.Contracts.Admin;
using FuelLoyalty.Loyalty.Api.Common.Mappings;
using FuelLoyalty.Loyalty.Api.Domain.Cards;
using FuelLoyalty.Loyalty.Api.Infrastructure.Persistence;
using FuelLoyalty.ServiceDefaults.Handlers;
using FuelLoyalty.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace FuelLoyalty.Loyalty.Api.Features.Admin.UpdateCardLimit
{
    public sealed class UpdateCardLimitHandler(
       LoyaltyDbContext db,
       ILogger<UpdateCardLimitHandler> logger) : IHandler
    {
        public async Task<Result<CardDto>> HandleAsync(
            string cardNumber,
            UpdateCardLimitRequest request,
            CancellationToken cancellationToken)
        {
            var card = await db.Cards.FirstOrDefaultAsync(c => c.CardNumber == cardNumber, cancellationToken);
            if (card is null)
                return CardErrors.NotFound(cardNumber);

            var result = card.SetLimit(request.LimitType, request.Balance);
            if (result.IsFailure)
                return result.Error;

            try
            {
                await db.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                return CardErrors.ConcurrencyConflict;
            }

            logger.LogInformation("Kart limiti güncellendi. Kart: {CardNumber}, Yeni limit: {Balance} {LimitType}",
                card.CardNumber, card.Balance, card.LimitType);

            return card.ToDto();
        }
    }
}
