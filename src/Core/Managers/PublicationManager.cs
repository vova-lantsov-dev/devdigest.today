using System;
using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Logging;
using X.PagedList;

namespace Core.Managers
{
    public interface IPublicationManager
    {
        Task<IPagedList<Publication>> GetPublications(
            int? categoryId = null, 
            int page = 1,
            int pageSize = 10, 
            int languageId = Core.Language.EnglishId);

        Task<IReadOnlyCollection<Category>> GetCategories();
        Task<Publication> Get(int id);
        Task<Publication> Save(Publication publication);
        Task IncreaseViewCount(int id);
        Publication Get(Uri uri);
    }

    public class PublicationManager : IManager, IPublicationManager
    {
        private readonly ILogger _logger;
        private readonly IMemoryCache _cache;
        private readonly DatabaseContext _database;

        public PublicationManager(IMemoryCache cache, DatabaseContext database, ILogger logger)
        {
            _cache = cache;
            _database = database;
            _logger = logger;
        }

        public async Task<IPagedList<Publication>> GetPublications(
            int? categoryId = null, 
            int page = 1,
            int pageSize = 10, 
            int languageId = Core.Language.EnglishId)
        {
            var key = $"page_{page}_{pageSize}_{categoryId}";

            var result = _cache.Get(key) as IPagedList<Publication>;

            if (result == null)
            {
                var skip = (page - 1) * pageSize;

                var items = _database
                    .Publication
                    .Where(o => o.CategoryId == categoryId || categoryId == null)
                    .Where(o => o.LanguageId == languageId)
                    .OrderByDescending(o => o.DateTime)
                    .Skip(skip)
                    .Take(pageSize).ToList();

                var totalItemsCount = await _database.Publication
                    .Where(o => o.CategoryId == categoryId || categoryId == null)
                    .Where(o => o.LanguageId == languageId)
                    .CountAsync();

                result = new StaticPagedList<Publication>(items, page, pageSize, totalItemsCount);
                _cache.Set(key, result, GetMemoryCacheEntryOptions());
            }

            return result;
        }
        
        public async Task<IReadOnlyCollection<Category>> GetCategories() => await _database.Category.ToListAsync();

        public async Task<Publication> Get(int id)
        {
            var key = $"publication_{id}";

            var result = _cache.Get(key) as Publication;

            if (result == null)
            {
                result = _database.Publication.SingleOrDefault(o => o.Id == id);
                _cache.Set(key, result, GetMemoryCacheEntryOptions());
            }

            return await Task.FromResult(result);
        }

        public async Task<Publication> Save(Publication publication)
        {
            _database.Add(publication);
            await _database.SaveChangesAsync();
            publication = _database.Publication.LastOrDefault();

            _logger.Write(LogLevel.Info, $"Publication `{publication.Title}`  was saved. Id: {publication.Id}");

            return publication;
        }

        public async Task IncreaseViewCount(int id)
        {
            var publication = _database.Publication.SingleOrDefault(o => o.Id == id);

            if (publication != null)
            {
                publication.Views++;
                await _database.SaveChangesAsync();
            }
        }

        public Publication Get(Uri uri)
        {
            return _database.Publication.SingleOrDefault(o => o.Link.ToLower() == uri.ToString().ToLower());
        }
        
        private static MemoryCacheEntryOptions GetMemoryCacheEntryOptions() => new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(1)
        };
    }
}