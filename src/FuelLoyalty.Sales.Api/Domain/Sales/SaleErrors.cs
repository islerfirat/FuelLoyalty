using FuelLoyalty.SharedKernel;

namespace FuelLoyalty.Sales.Api.Domain.Sales
{
    /// <summary>
    /// Satışla ilgili tüm iş hataları tek yerde.
    /// </summary>
    public static class SaleErrors
    {
        public static readonly Error InvalidQuantity =
            Error.Validation("Sale.InvalidQuantity", "Litre ve tutar sıfırdan büyük olmalı.");

        public static readonly Error InvalidDateRange =
            Error.Validation("Sale.InvalidDateRange", "Başlangıç tarihi bitiş tarihinden sonra olamaz.");
    }
}
