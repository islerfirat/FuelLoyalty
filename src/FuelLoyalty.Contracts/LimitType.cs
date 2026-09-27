using System.Text.Json.Serialization;

namespace FuelLoyalty.Contracts
{
    /// <summary>
    /// Kartın limitinin hangi birimde tutulduğu.
    /// JSON'da sayı yerine "Amount" / "Liters" olarak yazılır ve okunur.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<LimitType>))]
    public enum LimitType
    {
        Amount = 1,   // TL
        Liters = 2    // Litre
    }
}
