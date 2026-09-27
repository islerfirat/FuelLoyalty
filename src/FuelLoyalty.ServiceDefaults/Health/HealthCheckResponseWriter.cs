using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace FuelLoyalty.ServiceDefaults.Health
{
    /// <summary>
    /// Health check sonucunu okunabilir JSON olarak yazar.
    /// Varsayılan çıktı sadece "Healthy" kelimesidir; bu sınıf her kontrolün ayrıntısını gösterir.
    /// </summary>
    public static class HealthCheckResponseWriter
    {
        public static Task WriteAsync(HttpContext context, HealthReport report)
        {
            var response = new
            {
                status = report.Status.ToString(),
                totalDurationMs = report.TotalDuration.TotalMilliseconds,
                checks = report.Entries.Select(entry => new
                {
                    name = entry.Key,
                    status = entry.Value.Status.ToString(),
                    durationMs = entry.Value.Duration.TotalMilliseconds,
                    description = entry.Value.Description,
                    error = entry.Value.Exception?.Message
                })
            };

            return context.Response.WriteAsJsonAsync(response);
        }
    }
}
