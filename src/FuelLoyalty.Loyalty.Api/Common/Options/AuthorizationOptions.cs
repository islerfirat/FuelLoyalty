using System.ComponentModel.DataAnnotations;

namespace FuelLoyalty.Loyalty.Api.Common.Options
{
    /// <summary>
    /// appsettings.json'daki "Authorization" bölümü.
    /// </summary>
    public sealed class AuthorizationOptions
    {
        public const string SectionName = "Authorization";

        /// <summary>Provizyon kaç dakika geçerli</summary>
        [Range(1, 120)]
        public int ExpiryMinutes { get; init; } = 10;

        /// <summary>Süresi dolan provizyonlar kaç saniyede bir kontrol edilsin</summary>
        [Range(5, 3600)]
        public int ExpiryCheckSeconds { get; init; } = 30;
    }
}
