using System;
using System.Threading.Tasks;
using Core;
using Core.Repositories;
using Microsoft.AspNetCore.Mvc;
using WebSite.ViewModels;

namespace WebSite.Controllers;

public class ContentController : Controller
{
    private readonly IPageRepository _pageRepository;
    private readonly Settings _settings;
        
    public ContentController(IPageRepository pageRepository, Settings settings)
    {
        _pageRepository = pageRepository;
        _settings = settings;
    }

    [Route("content/{name}")]
    public async Task<IActionResult> Index(string name)
    {
        var page = await _pageRepository.GetPage(name);
            
        if (page == null)
            return NotFound();

        var model = new ContentPageViewModel
        {
            Title = page.Title,
            Description = page.Description,
            Url = new Uri($"{_settings.WebSiteUrl}content/{page.Name}"),
            Image = _settings.FacebookImage,
            Keywords = _settings.DefaultDescription,
            Content = page.Content
        };
            
        ViewBag.PageMetaData = model;

        return View(model);
    }

    [Route("content/telegram")]
    public async Task<IActionResult> Telegram()=> RedirectPermanent("/content/platform");

    [Route("partners")]
    public async Task<IActionResult> PartnersRedirect() => RedirectPermanent("/content/partners");  
        
    [Route("search")]
    public async Task<IActionResult> SearchRedirect() => RedirectPermanent("/content/search");
        
    [Route("content/platform")]
    [Route("content/telegram")]
    public async Task<IActionResult> Platform()=> RedirectPermanent("/platform");
}