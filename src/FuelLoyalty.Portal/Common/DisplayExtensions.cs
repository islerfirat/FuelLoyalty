using FuelLoyalty.Contracts;

namespace FuelLoyalty.Portal.Common
{
    /// <summary>
    /// Enum değerlerinin ekranda gösterilecek karşılıkları.
    /// </summary>
    public static class DisplayExtensions
    {
        /// <summary>Tutarın yanına yazılacak birim: "TL" ya da "L".</summary>
        public static string ToUnit(this LimitType limitType)
            => limitType == LimitType.Liters ? "L" : "TL";

        /// <summary>Limit türünün açıklaması.</summary>
        public static string ToDisplayName(this LimitType limitType)
            => limitType == LimitType.Liters ? "Litre" : "Tutar (TL)";

        /// <summary>Yakıt tipinin raporlarda kullanılan renk sınıfı.</summary>
        public static string ToCssClass(this FuelType fuelType) => fuelType switch
        {
            FuelType.Benzin => "fuel-benzin",
            FuelType.Motorin => "fuel-motorin",
            FuelType.LPG => "fuel-lpg",
            _ => "fuel-other"
        };
    }
}
