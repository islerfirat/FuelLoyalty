using FuelLoyalty.Contracts;
using FuelLoyalty.PumpSimulator.Models;

namespace FuelLoyalty.PumpSimulator.Tests.Models
{
    public class FuelingSessionTests
    {
        private const decimal Step = 0.25m;

        [Fact]
        public void Advance_LimitAltinda_SayacArtarVeFalseDoner()
        {
            var session = new FuelingSession(LimitType.Amount, maxValue: 1000m, unitPrice: 40m);

            var limitReached = session.Advance(Step);

            Assert.False(limitReached);
            Assert.Equal(0.25m, session.Liters);
            Assert.Equal(10.00m, session.Amount);
        }

        [Fact]
        public void Advance_TutarLimiti_TamLimitteDurur()
        {
            // 100 TL limit, 40 TL/L: her adımda 10 TL, 10. adımda limite ulaşılır
            var session = new FuelingSession(LimitType.Amount, maxValue: 100m, unitPrice: 40m);

            var steps = AdvanceUntilLimit(session);

            Assert.Equal(10, steps);
            Assert.Equal(100m, session.Amount);
            Assert.Equal(2.50m, session.Liters);
        }

        [Fact]
        public void Advance_TutarLimiti_TutarLimitiAsmaz()
        {
            // 47,50 TL/L fiyatla 100 TL tam bir adıma denk gelmez; tutar yine de 100'ü geçmemeli
            var session = new FuelingSession(LimitType.Amount, maxValue: 100m, unitPrice: 47.50m);

            AdvanceUntilLimit(session);

            Assert.Equal(100m, session.Amount);
            Assert.Equal(2.11m, session.Liters);
        }

        [Fact]
        public void Advance_LitreLimiti_TamLimitteDurur()
        {
            var session = new FuelingSession(LimitType.Liters, maxValue: 1m, unitPrice: 40m);

            var steps = AdvanceUntilLimit(session);

            Assert.Equal(4, steps);
            Assert.Equal(1m, session.Liters);
            Assert.Equal(40m, session.Amount);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public void Constructor_GecersizBirimFiyat_HataFirlatir(int unitPrice)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new FuelingSession(LimitType.Amount, maxValue: 100m, unitPrice: unitPrice));
        }

        [Fact]
        public void Constructor_SifirLimit_HataFirlatir()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new FuelingSession(LimitType.Amount, maxValue: 0m, unitPrice: 40m));
        }

        /// <summary>Limite ulaşana kadar sayacı ilerletir, kaç adım sürdüğünü döner.</summary>
        private static int AdvanceUntilLimit(FuelingSession session)
        {
            const int safetyLimit = 100_000;   // Hata olursa test sonsuz döngüye girmesin

            for (var step = 1; step <= safetyLimit; step++)
            {
                if (session.Advance(Step))
                    return step;
            }

            throw new InvalidOperationException("Limit hiç dolmadı.");
        }
    }
}
