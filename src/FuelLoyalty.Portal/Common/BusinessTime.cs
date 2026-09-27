namespace FuelLoyalty.Portal.Common
{
    /// <summary>
    /// İş saat dilimi (Türkiye) ile UTC arasındaki dönüşümler.
    /// Veriler UTC saklanır; kullanıcı tarihleri Türkiye saatiyle girer ve görür.
    /// </summary>
    public static class BusinessTime
    {
        public static readonly TimeZoneInfo Zone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Istanbul");

        /// <summary>Türkiye saatine göre bugünün tarihi.</summary>
        public static DateOnly Today(TimeProvider timeProvider)
            => DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(timeProvider.GetUtcNow().UtcDateTime, Zone));

        /// <summary>Verilen günün Türkiye saatiyle başlangıcını (00:00) UTC olarak döner.</summary>
        public static DateTime StartOfDayUtc(DateOnly date)
            => TimeZoneInfo.ConvertTimeToUtc(date.ToDateTime(TimeOnly.MinValue), Zone);

        /// <summary>UTC bir zamanı Türkiye saatine çevirir.</summary>
        public static DateTime ToLocal(DateTime utc)
            => TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), Zone);
    }
}
