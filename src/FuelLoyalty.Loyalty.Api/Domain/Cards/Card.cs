using FuelLoyalty.Contracts;
using FuelLoyalty.Loyalty.Api.Domain.Authorizations;
using FuelLoyalty.SharedKernel;

namespace FuelLoyalty.Loyalty.Api.Domain.Cards
{
    /// <summary>
    /// Yakıt kartı. Bakiye ve bloke ile ilgili tüm kurallar bu sınıfın içindedir;
    /// alanlar dışarıdan değiştirilemez, sadece metotlar aracılığıyla değişir.
    /// </summary>
    public sealed class Card
    {
        // EF Core veritabanından okurken bu kurucuyu kullanır.
        private Card() { }

        public Guid Id { get; private set; }
        public string CardNumber { get; private set; } = string.Empty;
        public string HolderName { get; private set; } = string.Empty;
        public FuelType AllowedFuelType { get; private set; }
        public LimitType LimitType { get; private set; }
        public decimal Balance { get; private set; }
        public decimal ReservedBalance { get; private set; }
        public bool IsActive { get; private set; }

        /// <summary>Eşzamanlılık kontrolü için sürüm numarası (PostgreSQL xmin).</summary>
        public uint Version { get; private set; }

        /// <summary>Kullanılabilir = Bakiye - Bloke.</summary>
        public decimal AvailableBalance => Balance - ReservedBalance;

        /// <summary>
        /// Yeni kart oluşturur. Kart aktif ve blokesiz başlar.
        /// </summary>
        public static Result<Card> Create(
            string cardNumber,
            string holderName,
            FuelType allowedFuelType,
            LimitType limitType,
            decimal balance)
        {
            if (balance < 0)
                return CardErrors.NegativeBalance;

            return new Card
            {
                Id = Guid.CreateVersion7(),
                CardNumber = cardNumber.Trim(),
                HolderName = holderName.Trim(),
                AllowedFuelType = allowedFuelType,
                LimitType = limitType,
                Balance = balance,
                ReservedBalance = 0,
                IsActive = true
            };
        }

        /// <summary>
        /// Kartın bu yakıt tipiyle yakıt alıp alamayacağını kontrol eder.
        /// </summary>
        public Result EnsureCanFuel(FuelType fuelType)
        {
            if (!IsActive)
                return CardErrors.Inactive;

            if (AllowedFuelType != fuelType)
                return CardErrors.FuelTypeNotAllowed(fuelType, AllowedFuelType);

            if (AvailableBalance <= 0)
                return CardErrors.InsufficientBalance;

            return Result.Success();
        }

        /// <summary>
        /// Kart okutuldu: kontrolleri yapar, kullanılabilir bakiyenin tamamını bloke eder
        /// ve bir provizyon oluşturur.
        /// </summary>
        public Result<CardAuthorization> Authorize(
            FuelType fuelType,
            string stationCode,
            int pumpNo,
            DateTime now,
            TimeSpan validity)
        {
            var canFuel = EnsureCanFuel(fuelType);
            if (canFuel.IsFailure)
                return canFuel.Error;

            var reservedValue = AvailableBalance;
            ReservedBalance += reservedValue;

            return CardAuthorization.Create(this, fuelType, stationCode, pumpNo, reservedValue, now, now.Add(validity));
        }

        /// <summary>
        /// Satış tamamlandı: provizyonun blokesini kaldırır (hâlâ bekliyorsa),
        /// gerçekte kullanılan TL veya litreyi bakiyeden düşer ve provizyonu kapatır.
        /// </summary>
        public Result SettleAuthorization(CardAuthorization authorization, decimal liters, decimal amount, DateTime now)
        {
            if (authorization.CardId != Id)
                return AuthorizationErrors.CardMismatch;

            if (authorization.Status == AuthorizationStatus.Completed)
                return AuthorizationErrors.AlreadyCompleted(authorization.Id);

            var usedValue = authorization.CalculateUsage(liters, amount);

            // Süresi dolmuş provizyonun blokesi zaten kaldırılmıştır.
            if (authorization.Status == AuthorizationStatus.Pending)
                ReleaseReservation(authorization.ReservedValue);

            Balance -= usedValue;
            authorization.MarkCompleted(usedValue, now);

            return Result.Success();
        }

        /// <summary>
        /// Süresi dolan provizyonun blokesini kaldırır.
        /// </summary>
        public Result ExpireAuthorization(CardAuthorization authorization, DateTime now)
        {
            if (authorization.CardId != Id)
                return AuthorizationErrors.CardMismatch;

            if (authorization.Status != AuthorizationStatus.Pending)
                return AuthorizationErrors.NotPending(authorization.Id);

            ReleaseReservation(authorization.ReservedValue);
            authorization.MarkExpired(now);

            return Result.Success();
        }

        /// <summary>
        /// Limit türünü ve bakiyeyi belirler. Devam eden pompa işlemi varken yapılamaz.
        /// </summary>
        public Result SetLimit(LimitType limitType, decimal balance)
        {
            if (balance < 0)
                return CardErrors.NegativeBalance;

            if (ReservedBalance > 0)
                return CardErrors.ActiveReservationExists;

            LimitType = limitType;
            Balance = balance;

            return Result.Success();
        }

        public void Activate() => IsActive = true;

        public void Deactivate() => IsActive = false;

        private void ReleaseReservation(decimal value)
        {
            ReservedBalance = Math.Max(0, ReservedBalance - value);
        }
    }
}
