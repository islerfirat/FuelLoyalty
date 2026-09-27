using FuelLoyalty.Contracts;
using FuelLoyalty.Contracts.Admin;
using System.ComponentModel.DataAnnotations;

namespace FuelLoyalty.Portal.Models.Cards
{
    /// <summary>
    /// Yeni kart formunun verisi ve ekran tarafı doğrulama kuralları.
    /// </summary>
    public sealed class CreateCardFormModel
    {
        [Display(Name = "Kart numarası")]
        [Required(ErrorMessage = "{0} zorunlu.")]
        [RegularExpression(@"^\d{8,20}$", ErrorMessage = "Kart numarası 8-20 haneli ve sadece rakam olmalı.")]
        public string CardNumber { get; set; } = string.Empty;

        [Display(Name = "Kart sahibi / plaka")]
        [Required(ErrorMessage = "{0} zorunlu.")]
        [MaxLength(100, ErrorMessage = "{0} en fazla {1} karakter olabilir.")]
        public string HolderName { get; set; } = string.Empty;

        [Display(Name = "İzinli yakıt")]
        [Required(ErrorMessage = "{0} seçilmeli.")]
        public FuelType? AllowedFuelType { get; set; }

        [Display(Name = "Limit türü")]
        [Required(ErrorMessage = "{0} seçilmeli.")]
        public LimitType? LimitType { get; set; }

        [Display(Name = "Başlangıç bakiyesi")]
        [Required(ErrorMessage = "{0} zorunlu.")]
        [Range(typeof(decimal), "0", "99999999", ErrorMessage = "{0} 0 ile 99.999.999 arasında olmalı.")]
        public decimal? Balance { get; set; }

        public CreateCardRequest ToRequest() => new(
            CardNumber.Trim(),
            HolderName.Trim(),
            AllowedFuelType!.Value,
            LimitType!.Value,
            Balance!.Value);
    }
}
