using FuelLoyalty.Contracts.Admin;
using FuelLoyalty.SharedKernel;

namespace FuelLoyalty.Portal.Models.Cards
{
    /// <summary>
    /// Kart listesi sayfasının verisi.
    /// </summary>
    public sealed class CardListViewModel
    {
        public IReadOnlyList<CardDto> Cards { get; init; } = [];

        public string? Search { get; init; }

        public Error? Error { get; init; }

        public int ActiveCount => Cards.Count(c => c.IsActive);

        public int PassiveCount => Cards.Count(c => !c.IsActive);

        /// <summary>Pompada devam eden işlemi (blokesi) olan kart sayısı.</summary>
        public int InProgressCount => Cards.Count(c => c.ReservedBalance > 0);
    }
}
