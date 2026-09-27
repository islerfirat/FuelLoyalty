using FuelLoyalty.Portal.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FuelLoyalty.Portal.Controllers
{
    public sealed class HomeController : Controller
    {
        public IActionResult Index() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
            => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
