using System.Collections.Immutable;
using System.Data;
using Core;
using Core.Models;
using Core.Repositories;
using Core.Services;
using Core.Services.Posting;
using Core.Web;
using WebSite.ViewModels;
using X.PagedList;

namespace WebSite.AppCode;

public interface IWebAppPublicationService
{
    Task<PostViewModel> CreatePublication(CreatePostRequest request, int userId);
    Task<IReadOnlyCollection<Core.Models.Category>> GetCategories();
    Task<StaticPagedList<PostViewModel>> GetPublications(int? categoryId = null, int page = 1);
    Task<IReadOnlyCollection<PostViewModel>> FindPublications(params  string[] keywords);
    Task<PostViewModel> GetPublication(int id);
    Task<IReadOnlyCollection<VacancyViewModel>> LoadHotVacancies();
    Task<IPagedList<VacancyViewModel>> GetVacancies(int page = 1);
    Task<VacancyViewModel> GetVacancy(int id);
    Task<PlatformViewModel> GetPlatformInformation();
    Task<HomePageViewModel> GetHomePageInformation();
}

public class WebAppPublicationService : IWebAppPublicationService
{
    private readonly ILanguageService _languageService;
    private readonly IPostService _postService;
    private readonly ISocialRepository _socialRepository;
    private readonly ILanguageAnalyzerService _languageAnalyzer;
    private readonly PostingServiceFactory _factory;
    private readonly IVacancyService _vacancyService;
    private readonly Settings _settings;
    private readonly PostUrlBuilder _postUrlBuilder;
    private readonly ILogger _logger;
        
    private IReadOnlyCollection<Category> _categories;

    public WebAppPublicationService(
        ILanguageService languageService,
        IPostService postService,
        ISocialRepository socialRepository,
        PostingServiceFactory factory,
        Settings settings,
        ILanguageAnalyzerService languageAnalyzer, 
        IVacancyService vacancyService,
        PostUrlBuilder postUrlBuilder,
        ILogger<WebAppPublicationService> logger)
    {
        _logger = logger;
        _postUrlBuilder = postUrlBuilder;
        _languageAnalyzer = languageAnalyzer;
        _vacancyService = vacancyService;
        _factory = factory;
        _settings = settings;
        _socialRepository = socialRepository;
        _languageService = languageService;
        _postService = postService;
    }

    public async Task<PostViewModel> CreatePublication(CreatePostRequest request, int userId)
    {
        var extractor = new X.Web.MetaExtractor.Extractor();

        var metadata = await extractor.ExtractAsync(request.Link);

        var exist = await _postService.IsPostExist(metadata.Url);

        if (exist)
        {
            throw new DuplicateNameException("Publication with this URL already exist");
        }

        var languageCode = _languageAnalyzer.GetTextLanguage(metadata.Description);
        var languageId = await _languageService.GetLanguageId(languageCode) ?? Language.EnglishId;
        var player = EmbeddedPlayerFactory.CreatePlayer(request.Link);
        var playerCode = player != null ? await player.GetEmbeddedPlayerUrl(request.Link) : null;
        var categories = await GetCategories();

        var post = await _postService.Create(
            metadata,
            userId,
            languageId,
            playerCode,
            request.CategoryId,
            request.Title,
            request.Comment,
            request.TitleUa,
            request.CommentUa);

        var categoryName = categories
            .Where(o => o.Id == request.CategoryId)
            .Select(o => o.Name).SingleOrDefault();

        var model = new PostViewModel
        {
            Id = post.Id,
            Category = new CategoryViewModel
            {
                Id = request.CategoryId,
                Name = categoryName,
            },
            Description = post.Description,
            Image = post.Image,
            Title = post.Title,
            Url = post.Url,
            DateTime = post.DateTime,
            ShareUrl = _postUrlBuilder.Build(post.Id),
            ViewsCount = post.Views,
            EmbeddedPlayerCode = post.EmbeddedPlayerCode
        };

        var redirectUrl = _postUrlBuilder.BuildRedirectUrl(post.Id);

        //If we can embed main content into site page, so we can share this page.
        var url = string.IsNullOrEmpty(model.EmbeddedPlayerCode) ? redirectUrl : model.ShareUrl;

        var services = await GetPostingService(request.CategoryId);
        var serviceUa = _factory.CreateTelegramService("@devdigest_ua");
        
        foreach (var service in services)
        {
            await service.Send(request.Title, request.Comment, url);
        }

        await serviceUa.Send(request.TitleUa, request.CommentUa, url);

        return model;
    }

