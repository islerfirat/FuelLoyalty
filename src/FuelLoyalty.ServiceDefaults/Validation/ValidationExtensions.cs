using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace FuelLoyalty.ServiceDefaults.Validation
{
    public static class ValidationExtensions
    {
        /// <summary>
        /// Endpoint'e doğrulama filtresi ekler: <c>.WithRequestValidation&lt;AuthorizeRequest&gt;()</c>
        /// </summary>
        public static RouteHandlerBuilder WithRequestValidation<TRequest>(this RouteHandlerBuilder builder)
            => builder
                .AddEndpointFilter<ValidationFilter<TRequest>>()
                .ProducesValidationProblem();
    }
}
