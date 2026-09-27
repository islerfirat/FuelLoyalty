using FuelLoyalty.Contracts.Admin;
using FuelLoyalty.SharedKernel;

namespace FuelLoyalty.Portal.Services.Sales
{
    /// <summary>
    /// Satış raporu işlemleri.
    /// </summary>
    public interface ISalesApiClient
    {
        Task<Result<SalesReportResponse>> GetReportAsync(SalesReportFilter filter, CancellationToken cancellationToken);
    }
}
