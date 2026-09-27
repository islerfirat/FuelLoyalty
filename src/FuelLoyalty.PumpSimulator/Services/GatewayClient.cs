using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using FuelLoyalty.Contracts;
using FuelLoyalty.Contracts.Admin;
using FuelLoyalty.PumpSimulator.Models;

namespace FuelLoyalty.PumpSimulator.Services
{
    /// <summary>
    /// IGatewayClient'ın HTTP ile çalışan uygulaması.
    /// Gateway adresi her istekte ayarlardan okunur; ekrandan değiştirilince hemen geçerli olur.
    /// </summary>
    public sealed class GatewayClient(HttpClient httpClient, SimulatorSettings settings) : IGatewayClient
    {
        public async Task<AuthorizeResponse> AuthorizeAsync(AuthorizeRequest request, CancellationToken cancellationToken)
        {
            using var response = await httpClient.PostAsJsonAsync(BuildUri("/authorizations"), request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.TooManyRequests)
                return AuthorizeResponse.Reject("Çok fazla istek (429). Biraz bekleyip tekrar deneyin.");

            await EnsureSuccessAsync(response, cancellationToken);

            return await response.Content.ReadFromJsonAsync<AuthorizeResponse>(cancellationToken)
                   ?? AuthorizeResponse.Reject("Gateway'den geçersiz cevap alındı.");
        }

        public async Task<CreateSaleResponse> SendSaleAsync(CreateSaleRequest request, CancellationToken cancellationToken)
        {
            using var response = await httpClient.PostAsJsonAsync(BuildUri("/sales"), request, cancellationToken);

            await EnsureSuccessAsync(response, cancellationToken);

            return await response.Content.ReadFromJsonAsync<CreateSaleResponse>(cancellationToken)
                   ?? throw new GatewayException("Gateway'den geçersiz cevap alındı.", response.StatusCode);
        }

        public async Task<CardDto?> GetCardAsync(string cardNumber, CancellationToken cancellationToken)
        {
            using var response = await httpClient.GetAsync(BuildUri($"/cards/{Uri.EscapeDataString(cardNumber)}"), cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            await EnsureSuccessAsync(response, cancellationToken);

            return await response.Content.ReadFromJsonAsync<CardDto>(cancellationToken);
        }

        private Uri BuildUri(string path) => new(new Uri(settings.GatewayUrl.Trim()), path);

        /// <summary>
        /// Cevap başarısızsa ProblemDetails gövdesini okuyup anlaşılır bir mesajla GatewayException fırlatır.
        /// </summary>
        private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            if (response.IsSuccessStatusCode)
                return;

            ApiProblem? problem = null;
            try
            {
                problem = await response.Content.ReadFromJsonAsync<ApiProblem>(cancellationToken);
            }
            catch (Exception ex) when (ex is JsonException or NotSupportedException)
            {
                // Gövde boş ya da JSON değil: durum koduna göre mesaj üretilecek.
            }

            throw new GatewayException(BuildErrorMessage(response.StatusCode, problem), response.StatusCode);
        }

        private static string BuildErrorMessage(HttpStatusCode statusCode, ApiProblem? problem)
        {
            if (problem?.Errors is { Count: > 0 } errors)
                return string.Join(" ", errors.SelectMany(error => error.Value));

            if (!string.IsNullOrWhiteSpace(problem?.Detail))
                return problem.Detail;

            return statusCode switch
            {
                HttpStatusCode.TooManyRequests => "Çok fazla istek (429). Biraz bekleyip tekrar deneyin.",
                HttpStatusCode.ServiceUnavailable => "Servis şu anda kullanılamıyor (503).",
                _ => $"Gateway hata döndü: {(int)statusCode} {statusCode}"
            };
        }
    }
}
