using FluentValidation;
using FuelLoyalty.Contracts;

namespace FuelLoyalty.Loyalty.Api.Features.AuthorizeCard
{
    public sealed class AuthorizeCardValidator : AbstractValidator<AuthorizeRequest>
    {
        public AuthorizeCardValidator()
        {
            RuleFor(x => x.CardNumber)
                .NotEmpty().WithMessage("Kart numarası zorunlu.")
                .Matches(@"^\d{8,20}$").WithMessage("Kart numarası 8-20 haneli ve sadece rakam olmalı.");

            RuleFor(x => x.FuelType)
                .IsInEnum().WithMessage("Geçersiz yakıt tipi.");

            RuleFor(x => x.StationCode)
                .NotEmpty().WithMessage("İstasyon kodu zorunlu.")
                .MaximumLength(20);

            RuleFor(x => x.PumpNo)
                .InclusiveBetween(1, 99).WithMessage("Pompa numarası 1-99 arasında olmalı.");
        }
    }
}

