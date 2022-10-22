using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Core.Repositories;

public interface IPostRepository
{
    Task<IReadOnlyCollection<Post>> GetList(int? categoryId, int languageId, int page, int pageSize);
    Task<int> GetCount(int? categoryId, int languageId);
    Task<IReadOnlyCollection<Category>> GetCategories();
    Task<Post> Get(int id);
    Task<Post> Create(Post post);
    Task IncreaseViewCount(int postId);
    Task<Post> Get(Uri uri);
    Task<IReadOnlyCollection<string>> GetCategoryTags(int categoryId);
    Task<IReadOnlyCollection<Post>> GetTop(int languageId);
    Task<IReadOnlyCollection<Post>> Find(params string[] keywords);
}

public class PostRepository : IPostRepository
{
    private readonly DatabaseContext _database;
    private readonly ILogger _logger;

    public PostRepository(DatabaseContext database, ILogger<PostRepository> logger)
    {
        _database = database;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<Post>> GetList(int? categoryId, int languageId, int page, int pageSize)
    {
        var skip = (page - 1) * pageSize;

        return await _database
            .Posts
            .Where(o => o.CategoryId == categoryId || categoryId == null)
            .Where(o => o.LanguageId == languageId)
            .OrderByDescending(o => o.DateTime)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync();
    }

    public Task<int> GetCount(int? categoryId, int languageId) =>
        _database.Posts
            .Where(o => o.CategoryId == categoryId || categoryId == null)
            .Where(o => o.LanguageId == languageId)
            .CountAsync();

    public async Task<IReadOnlyCollection<Category>> GetCategories() =>
        await _database.Categories.ToListAsync();

    public Task<Post> Get(int id)
    {
        return _database.Posts.SingleOrDefaultAsync(o => o.Id == id);
    }

    public async Task<Post> Create(Post post)
    {
        _database.Add(post);
            
        await _database.SaveChangesAsync();

        post = await _database.Posts.OrderBy(o => o.Id).LastOrDefaultAsync();
            
        _logger.LogInformation($"Post `{post?.Title}`  was saved. Id: {post?.Id}");

        return post;
    }

    public async Task IncreaseViewCount(int postId)
    {
        var post = _database.Posts.SingleOrDefault(o => o.Id == postId);

        if (post != null)
        {
            post.Views++;
            await _database.SaveChangesAsync();
        }
    }

    public Task<Post> Get(Uri uri) =>
        _database.Posts.SingleOrDefaultAsync(o => o.Link.ToLower() == uri.ToString().ToLower());

    public async Task<IReadOnlyCollection<string>> GetCategoryTags(int categoryId)
    {
        var value = await _database.Categories
            .Where(o => o.Id == categoryId)
            .Select(o => o.Tags)
            .SingleOrDefaultAsync();

        if (string.IsNullOrWhiteSpace(value))
            return ImmutableArray<string>.Empty;

        return value.Split(' ').ToImmutableList();
    }

    public async Task<IReadOnlyCollection<Post>> GetTop(int languageId)
    {
        var posts = await _database.Posts
            .Where(p => p.LanguageId == languageId)
            .Where(p => p.DateTime > DateTime.Now.AddDays(-30))
            .ToListAsync();

        return posts
            .GroupBy(p => p.CategoryId)
            .Select(g => g.OrderByDescending(o => o.DateTime).FirstOrDefault())
            .ToImmutableList();
    }

    public async Task<IReadOnlyCollection<Post>> Find(params string[] keywords)
    {
        var result = new List<Post>();
            
        foreach (var keyword in keywords)
        {
            var items = await _database.Posts
                .Where(p =>
                    EF.Functions.Like(p.Title, $"%{keyword}%") ||
                    EF.Functions.Like(p.Description, $"%{keyword}%") ||
                    EF.Functions.Like(p.Comment, $"%{keyword}%"))
                .ToListAsync();
                
            if (items.Any())
            {
                result.AddRange(items);
            }
        }

        return result
            .Distinct()
            .OrderByDescending(o => o.DateTime)
            .ToImmutableList();

    }
}