using FuelLoyalty.SharedKernel;
using System.Net;
using System.Text.Json;

namespace FuelLoyalty.Portal.Services.Http
{
    /// <summary>
    /// HTTP cevabını Result'a çevirir: başarılıysa gövdeyi okur,
    /// değilse ProblemDetails'i okuyup türü belli bir Error üretir.
    /// </summary>
    public static class HttpResponseMessageExtensions
    {
        public static async Task<Result<T>> ToResultAsync<T>(this HttpResponseMessage response, CancellationToken cancellationToken)
        {
            if (response.IsSuccessStatusCode)
            {
                var value = await response.Content.ReadFromJsonAsync<T>(cancellationToken);

                return value is null
                    ? Result.Failure<T>(ApiErrors.EmptyResponse)
                    : Result.Success(value);
            }

            var problem = await TryReadProblemAsync(response, cancellationToken);
            return Result.Failure<T>(ToError(response.StatusCode, problem));
        }

        private static async Task<ApiProblem?> TryReadProblemAsync(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            try
            {
                return await response.Content.ReadFromJsonAsync<ApiProblem>(cancellationToken);
            }
            catch (Exception ex) when (ex is JsonException or NotSupportedException)
            {
                // Gövde boş ya da JSON değil (örneğin gateway'in rate limit cevabı).
                return null;
            }
        }

        private static Error ToError(HttpStatusCode statusCode, ApiProblem? problem)
        {
            var code = string.IsNullOrWhiteSpace(problem?.Title) || problem.Errors is { Count: > 0 }
                ? $"Http.{(int)statusCode}"
                : problem.Title;

            var message = BuildMessage(statusCode, problem);

            return statusCode switch
            {
                HttpStatusCode.BadRequest => Error.Validation(code, message),
                HttpStatusCode.NotFound => Error.NotFound(code, message),
                HttpStatusCode.Conflict => Error.Conflict(code, message),
                _ => Error.Failure(code, message)
            };
        }

        private static string BuildMessage(HttpStatusCode statusCode, ApiProblem? problem)
        {
            if (problem?.Errors is { Count: > 0 } errors)
                return string.Join(" ", errors.SelectMany(error => error.Value));

            if (!string.IsNullOrWhiteSpace(problem?.Detail))
                return problem.Detail;

            return statusCode switch
            {
                HttpStatusCode.NotFound => "Kayıt bulunamadı.",
                HttpStatusCode.TooManyRequests => "Çok fazla istek gönderildi. Biraz bekleyip tekrar deneyin.",
                HttpStatusCode.ServiceUnavailable => "Servis şu anda kullanılamıyor.",
                _ => $"Sunucu hata döndü: {(int)statusCode}"
            };
        }
    }
}
