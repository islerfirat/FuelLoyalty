using FuelLoyalty.SharedKernel;

namespace FuelLoyalty.Portal.Services.Http
{
    /// <summary>
    /// Gateway'e yapılan her isteğin ortak akışı: gönder, cevabı Result'a çevir,
    /// bağlantı ve zaman aşımı hatalarını yakalayıp Result.Failure olarak dön.
    /// Böylece sayfalar hiçbir zaman try/catch yazmak zorunda kalmaz.
    /// </summary>
    public static class ApiRequest
    {
        public static async Task<Result<T>> ExecuteAsync<T>(
            Func<CancellationToken, Task<HttpResponseMessage>> send,
            ILogger logger,
            CancellationToken cancellationToken)
        {
            try
            {
                using var response = await send(cancellationToken);
                return await response.ToResultAsync<T>(cancellationToken);
            }
            catch (HttpRequestException ex)
            {
                logger.LogError(ex, "Gateway'e ulaşılamadı.");
                return Result.Failure<T>(ApiErrors.Unreachable);
            }
            catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
            {
                // İptal isteyen biz değilsek, HttpClient'ın zaman aşımı dolmuştur.
                logger.LogError(ex, "Gateway isteği zaman aşımına uğradı.");
                return Result.Failure<T>(ApiErrors.Timeout);
            }
        }
    }
}
