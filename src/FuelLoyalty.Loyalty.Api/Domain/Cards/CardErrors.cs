using FuelLoyalty.Contracts;
using FuelLoyalty.SharedKernel;

namespace FuelLoyalty.Loyalty.Api.Domain.Cards
{
    /// <summary>
    /// Kartla ilgili tüm iş hataları tek yerde.
    /// </summary>
    public static class CardErrors
    {
        public static Error NotFound(string cardNumber) =>
            Error.NotFound("Card.NotFound", $"Kart bulunamadı: {cardNumber}");

        public static Error AlreadyExists(string cardNumber) =>
            Error.Conflict("Card.AlreadyExists", $"Bu kart numarası zaten kayıtlı: {cardNumber}");

        public static readonly Error Inactive =
            Error.Conflict("Card.Inactive", "Kart pasif durumda.");

        public static Error FuelTypeNotAllowed(FuelType requested, FuelType allowed) =>
            Error.Conflict("Card.FuelTypeNotAllowed", $"Bu kart ile {requested} alınamaz. İzin verilen yakıt: {allowed}.");

        public static readonly Error InsufficientBalance =
            Error.Conflict("Card.InsufficientBalance", "Kullanılabilir bakiye yok.");

        public static readonly Error NegativeBalance =
            Error.Validation("Card.NegativeBalance", "Bakiye sıfırdan küçük olamaz.");

        public static readonly Error ActiveReservationExists =
            Error.Conflict("Card.ActiveReservationExists", "Kartta devam eden bir pompa işlemi var. İşlem bitince tekrar deneyin.");

        public static readonly Error ConcurrencyConflict =
            Error.Conflict("Card.ConcurrencyConflict", "Kart bu sırada başka bir işlemle güncellendi. Lütfen tekrar deneyin.");
    }
}
