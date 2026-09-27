using FuelLoyalty.Contracts.Admin;
using FuelLoyalty.ServiceDefaults.Handlers;
using FuelLoyalty.SharedKernel;
using FuelLoyalty.Sales.Api.Domain.Sales;
using FuelLoyalty.Sales.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FuelLoyalty.Sales.Api.Features.Admin.GetSalesReport
{
    /// <summary>
    /// Satış raporu: filtreye uyan en yeni 500 satır ve filtreye uyan tüm satışların toplamları.
    /// </summary>
    public sealed class GetSalesReportHandler(SalesDbContext db) : IHandler
    {
        private const int MaxRows = 500;

        public async Task<Result<SalesReportResponse>> HandleAsync(SalesReportQuery filter, CancellationToken cancellationToken)
        {
            if (filter.From.HasValue && filter.To.HasValue && filter.From > filter.To)
                return SaleErrors.InvalidDateRange;

            var query = db.Sales.AsNoTracking();

            if (filter.From.HasValue)
            {
                var fromUtc = ToUtc(filter.From.Value);
                query = query.Where(s => s.SoldAt >= fromUtc);
            }

            if (filter.To.HasValue)
            {
                var toUtc = ToUtc(filter.To.Value);
                query = query.Where(s => s.SoldAt < toUtc);
            }

            if (!string.IsNullOrWhiteSpace(filter.StationCode))
                query = query.Where(s => s.StationCode == filter.StationCode);

            if (!string.IsNullOrWhiteSpace(filter.CardNumber))
                query = query.Where(s => s.CardNumber == filter.CardNumber);

            if (filter.FuelType.HasValue)
                query = query.Where(s => s.FuelType == filter.FuelType.Value);

            // Toplamlar veritabanında hesaplanır (GROUP BY), satırlar belleğe çekilmez.
            var byFuelType = await query
                .GroupBy(s => s.FuelType)
                .Select(g => new FuelTypeSummary(g.Key, g.Count(), g.Sum(s => s.Liters), g.Sum(s => s.Amount)))
                .ToListAsync(cancellationToken);

            var sales = await query
                .OrderByDescending(s => s.SoldAt)
                .Take(MaxRows)
                .Select(s => new SaleDto(
                    s.Id, s.AuthorizationId, s.StationCode, s.CardNumber,
                    s.FuelType, s.Liters, s.Amount, s.SoldAt))
                .ToListAsync(cancellationToken);

            return new SalesReportResponse(
                Sales: sales,
                TotalCount: byFuelType.Sum(x => x.Count),
                TotalLiters: byFuelType.Sum(x => x.TotalLiters),
                TotalAmount: byFuelType.Sum(x => x.TotalAmount),
                ByFuelType: byFuelType.OrderBy(x => x.FuelType).ToList());
        }

        /// <summary>
        /// PostgreSQL "timestamp with time zone" kolonları UTC tarih ister.
        /// Türü belirsiz gelen tarih UTC kabul edilir.
        /// </summary>
        private static DateTime ToUtc(DateTime value) => value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
    }
}
