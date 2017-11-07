using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebSite.Controllers
{
    public class ContentController : Controller
    {
        [Route("content/partners")]
        public async Task<IActionResult> Partners()
        {
            ViewData["Title"] = Core.Pages.Partners;

            return View("~/Views/Content/Partners.cshtml");
        }

        [Route("content/about")]
        public async Task<IActionResult> About()
        {
            ViewData["Title"] = Core.Pages.AboutUs;

            return View("~/Views/Content/About.cshtml");
        }

        [Route("content/telegram")]
        public async Task<IActionResult> Telegram()
        {
            ViewData["Title"] = Core.Pages.Telegram;

            return View("~/Views/Content/Telegram.cshtml");
        }


        [Route("partners")]
        public async Task<IActionResult> PartnersRedirect() => RedirectPermanent("content/partners");

        [Route("about")]
        public async Task<IActionResult> AboutRedirect() => RedirectPermanent("content/about");

    }
}