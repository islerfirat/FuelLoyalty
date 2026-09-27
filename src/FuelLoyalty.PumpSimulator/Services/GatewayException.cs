using System.Net;

namespace FuelLoyalty.PumpSimulator.Services
{
    /// <summary>
    /// Gateway'in başarısız bir HTTP cevabı döndüğünü belirtir.
    /// Mesaj, kullanıcıya gösterilebilecek şekilde hazırlanmıştır.
    /// </summary>
    public sealed class GatewayException(string message, HttpStatusCode statusCode) : Exception(message)
    {
        public HttpStatusCode StatusCode { get; } = statusCode;
    }
}
