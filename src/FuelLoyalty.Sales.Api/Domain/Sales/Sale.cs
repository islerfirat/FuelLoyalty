using FuelLoyalty.Contracts;
using FuelLoyalty.SharedKernel;

namespace FuelLoyalty.Sales.Api.Domain.Sales
{
    /// <summary>
    /// Pompada tamamlanan bir satış. Oluşturulduktan sonra değiştirilemez.
    /// </summary>
    public sealed class Sale
    {
        // EF Core veritabanından okurken bu kurucuyu kullanır.
        private Sale() { }

        public Guid Id { get; private set; }
        public Guid AuthorizationId { get; private set; }
        public string StationCode { get; private set; } = string.Empty;
        public string CardNumber { get; private set; } = string.Empty;
        public FuelType FuelType { get; private set; }
        public decimal Liters { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime SoldAt { get; private set; }

        public static Result<Sale> Create(
            Guid authorizationId,
            string stationCode,
            string cardNumber,
            FuelType fuelType,
            decimal liters,
            decimal amount,
            DateTime soldAt)
        {
            if (liters <= 0 || amount <= 0)
                return SaleErrors.InvalidQuantity;

            return new Sale
            {
                Id = Guid.CreateVersion7(),
                AuthorizationId = authorizationId,
                StationCode = stationCode.Trim(),
                CardNumber = cardNumber.Trim(),
                FuelType = fuelType,
                Liters = liters,
                Amount = amount,
                SoldAt = soldAt
            };
        }
    }
}
