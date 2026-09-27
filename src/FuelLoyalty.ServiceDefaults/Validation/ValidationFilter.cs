using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace FuelLoyalty.ServiceDefaults.Validation
{
    /// <summary>
    /// Endpoint çalışmadan önce isteği doğrular.
    /// Hatalıysa 400 ve alan bazlı hata listesi döner, endpoint hiç çalışmaz.
    /// </summary>
    public sealed class ValidationFilter<TRequest>(IValidator<TRequest> validator) : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var request = context.Arguments.OfType<TRequest>().FirstOrDefault();

            if (request is null)
                return TypedResults.Problem(detail: "İstek gövdesi okunamadı.", statusCode: StatusCodes.Status400BadRequest);

            var validationResult = await validator.ValidateAsync(request, context.HttpContext.RequestAborted);

            if (!validationResult.IsValid)
                return TypedResults.ValidationProblem(validationResult.ToDictionary());

            return await next(context);
        }
    }
}
