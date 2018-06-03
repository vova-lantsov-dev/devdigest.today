using Core;
using Core.Managers;
using Core.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using X.PagedList;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebSite.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPublicationManager _publicationManager;
        private readonly IVacancyManager _vacancyManager;
        private readonly IHostingEnvironment _env;
        private readonly IMemoryCache _cache;
        private readonly Settings _settings;

        public HomeController(
            IMemoryCache cache, 
            IHostingEnvironment env, 
            IVacancyManager vacancyManager, 
            IPublicationManager publicationManager, 
            Settings settings)
        {
            _cache = cache;
            _env = env;
            _vacancyManager = vacancyManager;
            _publicationManager = publicationManager;
            _settings = settings;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await LoadHotVacanciesToViewData();

            await base.OnActionExecutionAsync(context, next);
        }

        private async Task LoadHotVacanciesToViewData()
        {
            var vacancies = (await _vacancyManager
                                .GetHotVacancies())
                                .Select(o => new VacancyViewModel(o, _settings.WebSiteUrl))
                                .ToList();

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
        public async Task<IActionResult> Page(int? categoryId = null, int page = 1, string lanugage = Core.Language.English)
        {
            ViewData["Title"] = $"{Core.Pages.Page} {page}";

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
            ViewData["Title"] = $"{Core.Pages.Vacancies}";

            var pagedResult = await _vacancyManager.GetVacancies(page);

            var model = new StaticPagedList<VacancyViewModel>(pagedResult.Select(o => new VacancyViewModel(o, _settings.WebSiteUrl)), pagedResult);

            return View("~/Views/Home/Vacancies.cshtml", model);
        }

        [Route("vacancy/{id}")]
        public async Task<IActionResult> Vacancy(int id)
        {
            var vacancy = await _vacancyManager.Get(id);
            await _vacancyManager.IncreaseViewCount(id);

            if (vacancy == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound);
            }

            var path = Path.Combine(_env.WebRootPath, "images/vacancy");
            var file = Directory.GetFiles(path).OrderBy(o => Guid.NewGuid()).Select(o => Path.GetFileName(o)).FirstOrDefault();
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
                return StatusCode((int)HttpStatusCode.NotFound);
            }

            var categories = await _publicationManager.GetCategories();
            var model = new PublicationViewModel(publication, _settings.WebSiteUrl, categories);
            ViewData["Title"] = model.Title;

            return View("~/Views/Home/Post.cshtml", model);
        }

        public async Task<IActionResult> Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}