using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FuelLoyalty.Contracts
{
    /// <summary>
    /// Yakıt tipi. JSON'da ve veritabanında "Benzin", "Motorin", "LPG" metni olarak saklanır,
    /// böylece mevcut veriler değişmeden okunabilir.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<FuelType>))]
    public enum FuelType
    {
        Benzin = 1,
        Motorin = 2,
        LPG = 3
    }
}
