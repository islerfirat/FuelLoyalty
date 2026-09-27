using System.Globalization;

namespace FuelLoyalty.Portal.Models.Sales
{
    /// <summary>
    /// Hızlı tarih seçimi düğmesi (Bugün, Son 7 gün...).
    /// </summary>
    public sealed record QuickRange(string Label, DateOnly From, DateOnly To)
    {
        /// <summary>Düğmenin bağlantısında kullanılacak sorgu parametreleri.</summary>
        public IDictionary<string, string> RouteValues => new Dictionary<string, string>
        {
            ["Filter.From"] = From.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            ["Filter.To"] = To.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
        };

        /// <summary>Mevcut filtre bu aralıkla aynı mı (düğmeyi seçili göstermek için).</summary>
        public bool Matches(SalesReportFilterModel filter) => filter.From == From && filter.To == To;
    }
}
