using FuelLoyalty.Contracts;
using FuelLoyalty.Contracts.Admin;
using FuelLoyalty.Portal.Common;
using FuelLoyalty.Portal.Models.Cards;
using FuelLoyalty.Portal.Services.Cards;
using FuelLoyalty.SharedKernel;
using Microsoft.AspNetCore.Mvc;

namespace FuelLoyalty.Portal.Controllers
{

    /// <summary>
    /// Kart yönetimi sayfaları. İş mantığı içermez: formu doğrular, istemciyi çağırır,
    /// sonucu view'a ya da yönlendirmeye çevirir.
    /// </summary>
    public sealed class CardsController(ICardApiClient cardApi) : Controller
    {
        // ================= Liste =================

        [HttpGet]
        public async Task<IActionResult> Index(string? search, CancellationToken cancellationToken)
        {
            var result = await cardApi.GetCardsAsync(search, cancellationToken);

            var model = new CardListViewModel
            {
                Search = search,
                Cards = result.IsSuccess ? result.Value : [],
                Error = result.IsFailure ? result.Error : null
            };

            return View(model);
        }

        // ================= Yeni kart =================

        [HttpGet]
        public IActionResult Create()
            => View(new CreateCardFormModel
            {
                AllowedFuelType = FuelType.Benzin,
                LimitType = LimitType.Amount
            });

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCardFormModel form, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return View(form);

            var result = await cardApi.CreateCardAsync(form.ToRequest(), cancellationToken);

            if (result.IsFailure)
            {
                // Yeni kartta tek çakışma durumu, kart numarasının zaten kayıtlı olmasıdır.
                var field = result.Error.Type == ErrorType.Conflict ? nameof(CreateCardFormModel.CardNumber) : string.Empty;
                ModelState.AddModelError(field, result.Error.Message);
                return View(form);
            }

            TempData[TempDataKeys.SuccessMessage] = $"{result.Value.CardNumber} numaralı kart eklendi.";
            return RedirectToAction(nameof(Details), new { id = result.Value.CardNumber });
        }

        // ================= Detay =================

        [HttpGet]
        public async Task<IActionResult> Details(string id, CancellationToken cancellationToken)
            => View(await BuildDetailsAsync(id, limitForm: null, cancellationToken));

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateLimit(string id, UpdateLimitFormModel limitForm, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                var result = await cardApi.UpdateLimitAsync(id, limitForm.ToRequest(), cancellationToken);

                if (result.IsSuccess)
                {
                    TempData[TempDataKeys.SuccessMessage] = "Kart limiti güncellendi.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                ModelState.AddModelError(string.Empty, result.Error.Message);
            }

            // Hata varsa formu, kullanıcının girdiği değerlerle birlikte tekrar göster.
            var model = await BuildDetailsAsync(id, limitForm, cancellationToken);
            return View(nameof(Details), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(string id, bool isActive, CancellationToken cancellationToken)
        {
            var result = await cardApi.UpdateStatusAsync(id, new UpdateCardStatusRequest(isActive), cancellationToken);

            if (result.IsSuccess)
                TempData[TempDataKeys.SuccessMessage] = isActive ? "Kart aktif edildi." : "Kart pasif edildi.";
            else
                TempData[TempDataKeys.ErrorMessage] = result.Error.Message;

            return RedirectToAction(nameof(Details), new { id });
        }

        // ================= Yardımcı =================

        private async Task<CardDetailsViewModel> BuildDetailsAsync(
            string cardNumber,
            UpdateLimitFormModel? limitForm,
            CancellationToken cancellationToken)
        {
            var result = await cardApi.GetCardAsync(cardNumber, cancellationToken);

            if (result.IsFailure)
                return new CardDetailsViewModel { CardNumber = cardNumber, Error = result.Error };

            return new CardDetailsViewModel
            {
                CardNumber = cardNumber,
                Card = result.Value,
                LimitForm = limitForm ?? UpdateLimitFormModel.From(result.Value)
            };
        }
    }
}
