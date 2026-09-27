using FluentValidation;
using FuelLoyalty.Contracts.Admin;

namespace FuelLoyalty.Loyalty.Api.Features.Admin.CreateCard
{
    public sealed class CreateCardValidator : AbstractValidator<CreateCardRequest>
    {
        public CreateCardValidator()
        {
            RuleFor(x => x.CardNumber)
                .NotEmpty().WithMessage("Kart numarası zorunlu.")
                .Matches(@"^\d{8,20}$").WithMessage("Kart numarası 8-20 haneli ve sadece rakam olmalı.");

            RuleFor(x => x.HolderName)
                .NotEmpty().WithMessage("Kart sahibi / plaka zorunlu.")
                .MaximumLength(100);

            RuleFor(x => x.AllowedFuelType)
                .IsInEnum().WithMessage("Geçersiz yakıt tipi.");

            RuleFor(x => x.LimitType)
                .IsInEnum().WithMessage("Geçersiz limit türü.");

            RuleFor(x => x.Balance)
                .GreaterThanOrEqualTo(0).WithMessage("Bakiye sıfırdan küçük olamaz.");
        }
    }
}
