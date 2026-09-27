using System.Globalization;
using FuelLoyalty.Portal;
using Microsoft.AspNetCore.Localization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    // Formdaki deðer okunamazsa gösterilecek Türkçe mesajlar
    var messages = options.ModelBindingMessageProvider;
    messages.SetAttemptedValueIsInvalidAccessor((value, field) => $"'{value}' deðeri {field} için geçerli deðil.");
    messages.SetValueMustBeANumberAccessor(field => $"{field} alaný sayý olmalý.");
    messages.SetValueMustNotBeNullAccessor(field => $"{field} alaný zorunlu.");
    messages.SetValueIsInvalidAccessor(value => $"'{value}' deðeri geçerli deðil.");
    messages.SetMissingBindRequiredValueAccessor(field => $"{field} alaný zorunlu.");
});

builder.Services.AddPortalServices(builder.Configuration);

var app = builder.Build();

// Sayý ve tarih biçimleri Türkçe olsun (1.234,56 / 26.09.2026).
// Formlardan gelen "47,50" gibi deðerler de Türkçe kurallarla okunur.
var turkish = new CultureInfo("tr-TR");
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(turkish),
    SupportedCultures = [turkish],
    SupportedUICultures = [turkish]
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();
app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();