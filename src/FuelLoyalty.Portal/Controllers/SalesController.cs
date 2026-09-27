using FuelLoyalty.Portal.Common;
using FuelLoyalty.Portal.Models.Sales;
using FuelLoyalty.Portal.Services.Sales;
using Microsoft.AspNetCore.Mvc;

namespace FuelLoyalty.Portal.Controllers
{
    /// <summary>
    /// Satış raporu. Filtreler adres çubuğunda taşınır (GET), rapor paylaşılabilir.
    /// </summary>
    public sealed class SalesController(ISalesApiClient salesApi, TimeProvider timeProvider) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index(SalesReportFilterModel filter, CancellationToken cancellationToken)
        {
            var today = BusinessTime.Today(timeProvider);
            var quickRanges = BuildQuickRanges(today);

            // Sayfa hiç filtre olmadan açıldıysa varsayılan olarak son 7 gün gösterilir.
            if (Request.Query.Count == 0)
                filter = SalesReportFilterModel.ForRange(today.AddDays(-6), today);

            if (!ModelState.IsValid)
                return View(new SalesReportViewModel { Filter = filter, QuickRanges = quickRanges });

            var result = await salesApi.GetReportAsync(filter.ToApiFilter(), cancellationToken);

            return View(new SalesReportViewModel
            {
                Filter = filter,
                QuickRanges = quickRanges,
                Report = result.IsSuccess ? result.Value : null,
                Error = result.IsFailure ? result.Error : null
            });
        }

        private static IReadOnlyList<QuickRange> BuildQuickRanges(DateOnly today) =>
        [
            new("Bugün", today, today),
        new("Son 7 gün", today.AddDays(-6), today),
        new("Son 30 gün", today.AddDays(-29), today),
        new("Bu ay", new DateOnly(today.Year, today.Month, 1), today)
        ];
    }
}
