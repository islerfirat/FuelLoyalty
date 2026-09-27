using FuelLoyalty.Contracts.Admin;
using FuelLoyalty.Loyalty.Api.Common.Mappings;
using FuelLoyalty.Loyalty.Api.Domain.Cards;
using FuelLoyalty.Loyalty.Api.Infrastructure.Persistence;
using FuelLoyalty.ServiceDefaults.Handlers;
using FuelLoyalty.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace FuelLoyalty.Loyalty.Api.Features.Admin.CreateCard
{
    public sealed class CreateCardHandler(
        LoyaltyDbContext db,
        ILogger<CreateCardHandler> logger) : IHandler
    {
        public async Task<Result<CardDto>> HandleAsync(CreateCardRequest request, CancellationToken cancellationToken)
        {
            var cardNumber = request.CardNumber.Trim();

            if (await db.Cards.AnyAsync(c => c.CardNumber == cardNumber, cancellationToken))
                return CardErrors.AlreadyExists(cardNumber);

            var result = Card.Create(cardNumber, request.HolderName, request.AllowedFuelType, request.LimitType, request.Balance);
            if (result.IsFailure)
                return result.Error;

            var card = result.Value;
            db.Cards.Add(card);
            await db.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Kart eklendi. Kart: {CardNumber}, Limit: {Balance} {LimitType}",
                card.CardNumber, card.Balance, card.LimitType);

            return card.ToDto();
        }
    }
}
