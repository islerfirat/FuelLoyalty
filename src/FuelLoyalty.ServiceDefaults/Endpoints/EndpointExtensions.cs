using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FuelLoyalty.ServiceDefaults.Endpoints
{
    public static class EndpointExtensions
    {
        /// <summary>
        /// Verilen assembly içinde IEndpoint uygulayan tüm sınıfları bulup DI'a ekler.
        /// </summary>
        public static IServiceCollection AddEndpoints(this IServiceCollection services, Assembly assembly)
        {
            var endpointTypes = assembly.DefinedTypes
                .Where(type => type is { IsAbstract: false, IsInterface: false }
                               && type.IsAssignableTo(typeof(IEndpoint)));

            foreach (var type in endpointTypes)
            {
                services.TryAddEnumerable(ServiceDescriptor.Transient(typeof(IEndpoint), type));
            }

            return services;
        }

        /// <summary>
        /// DI'a eklenmiş tüm endpoint'lerin adreslerini tanımlar.
        /// </summary>
        public static WebApplication MapEndpoints(this WebApplication app)
        {
            var endpoints = app.Services.GetRequiredService<IEnumerable<IEndpoint>>();

            foreach (var endpoint in endpoints)
            {
                endpoint.MapEndpoint(app);
            }

            return app;
        }
    }
}