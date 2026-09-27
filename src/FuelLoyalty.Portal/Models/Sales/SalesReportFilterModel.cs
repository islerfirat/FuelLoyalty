using System.ComponentModel.DataAnnotations;
using FuelLoyalty.Contracts;
using FuelLoyalty.Portal.Common;
using FuelLoyalty.Portal.Services.Sales;

namespace FuelLoyalty.Portal.Models.Sales
{
    /// <summary>
    /// Satış raporu filtre formu. Tarihler Türkiye saatine göre gün olarak girilir.
    /// </summary>
    public sealed class SalesReportFilterModel : IValidatableObject
    {
        [Display(Name = "Başlangıç")]
        public DateOnly? From { get; set; }

        [Display(Name = "Bitiş")]
        public DateOnly? To { get; set; }

        [Display(Name = "İstasyon")]
        [MaxLength(20, ErrorMessage = "{0} en fazla {1} karakter olabilir.")]
        public string? StationCode { get; set; }

        [Display(Name = "Kart numarası")]
        [MaxLength(20, ErrorMessage = "{0} en fazla {1} karakter olabilir.")]
        public string? CardNumber { get; set; }

        [Display(Name = "Yakıt")]
        public FuelType? FuelType { get; set; }

        public static SalesReportFilterModel ForRange(DateOnly from, DateOnly to) => new() { From = from, To = to };

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (From.HasValue && To.HasValue && From > To)
                yield return new ValidationResult("Başlangıç tarihi bitiş tarihinden sonra olamaz.", [nameof(To)]);
        }

        /// <summary>
        /// Servise gidecek filtreye çevirir: günler Türkiye saatine göre UTC aralığına dönüşür.
        /// Bitiş günü dahil edilir (bitiş gününün ertesi günü 00:00'a kadar).
        /// </summary>
        public SalesReportFilter ToApiFilter() => new(
            FromUtc: From is { } from ? BusinessTime.StartOfDayUtc(from) : null,
            ToUtc: To is { } to ? BusinessTime.StartOfDayUtc(to.AddDays(1)) : null,
            StationCode: StationCode,
            CardNumber: CardNumber,
            FuelType: FuelType);
    }
}
