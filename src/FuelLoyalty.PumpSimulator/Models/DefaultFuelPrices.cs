using FuelLoyalty.Contracts;

namespace FuelLoyalty.PumpSimulator.Models
{
    /// <summary>
    /// Simülasyon için örnek birim fiyatlar (TL/L). Ekrandan değiştirilebilir.
    /// </summary>
    public static class DefaultFuelPrices
    {
        public static decimal For(FuelType fuelType) => fuelType switch
        {
            FuelType.Benzin => 47.50m,
            FuelType.Motorin => 48.90m,
            FuelType.LPG => 26.40m,
            _ => 0m
        };
    }
}
