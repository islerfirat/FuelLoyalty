using FuelLoyalty.Contracts;
using FuelLoyalty.ServiceDefaults.Endpoints;
using FuelLoyalty.ServiceDefaults.Validation;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FuelLoyalty.Loyalty.Api.Features.AuthorizeCard
{
    public sealed class AuthorizeCardEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/cards/authorize", HandleAsync)
                .WithTags("Cards")
                .WithRequestValidation<AuthorizeRequest>();
        }

        private static async Task<Ok<AuthorizeResponse>> HandleAsync(
            AuthorizeRequest request,
            AuthorizeCardHandler handler,
            CancellationToken cancellationToken)
            => TypedResults.Ok(await handler.HandleAsync(request, cancellationToken));
    }
}
