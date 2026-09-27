using FuelLoyalty.Contracts.Admin;
using FuelLoyalty.SharedKernel;

namespace FuelLoyalty.Portal.Services.Cards
{
    /// <summary>
    /// Kart yönetimi işlemleri. Sayfalar bu arayüzü bilir, HTTP detayını bilmez.
    /// </summary>
    public interface ICardApiClient
    {
        Task<Result<IReadOnlyList<CardDto>>> GetCardsAsync(string? search, CancellationToken cancellationToken);

        Task<Result<CardDto>> GetCardAsync(string cardNumber, CancellationToken cancellationToken);

        Task<Result<CardDto>> CreateCardAsync(CreateCardRequest request, CancellationToken cancellationToken);

        Task<Result<CardDto>> UpdateLimitAsync(string cardNumber, UpdateCardLimitRequest request, CancellationToken cancellationToken);

        Task<Result<CardDto>> UpdateStatusAsync(string cardNumber, UpdateCardStatusRequest request, CancellationToken cancellationToken);
    }
}
