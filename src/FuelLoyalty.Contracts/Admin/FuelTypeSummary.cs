namespace FuelLoyalty.Contracts.Admin
{
    /// <summary>Yakıt tipine göre toplamlar.</summary>
    public record FuelTypeSummary(
        FuelType FuelType,
        int Count,
        decimal TotalLiters,
        decimal TotalAmount);
}
