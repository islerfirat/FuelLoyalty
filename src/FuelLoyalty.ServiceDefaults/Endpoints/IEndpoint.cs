using Microsoft.AspNetCore.Routing;

namespace FuelLoyalty.ServiceDefaults.Endpoints
{
    /// <summary>
    /// Her özelliğin endpoint sınıfı bu arayüzü uygular.
    /// Uygulama açılırken otomatik bulunur ve kaydedilir.
    /// </summary>
    public interface IEndpoint
    {
        void MapEndpoint(IEndpointRouteBuilder app);
    }
}
