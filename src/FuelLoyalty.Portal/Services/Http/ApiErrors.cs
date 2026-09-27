using FuelLoyalty.SharedKernel;

namespace FuelLoyalty.Portal.Services.Http
{
    /// <summary>
    /// Portalın kendi iletişim hataları (servislerden gelen iş hataları dışında).
    /// </summary>
    public static class ApiErrors
    {
        public static readonly Error Unreachable =
            Error.Failure("Api.Unreachable", "Sunucuya ulaşılamadı. Lütfen daha sonra tekrar deneyin.");

        public static readonly Error Timeout =
            Error.Failure("Api.Timeout", "Sunucu zamanında cevap vermedi. Lütfen tekrar deneyin.");

        public static readonly Error EmptyResponse =
            Error.Failure("Api.EmptyResponse", "Sunucudan boş cevap alındı.");
    }
}
