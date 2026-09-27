using FuelLoyalty.Contracts;
using FuelLoyalty.Contracts.Admin;
using System.ComponentModel.DataAnnotations;

namespace FuelLoyalty.Portal.Models.Cards
{
    /// <summary>
    /// Kart detayındaki "Limit Belirle" formunun verisi.
    /// </summary>
    public sealed class UpdateLimitFormModel
    {
        [Display(Name = "Limit türü")]
        [Required(ErrorMessage = "{0} seçilmeli.")]
        public LimitType? LimitType { get; set; }

        [Display(Name = "Yeni bakiye")]
        [Required(ErrorMessage = "{0} zorunlu.")]
        [Range(typeof(decimal), "0", "99999999", ErrorMessage = "{0} 0 ile 99.999.999 arasında olmalı.")]
        public decimal? Balance { get; set; }

        /// <summary>Formu kartın mevcut değerleriyle doldurur.</summary>
        public static UpdateLimitFormModel From(CardDto card) => new()
        {
            LimitType = card.LimitType,
            Balance = card.Balance
        };

        public UpdateCardLimitRequest ToRequest() => new(LimitType!.Value, Balance!.Value);
    }
}
