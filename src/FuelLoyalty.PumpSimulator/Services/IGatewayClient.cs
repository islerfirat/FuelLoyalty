using FuelLoyalty.Contracts;
using FuelLoyalty.Contracts.Admin;

namespace FuelLoyalty.PumpSimulator.Services
{
    /// <summary>
    /// Pompanın dış dünyayla tek bağlantısı: API Gateway.
    /// ViewModel bu arayüzü bilir; HTTP detayını bilmez.
    /// </summary>
    public interface IGatewayClient
    {
        /// <summary>Kart okutuldu: onay ister. İstek sınırı aşılırsa ret döner.</summary>
        Task<AuthorizeResponse> AuthorizeAsync(AuthorizeRequest request, CancellationToken cancellationToken);

        /// <summary>Satış bitti: satışı gönderir. Başarısız olursa GatewayException fırlatır.</summary>
        Task<CreateSaleResponse> SendSaleAsync(CreateSaleRequest request, CancellationToken cancellationToken);

        /// <summary>Kart durumunu getirir. Kart yoksa null döner.</summary>
        Task<CardDto?> GetCardAsync(string cardNumber, CancellationToken cancellationToken);
    }
}
