using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebSite.AppCode;
using Core;
using Core.Models;

namespace WebSite.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebAppPublicationService _service;
        private readonly Settings _settings;

        public HomeController(IWebAppPublicationService service, Settings settings) 
        {
            _settings = settings;
            _service = service;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            ViewBag.Vacancies = await _service.LoadHotVacancies();

            await base.OnActionExecutionAsync(context, next);
        }

        public async Task<IActionResult> Index()
        {
            var model = await _service.GetHomePageInformation();
            
            ViewBag.PageMetaData = new Core.Models.PageMetaData
            {
                Title = "Welcome!",
                Description = _settings.DefaultDescription,
                Url = _settings.WebSiteUrl,
                Image = _settings.FacebookImage,
                Keywords = _settings.DefaultKeywords
            };

            return View(model);
        }
        
        [Route("covid")]
        public async Task<IActionResult> Covid()
        {
            var model = await _service.FindPublications("covid", "coronavirus");
            
            ViewBag.PageMetaData = new Core.Models.PageMetaData
            {
                Title = $"COVID-19  – news and updates",
                Description = "Coronavirus disease 2019 (COVID-19) is an infectious disease caused by severe acute respiratory syndrome coronavirus 2 (SARS-CoV-2). A lot of pandemic related news and events are happening in IT. We have gathered here the news and publications that directly relate to  coronavirus COVID-19. ",
                Url = new Uri($"{_settings.WebSiteUrl}covid"),
                Image = new Uri("https://127fc3e2e552.blob.core.windows.net/devdigest-images/white-red-and-blue-flower-petals-3993212.jpg"),
                Keywords = "covid, coronavirus, covid-19"
            };

            return View("~/Views/Home/Covid.cshtml", model);
        }

        [Route("page/{page}")]
        public async Task<IActionResult> Page(int? categoryId = null, int page = 1, string language = Core.Language.English)
        {
            var model = await _service.GetPublications(categoryId, page);
            
            ViewBag.Title = $"Page {page}";
            ViewBag.CategoryId = categoryId;
            
            ViewBag.PageMetaData = new PageMetaData
            {
                Title = ViewBag.Title,
                Description = _settings.DefaultDescription,
                Url = new Uri($"{_settings.WebSiteUrl}page/{page}"),
                Image = _settings.FacebookImage,
                Keywords = _settings.DefaultKeywords
            };

            return View("~/Views/Home/Page.cshtml", model);
        }

        [Route("vacancies/{page}")]
        public async Task<IActionResult> Vacancies(int page = 1)
        {
            var model= await _service.GetVacancies(page);
            
            ViewBag.Title = "Job";
            
            ViewBag.PageMetaData = new PageMetaData
            {
                Title = "Job",
                Description = _settings.DefaultDescription,
                Url = new Uri($"{_settings.WebSiteUrl}vacancies/{page}"),
                Image = _settings.FacebookImage,
                Keywords = _settings.DefaultDescription
            };
            
            return View("~/Views/Home/Vacancies.cshtml", model);
        }

        [Route("vacancy/{id}")]
        public async Task<IActionResult> Vacancy(int id)
        {
            var model = await _service.GetVacancy(id);
            
            if (model == null)
            {
                return NotFound();
            }
            
            ViewBag.PageMetaData = new PageMetaData
            {
                Title = model.Title,
                Description = _settings.DefaultDescription,
                Url = model.ShareUrl,
                Image = model.Image,
                Keywords = model.Description
            };

            return View("~/Views/Home/Vacancy.cshtml", model);
        }

        [Route("post/{id}")]
        public async Task<IActionResult> Post(int id)
        {
            var publication = await _service.GetPublication(id);
            
            if (publication == null)
            {
                return NotFound();
            }
            
            ViewBag.PageMetaData = new PageMetaData 
            {
                Title = publication.Title,
                Description = publication.Description,
                Url = publication.ShareUrl,
                Image = new Uri(publication.Image),        
                Keywords = _settings.DefaultKeywords
            };

            return View("~/Views/Home/Post.cshtml", publication);
        }
        
        [Route("platform")]
        public async Task<IActionResult> Platform()
        {
            var model = await _service.GetPlatformInformation();

            ViewBag.PageMetaData = new PageMetaData
            {
                Title = "Platform",
                Description = "This project is an information space for people who live in the modern " +
                              "world of IT technologies. For developers, analysts, architects, and engineers.",
                Url = new Uri($"{_settings.WebSiteUrl}platform"),
                Image = _settings.FacebookImage,
                Keywords = _settings.DefaultKeywords
            };

            return View("~/Views/Home/Platform.cshtml", model);
        }
    }
}