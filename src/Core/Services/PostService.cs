using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Core.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using X.PagedList;
using X.Web.MetaExtractor;

namespace Core.Services;

public interface IPostService
{
    Task<IPagedList<DAL.Post>> GetPublications(
        int? categoryId = null, 
        int page = 1,
        int pageSize = 10, 
        int languageId = Language.EnglishId);
        
    Task<IReadOnlyCollection<DAL.Post>> GetTop(int languageId = Language.EnglishId);
        
    Task<IReadOnlyCollection<Category>> GetCategories();
        
    Task<DAL.Post> Get(int id);

    Task IncreaseViewCount(int id);

    Task<Post> Create(Metadata metadata,
        int userId,
        int languageId,
        string playerCode,
        int categoryId,
        string title,
        string comment,
        string titleUa,
        string commentUa);

    Task<IReadOnlyCollection<string>> GetCategoryTags(int categoryId);
    
    Task<IReadOnlyCollection<DAL.Post>> Find(params string[] keywords);
    
    Task<bool> IsPostExist(string url);
}

public class PostService : IPostService
{
    private readonly ILogger _logger;
    private readonly IMemoryCache _cache;
    private readonly IPostRepository _repository;
        
    public PostService(
        IMemoryCache cache,
        IPostRepository repository,
        ILogger<PostService> logger)
    {
        _cache = cache;
        _logger = logger;
        _repository = repository;
    }

    public async Task<IPagedList<DAL.Post>> GetPublications(
        int? categoryId = null, 
        int page = 1,
        int pageSize = 10, 
        int languageId = Language.EnglishId)
    {
        var key = $"page_{page}_{pageSize}_{categoryId}";

        var result = _cache.Get(key) as IPagedList<DAL.Post>;

        if (result == null)
        {
            var items = await _repository.GetList(categoryId, languageId, page, pageSize);
            var totalItemsCount = await _repository.GetCount(categoryId, languageId);

            result = new StaticPagedList<DAL.Post>(items, page, pageSize, totalItemsCount);
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

    public async Task<DAL.Post> Get(int id)
    {
        var key = $"publication_{id}";

        var result = _cache.Get(key) as DAL.Post;

        if (result == null)
        {
            result = await _repository.Get(id);
            _cache.Set(key, result, GetMemoryCacheEntryOptions());
        }

        return await Task.FromResult(result);
    }
        
    public async Task IncreaseViewCount(int id) => await _repository.IncreaseViewCount(id);

    public async Task<DAL.Post> Get(Uri uri) => await _repository.Get(uri);

    public async Task<Post> Create(Metadata metadata,
        int userId,
        int languageId,
        string playerCode,
        int categoryId,
        string title,
        string comment,
        string titleUa,
        string commentUa)
    {
        var image = metadata.Images.FirstOrDefault();

        var post = new DAL.Post
        {
            Title = metadata.Title,
            Description = X.Text.TextHelper.Substring(metadata?.Description, 4500, "..."),
            Link = metadata.Url,
            Image = string.IsNullOrWhiteSpace(image) || image.Length > 250 ? string.Empty : image,
            DateTime = DateTime.Now,
            UserId = userId,
            CategoryId = categoryId,
            Header = title,
            Comment = comment,
            HeaderUa = titleUa,
            CommentUa = commentUa,
            LanguageId = languageId,
            EmbededPlayerCode = playerCode,
        };

        var savedPost = await _repository.Create(post);

        return new Post
        {
            Id = savedPost.Id,
            Description = savedPost.Description,
            Image = savedPost.Image,
            Title = savedPost.Title,
            Url = new Uri(savedPost.Link),
            Views = savedPost.Views,
            DateTime = savedPost.DateTime,
            EmbeddedPlayerCode = savedPost.EmbededPlayerCode
        };
    }

    public Task<IReadOnlyCollection<string>> GetCategoryTags(int categoryId)
    {
        return _repository.GetCategoryTags(categoryId);
    }

    public Task<IReadOnlyCollection<DAL.Post>> Find(params string[] keywords)
    {
        return _repository.Find(keywords);
    }

    public async Task<bool> IsPostExist(string url)
    {
        var post = await _repository.Get(new Uri(url));
        return post != null;
    }

    private static MemoryCacheEntryOptions GetMemoryCacheEntryOptions() => new MemoryCacheEntryOptions
    {
        AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(1)
    };

    public Task<IReadOnlyCollection<DAL.Post>> GetTop(int languageId = Language.EnglishId)
    {
        return _repository.GetTop(languageId);
    }
}