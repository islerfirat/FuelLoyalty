using FuelLoyalty.Contracts.Admin;
using FuelLoyalty.SharedKernel;

namespace FuelLoyalty.Portal.Models.Cards
{
    /// <summary>
    /// Kart detay sayfasının verisi: kart bilgisi, limit formu ve olası hata.
    /// </summary>
    public sealed class CardDetailsViewModel
    {
        public required string CardNumber { get; init; }

        public CardDto? Card { get; init; }

        public Error? Error { get; init; }

        public UpdateLimitFormModel LimitForm { get; init; } = new();

        /// <summary>Pompada devam eden işlem varsa limit değiştirilemez.</summary>
        public bool HasActiveReservation => Card?.ReservedBalance > 0;
    }
}
