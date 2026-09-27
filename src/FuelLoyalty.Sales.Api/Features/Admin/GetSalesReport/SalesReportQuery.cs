using FuelLoyalty.Contracts;

namespace FuelLoyalty.Sales.Api.Features.Admin.GetSalesReport
{
    /// <summary>
    /// Satış raporu filtreleri. Adresteki sorgu parametrelerinden doldurulur:
    /// /admin/sales?from=2026-09-01&amp;to=2026-09-30&amp;stationCode=IST-001&amp;fuelType=Benzin
    /// </summary>
    public sealed record SalesReportQuery(
        DateTime? From,
        DateTime? To,
        string? StationCode,
        string? CardNumber,
        FuelType? FuelType);
}
