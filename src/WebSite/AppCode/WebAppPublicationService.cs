using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core;
using Core.Models;
using Core.Repositories;
using Core.Services;
using Core.Services.Posting;
using Core.Web;
using DAL;
using Microsoft.Extensions.Logging;
using WebSite.ViewModels;
using X.PagedList;

namespace WebSite.AppCode;

public interface IWebAppPublicationService
{
    Task<PublicationViewModel> CreatePublication(NewPostRequest request, User user);
    Task<IReadOnlyCollection<Category>> GetCategories();
    Task<StaticPagedList<PublicationViewModel>> GetPublications(int? categoryId = null, int page = 1);
    Task<IReadOnlyCollection<PublicationViewModel>> FindPublications(params  string[] keywords);
    Task<PublicationViewModel> GetPublication(int id);
    Task<IReadOnlyCollection<VacancyViewModel>> LoadHotVacancies();
    Task<IPagedList<VacancyViewModel>> GetVacancies(int page = 1);
    Task<VacancyViewModel> GetVacancy(int id);
    Task<PlatformViewModel> GetPlatformInformation();
    Task<HomePageViewModel> GetHomePageInformation();
}

public class WebAppPublicationService : IWebAppPublicationService
{
    private readonly ILocalizationService _localizationService;
    private readonly IPublicationService _publicationService;
    private readonly ISocialRepository _socialRepository;
    private readonly ILanguageAnalyzerService _languageAnalyzer;
    private readonly PostingServiceFactory _factory;
    private readonly IVacancyService _vacancyService;
    private readonly Settings _settings;
    private readonly ILogger _logger;
        
    private IReadOnlyCollection<Category> _categories;

    public WebAppPublicationService(
        ILocalizationService localizationService,
        IPublicationService publicationService,
        ISocialRepository socialRepository,
        PostingServiceFactory factory,
        Settings settings,
        ILanguageAnalyzerService languageAnalyzer, 
        IVacancyService vacancyService,
        ILogger<WebAppPublicationService> logger)
    {
        _logger = logger;
        _languageAnalyzer = languageAnalyzer;
        _vacancyService = vacancyService;
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
        var (title, body) = MessageParser.Split(request.Comment);
        var message = MessageParser.Glue(title, body);

        var publication = await _publicationService.CreatePublication(
            metadata,
            user.Id,
            languageId,
            playerCode,
            request.CategoryId,
            message);

        if (publication != null)
        {
            var model = new PublicationViewModel(publication, _settings.WebSiteUrl);

            //If we can embed main content into site page, so we can share this page.
            var url = string.IsNullOrEmpty(model.EmbeddedPlayerCode) ? model.RedirectUrl : model.ShareUrl;
                
            var services = await GetPostingService(publication);
                
            foreach (var service in services)
            {
                await service.Send(title, body, url, GetTags(request));
            }

            return model;
        }

        throw new Exception("Can't save publication to database");
    }

   

