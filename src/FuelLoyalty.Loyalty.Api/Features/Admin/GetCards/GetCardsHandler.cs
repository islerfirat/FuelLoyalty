using FuelLoyalty.Contracts.Admin;
using FuelLoyalty.Loyalty.Api.Common.Mappings;
using FuelLoyalty.Loyalty.Api.Infrastructure.Persistence;
using FuelLoyalty.ServiceDefaults.Handlers;
using Microsoft.EntityFrameworkCore;

namespace FuelLoyalty.Loyalty.Api.Features.Admin.GetCards
{
    public sealed class GetCardsHandler(LoyaltyDbContext db) : IHandler
    {
        public async Task<IReadOnlyList<CardDto>> HandleAsync(string? search, CancellationToken cancellationToken)
        {
            var query = db.Cards.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(c => c.CardNumber.Contains(term) || c.HolderName.Contains(term));
            }

            var cards = await query
                .OrderBy(c => c.CardNumber)
                .ToListAsync(cancellationToken);

            return cards.Select(c => c.ToDto()).ToList();
        }
    }
}
