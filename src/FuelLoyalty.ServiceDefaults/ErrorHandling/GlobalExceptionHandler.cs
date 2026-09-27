using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FuelLoyalty.ServiceDefaults.ErrorHandling
{
    /// <summary>
    /// Hiçbir yerde yakalanmayan beklenmeyen hataları yakalar:
    /// loglar ve istemciye iç detay sızdırmadan standart 500 cevabı döner.
    /// </summary>
    public sealed class GlobalExceptionHandler(
        IProblemDetailsService problemDetailsService,
        ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            logger.LogError(exception, "Beklenmeyen hata. Yol: {Path}", httpContext.Request.Path);

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Server.Error",
                    Detail = "Beklenmeyen bir hata oluştu. Lütfen daha sonra tekrar deneyin."
                }
            });
        }
    }
}
