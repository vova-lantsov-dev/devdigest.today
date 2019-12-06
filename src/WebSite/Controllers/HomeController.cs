using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Core;
using Core.Services;
using Core.Services.Crosspost;
using Core.ViewModels;
using Core.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using X.PagedList;

namespace WebSite.Controllers
{
    public class HomeController : Controller
    {
        private readonly FacebookCrosspostService _facebookCrosspostManager;
        private readonly TwitterCrosspostService _twitterCrosspostManager;
        private readonly TelegramCrosspostService _telegramCrosspostManager;
        private readonly IPublicationService _publicationManager;
        private readonly IVacancyService _vacancyManager;
        private readonly IWebHostEnvironment _env;
        private readonly IMemoryCache _cache;
        private readonly Settings _settings;

        public HomeController(
            IMemoryCache cache, 
            IWebHostEnvironment env, 
            IVacancyService vacancyManager, 
            IPublicationService publicationManager, 
            Settings settings, 
            TelegramCrosspostService telegramCrosspostManager, 
            FacebookCrosspostService facebookCrosspostManager,
            TwitterCrosspostService twitterCrosspostManager)
        {
            _cache = cache;
            _env = env;
            _vacancyManager = vacancyManager;
            _publicationManager = publicationManager;
            _settings = settings;
            _telegramCrosspostManager = telegramCrosspostManager;
            _facebookCrosspostManager = facebookCrosspostManager;
            _twitterCrosspostManager = twitterCrosspostManager;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await LoadHotVacanciesToViewData();

            await base.OnActionExecutionAsync(context, next);
        }

        private async Task LoadHotVacanciesToViewData()
        {
            var vacancies = (await _vacancyManager.GetHotVacancies())
                .Select(o => new VacancyViewModel(o, _settings.WebSiteUrl))
                .ToImmutableList();

            ViewData["vacancies"] = vacancies;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Welcome!";

            var pagedResult = await _publicationManager.GetPublications();
            var categories = await _publicationManager.GetCategories();
            var publications = pagedResult.Select(o => new PublicationViewModel(o, _settings.WebSiteUrl, categories));
            
            var model = new StaticPagedList<PublicationViewModel>(publications, pagedResult);

            return View(model);
        }

        [Route("page/{page}")]
        public async Task<IActionResult> Page(int? categoryId = null, int page = 1, string language = Language.English)
        {
            ViewData["Title"] = $"{Pages.Page} {page}";

            var pagedResult = await _publicationManager.GetPublications(categoryId, page);
            var categories = await _publicationManager.GetCategories();
            var pages = pagedResult.Select(o => new PublicationViewModel(o, _settings.WebSiteUrl, categories));
            
            var model = new StaticPagedList<PublicationViewModel>(pages, pagedResult);

            ViewBag.CategoryId = categoryId;

            return View("~/Views/Home/Page.cshtml", model);
        }

        [Route("vacancies/{page}")]
        public async Task<IActionResult> Vacancies(int page = 1)
        {
            ViewData["Title"] = Pages.Vacancies;

            var pagedResult = await _vacancyManager.GetVacancies(page);

            var model = new StaticPagedList<VacancyViewModel>(pagedResult.Select(o => new VacancyViewModel(o, _settings.WebSiteUrl)), pagedResult);

            return View("~/Views/Home/Vacancies.cshtml", model);
        }

        [Route("vacancy/{id}")]
        public async Task<IActionResult> Vacancy(int id)
        {
            var vacancy = await _vacancyManager.Get(id);
            
            if (vacancy == null)
            {
                return NotFound();
            }
            
            await _vacancyManager.IncreaseViewCount(id);

            var path = Path.Combine(_env.WebRootPath, "images/vacancy");
            var file = Directory.GetFiles(path).OrderBy(o => Guid.NewGuid()).Select(Path.GetFileName).FirstOrDefault();
            var image = $"{_settings.WebSiteUrl}images/vacancy/{file}";

            var model = new VacancyViewModel(vacancy, _settings.WebSiteUrl, image);
            
            ViewData["Title"] = model.Title;

            return View("~/Views/Home/Vacancy.cshtml", model);
        }

        [Route("post/{id}")]
        public async Task<IActionResult> Post(int id)
        {
            var publication = await _publicationManager.Get(id);
            
            await _publicationManager.IncreaseViewCount(id);

            if (publication == null)
            {
                return NotFound();
            }

            var categories = await _publicationManager.GetCategories();
            var model = new PublicationViewModel(publication, _settings.WebSiteUrl, categories);
            
            ViewData["Title"] = model.Title;

            return View("~/Views/Home/Post.cshtml", model);
        }
        
        [Route("platform")]
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
                }).ToImmutableList();


            var pages = (await _facebookCrosspostManager.GetPages())
                .Select(o => new FacebookViewModel()
                {
                    Title = o.Name,
                    Description = o.Description,
                    Logo = o.Logo,
                    Link = o.Url,
                }).ToImmutableList();


            var twitters = (await _twitterCrosspostManager.GetAccounts())
                .Select(o => new TwitterViewModel()
                {
                    Title = o.Name,
                    Description = o.Description,
                    Logo = o.Logo,
                    Link = o.Url
                }).ToImmutableList();

            var result = new List<SocialNetworkViewModel>();
            
            result.AddRange(pages);
            result.AddRange(channels);
            result.AddRange(twitters);

            return View("~/Views/Home/Platform.cshtml", result);
        }
    }
}