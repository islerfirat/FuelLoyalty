namespace FuelLoyalty.Contracts.Admin
{
    /// <summary>Satış raporu: satırlar ve özet toplamlar.</summary>
    public record SalesReportResponse(
        IReadOnlyList<SaleDto> Sales,
        int TotalCount,
        decimal TotalLiters,
        decimal TotalAmount,
        IReadOnlyList<FuelTypeSummary> ByFuelType);
}
