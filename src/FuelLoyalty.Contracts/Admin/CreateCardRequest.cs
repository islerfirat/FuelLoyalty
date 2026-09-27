namespace FuelLoyalty.Contracts.Admin
{
    /// <summary>Yeni kart ekleme isteği.</summary>
    public record CreateCardRequest(
        string CardNumber,
        string HolderName,
        FuelType AllowedFuelType,
        LimitType LimitType,
        decimal Balance);
}
