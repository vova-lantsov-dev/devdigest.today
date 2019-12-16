using System;
using System.Collections.Immutable;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Core;
using Core.Logging;
using Core.Services;
using Core.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Serilog.Events;
using WebSite.AppCode;

namespace WebSite.Controllers
{
    public class ApiController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly Settings _settings;
        private readonly ILogger _logger;
        private readonly IWebAppPublicationService _webAppPublicationService;

        public ApiController(
            IWebAppPublicationService webAppPublicationService,
            IUserService userService, 
            ILogger logger,
            Settings settings)
        {
            _logger = logger;
            _settings = settings;
            _userService = userService;
            _webAppPublicationService = webAppPublicationService;
        }
        
        [HttpGet]
        [Route("api")]
        public async Task<ActionResult<string>> GetApiVersion() => await Task.FromResult($"//devdigest API v{_settings.Version}");

        [HttpGet]
        [Route("api/categories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = (await _webAppPublicationService.GetCategories()).Select(o => new
            {
                o.Id,
                o.Name
            }).ToImmutableList();

            return Ok(categories);
        }

        [HttpPost]
        [Route("api/publications/new")]
        public async Task<IActionResult> AddPublication(NewPostRequest request)
        {
            var user = await _userService.GetBySecretKey(request.Key);

            if (user == null)
            {
                _logger.Write(LogEventLevel.Warning, $"Somebody tried to login with this key: `{request.Key}`. Text: `{request.Comment}`");

                return StatusCode((int)HttpStatusCode.Forbidden, "Incorrect security key");
            }

            try
            {
                var publication = await _webAppPublicationService.CreatePublication(request, user);
                
                if (publication != null) 
                    return Created(new Uri(publication.ShareUrl), publication);

                return BadRequest();
            }
            catch (DuplicateNameException ex)
            {
                _logger.Write(LogEventLevel.Error, "Error while creating new publication", ex);
                
                return StatusCode((int) HttpStatusCode.Conflict, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Write(LogEventLevel.Error, "Error while creating new publication", ex);
                
                return BadRequest(ex.Message);
            }
        }
        

        [HttpPost]
        [Route("api/vacancy/new")]
        public async Task<IActionResult> AddVacancy(NewVacancyRequest request)
        {
            var user = _userService.GetBySecretKey(request.Key);

            if (user == null)
            {
                _logger.Write(LogEventLevel.Warning, $"Somebody tried to login with this key: `{request.Key}`. Text: `{request.Comment}`");

                return StatusCode((int)HttpStatusCode.Forbidden, "Incorrect security key");
            }

            try
            {
                var vacancy = await _webAppPublicationService.CreateVacancy(request, user);
                
                if (vacancy != null) 
                    return Created(new Uri(vacancy.ShareUrl), vacancy);

                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.Write(LogEventLevel.Error, "Error while creating new vacancy", ex);
                
                return BadRequest(ex.Message);
            }
        }
    }
}
