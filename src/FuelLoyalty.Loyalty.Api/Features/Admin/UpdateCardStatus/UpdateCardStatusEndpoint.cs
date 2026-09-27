using FuelLoyalty.Contracts.Admin;
using FuelLoyalty.ServiceDefaults.Endpoints;
using FuelLoyalty.ServiceDefaults.ErrorHandling;

namespace FuelLoyalty.Loyalty.Api.Features.Admin.UpdateCardStatus
{
    public sealed class UpdateCardStatusEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("/admin/cards/{cardNumber}/status", HandleAsync)
                .WithTags("Admin - Cards");
        }

        private static async Task<IResult> HandleAsync(
            string cardNumber,
            UpdateCardStatusRequest request,
            UpdateCardStatusHandler handler,
            CancellationToken cancellationToken)
        {
            var result = await handler.HandleAsync(cardNumber, request, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(result.Value)
                : result.Error.ToProblem();
        }
    }
}
