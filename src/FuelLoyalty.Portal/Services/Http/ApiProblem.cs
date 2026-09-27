namespace FuelLoyalty.Portal.Services.Http
{
    /// <summary>
    /// Servislerin hata durumunda döndüğü ProblemDetails cevabı.
    /// Doğrulama hatalarında Errors alanı doludur.
    /// </summary>
    public sealed record ApiProblem(
        string? Title,
        string? Detail,
        int? Status,
        Dictionary<string, string[]>? Errors);
}
