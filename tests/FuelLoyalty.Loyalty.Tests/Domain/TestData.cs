using FuelLoyalty.Contracts;
using FuelLoyalty.Loyalty.Api.Domain.Authorizations;
using FuelLoyalty.Loyalty.Api.Domain.Cards;

namespace FuelLoyalty.Loyalty.Tests.Domain
{
    /// <summary>
    /// Testlerde tekrar tekrar kullanılan sabit değerler ve hazır nesneler.
    /// </summary>
    public static class TestData
    {
        /// <summary>Testlerin "şu an"ı. Sabit olduğu için testler her çalıştırmada aynı sonucu verir.</summary>
        public static readonly DateTime Now = new(2026, 9, 27, 10, 0, 0, DateTimeKind.Utc);

        public static readonly TimeSpan Validity = TimeSpan.FromMinutes(10);

        public static Card CreateCard(
            FuelType fuelType = FuelType.Benzin,
            LimitType limitType = LimitType.Amount,
            decimal balance = 5000m,
            string cardNumber = "1000200030004000")
            => Card.Create(cardNumber, "34 ABC 123", fuelType, limitType, balance).Value;

        /// <summary>Kart için geçerli bir provizyon oluşturur (kartın kendi yakıt tipiyle).</summary>
        public static CardAuthorization Authorize(Card card, int pumpNo = 1)
            => card.Authorize(card.AllowedFuelType, "IST-001", pumpNo, Now, Validity).Value;
    }
}
