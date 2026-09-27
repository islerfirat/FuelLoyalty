using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelLoyalty.Contracts.Admin
{
    /// <summary>Kartı aktif/pasif yapma isteği.</summary>
    public record UpdateCardStatusRequest(bool IsActive);
}
