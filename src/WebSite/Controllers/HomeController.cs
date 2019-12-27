using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core;
using Core.Models;
using Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using WebSite.AppCode;
using WebSite.ViewModels;
using X.PagedList;

namespace WebSite.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPublicationService _publicationService;
        private readonly IVacancyService _vacancyService;
        private readonly IMemoryCache _cache;
        private readonly Settings _settings;

        private readonly IWebAppPublicationService _webAppPublicationService;

        public HomeController(
            IMemoryCache cache,
            IVacancyService vacancyService,
            IPublicationService publicationService,
            Settings settings, 
            IWebAppPublicationService webAppPublicationService)
        {
            _cache = cache;
            _settings = settings;
            _vacancyService = vacancyService;
            _publicationService = publicationService;
            _webAppPublicationService = webAppPublicationService;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await LoadHotVacanciesToViewData();

            await base.OnActionExecutionAsync(context, next);
        }

        private async Task LoadHotVacanciesToViewData()
        {
            var vacancies = (await _vacancyService.GetHotVacancies())
                .Select(o => new VacancyViewModel(o, _settings.WebSiteUrl))
                .ToImmutableList();

            ViewData["vacancies"] = vacancies;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Welcome!";
            
            var model = new HomePageViewModel
            {
                Publications = await GetPublications(),
                TopPublications = await GetTopPublications()
            };
            
            return View(model);
        }

        private async Task<IReadOnlyCollection<PublicationViewModel>> GetTopPublications()
        {
            var publications = await _publicationService.GetTopPublications();
            var categories = await _publicationService.GetCategories();
            
            return publications
            .Select(o => new PublicationViewModel(o, _settings.WebSiteUrl, categories))
            .ToImmutableList();
        }

        private async Task<StaticPagedList<PublicationViewModel>> GetPublications()
        {
            var pagedResult = await _publicationService.GetPublications();
            var categories = await _publicationService.GetCategories();
            var publications = pagedResult.Select(o => new PublicationViewModel(o, _settings.WebSiteUrl, categories));
            return new StaticPagedList<PublicationViewModel>(publications, pagedResult);            
        }

        [Route("page/{page}")]
        public async Task<IActionResult> Page(int? categoryId = null, int page = 1, string language = Language.English)
        {
            ViewData["Title"] = $"Page {page}";

            var pagedResult = await _publicationService.GetPublications(categoryId, page);
            var categories = await _publicationService.GetCategories();
            var pages = pagedResult.Select(o => new PublicationViewModel(o, _settings.WebSiteUrl, categories));
            
            var model = new StaticPagedList<PublicationViewModel>(pages, pagedResult);

            ViewBag.CategoryId = categoryId;

            return View("~/Views/Home/Page.cshtml", model);
        }

        [Route("vacancies/{page}")]
        public async Task<IActionResult> Vacancies(int page = 1)
        {
            ViewData["Title"] = "Job";

            var pagedResult = await _vacancyService.GetVacancies(page);

            var model = new StaticPagedList<VacancyViewModel>(pagedResult.Select(o => new VacancyViewModel(o, _settings.WebSiteUrl)), pagedResult);

            return View("~/Views/Home/Vacancies.cshtml", model);
        }

        [Route("vacancy/{id}")]
        public async Task<IActionResult> Vacancy(int id)
        {
            var vacancy = await _vacancyService.Get(id);
            
            if (vacancy == null)
            {
                return NotFound();
            }
            
            await _vacancyService.IncreaseViewCount(id);

            var image = _vacancyService.GetVacancyImage();

            var model = new VacancyViewModel(vacancy, _settings.WebSiteUrl, image);
            
            ViewData["Title"] = model.Title;

            return View("~/Views/Home/Vacancy.cshtml", model);
        }

        [Route("post/{id}")]
        public async Task<IActionResult> Post(int id)
        {
            var publication = await _publicationService.Get(id);
            
            await _publicationService.IncreaseViewCount(id);

            if (publication == null)
            {
                return NotFound();
            }

            var categories = await _publicationService.GetCategories();
            var model = new PublicationViewModel(publication, _settings.WebSiteUrl, categories);
            
            ViewData["Title"] = model.Title;

            return View("~/Views/Home/Post.cshtml", model);
        }
        
        [Route("platform")]
        public async Task<IActionResult> Platform()
        {
            ViewData["Title"] = "Platform";

            var model = new PlatformViewModel
            {
                Telegram = await _webAppPublicationService.GetTelegramChannels(),
                Facebook = await _webAppPublicationService.GetFacebookPages(),
                Twitter = await _webAppPublicationService.GetTwitterAccounts()
            };

            return View("~/Views/Home/Platform.cshtml", model);
        }
    }
}