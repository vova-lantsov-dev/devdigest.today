using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Core.Repositories;
using Core.Services.Posting;
using Core.Web;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using X.PagedList;
using X.Web.MetaExtractor;

namespace Core.Services;

public interface IPostService
{
    Task<(Post post, Uri url)> Create(CreatePostRequest request, int userId);
    
    Task<IPagedList<Post>> GetList(
        int? categoryId = null, 
        int page = 1,
        int pageSize = 10, 
        int languageId = Language.EnglishId);
        
    Task<IReadOnlyCollection<Post>> GetTop(int languageId = Language.EnglishId);
        
    Task<IReadOnlyCollection<Category>> GetCategories();
        
    Task<Post> Get(int id);

    Task IncreaseViewCount(int id);
    
    Task<IReadOnlyCollection<Post>> Find(params string[] keywords);
}

public class PostService : IPostService
{
    private readonly ILogger _logger;
    private readonly IMemoryCache _cache;
    private readonly IPostRepository _repository;
    private readonly PostUrlBuilder _postUrlBuilder;
    private readonly ILanguageService _languageService;
    private readonly ILanguageAnalyzerService _languageAnalyzer;
    private readonly PostingServiceFactory _factory;
    private readonly ISocialRepository _socialRepository;
    
    public PostService(
        IMemoryCache cache,
        IPostRepository repository,
        PostUrlBuilder postUrlBuilder,
        ISocialRepository socialRepository,
        PostingServiceFactory factory,
        ILanguageAnalyzerService languageAnalyzer,
        ILanguageService languageService,
        ILogger<PostService> logger)
    {
        _cache = cache;
        _logger = logger;
        _postUrlBuilder = postUrlBuilder;
        _socialRepository = socialRepository;
        _factory = factory;
        _languageAnalyzer = languageAnalyzer;
        _languageService = languageService;
        _repository = repository;
    }

    public async Task<IPagedList<Post>> GetList(
        int? categoryId = null, 
        int page = 1,
        int pageSize = 10, 
        int languageId = Language.EnglishId)
    {
        var key = $"page_{page}_{pageSize}_{categoryId}";

        var result = _cache.Get(key) as IPagedList<Post>;

        if (result == null)
        {
            var list = (await _repository.GetList(categoryId, languageId, page, pageSize))
                .Select(Map)
                .ToImmutableList();
            
            var totalItemsCount = await _repository.GetCount(categoryId, languageId);

            result = new StaticPagedList<Post>(list, page, pageSize, totalItemsCount);
            
            _cache.Set(key, result, GetMemoryCacheEntryOptions());
        }

        return result;
    }

    public async Task<IReadOnlyCollection<Category>> GetCategories()
    {
        return (await _repository.GetCategories()).Select(o => new Category
        {
            Id = o.Id,
            Name = o.Name
        }).ToImmutableList();
    }

    public async Task<Post> Get(int id)
    {
        var key = $"publication_{id}";

        var result = _cache.Get(key) as Post;

        if (result == null)
        {
            result = Map(await _repository.Get(id));
            _cache.Set(key, result, GetMemoryCacheEntryOptions());
        }

        return await Task.FromResult(result);
    }
        
    public async Task IncreaseViewCount(int id) => await _repository.IncreaseViewCount(id);

