using FuelLoyalty.SharedKernel;
using Microsoft.AspNetCore.Http;

namespace FuelLoyalty.ServiceDefaults.ErrorHandling
{
    public static class ErrorExtensions
    {
        /// <summary>
        /// Bir iş hatasını, türüne uygun HTTP koduyla ProblemDetails cevabına çevirir.
        /// </summary>
        public static IResult ToProblem(this Error error)
        {
            var statusCode = error.Type switch
            {
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError
            };

            return TypedResults.Problem(
                title: error.Code,
                detail: error.Message,
                statusCode: statusCode);
        }
    }
}
