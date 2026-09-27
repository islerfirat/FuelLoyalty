namespace FuelLoyalty.Contracts.Admin
{
    /// <summary>Kartın limit türünü ve bakiyesini belirleme isteği.</summary>
    public record UpdateCardLimitRequest(
        LimitType LimitType,
        decimal Balance);
}
