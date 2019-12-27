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
                var url = string.IsNullOrEmpty(model.EmbededPlayerCode) ? model.Url : model.ShareUrl;
                var categoryTags = await _publicationService.GetCategoryTags(request.CategoryId);

                foreach (var service in _crossPostServices)
                {
                    await service.Send(request.CategoryId, request.Comment, url, GetTags(request));
                }

                return model;
            }

            throw new Exception("Can't save publication to database");
        }

        private IReadOnlyCollection<string> GetTags(NewPostRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Tags))
            {
                return ImmutableList<string>.Empty;
            }

            return request.Tags
                .Split(' ')
                .Where(o => !string.IsNullOrWhiteSpace(o))
                .Select(o => o.Trim())
                .ToImmutableList();
        }

        public Task<IReadOnlyCollection<Category>> GetCategories() => _publicationService.GetCategories();
    }
}