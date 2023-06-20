using System.Collections.Immutable;
using System.Data;
using Core;
using Core.Models;
using Core.Repositories;
using Core.Services;
using Core.Web;
using WebSite.ViewModels;
using X.PagedList;

namespace WebSite.AppCode;

public interface IContentService
{
    Task<StaticPagedList<PostViewModel>> GetPosts(int? categoryId = null, int page = 1);
    Task<IReadOnlyCollection<PostViewModel>> FindPosts(params string[] keywords);
    Task<PostViewModel> GetPost(int id);
    Task<IReadOnlyCollection<VacancyViewModel>> GetHotVacancies();
    Task<IPagedList<VacancyViewModel>> GetVacancies(int page = 1);
    Task<VacancyViewModel> GetVacancy(int id);
    Task<PlatformViewModel> GetPlatformInformation();
    Task<HomePageViewModel> GetHomePageInformation();
}

public class ContentService : IContentService
{
    private readonly IPostService _postService;
    private readonly ISocialRepository _socialRepository;
    private readonly IVacancyService _vacancyService;
    private readonly Settings _settings;
    private readonly PostUrlBuilder _postUrlBuilder;
    private readonly ILogger _logger;
        
    private IReadOnlyCollection<Category> _categories;

    public ContentService(IPostService postService,
        ISocialRepository socialRepository,
        Settings settings, 
        IVacancyService vacancyService,
        PostUrlBuilder postUrlBuilder,
        ILogger<ContentService> logger)
    {
        _logger = logger;
        _postUrlBuilder = postUrlBuilder;
        _vacancyService = vacancyService;
        _settings = settings;
        _socialRepository = socialRepository;
        _postService = postService;
    }
    
    private async Task<IReadOnlyCollection<Category>> GetCategories()
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

    public async Task<IReadOnlyCollection<PostViewModel>> FindPosts(params string[] keywords)
    {
        var publications = await _postService.Find(keywords);
        var categories = await GetCategories();

        return publications
            .Select(o => CreatePostViewModel(o, categories))
            .ToImmutableList();
    }

    public async Task<PostViewModel> GetPost(int id)
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

    public async Task<IReadOnlyCollection<VacancyViewModel>> GetHotVacancies()
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

        if (vacancy == null)
        {
            return null;
        }
        
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
            Publications = await GetPosts(),
            TopPublications = await GetTopPublications()
        };
    }

    public async Task<StaticPagedList<PostViewModel>> GetPosts(int? categoryId = null, int page = 1)
    {
        var categories = await GetCategories();
        var pagedResult = await _postService.GetList(categoryId, page);

        var publications = pagedResult
            .Select(o => CreatePostViewModel(o, categories))
            .ToImmutableList();
            
        return new StaticPagedList<PostViewModel>(publications, pagedResult);            
    }
    
    private PostViewModel CreatePostViewModel(Post p, IReadOnlyCollection<Category> categories)
    {
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
            Url = p.Url ?? _settings.WebSiteUrl,
            DateTime = p.DateTime,
            ViewsCount = p.Views,
            EmbeddedPlayerCode = p.EmbeddedPlayerCode,
            ShareUrl = _postUrlBuilder.Build(p.Id)
        };
    }
}