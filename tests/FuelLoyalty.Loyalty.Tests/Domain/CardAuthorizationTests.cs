using FuelLoyalty.Contracts;

namespace FuelLoyalty.Loyalty.Tests.Domain
{
    public class CardAuthorizationTests
    {
        [Fact]
        public void CalculateUsage_TutarLimitliKart_TutariDoner()
        {
            var card = TestData.CreateCard(limitType: LimitType.Amount);
            var authorization = TestData.Authorize(card);

            var usage = authorization.CalculateUsage(liters: 40.5m, amount: 2150.75m);

            Assert.Equal(2150.75m, usage);
        }

        [Fact]
        public void CalculateUsage_LitreLimitliKart_LitreyiDoner()
        {
            var card = TestData.CreateCard(fuelType: FuelType.Motorin, limitType: LimitType.Liters, balance: 200m);
            var authorization = TestData.Authorize(card);

            var usage = authorization.CalculateUsage(liters: 40.5m, amount: 2150.75m);

            Assert.Equal(40.5m, usage);
        }
    }
}
