using FuelLoyalty.Contracts;

namespace FuelLoyalty.PumpSimulator.Models
{
    /// <summary>
    /// Tek bir dolum işleminin hesabı: verilen litre ve tutar, onaylı limite göre durma kararı.
    /// Ekrandan ve zamanlayıcıdan bağımsızdır; tek başına test edilebilir.
    /// </summary>
    public sealed class FuelingSession
    {
        public FuelingSession(LimitType limitType, decimal maxValue, decimal unitPrice)
        {
            if (maxValue <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxValue), "Onaylı limit sıfırdan büyük olmalı.");

            if (unitPrice <= 0)
                throw new ArgumentOutOfRangeException(nameof(unitPrice), "Birim fiyat sıfırdan büyük olmalı.");

            LimitType = limitType;
            MaxValue = maxValue;
            UnitPrice = unitPrice;
        }

        public LimitType LimitType { get; }
        public decimal MaxValue { get; }
        public decimal UnitPrice { get; }
        public decimal Liters { get; private set; }
        public decimal Amount { get; private set; }

        /// <summary>
        /// Sayacı verilen litre kadar ilerletir. Limite ulaşıldıysa değeri tam limite eşitler ve true döner.
        /// TL limitinde tutara, litre limitinde litreye bakılır.
        /// </summary>
        public bool Advance(decimal litersStep)
        {
            var nextLiters = Liters + litersStep;
            var nextAmount = Math.Round(nextLiters * UnitPrice, 2);

            if (LimitType == LimitType.Liters && nextLiters >= MaxValue)
            {
                Liters = MaxValue;
                Amount = Math.Round(MaxValue * UnitPrice, 2);
                return true;
            }

            if (LimitType == LimitType.Amount && nextAmount >= MaxValue)
            {
                Amount = MaxValue;
                Liters = Math.Round(MaxValue / UnitPrice, 2);
                return true;
            }

            Liters = nextLiters;
            Amount = nextAmount;
            return false;
        }
    }
}
