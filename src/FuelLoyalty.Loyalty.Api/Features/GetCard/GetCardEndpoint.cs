using FuelLoyalty.ServiceDefaults.Endpoints;
using FuelLoyalty.ServiceDefaults.ErrorHandling;

namespace FuelLoyalty.Loyalty.Api.Features.GetCard
{
    public sealed class GetCardEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/cards/{cardNumber}", HandleAsync)
                .WithTags("Cards");
        }

        private static async Task<IResult> HandleAsync(
            string cardNumber,
            GetCardHandler handler,
            CancellationToken cancellationToken)
        {
            var result = await handler.HandleAsync(cardNumber, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(result.Value)
                : result.Error.ToProblem();
        }
    }
}