    /// <summary>
    /// Store post to database
    /// </summary>
    /// <param name="metadata"></param>
    /// <param name="userId"></param>
    /// <param name="languageId"></param>
    /// <param name="playerCode"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    private async Task<Post> StoreNewPostInDatabase(
        Metadata metadata,
        int userId,
        CreatePostRequest request)
    {
        var languageCode = _languageAnalyzer.GetTextLanguage(metadata.Description);
        var languageId = await _languageService.GetLanguageId(languageCode) ?? Language.EnglishId;
        var player = EmbeddedPlayerFactory.CreatePlayer(request.Link);
        var playerCode = player != null ? await player.GetEmbeddedPlayerUrl(request.Link) : null;

        var image = metadata.Images.FirstOrDefault();

        var post = new DAL.Post
        {
            Title = metadata.Title,
            Description = X.Text.TextHelper.Substring(metadata?.Description, 4500, "..."),
            Link = metadata.Url,
            Image = string.IsNullOrWhiteSpace(image) || image.Length > 250 ? string.Empty : image,
            DateTime = DateTime.Now,
            UserId = userId,
            CategoryId = request.CategoryId,
            Header = request.Title,
            Comment = request.Comment,
            HeaderUa = request.TitleUa,
            CommentUa = request.CommentUa,
            LanguageId = languageId,
            EmbededPlayerCode = playerCode,
        };

        var savedPost = await _repository.Create(post);

        return Map(savedPost);
    }

    /// <summary>
    /// Map DAL object to BLL model
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    private static Post Map(DAL.Post p) =>
        new()
        {
            Id = p.Id,
            Description = p.Description,
            Image = p.Image,
            Title = p.Title,
            Url = new Uri(p.Link),
            Views = p.Views,
            DateTime = p.DateTime,
            EmbeddedPlayerCode = p.EmbededPlayerCode,
            CategoryId = p.CategoryId
        };

    public async Task<(Post post, Uri url)> Create(CreatePostRequest request, int userId)
    {
        var extractor = new Extractor();

        var metadata = await extractor.ExtractAsync(request.Link);

        var exist = await IsPostExist(metadata.Url);

        if (exist)
        {
            throw new DuplicateNameException("Publication with this URL already exist");
        }
        
        var post = await StoreNewPostInDatabase(metadata, userId, request);

        var shareUrl = _postUrlBuilder.Build(post.Id);
        var redirectUrl = _postUrlBuilder.BuildRedirectUrl(post.Id);

        //If we can embed main content into site page, so we can share this page.
        var url = string.IsNullOrEmpty(post.EmbeddedPlayerCode) ? redirectUrl : shareUrl;

        var services = await GetPostingService(request.CategoryId);
        var serviceUa = _factory.CreateTelegramService("@devdigest_ua");
        
        foreach (var service in services)
        {
            await service.Post(request.Title, request.Comment, url);
        }

        await serviceUa.Post(request.TitleUa, request.CommentUa, url);

        return (post, shareUrl);
    }
    
    private async Task<IReadOnlyCollection<ISocialNetworkPostingService>> GetPostingService(int categoryId)
    {
        var telegramChannels = await _socialRepository.GetTelegramChannels(categoryId);
        var facebookPages = await _socialRepository.GetFacebookPages(categoryId);
        var twitterAccounts = await _socialRepository.GetTwitterAccounts();
        var slackApplications = await _socialRepository.GetSlackApplications();

        var services = new List<ISocialNetworkPostingService>();
            
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
                await GetCategoryTags(categoryId)));
        }
            
        foreach (var slack in slackApplications)
        {
            services.Add(_factory.CreateSlackService(slack.WebHookUrl));
        }

        return services.ToImmutableList();
    }

    public async Task<IReadOnlyCollection<string>> GetCategoryTags(int categoryId)
    {
        var value = await _repository.GetCategoryTags(categoryId);

        if (string.IsNullOrWhiteSpace(value))
        {
            return ImmutableArray<string>.Empty;
        }

        return value.Split(' ').ToImmutableList();
    }

    public async Task<IReadOnlyCollection<Post>> Find(params string[] keywords)
    {
        var list = await _repository.Find(keywords);
        
        return list.Select(Map).ToImmutableList();
    }

    public async Task<bool> IsPostExist(string url)
    {
        var post = await _repository.Get(new Uri(url));
        
        return post != null;
    }

    private static MemoryCacheEntryOptions GetMemoryCacheEntryOptions() => new()
    {
        AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(1)
    };

    public async Task<IReadOnlyCollection<Post>> GetTop(int languageId = Language.EnglishId)
    {
        var list = await _repository.GetTop(languageId);
        
        return list.Select(Map).ToImmutableList();
    }
}