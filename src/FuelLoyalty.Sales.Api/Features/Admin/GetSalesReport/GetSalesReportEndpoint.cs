using FuelLoyalty.ServiceDefaults.Endpoints;
using FuelLoyalty.ServiceDefaults.ErrorHandling;

namespace FuelLoyalty.Sales.Api.Features.Admin.GetSalesReport
{
    public sealed class GetSalesReportEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/admin/sales", HandleAsync)
                .WithTags("Admin - Sales");
        }

        private static async Task<IResult> HandleAsync(
            [AsParameters] SalesReportQuery filter,
            GetSalesReportHandler handler,
            CancellationToken cancellationToken)
        {
            var result = await handler.HandleAsync(filter, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(result.Value)
                : result.Error.ToProblem();
        }
    }
}
