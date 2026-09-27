using FuelLoyalty.Contracts.Admin;
using FuelLoyalty.Portal.Services.Http;
using FuelLoyalty.SharedKernel;
using Microsoft.AspNetCore.WebUtilities;

namespace FuelLoyalty.Portal.Services.Cards
{
    /// <summary>
    /// ICardApiClient'ın gateway üzerinden HTTP ile çalışan uygulaması.
    /// </summary>
    public sealed class CardApiClient(HttpClient httpClient, ILogger<CardApiClient> logger) : ICardApiClient
    {
        private const string AdminCardsPath = "/admin/cards";

        public Task<Result<IReadOnlyList<CardDto>>> GetCardsAsync(string? search, CancellationToken cancellationToken)
        {
            var uri = string.IsNullOrWhiteSpace(search)
                ? AdminCardsPath
                : QueryHelpers.AddQueryString(AdminCardsPath, "search", search.Trim());

            return ApiRequest.ExecuteAsync<IReadOnlyList<CardDto>>(
                token => httpClient.GetAsync(uri, token), logger, cancellationToken);
        }

        public Task<Result<CardDto>> GetCardAsync(string cardNumber, CancellationToken cancellationToken)
            => ApiRequest.ExecuteAsync<CardDto>(
                token => httpClient.GetAsync($"/cards/{Uri.EscapeDataString(cardNumber)}", token), logger, cancellationToken);

        public Task<Result<CardDto>> CreateCardAsync(CreateCardRequest request, CancellationToken cancellationToken)
            => ApiRequest.ExecuteAsync<CardDto>(
                token => httpClient.PostAsJsonAsync(AdminCardsPath, request, token), logger, cancellationToken);

        public Task<Result<CardDto>> UpdateLimitAsync(string cardNumber, UpdateCardLimitRequest request, CancellationToken cancellationToken)
            => ApiRequest.ExecuteAsync<CardDto>(
                token => httpClient.PutAsJsonAsync($"{AdminCardsPath}/{Uri.EscapeDataString(cardNumber)}/limit", request, token),
                logger, cancellationToken);

        public Task<Result<CardDto>> UpdateStatusAsync(string cardNumber, UpdateCardStatusRequest request, CancellationToken cancellationToken)
            => ApiRequest.ExecuteAsync<CardDto>(
                token => httpClient.PutAsJsonAsync($"{AdminCardsPath}/{Uri.EscapeDataString(cardNumber)}/status", request, token),
                logger, cancellationToken);
    }
}
