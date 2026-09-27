using FuelLoyalty.Contracts;

namespace FuelLoyalty.Portal.Services.Sales
{
    /// <summary>
    /// Satış raporu filtreleri. Tarihler UTC olmalıdır; To hariç tutulur (From &lt;= satış &lt; To).
    /// </summary>
    public sealed record SalesReportFilter(
        DateTime? FromUtc,
        DateTime? ToUtc,
        string? StationCode,
        string? CardNumber,
        FuelType? FuelType);
}
