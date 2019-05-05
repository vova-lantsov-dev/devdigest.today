using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core;
using Core.Managers;
using Core.Managers.Crosspost;
using Core.ViewModels;
using Core.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace WebSite.Controllers
{
    public class ContentController : Controller
    {
        private readonly FacebookCrosspostManager _facebookCrosspostManager;
        private readonly TelegramCrosspostManager _telegramCrosspostManager;

        public ContentController(
            IMemoryCache cache, 
            FacebookCrosspostManager facebookCrosspostManager, 
            TelegramCrosspostManager telegramCrosspostManager)
        {
            _facebookCrosspostManager = facebookCrosspostManager;
            _telegramCrosspostManager = telegramCrosspostManager;
        }

        [Route("content/partners")]
        public async Task<IActionResult> Partners() => View("~/Views/Content/Partners.cshtml");

        [Route("content/about")]
        public async Task<IActionResult> About() => View("~/Views/Content/About.cshtml");

        [Route("content/developex-tech-club")]
        public async Task<IActionResult> DevelopexTechClub() => View("~/Views/Content/DevelopexTechClub.cshtml");

        [Route("search")]
        public async Task<IActionResult> Search() => View("~/Views/Content/Search.cshtml");

        [Route("content/privacy")]
        public async Task<IActionResult> Privacy() => View("~/Views/Content/Privacy.cshtml");

        [Route("content/how-to-post-vacancy")]
        public async Task<IActionResult> HowToPostVacancy() => View("~/Views/Content/HowToPostVacancy.cshtml");

        [Route("content/microsoft-tech-summit-warsaw")]
        public async Task<IActionResult> MicrosoftTechSummitWarsaw() => View("~/Views/Content/MicrosoftTechSummitWarsaw.cshtml");

        [Route("content/cloud-developers-days")]
        public async Task<IActionResult> CloudDevelopersDaysPoland() => View("~/Views/Content/CloudDevelopersDaysPoland.cshtml");

        [Route("content/build-2018")]
        public async Task<IActionResult> Build2018() => View("~/Views/Content/Build2018.cshtml");

        [Route("content/build-2019")]
        public async Task<IActionResult> Build2019() => View("~/Views/Content/Build2019.cshtml");

        [Route("user-group")]
        [Route("content/net-core-user-group")]        
        public async Task<IActionResult> UkrainianNETCoreUserGroup() => View("~/Views/Content/UkrainianNETCoreUserGroup.cshtml");

        [Route("content/xamarin-user-group")]
        public async Task<IActionResult> XamarinUkraineUserGroup() => View("~/Views/Content/XamarinUkraineUserGroup.cshtml");

        [Route("content/microsoft-azure-user-group")]
        public async Task<IActionResult> MicrosoftAzureUkraineUserGroup() => View("~/Views/Content/MicrosoftAzureUkraineUserGroup.cshtml");
        
        [Route("content/telegram")]
        public async Task<IActionResult> Telegram()=> RedirectPermanent("/content/platform");

        [Route("partners")]
        public async Task<IActionResult> PartnersRedirect() => RedirectPermanent("/content/partners");

        [Route("about")]
        public async Task<IActionResult> AboutRedirect() => RedirectPermanent("/content/about");
        
        [Route("content/platform")]
        public async Task<IActionResult> Platform()
        {
            ViewData["Title"] = Pages.Platform;

            var channels = (await _telegramCrosspostManager.GetChannels())
                .Select(o => new TelegramViewModel()
                {
                    Title = o.Title,
                    Description = o.Description,
                    Logo = o.Logo,
                    Link = $"https://t.me/{o.Name.Replace("@", "")}"
                }).ToList();


            var pages = (await _facebookCrosspostManager.GetPages())
                .Select(o => new FacebookViewModel()
                {
                    Title = o.Name,
                    Description = o.Description,
                    Logo = o.Logo,
                    Link = o.Url,
                }).ToList();

            var result = new List<SocialNetworkViewModel>();
            
            result.AddRange(pages);
            result.AddRange(channels);

            return View("~/Views/Content/Platform.cshtml", result);
        }
    }
}