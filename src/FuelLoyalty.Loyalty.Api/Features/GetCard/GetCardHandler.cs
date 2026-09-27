using FuelLoyalty.Contracts.Admin;
using FuelLoyalty.Loyalty.Api.Common.Mappings;
using FuelLoyalty.Loyalty.Api.Domain.Cards;
using FuelLoyalty.Loyalty.Api.Infrastructure.Persistence;
using FuelLoyalty.ServiceDefaults.Handlers;
using FuelLoyalty.SharedKernel;
using Microsoft.EntityFrameworkCore;
namespace FuelLoyalty.Loyalty.Api.Features.GetCard
{
    public sealed class GetCardHandler(LoyaltyDbContext db) : IHandler
    {
        public async Task<Result<CardDto>> HandleAsync(string cardNumber, CancellationToken cancellationToken)
        {
            var card = await db.Cards.AsNoTracking()
                .FirstOrDefaultAsync(c => c.CardNumber == cardNumber, cancellationToken);

            if (card is null)
                return CardErrors.NotFound(cardNumber);

            return card.ToDto();
        }
    }
}
