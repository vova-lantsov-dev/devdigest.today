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

namespace WebSite.Controllers
{
    public class HomeController : Controller
    {
        private readonly PublicationManager _publicationManager;
        private readonly VacancyManager _vacancyManager;

        public HomeController(IMemoryCache cache)
        {
            _publicationManager = new PublicationManager(Core.Settings.Current.ConnectionString, cache);
            _vacancyManager = new VacancyManager(Core.Settings.Current.ConnectionString, cache);
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Welcome!";

            var pagedResult = await _publicationManager.GetPublications();
            var categories = _publicationManager.GetCategories();
            var model = new StaticPagedList<PublicationViewModel>(pagedResult.Select(o => new PublicationViewModel(o, Settings.Current.WebSiteUrl, categories)), pagedResult);

            return View(model);
        }

        [Route("page/{page}")]
        public async Task<IActionResult> Page(int? categoryId = null, int page = 1)
        {
            ViewData["Title"] = $"{Core.Pages.Page} {page}";

            var pagedResult = await _publicationManager.GetPublications(categoryId, page);
            var categories = _publicationManager.GetCategories();
            var model = new StaticPagedList<PublicationViewModel>(pagedResult.Select(o => new PublicationViewModel(o, Settings.Current.WebSiteUrl, categories)), pagedResult);

            ViewBag.CategoryId = categoryId;

            return View("~/Views/Home/Page.cshtml", model);
        }

        [Route("vacancies/{page}")]
        public async Task<IActionResult> Vacancies(int page = 1)
        {
            ViewData["Title"] = $"{Core.Pages.Vacancies}";

            var pagedResult = await _vacancyManager.GetVacancies(page);

            var model = new StaticPagedList<VacancyViewModel>(pagedResult.Select(o => new VacancyViewModel(o, Settings.Current.WebSiteUrl)), pagedResult);


            return View("~/Views/Home/Vacancies.cshtml", model);
        }

        [Route("vacancy/{id}")]
        public async Task<IActionResult> Vacancy(int id)
        {
            var vacancy = await _vacancyManager.Get(id);

            if (vacancy == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound);
            }

            var model = new VacancyViewModel(vacancy, Settings.Current.WebSiteUrl);
            ViewData["Title"] = model.Title;

            return View("~/Views/Home/Vacancy.cshtml", model);
        }

        [Route("partners")]
        public async Task<IActionResult> Partners()
        {
            ViewData["Title"] = Core.Pages.Partners;

            return View("~/Views/Home/Partners.cshtml");
        }

        [Route("about")]
        public async Task<IActionResult> About()
        {
            ViewData["Title"] = Core.Pages.AboutUs;

            return View("~/Views/Home/About.cshtml");
        }

        [Route("post/{id}")]
        public async Task<IActionResult> Post(int id)
        {
            var publication = await _publicationManager.Get(id);

            if (publication == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound);
            }

            var model = new PublicationViewModel(publication, Settings.Current.WebSiteUrl);
            ViewData["Title"] = model.Title;

            return View("~/Views/Home/Post.cshtml", model);
        }

        public async Task<IActionResult> Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}