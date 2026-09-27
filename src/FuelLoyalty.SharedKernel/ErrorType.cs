namespace FuelLoyalty.SharedKernel
{
    /// <summary>
    /// Hatanın türü. HTTP cevap kodu bu türe göre belirlenir.
    /// </summary>
    public enum ErrorType
    {
        Failure = 0,      // Beklenmeyen / genel hata        → 500
        Validation = 1,   // Gönderilen veri hatalı           → 400
        NotFound = 2,     // Kayıt bulunamadı                 → 404
        Conflict = 3      // Mevcut durumla çakışıyor         → 409
    }
}
