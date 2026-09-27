using FluentValidation;
using FuelLoyalty.Contracts;

namespace FuelLoyalty.Sales.Api.Features.CreateSale
{
    public sealed class CreateSaleValidator : AbstractValidator<CreateSaleRequest>
    {
        public CreateSaleValidator()
        {
            RuleFor(x => x.AuthorizationId)
                .NotEmpty().WithMessage("Provizyon numarası zorunlu.");

            RuleFor(x => x.StationCode)
                .NotEmpty().WithMessage("İstasyon kodu zorunlu.")
                .MaximumLength(20);

            RuleFor(x => x.CardNumber)
                .NotEmpty().WithMessage("Kart numarası zorunlu.")
                .Matches(@"^\d{8,20}$").WithMessage("Kart numarası 8-20 haneli ve sadece rakam olmalı.");

            RuleFor(x => x.FuelType)
                .IsInEnum().WithMessage("Geçersiz yakıt tipi.");

            RuleFor(x => x.Liters)
                .GreaterThan(0).WithMessage("Litre sıfırdan büyük olmalı.");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Tutar sıfırdan büyük olmalı.");
        }
    }
}
