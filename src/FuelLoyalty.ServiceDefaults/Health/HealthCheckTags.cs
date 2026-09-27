namespace FuelLoyalty.ServiceDefaults.Health
{
    /// <summary>
    /// Health check etiketleri. Hangi kontrolün hangi adreste çalışacağını belirler.
    /// </summary>
    public static class HealthCheckTags
    {
        /// <summary>
        /// Servisin istek karşılayabilmesi için zorunlu bağımlılıklar (örneğin veritabanı).
        /// /health/ready adresi sadece bu etiketli kontrolleri çalıştırır.
        /// </summary>
        public const string Ready = "service-ready";
    }
}
