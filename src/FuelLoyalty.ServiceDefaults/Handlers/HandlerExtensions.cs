using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace FuelLoyalty.ServiceDefaults.Handlers
{
    public static class HandlerExtensions
    {
        /// <summary>
        /// Verilen assembly içinde IHandler uygulayan tüm sınıfları Scoped olarak DI'a ekler.
        /// </summary>
        public static IServiceCollection AddHandlers(this IServiceCollection services, Assembly assembly)
        {
            var handlerTypes = assembly.DefinedTypes
                .Where(type => type is { IsAbstract: false, IsInterface: false }
                               && type.IsAssignableTo(typeof(IHandler)));

            foreach (var type in handlerTypes)
            {
                services.TryAddScoped(type);
            }

            return services;
        }
    }
}
