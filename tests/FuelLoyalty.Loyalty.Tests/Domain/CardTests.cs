using FuelLoyalty.Contracts;
using FuelLoyalty.Loyalty.Api.Domain.Authorizations;
using FuelLoyalty.Loyalty.Api.Domain.Cards;

namespace FuelLoyalty.Loyalty.Tests.Domain
{
    public class CardTests
    {
        // ================= Oluşturma =================

        [Fact]
        public void Create_EksiBakiye_NegativeBalanceHatasiDoner()
        {
            var result = Card.Create("1000200030004000", "34 ABC 123", FuelType.Benzin, LimitType.Amount, -1m);

            Assert.True(result.IsFailure);
            Assert.Equal(CardErrors.NegativeBalance, result.Error);
        }

        [Fact]
        public void Create_GecerliBilgiler_AktifVeBlokesizKartOlusur()
        {
            var card = TestData.CreateCard(balance: 5000m);

            Assert.True(card.IsActive);
            Assert.Equal(5000m, card.Balance);
            Assert.Equal(0m, card.ReservedBalance);
            Assert.Equal(5000m, card.AvailableBalance);
        }

        // ================= Onay (provizyon) =================

        [Fact]
        public void Authorize_UygunKart_KullanilabilirBakiyeninTamaminiBlokeEder()
        {
            // Arrange
            var card = TestData.CreateCard(balance: 5000m);

            // Act
            var result = card.Authorize(FuelType.Benzin, "IST-001", 1, TestData.Now, TestData.Validity);

            // Assert
            Assert.True(result.IsSuccess);

            var authorization = result.Value;
            Assert.Equal(card.Id, authorization.CardId);
            Assert.Equal(5000m, authorization.ReservedValue);
            Assert.Equal(AuthorizationStatus.Pending, authorization.Status);
            Assert.Equal(TestData.Now.Add(TestData.Validity), authorization.ExpiresAt);

            Assert.Equal(5000m, card.ReservedBalance);
            Assert.Equal(0m, card.AvailableBalance);
        }

        [Fact]
        public void Authorize_PasifKart_InactiveHatasiDoner()
        {
            var card = TestData.CreateCard();
            card.Deactivate();

            var result = card.Authorize(FuelType.Benzin, "IST-001", 1, TestData.Now, TestData.Validity);

            Assert.Equal(CardErrors.Inactive, result.Error);
            Assert.Equal(0m, card.ReservedBalance);
        }

        [Fact]
        public void Authorize_FarkliYakitTipi_FuelTypeNotAllowedHatasiDoner()
        {
            var card = TestData.CreateCard(fuelType: FuelType.Motorin);

            var result = card.Authorize(FuelType.Benzin, "IST-001", 1, TestData.Now, TestData.Validity);

            Assert.Equal("Card.FuelTypeNotAllowed", result.Error.Code);
            Assert.Equal(0m, card.ReservedBalance);
        }

        [Fact]
        public void Authorize_BakiyeSifir_InsufficientBalanceHatasiDoner()
        {
            var card = TestData.CreateCard(balance: 0m);

            var result = card.Authorize(FuelType.Benzin, "IST-001", 1, TestData.Now, TestData.Validity);

            Assert.Equal(CardErrors.InsufficientBalance, result.Error);
        }

        [Fact]
        public void Authorize_KartBaskaPompadaBlokeliyken_InsufficientBalanceHatasiDoner()
        {
            // Arrange: kart 1. pompada okutuldu
            var card = TestData.CreateCard(balance: 5000m);
            TestData.Authorize(card, pumpNo: 1);

            // Act: aynı kart 2. pompada okutuluyor
            var result = card.Authorize(FuelType.Benzin, "IST-001", 2, TestData.Now, TestData.Validity);

            // Assert
            Assert.Equal(CardErrors.InsufficientBalance, result.Error);
            Assert.Equal(5000m, card.ReservedBalance);
        }

        // ================= Satış düşümü =================

        [Fact]
        public void SettleAuthorization_TutarLimitliKart_TutarDusulurVeBlokeKalkar()
        {
            var card = TestData.CreateCard(limitType: LimitType.Amount, balance: 5000m);
            var authorization = TestData.Authorize(card);

            var result = card.SettleAuthorization(authorization, liters: 40.5m, amount: 2150.75m, TestData.Now.AddMinutes(3));

            Assert.True(result.IsSuccess);
            Assert.Equal(2849.25m, card.Balance);
            Assert.Equal(0m, card.ReservedBalance);
            Assert.Equal(AuthorizationStatus.Completed, authorization.Status);
            Assert.Equal(2150.75m, authorization.UsedValue);
        }

        [Fact]
        public void SettleAuthorization_LitreLimitliKart_LitreDusulur()
        {
            var card = TestData.CreateCard(fuelType: FuelType.Motorin, limitType: LimitType.Liters, balance: 200m);
            var authorization = TestData.Authorize(card);

            var result = card.SettleAuthorization(authorization, liters: 50m, amount: 2400m, TestData.Now.AddMinutes(3));

            Assert.True(result.IsSuccess);
            Assert.Equal(150m, card.Balance);
            Assert.Equal(0m, card.ReservedBalance);
        }

