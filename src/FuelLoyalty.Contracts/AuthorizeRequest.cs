namespace FuelLoyalty.Contracts
{
    /// <summary>
    /// Pompada kart okutulduğunda gönderilen onay isteği.
    /// </summary>
    public record AuthorizeRequest(
        string CardNumber,
        FuelType FuelType,
        string StationCode,
        int PumpNo);
}
