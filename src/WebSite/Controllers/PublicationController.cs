using System;
using System.Net;
using System.Threading.Tasks;
using Core;
using Core.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebSite.ViewModels;

namespace WebSite.Controllers;

public class PublicationController : Controller
{
    private readonly ILogger _logger;
    private readonly Settings _settings;
    private readonly IPublicationRepository _publicationRepository;
    
    public PublicationController(
        IPublicationRepository publicationRepository, 
        Settings settings, 
        ILogger<PublicationController> logger)
    {
        _publicationRepository = publicationRepository;
        _logger = logger;
        _settings = settings;
    }

    [Route("pub/{id}")]
    public async Task<IActionResult> Index(int id)
    {
        var publication = await _publicationRepository.GetPublication(id);

        if (publication == null)
        {
            return NotFound();
        }

        var model = new PageRedirectModel
        {
            Title = publication.Title,
            Description = publication.Description,
            Url = new Uri(publication.Link),
            Image = _settings.FacebookImage,
            Keywords = _settings.DefaultDescription,
            Content = publication.Content
        };

        Response.StatusCode = (int)HttpStatusCode.PermanentRedirect;
        Response.Headers.Add("location",publication.Link);

        return View(model);
    }
}