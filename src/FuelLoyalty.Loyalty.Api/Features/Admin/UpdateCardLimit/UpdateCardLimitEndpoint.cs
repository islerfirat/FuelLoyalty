using FuelLoyalty.Contracts.Admin;
using FuelLoyalty.ServiceDefaults.Endpoints;
using FuelLoyalty.ServiceDefaults.ErrorHandling;
using FuelLoyalty.ServiceDefaults.Validation;

namespace FuelLoyalty.Loyalty.Api.Features.Admin.UpdateCardLimit
{
    public sealed class UpdateCardLimitEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("/admin/cards/{cardNumber}/limit", HandleAsync)
                .WithTags("Admin - Cards")
                .WithRequestValidation<UpdateCardLimitRequest>();
        }

        private static async Task<IResult> HandleAsync(
            string cardNumber,
            UpdateCardLimitRequest request,
            UpdateCardLimitHandler handler,
            CancellationToken cancellationToken)
        {
            var result = await handler.HandleAsync(cardNumber, request, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(result.Value)
                : result.Error.ToProblem();
        }
    }
}
