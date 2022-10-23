using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebSite.AppCode;
using Core;
using Core.Models;
using WebSite.ViewModels;

namespace WebSite.Controllers;

public class HomeController : Controller
{
    private readonly IContentService _service;
    private readonly Settings _settings;

    public HomeController(IContentService service, Settings settings) 
    {
        _settings = settings;
        _service = service;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        ViewBag.Vacancies = await _service.GetHotVacancies();

        await base.OnActionExecutionAsync(context, next);
    }

    public async Task<IActionResult> Index()
    {
        var model = await _service.GetHomePageInformation();

        SetPageMetaData(new PageMetaData
        {
            Title = "Welcome!",
            Url = _settings.WebSiteUrl
        });
            
        return View(model);
    }

    [Route("covid")]
    public async Task<IActionResult> Covid()
    {
        var model = await _service.FindPosts("covid", "coronavirus");

        SetPageMetaData(new PageMetaData
        {
            Title = "COVID-19  – news and updates",
            Description = "Coronavirus disease 2019 (COVID-19) is an infectious disease caused by severe acute " +
                          "respiratory syndrome coronavirus 2 (SARS-CoV-2). A lot of pandemic related news and " +
                          "events are happening in IT. We have gathered here the news and publications that " +
                          "directly relate to  coronavirus COVID-19. ",
            Url = new Uri($"{_settings.WebSiteUrl}covid"),
            Image = new Uri("https://127fc3e2e552.blob.core.windows.net/devdigest-images/white-red-and-blue-flower-petals-3993212.jpg"),
            Keywords = "covid, coronavirus, covid-19"
        });

        return View("~/Views/Home/Covid.cshtml", model);
    }

    [Route("page/{page}")]
    public async Task<IActionResult> Page(int? categoryId = null, int page = 1, string language = Core.Language.English)
    {
        var publications = await _service.GetPosts(categoryId, page);
            
        SetPageMetaData(new PageMetaData
        {
            Title = $"Page {page}",
            Url = new Uri($"{_settings.WebSiteUrl}page/{page}")
        });

        return View("~/Views/Home/Page.cshtml", new PostListViewModel
        {
            List = publications,
            CategoryId = categoryId
        });
    }

    [Route("vacancies/{page}")]
    public async Task<IActionResult> Vacancies(int page = 1, int? categoryId = null)
    {
        SetPageMetaData(new PageMetaData
        {
            Title = "Job",
            Url = new Uri($"{_settings.WebSiteUrl}vacancies/{page}")
        });

        var model = new VacancyListViewModel
        {
            List = await _service.GetVacancies(page),
            CategoryId = categoryId
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

        SetPageMetaData(new PageMetaData
        {
            Title = model.Title,
            Url = model.ShareUrl,
            Image = model.Image,
            Keywords = model.Description,
            Description = model.Description
        });
            
        return View("~/Views/Home/Vacancy.cshtml", model);
    }

    [Route("post/{id}")]
    public async Task<IActionResult> Post(int id)
    {
        var publication = await _service.GetPost(id);
            
        if (publication == null)
        {
            return NotFound();
        }

        SetPageMetaData(new PageMetaData
        {
            Title = publication.Title,
            Description = publication.Description,
            Keywords = publication.Keywords,
            Url = publication.ShareUrl,
            Image = Uri.TryCreate(publication.Image, UriKind.RelativeOrAbsolute, out var uri) ? uri : _settings.FacebookImage
        });

        return View("~/Views/Home/Post.cshtml", publication);
    }


    [Route("platform")]
    public async Task<IActionResult> Platform()
    {
        var model = await _service.GetPlatformInformation();

        SetPageMetaData(new PageMetaData
        {
            Title = "Platform",
            Description = "This project is an information space for people who live in the modern " +
                          "world of IT technologies. For developers, analysts, architects, and engineers.",
            Url = new Uri($"{_settings.WebSiteUrl}platform"),
        });

        return View("~/Views/Home/Platform.cshtml", model);
    }
        
    private void SetPageMetaData(PageMetaData meta)
    {
        if (string.IsNullOrWhiteSpace(meta.Description))
        {
            meta.Description = _settings.DefaultDescription;
        }
            
        if (string.IsNullOrWhiteSpace(meta.Keywords))
        {
            meta.Keywords = _settings.DefaultKeywords;
        }
            
        if (meta.Image == null)
        {
            meta.Image = _settings.FacebookImage;
        }

        ViewBag.PageMetaData = meta;
    }
}