using FluentValidation;
using FuelLoyalty.Contracts.Admin;
using FuelLoyalty.Loyalty.Api.Features.Admin.UpdateCardLimit;

namespace FuelLoyalty.Loyalty.Api.Features.Admin.UpdateCardLimit
{
    public sealed class UpdateCardLimitValidator : AbstractValidator<UpdateCardLimitRequest>
    {
        public UpdateCardLimitValidator()
        {
            RuleFor(x => x.LimitType)
                .IsInEnum().WithMessage("Geçersiz limit türü.");

            RuleFor(x => x.Balance)
                .GreaterThanOrEqualTo(0).WithMessage("Bakiye sıfırdan küçük olamaz.");
        }
    }
}