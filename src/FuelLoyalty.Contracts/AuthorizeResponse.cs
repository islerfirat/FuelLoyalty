using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelLoyalty.Contracts
{
    /// <summary>
    /// Onay cevabı. Onaylandıysa en fazla ne kadar yakıt verilebileceğini
    /// kartın limit türüne göre (TL veya Litre) döner.
    /// </summary>
    public record AuthorizeResponse(
        bool Approved,
        Guid? AuthorizationId,
        LimitType? LimitType,
        decimal? MaxValue,
        string? RejectReason)
    {
        public static AuthorizeResponse Approve(Guid authorizationId, LimitType limitType, decimal maxValue)
            => new(true, authorizationId, limitType, maxValue, null);

        public static AuthorizeResponse Reject(string reason)
            => new(false, null, null, null, reason);
    }
}
