using FuelLoyalty.Contracts.Admin;
using FuelLoyalty.Portal.Services.Http;
using FuelLoyalty.SharedKernel;
using Microsoft.AspNetCore.WebUtilities;
using System.Globalization;

namespace FuelLoyalty.Portal.Services.Sales
{
    /// <summary>
    /// ISalesApiClient'ın gateway üzerinden HTTP ile çalışan uygulaması.
    /// </summary>
    public sealed class SalesApiClient(HttpClient httpClient, ILogger<SalesApiClient> logger) : ISalesApiClient
    {
        private const string AdminSalesPath = "/admin/sales";

        public Task<Result<SalesReportResponse>> GetReportAsync(SalesReportFilter filter, CancellationToken cancellationToken)
        {
            var uri = QueryHelpers.AddQueryString(AdminSalesPath, BuildQuery(filter));

            return ApiRequest.ExecuteAsync<SalesReportResponse>(
                token => httpClient.GetAsync(uri, token), logger, cancellationToken);
        }

        /// <summary>
        /// Sadece dolu filtreleri sorgu parametresi olarak ekler.
        /// Tarihler ISO 8601 biçiminde gönderilir (örnek: 2026-09-01T21:00:00.0000000Z).
        /// </summary>
        private static Dictionary<string, string?> BuildQuery(SalesReportFilter filter)
        {
            var query = new Dictionary<string, string?>();

            if (filter.FromUtc.HasValue)
                query["from"] = filter.FromUtc.Value.ToString("O", CultureInfo.InvariantCulture);

            if (filter.ToUtc.HasValue)
                query["to"] = filter.ToUtc.Value.ToString("O", CultureInfo.InvariantCulture);

            if (!string.IsNullOrWhiteSpace(filter.StationCode))
                query["stationCode"] = filter.StationCode.Trim();

            if (!string.IsNullOrWhiteSpace(filter.CardNumber))
                query["cardNumber"] = filter.CardNumber.Trim();

            if (filter.FuelType.HasValue)
                query["fuelType"] = filter.FuelType.Value.ToString();

            return query;
        }
    }
}
