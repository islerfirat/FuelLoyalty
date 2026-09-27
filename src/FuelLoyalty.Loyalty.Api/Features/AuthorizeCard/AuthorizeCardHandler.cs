using FuelLoyalty.Contracts;
using FuelLoyalty.Loyalty.Api.Common.Options;
using FuelLoyalty.Loyalty.Api.Domain.Cards;
using FuelLoyalty.Loyalty.Api.Infrastructure.Persistence;
using FuelLoyalty.ServiceDefaults.Handlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;


namespace FuelLoyalty.Loyalty.Api.Features.AuthorizeCard
{
    /// <summary>
    /// Kart okutulduğunda: kartı bulur, kartın kendisine onay ister, sonucu döner.
    /// Ret bir hata değil, normal bir iş sonucudur; bu yüzden her durumda AuthorizeResponse döner.
    /// </summary>
    public sealed class AuthorizeCardHandler(
        LoyaltyDbContext db,
        TimeProvider timeProvider,
        IOptions<AuthorizationOptions> options,
        ILogger<AuthorizeCardHandler> logger) : IHandler
    {
        public async Task<AuthorizeResponse> HandleAsync(AuthorizeRequest request, CancellationToken cancellationToken)
        {
            var card = await db.Cards.FirstOrDefaultAsync(c => c.CardNumber == request.CardNumber, cancellationToken);
            if (card is null)
                return Reject(request, CardErrors.NotFound(request.CardNumber).Message);

            var now = timeProvider.GetUtcNow().UtcDateTime;
            var validity = TimeSpan.FromMinutes(options.Value.ExpiryMinutes);

            var result = card.Authorize(request.FuelType, request.StationCode, request.PumpNo, now, validity);
            if (result.IsFailure)
                return Reject(request, result.Error.Message);

            var authorization = result.Value;
            db.Authorizations.Add(authorization);

            try
            {
                await db.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                // Aynı kart aynı anda başka pompada okutuldu ve o işlem önce kaydedildi.
                return Reject(request, CardErrors.ConcurrencyConflict.Message);
            }

            logger.LogInformation(
                "Onay verildi. Kart: {CardNumber}, Pompa: {PumpNo}, Provizyon: {AuthorizationId}, En fazla: {MaxValue} {LimitType}",
                card.CardNumber, request.PumpNo, authorization.Id, authorization.ReservedValue, authorization.LimitType);

            return AuthorizeResponse.Approve(authorization.Id, authorization.LimitType, authorization.ReservedValue);
        }

        private AuthorizeResponse Reject(AuthorizeRequest request, string reason)
        {
            logger.LogInformation("Onay reddedildi. Kart: {CardNumber}, Neden: {Reason}", request.CardNumber, reason);
            return AuthorizeResponse.Reject(reason);
        }
    }
}
