using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Core;
using Core.Logging;
using Core.Models;
using Core.Repositories;
using Core.Services;
using Core.Services.Crosspost;
using Core.Web;
using DAL;
using WebSite.ViewModels;
using X.Web.MetaExtractor;

namespace WebSite.AppCode
{
    public interface IWebAppPublicationService
    {
        Task<PublicationViewModel> CreatePublication(NewPostRequest request, User user);
        Task<IReadOnlyCollection<Category>> GetCategories();
        Task<IReadOnlyCollection<SocialAccount>> GetTelegramChannels();
        Task<IReadOnlyCollection<SocialAccount>> GetFacebookPages();
        Task<IReadOnlyCollection<SocialAccount>> GetTwitterAccounts();
    }

    public class WebAppPublicationService : IWebAppPublicationService
    {
        private readonly ILocalizationService _localizationService;
        private readonly IPublicationService _publicationService;
        private readonly ISocialRepository _socialRepository;
        private readonly Settings _settings;
        private readonly ILogger _logger;
        private readonly ILanguageAnalyzerService _languageAnalyzer;
        private readonly CrossPostServiceFactory _factory;

        public WebAppPublicationService(
            ILocalizationService localizationService,
            IPublicationService publicationService,
            ISocialRepository socialRepository,
            CrossPostServiceFactory factory,
            Settings settings,
            ILogger logger, 
            ILanguageAnalyzerService languageAnalyzer)
        {
            _logger = logger;
            _languageAnalyzer = languageAnalyzer;
            _factory = factory;
            _settings = settings;
            _socialRepository = socialRepository;
            _localizationService = localizationService;
            _publicationService = publicationService;
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
            var player = EmbeddedPlayerFactory.CreatePlayer(request.Link);
            var playerCode = player != null ? await player.GetEmbeddedPlayerUrl(request.Link) : null;

            var publication = await _publicationService.CreatePublication(
                metadata,
                user.Id,
                languageId,
                playerCode,
                request.CategoryId,
                request.Comment);

            if (publication != null)
            {
                var model = new PublicationViewModel(publication, _settings.WebSiteUrl);

                //If we can embed main content into site page, so we can share this page.
                var url = string.IsNullOrEmpty(model.EmbededPlayerCode) ? model.Url : model.ShareUrl;
                
                var services = await GetServices(publication);
                
                foreach (var service in services)
                {
                    await service.Send(request.Comment, url, GetTags(request));
                }

                return model;
            }

            throw new Exception("Can't save publication to database");
        }

        private async Task<IReadOnlyCollection<ICrossPostService>> GetServices(Publication publication)
        {
            var categoryId = publication.CategoryId;
            
            var telegramChannels = await _socialRepository.GetTelegramChannels(categoryId);
            var facebookPages = await _socialRepository.GetFacebookPages(categoryId);
            var twitterAccounts = await _socialRepository.GetTwitterAccounts();

            var services = new List<ICrossPostService>();
            
            foreach (var telegramChannel in telegramChannels)
            {
                services.Add(_factory.CreateTelegramService(
                    telegramChannel.Token,
                    telegramChannel.Name));
            }

            foreach (var facebookPage in facebookPages)
            {
                services.Add(_factory.CreateFacebookService(
                    facebookPage.Token,
                    facebookPage.Name));
            }

            foreach (var twitterAccount in twitterAccounts)
            {
                services.Add(_factory.CreateTwitterService(
                    twitterAccount.ConsumerKey,
                    twitterAccount.ConsumerSecret,
                    twitterAccount.AccessToken,
                    twitterAccount.AccessTokenSecret,
                    twitterAccount.Name,
                    await _publicationService.GetCategoryTags(categoryId)));
            }

            return services.ToImmutableList();
        }

        private static IReadOnlyCollection<string> GetTags(NewPostRequest request)
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
        
        public async Task<IReadOnlyCollection<SocialAccount>> GetTelegramChannels() =>
            (await _socialRepository.GetTelegramChannels())
            .Select(o => new SocialAccount
            {
                Description = o.Description,
                Logo = o.Logo,
                Title = o.Title,
                Url = $"https://t.me/{o.Name.Replace("@", "")}"
            })
            .ToImmutableList();

        public async Task<IReadOnlyCollection<SocialAccount>> GetFacebookPages() =>
            (await _socialRepository.GetFacebookPages())
            .Select(o => new SocialAccount
            {
                Description = o.Description,
                Logo = o.Logo,
                Title = o.Name,
                Url = o.Url
            })
            .ToImmutableList();

        public async Task<IReadOnlyCollection<SocialAccount>> GetTwitterAccounts() =>
            (await _socialRepository.GetTwitterAccounts())
            .Select(o => new SocialAccount
            {
                Description = o.Description,
                Logo = o.Logo,
                Title = o.Name,
                Url = o.Url
            })
            .ToImmutableList();
    }
}