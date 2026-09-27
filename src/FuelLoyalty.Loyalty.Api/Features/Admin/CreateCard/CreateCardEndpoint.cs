using FuelLoyalty.Contracts.Admin;
using FuelLoyalty.ServiceDefaults.Endpoints;
using FuelLoyalty.ServiceDefaults.ErrorHandling;
using FuelLoyalty.ServiceDefaults.Validation;

namespace FuelLoyalty.Loyalty.Api.Features.Admin.CreateCard
{
    public sealed class CreateCardEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/admin/cards", HandleAsync)
                .WithTags("Admin - Cards")
                .WithRequestValidation<CreateCardRequest>();
        }

        private static async Task<IResult> HandleAsync(
            CreateCardRequest request,
            CreateCardHandler handler,
            CancellationToken cancellationToken)
        {
            var result = await handler.HandleAsync(request, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Created($"/admin/cards/{result.Value.CardNumber}", result.Value)
                : result.Error.ToProblem();
        }
    }
}
