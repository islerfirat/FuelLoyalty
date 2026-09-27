using FuelLoyalty.Contracts.Admin;
using FuelLoyalty.Loyalty.Api.Common.Mappings;
using FuelLoyalty.Loyalty.Api.Domain.Cards;
using FuelLoyalty.Loyalty.Api.Infrastructure.Persistence;
using FuelLoyalty.ServiceDefaults.Handlers;
using FuelLoyalty.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace FuelLoyalty.Loyalty.Api.Features.Admin.UpdateCardStatus
{
    public sealed class UpdateCardStatusHandler(
        LoyaltyDbContext db,
        ILogger<UpdateCardStatusHandler> logger) : IHandler
    {
        public async Task<Result<CardDto>> HandleAsync(
            string cardNumber,
            UpdateCardStatusRequest request,
            CancellationToken cancellationToken)
        {
            var card = await db.Cards.FirstOrDefaultAsync(c => c.CardNumber == cardNumber, cancellationToken);
            if (card is null)
                return CardErrors.NotFound(cardNumber);

            if (request.IsActive)
                card.Activate();
            else
                card.Deactivate();

            try
            {
                await db.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                return CardErrors.ConcurrencyConflict;
            }

            logger.LogInformation("Kart durumu güncellendi. Kart: {CardNumber}, Aktif: {IsActive}",
                card.CardNumber, card.IsActive);

            return card.ToDto();
        }
    }
}