    private async Task<IReadOnlyCollection<IPostingService>> GetPostingService(int categoryId)
    {
        var telegramChannels = await _socialRepository.GetTelegramChannels(categoryId);
        var facebookPages = await _socialRepository.GetFacebookPages(categoryId);
        var twitterAccounts = await _socialRepository.GetTwitterAccounts();
        var slackApplications = await _socialRepository.GetSlackApplications();

        var services = new List<IPostingService>();
            
        foreach (var telegramChannel in telegramChannels)
        {
            services.Add(_factory.CreateTelegramService(telegramChannel.Name));
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
                await _postService.GetCategoryTags(categoryId)));
        }
            
        foreach (var slack in slackApplications)
        {
            services.Add(_factory.CreateSlackService(slack.WebHookUrl));
        }

        return services.ToImmutableList();
    }

    public async Task<IReadOnlyCollection<Category>> GetCategories()
    {
        return _categories ??= (await _postService.GetCategories())
            .Select(o => new Category
            {
                Id = o.Id,
                Name = o.Name
            })
            .ToImmutableList();
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

    private async Task<IReadOnlyCollection<PostViewModel>> GetTopPublications()
    {
        var publications = await _postService.GetTop();
        var categories = await GetCategories();
            
        return publications
            .Select(o => CreatePostViewModel(o, categories))
            .ToImmutableList();
    }

    public async Task<IReadOnlyCollection<PostViewModel>> FindPublications(params string[] keywords)
    {
        var publications = await _postService.Find(keywords);
        var categories = await GetCategories();

        return publications
            .Select(o => CreatePostViewModel(o, categories))
            .ToImmutableList();
    }

    public async Task<PostViewModel> GetPublication(int id)
    {
        var publication = await _postService.Get(id);
        var categories = await GetCategories();
            
        if (publication != null)
        {
            await _postService.IncreaseViewCount(id);
            return CreatePostViewModel(publication, categories);
        }

        return null;
    }

    public async Task<IReadOnlyCollection<VacancyViewModel>> LoadHotVacancies()
    {
        var vacancies = (await _vacancyService.GetHot())
            .Select(o => new VacancyViewModel(o, _settings.WebSiteUrl))
            .ToImmutableList();

        return vacancies;
    }

    public async Task<IPagedList<VacancyViewModel>> GetVacancies(int page)
    {
        var vacancies = await _vacancyService.GetList(page, Settings.DefaultPageSize);
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

    public async Task<StaticPagedList<PostViewModel>> GetPublications(int? categoryId = null, int page = 1)
    {
        var categories = await GetCategories();
        var pagedResult = await _postService.GetPublications(categoryId, page);

        var publications = pagedResult
            .Select(o => CreatePostViewModel(o, categories))
            .ToImmutableList();
            
        return new StaticPagedList<PostViewModel>(publications, pagedResult);            
    }
    
    private PostViewModel CreatePostViewModel(DAL.Post p, IReadOnlyCollection<Category> categories)
    {
        Uri.TryCreate(p.Link, UriKind.RelativeOrAbsolute, out var url);

        var categoryName = categories.Where(o => o.Id == p.CategoryId)
            .Select(o => o.Name)
            .SingleOrDefault();
        
        return new PostViewModel
        {
            Id = p.Id,
            Category = new CategoryViewModel
            {
                Id = p.CategoryId,
                Name = categoryName
            },
            Description = p.Description,
            Image = p.Image,
            Title = p.Title,
            Url = url ?? _settings.WebSiteUrl,
            DateTime = p.DateTime,
            ViewsCount = p.Views,
            EmbeddedPlayerCode = p.EmbededPlayerCode,
            ShareUrl = _postUrlBuilder.Build(p.Id)
        };
    }
}