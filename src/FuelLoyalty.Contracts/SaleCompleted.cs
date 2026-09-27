namespace FuelLoyalty.Contracts
{
    /// <summary>
    /// Bir akaryakıt satışı tamamlandığında yayınlanan mesaj.
    /// Bu mesajı dinleyen servisler (örneğin Loyalty) kendi işlerini yapar.
    /// </summary>
    public record SaleCompleted(
        Guid SaleId,            // Satışın benzersiz numarası
        Guid AuthorizationId,   // Kart okutulurken alınan provizyon numarası
        string StationCode,     // Hangi istasyon (örnek: "IST-001")
        string CardNumber,      // Kart numarası
        FuelType FuelType,      // Yakıt tipi
        decimal Liters,         // Verilen litre
        decimal Amount,         // Tutar (TL)
        DateTime SoldAt);       // Satış zamanı (UTC)
}
