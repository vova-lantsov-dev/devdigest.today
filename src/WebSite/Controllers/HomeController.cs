using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Core;
using Core.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using X.PagedList;

namespace WebSite.Controllers
{
    public class HomeController : Controller
    {
        private readonly PublicationManager _manager;

        public HomeController(IMemoryCache cache)
        {
            _manager = new PublicationManager(Core.Settings.Current.ConnectionString, cache);
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Добро пожаловать!";

            var pagedResult = await _manager.GetPublications();
            var categories = _manager.GetCategories();
            var model = new StaticPagedList<PublicationViewModel>(pagedResult.Select(o => new PublicationViewModel(o, Settings.Current.WebSiteUrl, categories)), pagedResult);

            return View(model);
        }

        [Route("page/{page}")]
        public async Task<IActionResult> Page(int? categoryId = null, int page = 1)
        {
            ViewData["Title"] = $"Страница {page}";

            var pagedResult = await _manager.GetPublications(categoryId, page);
            var categories = _manager.GetCategories();
            var model = new StaticPagedList<PublicationViewModel>(pagedResult.Select(o => new PublicationViewModel(o, Settings.Current.WebSiteUrl, categories)), pagedResult);

            ViewBag.CategoryId = categoryId;

            return View("~/Views/Home/Page.cshtml", model);
        }

        [Route("partners")]
        public async Task<IActionResult> Partners()
        {
            ViewData["Title"] = $"Партнеры";

            return View("~/Views/Home/Partners.cshtml");
        }

        [Route("about")]
        public async Task<IActionResult> About()
        {
            ViewData["Title"] = $"О проекте";

            return View("~/Views/Home/About.cshtml");
        }

        [Route("post/{id}")]
        public async Task<IActionResult> Post(int id)
        {
            var publication = await _manager.Get(id);

            if (publication == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound);
            }

            var model = new PublicationViewModel(publication, Settings.Current.WebSiteUrl);
            ViewData["Title"] = model.Title;

            return View("~/Views/Home/Post.cshtml", model);
        }

        [Route("post/new")]
        public async Task<IActionResult> NewPost()
        {
            return View("~/Views/Home/NewPost.cshtml");
        }

        public async Task<IActionResult> Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}