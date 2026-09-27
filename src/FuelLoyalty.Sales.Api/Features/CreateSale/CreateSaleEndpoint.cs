using FuelLoyalty.Contracts;
using FuelLoyalty.ServiceDefaults.Endpoints;
using FuelLoyalty.ServiceDefaults.ErrorHandling;
using FuelLoyalty.ServiceDefaults.Validation;

namespace FuelLoyalty.Sales.Api.Features.CreateSale
{
    public sealed class CreateSaleEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/sales", HandleAsync)
                .WithTags("Pump")
                .WithRequestValidation<CreateSaleRequest>();
        }

        private static async Task<IResult> HandleAsync(
            CreateSaleRequest request,
            CreateSaleHandler handler,
            CancellationToken cancellationToken)
        {
            var result = await handler.HandleAsync(request, cancellationToken);

            if (result.IsFailure)
                return result.Error.ToProblem();

            var response = new CreateSaleResponse(result.Value.SaleId);

            // Yeni satış: 202 (bakiye düşümü arka planda). Tekrar gelen satış: 200 (zaten işlenmiş).
            return result.Value.IsDuplicate
                ? TypedResults.Ok(response)
                : TypedResults.Accepted((string?)null, response);
        }
    }
}
