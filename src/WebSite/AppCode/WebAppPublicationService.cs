using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core;
using Core.Logging;
using Core.Services;
using Core.Services.Crosspost;
using Core.ViewModels;
using Core.Web;
using DAL;

namespace WebSite.AppCode
{
    public interface IWebAppPublicationService
    {
        Task<PublicationViewModel> CreatePublication(NewPostRequest request, User user);
        Task<VacancyViewModel> CreateVacancy(NewVacancyRequest request, Task<User> user);
        Task<IReadOnlyCollection<Category>> GetCategories();
    }

    public class WebAppPublicationService : IWebAppPublicationService
    {
        private readonly ILocalizationService _localizationService;
        private readonly IPublicationService _publicationService;
        private readonly IVacancyService _vacancyService;
        private readonly IReadOnlyCollection<ICrossPostService> _crossPostServices;
        private readonly Settings _settings;
        private readonly ILogger _logger;
        private readonly LanguageAnalyzerService _languageAnalyzer;

        public WebAppPublicationService(
            ILocalizationService localizationService, 
            IPublicationService publicationService, 
            IVacancyService vacancyService, 
            Settings settings,
            FacebookCrosspostService facebookService, 
            TelegramCrosspostService telegramService,
            TwitterCrosspostService twitterService, 
            ILogger logger)
        {
            _localizationService = localizationService;
            _publicationService = publicationService;
            _vacancyService = vacancyService;
            _settings = settings;
            _logger = logger;

            _crossPostServices = ImmutableList.Create<ICrossPostService>(facebookService, telegramService, twitterService);
            _languageAnalyzer = new LanguageAnalyzerService(_settings.CognitiveServicesTextAnalyticsKey, _logger);
        }

        public async Task<PublicationViewModel> CreatePublication(NewPostRequest request, User user)
        {
            var extractor = new X.Web.MetaExtractor.Extractor();

            var metadata = await extractor.ExtractAsync(request.Link);

            var existingPublication = await _publicationService.Get(new Uri(metadata.Url));

            if (existingPublication != null)
            {
                throw new DuplicateNameException("Publication with this URL already exist");
            }

            var languageCode = _languageAnalyzer.GetTextLanguage(metadata.Description);
            var languageId = await _localizationService.GetLanguageId(languageCode) ?? Core.Language.EnglishId;
            var image = metadata.Images.FirstOrDefault();

            var publication = new DAL.Publication
            {
                Title = metadata.Title,
                Description = metadata.Description,
                Link = metadata.Url,
                Image = string.IsNullOrWhiteSpace(image) || image.Length > 250 ? string.Empty : image,
                Type = "article",
                DateTime = DateTime.Now,
                UserId = user.Id,
                CategoryId = request.CategoryId,
                Comment = request.Comment,
                LanguageId = languageId
            };

            var player = EmbeddedPlayerFactory.CreatePlayer(request.Link);

            if (player != null)
            {
                publication.EmbededPlayerCode = await player.GetEmbeddedPlayerUrl(request.Link);
            }

            publication = await _publicationService.Save(publication);

            if (publication != null)
            {
                var model = new PublicationViewModel(publication, _settings.WebSiteUrl);

                //If we can embed main content into site page, so we can share this page.
                var url = string.IsNullOrEmpty(model.EmbededPlayerCode) ? model.Link : model.ShareUrl;

                IReadOnlyCollection<string> tags = await GetTags(request.CategoryId);

                foreach (var crossPostManager in _crossPostServices)
                {
                    await crossPostManager.Send(request.CategoryId, request.Comment, url, tags);
                }

                return model;
            }

            throw new Exception("Can't save publication to database");
        }

        private async Task<IReadOnlyCollection<string>> GetTags(int categoryId)
        {
            //TODO: move categories tags to database
            
            var categoryName = await _publicationService.GetCategoryName(categoryId);

            var tags = new List<string>
            {
                "#devdigest",
                ConvertToHashTag(categoryName)
            };
            
            if (categoryId == Categories.Azure)
            {
                tags.AddRange(new []{ "#cloud"});
            }
            
            if (categoryId == Categories.News)
            {
                
            }
            
            if (categoryId == Categories.Xamarin)
            {
                tags.AddRange(new []{ "#mobile_development"});
            }
            
            if (categoryId == Categories.DataScience)
            {
                
            }
            
            if (categoryId == Categories.LiveEvents)
            {
                tags.AddRange(new []{ "#event #news"});
            }
            
            if (categoryId == Categories.UAEvents)
            {
                tags.AddRange(new []{ "#event #news #ukraine"});
            }
            
            if (categoryId == Categories.NETCore)
            {
                tags.AddRange(new []{ "#software_development"});
            }

            return tags;
        }
        
        private static string ConvertToHashTag(string text) =>
            "#" + text
                .Replace(".", "")
                .Replace("#", "")
                .Replace(" ", "_")
                .ToLower();

        public async Task<VacancyViewModel> CreateVacancy(NewVacancyRequest request, Task<User> user)
        {
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

            vacancy = await _vacancyService.Save(vacancy);

            if (vacancy != null)
            {
                var model = new VacancyViewModel(vacancy, _settings.WebSiteUrl);

                foreach (var crossPostManager in _crossPostServices)
                {
                    await crossPostManager.Send(
                        request.CategoryId,
                        request.Comment,
                        model.ShareUrl,
                        ImmutableList<string>.Empty);
                }

                return model;
            }

            throw new Exception("Can't save vacancy to database");
        }

        public async Task<IReadOnlyCollection<Category>> GetCategories()
        {
            return await _publicationService.GetCategories();
        }
    }
}