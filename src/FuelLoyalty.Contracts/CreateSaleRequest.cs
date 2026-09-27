namespace FuelLoyalty.Contracts
{
    /// <summary>
    /// Pompada satış bittiğinde gönderilen bilgi.
    /// </summary>
    public record CreateSaleRequest(
        Guid AuthorizationId,
        string StationCode,
        string CardNumber,
        FuelType FuelType,
        decimal Liters,
        decimal Amount);
}
