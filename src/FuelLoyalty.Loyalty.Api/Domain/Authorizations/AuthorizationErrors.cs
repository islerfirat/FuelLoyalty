using FuelLoyalty.SharedKernel;

namespace FuelLoyalty.Loyalty.Api.Domain.Authorizations
{
    /// <summary>
    /// Provizyonla ilgili tüm iş hataları tek yerde.
    /// </summary>
    public static class AuthorizationErrors
    {
        public static Error NotFound(Guid authorizationId) =>
            Error.NotFound("Authorization.NotFound", $"Provizyon bulunamadı: {authorizationId}");

        public static Error AlreadyCompleted(Guid authorizationId) =>
            Error.Conflict("Authorization.AlreadyCompleted", $"Provizyon zaten tamamlanmış: {authorizationId}");

        public static Error NotPending(Guid authorizationId) =>
            Error.Conflict("Authorization.NotPending", $"Provizyon beklemede değil: {authorizationId}");

        public static readonly Error CardMismatch =
            Error.Conflict("Authorization.CardMismatch", "Provizyon bu karta ait değil.");
    }
}
