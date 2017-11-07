using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core;
using Core.Managers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace WebSite.Controllers
{
    public class ContentController : Controller
    {
        private readonly CrossPostManager _crossPostManager;

        public ContentController(IMemoryCache cache)
        {
            _crossPostManager = new CrossPostManager(Settings.Current.ConnectionString);
        }

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

            var channels = _crossPostManager
                .GetTelegramChannels()
                .Select(o => new Core.ViewModels.TelegramViewModel(o.Name)
                {
                    Title = o.Title,
                    Description = o.Description,                    
                    Logo = o.Logo
                }).ToList();


            return View("~/Views/Content/Telegram.cshtml", channels);
        }


        [Route("partners")]
        public async Task<IActionResult> PartnersRedirect() => RedirectPermanent("content/partners");

        [Route("about")]
        public async Task<IActionResult> AboutRedirect() => RedirectPermanent("content/about");

    }
}