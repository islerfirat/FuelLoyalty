using FuelLoyalty.Contracts.Admin;
using FuelLoyalty.Loyalty.Api.Domain.Cards;

namespace FuelLoyalty.Loyalty.Api.Common.Mappings
{
    /// <summary>
    /// Domain nesnesini dışarıya dönülen sözleşmeye çevirir.
    /// Domain nesnesi hiçbir zaman doğrudan API cevabı olarak dönülmez.
    /// </summary>
    public static class CardMappings
    {
        public static CardDto ToDto(this Card card) => new(
            card.Id,
            card.CardNumber,
            card.HolderName,
            card.AllowedFuelType,
            card.LimitType,
            card.Balance,
            card.ReservedBalance,
            card.AvailableBalance,
            card.IsActive);
    }
}