    public async Task<IReadOnlyCollection<IPostingService>> GetPostingService(Publication publication)
    {
        var categoryId = publication.CategoryId;
            
        var telegramChannels = await _socialRepository.GetTelegramChannels(categoryId);
        var facebookPages = await _socialRepository.GetFacebookPages(categoryId);
        var twitterAccounts = await _socialRepository.GetTwitterAccounts();
        var slackApplications = await _socialRepository.GetSlackApplications();

        var services = new List<IPostingService>();
            
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
            
        foreach (var slack in slackApplications)
        {
            services.Add(_factory.CreateSlackService(slack.WebHookUrl));
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

    public async Task<IReadOnlyCollection<Category>> GetCategories()
    {
        return _categories ??= await _publicationService.GetCategories();
    }

    private async Task<IReadOnlyCollection<SocialAccount>> GetTelegramChannels() =>
        (await _socialRepository.GetTelegramChannels())
        .Select(o => new SocialAccount
        {
            Description = o.Description,
            Logo = o.Logo,
            Title = o.Title,
            Url = $"https://t.me/{o.Name.Replace("@", "")}"
        })
        .ToImmutableList();

    private async Task<IReadOnlyCollection<SocialAccount>> GetFacebookPages() =>
        (await _socialRepository.GetFacebookPages())
        .Select(o => new SocialAccount
        {
            Description = o.Description,
            Logo = o.Logo,
            Title = o.Name,
            Url = o.Url
        })
        .ToImmutableList();

    private async Task<IReadOnlyCollection<SocialAccount>> GetTwitterAccounts() =>
        (await _socialRepository.GetTwitterAccounts())
        .Select(o => new SocialAccount
        {
            Description = o.Description,
            Logo = o.Logo,
            Title = o.Name,
            Url = o.Url
        })
        .ToImmutableList();
        
    public async Task<IReadOnlyCollection<PublicationViewModel>> GetTopPublications()
    {
        var publications = await _publicationService.GetTopPublications();
        var categories = await GetCategories();
            
        return publications
            .Select(o => new PublicationViewModel(o, _settings.WebSiteUrl, categories))
            .ToImmutableList();
    }

    public async Task<IReadOnlyCollection<PublicationViewModel>> FindPublications(params string[] keywords)
    {
        var publications = await _publicationService.FindPublications(keywords);
        var categories = await GetCategories();

        return publications
            .Select(o => new PublicationViewModel(o, _settings.WebSiteUrl, categories))
            .ToImmutableList();
    }

    public async Task<PublicationViewModel> GetPublication(int id)
    {
        var publication = await _publicationService.Get(id);
        var categories = await GetCategories();
            
        if (publication != null)
        {
            await _publicationService.IncreaseViewCount(id);
            return new PublicationViewModel(publication, _settings.WebSiteUrl, categories);
        }

        return null;
    }

    public Task IncreaseViewCount(int publicationId)
    {
        return _publicationService.IncreaseViewCount(publicationId);
    }

    public async Task<IReadOnlyCollection<VacancyViewModel>> LoadHotVacancies()
    {
        var vacancies = (await _vacancyService.GetHotVacancies())
            .Select(o => new VacancyViewModel(o, _settings.WebSiteUrl))
            .ToImmutableList();

        return vacancies;
    }

    public async Task<IPagedList<VacancyViewModel>> GetVacancies(int page)
    {
        var vacancies = await _vacancyService.GetVacancies(page);
        var subset = vacancies.Select(o => new VacancyViewModel(o, _settings.WebSiteUrl));
            
        return new StaticPagedList<VacancyViewModel>(subset, vacancies);
    }

    public async Task<VacancyViewModel> GetVacancy(int id)
    {
        var vacancy = await _vacancyService.Get(id);
        await _vacancyService.IncreaseViewCount(id);

        var image = _vacancyService.GetVacancyImage();

        return new VacancyViewModel(vacancy, _settings.WebSiteUrl, image);
    }

    public async Task<PlatformViewModel> GetPlatformInformation()
    {
        return new PlatformViewModel
        {
            Telegram = await GetTelegramChannels(),
            Facebook = await GetFacebookPages(),
            Twitter = await GetTwitterAccounts()
        };
    }

    public async Task<HomePageViewModel> GetHomePageInformation()
    {
        return new HomePageViewModel
        {
            Publications = await GetPublications(),
            TopPublications = await GetTopPublications()
        };
    }

    public async Task<StaticPagedList<PublicationViewModel>> GetPublications(int? categoryId = null, int page = 1)
    {
        var categories = await GetCategories();
        var pagedResult = await _publicationService.GetPublications(categoryId, page);

        var publications = pagedResult
            .Select(o => new PublicationViewModel(o, _settings.WebSiteUrl, categories))
            .ToImmutableList();
            
        return new StaticPagedList<PublicationViewModel>(publications, pagedResult);            
    }
}