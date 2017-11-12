using Core;
using Core.Managers;
using Core.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace WebSite.Controllers
{
    public class ApiController : Controller
    {
        private readonly PublicationManager _publicationManager;
        private readonly VacancyManager _vacancyManager;
        private readonly UserManager _userManager;
        private readonly CrossPostManager _crossPostManager;

        public ApiController(IMemoryCache cache)
        {
            _publicationManager = new PublicationManager(Settings.Current.ConnectionString, cache);
            _userManager = new UserManager(Settings.Current.ConnectionString);
            _vacancyManager = new VacancyManager(Settings.Current.ConnectionString, cache);
            _crossPostManager = new CrossPostManager(Settings.Current.ConnectionString);
        }

        [HttpGet]
        [Route("api/categories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = _publicationManager.GetCategories().Select(o => new
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
            DAL.User user = _userManager.GetBySecretKey(request.Key);

            if (user == null)
            {
                return StatusCode((int)HttpStatusCode.Forbidden);
            }

            var extractor = new X.Web.MetaExtractor.Extractor();
            var languageAnalyzer = new LanguageAnalyzer(Settings.Current.CognitiveServicesTextAnalyticsKey);
            
            try
            {
                var metadata = await extractor.Extract(new Uri(request.Link));

                var languageCode = languageAnalyzer.GetTextLanguage(metadata.Description);
                var languageId = Language.GetLanguageId(languageCode) ?? Language.EnglishId;
                
                var publication = new DAL.Publication
                {
                    Title = metadata.Title,
                    Description = metadata.Description,
                    Link = metadata.Url,
                    Image = metadata.Image.FirstOrDefault(),
                    Type = metadata.Type,
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
                    var model = new PublicationViewModel(publication, Settings.Current.WebSiteUrl);

                    //If we can embed main content into site page, so we can share this page.
                    var url = string.IsNullOrEmpty(model.EmbededPlayerCode) ? model.Link : model.ShareUrl;

                    await _crossPostManager.Send(request.CategoryId, request.Comment, url);

                    return Created(new Uri(model.ShareUrl), model);
                }
                else
                {
                    throw new Exception("Can't save publication to databse");
                }
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
                Content = null,
                Image = null,
                Url = null,
            };

            vacancy = await _vacancyManager.Save(vacancy);

            if (vacancy != null)
            {
                var model = new VacancyViewModel(vacancy, Settings.Current.WebSiteUrl);

                await _crossPostManager.Send(request.CategoryId, request.Comment, model.ShareUrl);

                return Created(new Uri(model.ShareUrl), model);
            }

            return StatusCode((int)HttpStatusCode.BadRequest);
        }
    }
}
