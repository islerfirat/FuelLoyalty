namespace FuelLoyalty.PumpSimulator.Models
{
    /// <summary>
    /// Gateway'in hata durumunda döndüğü ProblemDetails cevabı.
    /// Doğrulama hatalarında Errors alanı doludur.
    /// </summary>
    public sealed record ApiProblem(
        string? Title,
        string? Detail,
        int? Status,
        Dictionary<string, string[]>? Errors);
}
