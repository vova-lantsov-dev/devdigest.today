using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Logging;
using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using X.PagedList;

namespace Core.Managers
{
    public interface IVacancyManager
    {
        Task<IPagedList<Vacancy>> GetVacancies(int page = 1, int pageSize = 10);
        Task<IReadOnlyCollection<Vacancy>> GetHotVacancies();
        Task<Vacancy> Get(int id);
        Task<Vacancy> Save(Vacancy vacancy);
        Task IncreaseViewCount(int id);
    }

    public class VacancyManager : IManager, IVacancyManager
    {
        private readonly ILogger _logger;
        private readonly IMemoryCache _cache;
        private readonly DatabaseContext _database;

        public VacancyManager(IMemoryCache cache, DatabaseContext database, ILogger logger)
        {
            _cache = cache;
            _database = database;
            _logger = logger;
        }

        public async Task<IPagedList<Vacancy>> GetVacancies(int page = 1, int pageSize = 10)
        {
            var key = $"vacancy{page}_{pageSize}";

            var result = _cache.Get(key) as IPagedList<Vacancy>;

            if (result == null)
            {
                var skip = (page - 1) * pageSize;

                var items = _database
                    .Vacancy
                    .Include(o => o.Category)
                    .Where(o => o.Active && o.LanguageId == Language.EnglishId)
                    .OrderByDescending(o => o.Id)
                    .Skip(skip)
                    .Take(pageSize).ToList();

                var totalItemsCount = await _database.Vacancy.Where(o => o.Active).CountAsync();

                result = new StaticPagedList<Vacancy>(items, page, pageSize, totalItemsCount);
                _cache.Set(key, result, GetMemoryCacheEntryOptions());
            }

            return result;
        }

        public async Task<IReadOnlyCollection<Vacancy>> GetHotVacancies()
        {
            var key = $"hot_vacancies";
            var size = 5;

            var result = _cache.Get(key) as IReadOnlyCollection<Vacancy>;

            if (result == null)
            {
                result = await _database
                    .Vacancy
                    .Include(o => o.Category)
                    .Where(o => o.Active && o.LanguageId == Language.EnglishId)
                    .OrderByDescending(o => o.Id)
                    .Take(size)
                    .ToListAsync();

                _cache.Set(key, result, GetMemoryCacheEntryOptions());
            }

            return result;
        }

        public async Task<Vacancy> Get(int id)
        {
            var key = $"vacancy_{id}";

            var result = _cache.Get(key) as Vacancy;

            if (result == null)
            {
                result = await _database.Vacancy.SingleOrDefaultAsync(o => o.Id == id);
                _cache.Set(key, result, GetMemoryCacheEntryOptions());
            }

            return result;
        }

        public async Task<Vacancy> Save(Vacancy vacancy)
        {
            _database.Add(vacancy);
            await _database.SaveChangesAsync();
            vacancy = _database.Vacancy.LastOrDefault();

            return vacancy;
        }

        public async Task IncreaseViewCount(int id)
        {
            var vacancy = _database.Vacancy.SingleOrDefault(o => o.Id == id);
            
            if (vacancy != null)
            {
                vacancy.Views++;
                await _database.SaveChangesAsync();
            }
        }
        
        private static MemoryCacheEntryOptions GetMemoryCacheEntryOptions() => new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(1)
        };
    }
}
