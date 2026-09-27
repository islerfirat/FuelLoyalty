using FuelLoyalty.Contracts;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FuelLoyalty.Portal.Common
{
    /// <summary>
    /// Formlardaki açılır listelerin (select) seçenekleri.
    /// </summary>
    public static class SelectListFactory
    {
        public static IEnumerable<SelectListItem> FuelTypes()
            => Enum.GetValues<FuelType>()
                .Select(fuelType => new SelectListItem(fuelType.ToString(), fuelType.ToString()));

        public static IEnumerable<SelectListItem> LimitTypes()
            => Enum.GetValues<LimitType>()
                .Select(limitType => new SelectListItem(limitType.ToDisplayName(), limitType.ToString()));
    }
}
