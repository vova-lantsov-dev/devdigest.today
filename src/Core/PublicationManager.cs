using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using X.PagedList;
using DAL;
using Microsoft.EntityFrameworkCore;

namespace Core
{
    public class PublicationManager : ManagerBase
    {

        private readonly IMemoryCache _cache;

        public PublicationManager(string connectionString, IMemoryCache cache)
            : base(connectionString)
        {
            _cache = cache;
        }

        public async Task<IPagedList<Publication>> GetPublications(int page = 1, int pageSize = 10)
        {
            var key = $"page_{page}_{pageSize}";

            var result = _cache.Get(key) as IPagedList<Publication>;

            if (result == null)
            {
                var skip = (page - 1) * pageSize;

                var items = _database.Publication.OrderByDescending(o => o.DateTime).Skip(skip).Take(pageSize).ToList();
                var totalItemsCount = await _database.Publication.CountAsync();

                result = new StaticPagedList<Publication>(items, page, pageSize, totalItemsCount);
                _cache.Set(key, result, GetMemoryCacheEntryOptions());
            }

            return result;
        }

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
            publication = _database.Publication.LastOrDefault(); //for some reasons SaveAsync work not properly and do not update publication Id

            return publication;
        }

        private static MemoryCacheEntryOptions GetMemoryCacheEntryOptions()
        {
            return new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(1)
            };
        }
    }
}