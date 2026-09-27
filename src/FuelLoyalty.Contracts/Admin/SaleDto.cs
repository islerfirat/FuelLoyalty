using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelLoyalty.Contracts.Admin
{
    /// <summary>Rapordaki tek satış satırı.</summary>
    public record SaleDto(
        Guid Id,
        Guid AuthorizationId,
        string StationCode,
        string CardNumber,
        FuelType FuelType,
        decimal Liters,
        decimal Amount,
        DateTime SoldAt);
}
