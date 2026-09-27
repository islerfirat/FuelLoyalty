namespace FuelLoyalty.Loyalty.Api.Domain.Authorizations
{
    public enum AuthorizationStatus
    {
        Pending = 1,     // Onay verildi, yakıt bekleniyor (tutar bloke)
        Completed = 2,   // Satış bitti, bakiyeden düşüldü
        Expired = 3      // Süresi doldu, bloke kaldırıldı
    }
}