        [Fact]
        public void SettleAuthorization_AyniSatisIkinciKez_HataDonerVeBakiyeTekrarDusmez()
        {
            // Arrange: satış bir kez işlendi
            var card = TestData.CreateCard(balance: 5000m);
            var authorization = TestData.Authorize(card);
            card.SettleAuthorization(authorization, 40.5m, 2150.75m, TestData.Now.AddMinutes(3));

            // Act: aynı mesaj RabbitMQ'dan ikinci kez geldi
            var secondResult = card.SettleAuthorization(authorization, 40.5m, 2150.75m, TestData.Now.AddMinutes(4));

            // Assert: bakiye iki kez düşmedi (idempotency)
            Assert.Equal("Authorization.AlreadyCompleted", secondResult.Error.Code);
            Assert.Equal(2849.25m, card.Balance);
        }

        [Fact]
        public void SettleAuthorization_BaskaKartinProvizyonu_CardMismatchHatasiDoner()
        {
            var card = TestData.CreateCard(cardNumber: "1000200030004000");
            var otherCard = TestData.CreateCard(cardNumber: "1000200030004099");
            var otherAuthorization = TestData.Authorize(otherCard);

            var result = card.SettleAuthorization(otherAuthorization, 10m, 500m, TestData.Now);

            Assert.Equal(AuthorizationErrors.CardMismatch, result.Error);
            Assert.Equal(5000m, card.Balance);
        }

        // ================= Zaman aşımı =================

        [Fact]
        public void ExpireAuthorization_BekleyenProvizyon_BlokeKalkar()
        {
            var card = TestData.CreateCard(balance: 5000m);
            var authorization = TestData.Authorize(card);

            var result = card.ExpireAuthorization(authorization, TestData.Now.AddMinutes(11));

            Assert.True(result.IsSuccess);
            Assert.Equal(0m, card.ReservedBalance);
            Assert.Equal(5000m, card.AvailableBalance);
            Assert.Equal(AuthorizationStatus.Expired, authorization.Status);
        }

        [Fact]
        public void SettleAuthorization_SuresiDolmusProvizyon_SadeceKullanimDusulur()
        {
            // Arrange: provizyonun süresi doldu, bloke kalktı
            var card = TestData.CreateCard(balance: 5000m);
            var authorization = TestData.Authorize(card);
            card.ExpireAuthorization(authorization, TestData.Now.AddMinutes(11));

            // Act: satış bilgisi geç geldi (yakıt verilmişti)
            var result = card.SettleAuthorization(authorization, 40.5m, 2150.75m, TestData.Now.AddMinutes(12));

            // Assert: kullanım düşüldü, bloke eksiye düşmedi
            Assert.True(result.IsSuccess);
            Assert.Equal(2849.25m, card.Balance);
            Assert.Equal(0m, card.ReservedBalance);
            Assert.Equal(AuthorizationStatus.Completed, authorization.Status);
        }

        [Fact]
        public void ExpireAuthorization_TamamlanmisProvizyon_NotPendingHatasiDoner()
        {
            var card = TestData.CreateCard();
            var authorization = TestData.Authorize(card);
            card.SettleAuthorization(authorization, 40.5m, 2150.75m, TestData.Now.AddMinutes(3));

            var result = card.ExpireAuthorization(authorization, TestData.Now.AddMinutes(11));

            Assert.Equal("Authorization.NotPending", result.Error.Code);
            Assert.Equal(AuthorizationStatus.Completed, authorization.Status);
        }

        // ================= Limit belirleme =================

        [Fact]
        public void SetLimit_BlokeYokken_LimitTuruVeBakiyeGuncellenir()
        {
            var card = TestData.CreateCard(limitType: LimitType.Amount, balance: 5000m);

            var result = card.SetLimit(LimitType.Liters, 300m);

            Assert.True(result.IsSuccess);
            Assert.Equal(LimitType.Liters, card.LimitType);
            Assert.Equal(300m, card.Balance);
        }

        [Fact]
        public void SetLimit_KartBlokeliyken_ActiveReservationExistsHatasiDoner()
        {
            var card = TestData.CreateCard(limitType: LimitType.Amount, balance: 5000m);
            TestData.Authorize(card);

            var result = card.SetLimit(LimitType.Liters, 300m);

            Assert.Equal(CardErrors.ActiveReservationExists, result.Error);
            Assert.Equal(LimitType.Amount, card.LimitType);
            Assert.Equal(5000m, card.Balance);
        }

        [Fact]
        public void SetLimit_EksiBakiye_NegativeBalanceHatasiDoner()
        {
            var card = TestData.CreateCard();

            var result = card.SetLimit(LimitType.Amount, -10m);

            Assert.Equal(CardErrors.NegativeBalance, result.Error);
        }
    }
}
