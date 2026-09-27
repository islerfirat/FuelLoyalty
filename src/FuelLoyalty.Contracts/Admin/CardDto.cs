using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelLoyalty.Contracts.Admin
{
    /// <summary>Kart bilgileri (liste ve detay ekranlarında).</summary>
    public record CardDto(
        Guid Id,
        string CardNumber,
        string HolderName,
        FuelType AllowedFuelType,
        LimitType LimitType,
        decimal Balance,
        decimal ReservedBalance,
        decimal AvailableBalance,
        bool IsActive);
}
