using Core;
using Core.Managers;
using Core.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Core.Logging;

namespace WebSite.Controllers
{
    public class ApiController : ControllerBase
    {
        private readonly IPublicationManager _publicationManager;
        private readonly IVacancyManager _vacancyManager;
        private readonly IUserManager _userManager;
        private readonly ILocalizationManager _localizationManager;
        private readonly IReadOnlyCollection<ICrossPostManager> _crossPostManagers;
        private readonly Settings _settings;

        public ApiController(
            IPublicationManager publicationManager, 
            IVacancyManager vacancyManager, 
            IUserManager userManager, 
            ILocalizationManager localizationManager,
            ILogger logger,
            Settings settings, 
            FacebookCrosspostManager facebookCrosspostManager, 
            TelegramCrosspostManager telegramCrosspostManager)
        {
            _settings = settings;
            _userManager = userManager;
            _vacancyManager = vacancyManager;
            _localizationManager = localizationManager;
            _publicationManager = publicationManager;

            _crossPostManagers = new List<ICrossPostManager>
            {
                facebookCrosspostManager,
                telegramCrosspostManager,
                //new FakeCrosspostManager(logger)
            };
        }
        
        [HttpGet]
        [Route("api")]
        public async Task<ActionResult<string>> GetApiVersion()
        {
            return await Task.FromResult($"//devdigest API v{_settings.Version}");
        }

        [HttpGet]
        [Route("api/categories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = (await _publicationManager.GetCategories()).Select(o => new
            {
                o.Id,
                o.Name
            }).ToList();

            return Ok(categories);
        }

        [HttpPost]
        [Route("api/publications/new")]
        public async Task<IActionResult> AddPublicaton(NewPostRequest request)
        {
            var user = _userManager.GetBySecretKey(request.Key);

            if (user == null)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, "Incorrect security key");
            }

            var extractor = new X.Web.MetaExtractor.Extractor();
            var languageAnalyzer = new LanguageAnalyzer(_settings.CognitiveServicesTextAnalyticsKey);
            
            try
            {
                var metadata = await extractor.ExtractAsync(new Uri(request.Link));

                var existingPublication =  _publicationManager.Get(new Uri(metadata.Url));

                if (existingPublication != null)
                {
                    return StatusCode((int)HttpStatusCode.Conflict, "Publication with this URL already exist");
                }
                
                var languageCode = languageAnalyzer.GetTextLanguage(metadata.Description);
                var languageId = _localizationManager.GetLanguageId(languageCode) ?? Language.EnglishId;
                
                var publication = new DAL.Publication
                {
                    Title = metadata.Title,
                    Description = metadata.Description,
                    Link = metadata.Url,
                    Image = metadata.Images.FirstOrDefault(),
                    Type = "article",
                    DateTime = DateTime.Now,
                    UserId = user.Id,
                    CategoryId = request.CategoryId,
                    Comment = request.Comment,
                    LanguageId = languageId
                };

                if (EmbededPlayer.GetPlayerSoure(request.Link) != null)
                {
                    var player = new EmbededPlayer(request.Link);
                    publication.EmbededPlayerCode = player.Render();
                }

                publication = await _publicationManager.Save(publication);

                if (publication != null)
                {
                    var model = new PublicationViewModel(publication, _settings.WebSiteUrl);

                    //If we can embed main content into site page, so we can share this page.
                    var url = string.IsNullOrEmpty(model.EmbededPlayerCode) ? model.Link : model.ShareUrl;

                    foreach (var crossPostManager in _crossPostManagers)
                    {
                        await crossPostManager .Send(request.CategoryId, request.Comment, url);    
                    }

                    return Created(new Uri(model.ShareUrl), model);
                }

                
                throw new Exception("Can't save publication to databse");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("api/vacancy/new")]
        public async Task<IActionResult> AddVacancy(NewVacancyRequest request)
        {
            var user = _userManager.GetBySecretKey(request.Key);

            if (user == null)
            {
                return StatusCode((int)HttpStatusCode.Forbidden);
            }

            var vacancy = new DAL.Vacancy
            {
                Title = request.Title,
                Description = request.Description,
                Contact = request.Contact,
                UserId = user.Id,
                CategoryId = request.CategoryId,
                Date = DateTime.Now,
                Active = true,
                Content = request.Content,
                Image = null,
                Url = null,
            };

            vacancy = await _vacancyManager.Save(vacancy);

            if (vacancy != null)
            {
                var model = new VacancyViewModel(vacancy, _settings.WebSiteUrl);
                
                foreach (var crossPostManager in _crossPostManagers)
                {
                    await crossPostManager.Send(request.CategoryId, request.Comment, model.ShareUrl);
                }

                return Created(new Uri(model.ShareUrl), model);
            }

            return StatusCode((int)HttpStatusCode.BadRequest);
        }
    }
}
