using System.ComponentModel.DataAnnotations;

namespace FuelLoyalty.ServiceDefaults.Messaging
{
    /// <summary>
    /// appsettings.json'daki "RabbitMq" bölümü.
    /// Zorunlu alanlar eksikse uygulama açılırken hata verir.
    /// </summary>
    public sealed class RabbitMqOptions
    {
        public const string SectionName = "RabbitMq";

        [Required]
        public string Host { get; init; } = string.Empty;

        public string VirtualHost { get; init; } = "/";

        [Required]
        public string Username { get; init; } = string.Empty;

        [Required]
        public string Password { get; init; } = string.Empty;

        /// <summary>Mesaj işlenemezse kaç kez tekrar denensin</summary>
        [Range(0, 10)]
        public int RetryCount { get; init; } = 3;

        /// <summary>Tekrar denemeler arası bekleme (saniye)</summary>
        [Range(1, 60)]
        public int RetryIntervalSeconds { get; init; } = 1;
    }
}
