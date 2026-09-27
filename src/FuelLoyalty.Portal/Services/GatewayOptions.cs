using System.ComponentModel.DataAnnotations;

namespace FuelLoyalty.Portal.Services
{
    /// <summary>
    /// appsettings.json'daki "Gateway" bölümü.
    /// </summary>
    public sealed class GatewayOptions
    {
        public const string SectionName = "Gateway";

        [Required, Url]
        public string BaseUrl { get; init; } = string.Empty;

        [Range(1, 60)]
        public int TimeoutSeconds { get; init; } = 10;
    }
}
