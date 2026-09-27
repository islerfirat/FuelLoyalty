using FuelLoyalty.Contracts.Admin;
using FuelLoyalty.SharedKernel;

namespace FuelLoyalty.Portal.Models.Sales
{
    /// <summary>
    /// Satış raporu sayfasının verisi: filtre, rapor sonucu ve hesaplanan göstergeler.
    /// </summary>
    public sealed class SalesReportViewModel
    {
        public required SalesReportFilterModel Filter { get; init; }

        public SalesReportResponse? Report { get; init; }

        public Error? Error { get; init; }

        public IReadOnlyList<QuickRange> QuickRanges { get; init; } = [];

        /// <summary>Listede, filtreye uyan satışların hepsi değil en yenileri mi gösteriliyor?</summary>
        public bool IsTruncated => Report is not null && Report.Sales.Count < Report.TotalCount;

        public decimal AverageAmount => Report is { TotalCount: > 0 } report
            ? report.TotalAmount / report.TotalCount
            : 0;

        /// <summary>Yakıt tipinin toplam tutar içindeki payı (yüzde).</summary>
        public decimal ShareOf(FuelTypeSummary summary) => Report is { TotalAmount: > 0 } report
            ? summary.TotalAmount / report.TotalAmount * 100
            : 0;
    }
}
