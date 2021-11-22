using System.Net;
using Core;
using Core.Repositories;
using Microsoft.AspNetCore.Mvc;
using WebSite.ViewModels;

namespace WebSite.Controllers;

public class RedirectController : Controller
{
    private readonly ILogger _logger;
    private readonly Settings _settings;
    private readonly IPublicationRepository _publicationRepository;
    
    public RedirectController(
        IPublicationRepository publicationRepository, 
        Settings settings, 
        ILogger<RedirectController> logger)
    {
        _publicationRepository = publicationRepository;
        _logger = logger;
        _settings = settings;
    }

    [Route("pub/{id}")]
    [Route("goto/{id}")]
    public async Task<IActionResult> Index(int id)
    {
        var publication = await _publicationRepository.GetPublication(id);

        if (publication == null)
        {
            return NotFound();
        }

        var model = new RedirectModel
        {
            Title = publication.Title,
            Description = publication.Description,
            Url = new Uri(publication.Link),
            Image = _settings.FacebookImage,
            Keywords = _settings.DefaultDescription
        };

        Response.StatusCode = (int)HttpStatusCode.PermanentRedirect;
        Response.Headers.Add("location",publication.Link);

        return View(model);
    }
}