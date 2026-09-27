using FuelLoyalty.Contracts.Admin;
using FuelLoyalty.ServiceDefaults.Endpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FuelLoyalty.Loyalty.Api.Features.Admin.GetCards
{
    public sealed class GetCardsEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/admin/cards", HandleAsync)
                .WithTags("Admin - Cards");
        }

        private static async Task<Ok<IReadOnlyList<CardDto>>> HandleAsync(
            string? search,
            GetCardsHandler handler,
            CancellationToken cancellationToken)
            => TypedResults.Ok(await handler.HandleAsync(search, cancellationToken));
    }
}
