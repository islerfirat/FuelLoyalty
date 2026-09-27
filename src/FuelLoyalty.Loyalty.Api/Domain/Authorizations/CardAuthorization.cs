using FuelLoyalty.Contracts;
using FuelLoyalty.Loyalty.Api.Domain.Cards;

namespace FuelLoyalty.Loyalty.Api.Domain.Authorizations
{
    /// <summary>
    /// Kart okutulduğunda verilen onayın (provizyonun) kaydı.
    /// Sadece Card tarafından oluşturulur ve durumu sadece Card tarafından değiştirilir.
    /// </summary>
    public sealed class CardAuthorization
    {
        // EF Core veritabanından okurken bu kurucuyu kullanır.
        private CardAuthorization() { }

        public Guid Id { get; private set; }
        public Guid CardId { get; private set; }
        public string CardNumber { get; private set; } = string.Empty;
        public string StationCode { get; private set; } = string.Empty;
        public int PumpNo { get; private set; }
        public FuelType FuelType { get; private set; }
        public LimitType LimitType { get; private set; }
        public decimal ReservedValue { get; private set; }
        public decimal? UsedValue { get; private set; }
        public AuthorizationStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public DateTime? ClosedAt { get; private set; }

        internal static CardAuthorization Create(
            Card card,
            FuelType fuelType,
            string stationCode,
            int pumpNo,
            decimal reservedValue,
            DateTime now,
            DateTime expiresAt)
            => new()
            {
                Id = Guid.CreateVersion7(),
                CardId = card.Id,
                CardNumber = card.CardNumber,
                StationCode = stationCode,
                PumpNo = pumpNo,
                FuelType = fuelType,
                LimitType = card.LimitType,
                ReservedValue = reservedValue,
                Status = AuthorizationStatus.Pending,
                CreatedAt = now,
                ExpiresAt = expiresAt
            };

        /// <summary>
        /// Satıştaki litre ve tutardan, kartın limit türüne göre düşülecek değeri seçer.
        /// </summary>
        public decimal CalculateUsage(decimal liters, decimal amount)
            => LimitType == LimitType.Liters ? liters : amount;

        internal void MarkCompleted(decimal usedValue, DateTime now)
        {
            UsedValue = usedValue;
            Status = AuthorizationStatus.Completed;
            ClosedAt = now;
        }

        internal void MarkExpired(DateTime now)
        {
            Status = AuthorizationStatus.Expired;
            ClosedAt = now;
        }
    }
}
