using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Logging;
using Core.Repositories;
using DAL;
using Microsoft.Extensions.Caching.Memory;
using X.PagedList;

namespace Core.Services
{
    public interface IVacancyService
    {
        Task<IPagedList<Vacancy>> GetVacancies(int page = 1, int pageSize = 10);
        Task<IReadOnlyCollection<Vacancy>> GetHotVacancies();
        Task<Vacancy> Get(int id);
        Task IncreaseViewCount(int id);
        string GetVacancyImage();
    }

    public class VacancyService : IVacancyService
    {
        private readonly ILogger _logger;
        private readonly IMemoryCache _cache;
        private readonly IVacancyRepository _repository;
        private readonly Settings _settings;

        public VacancyService(IMemoryCache cache, ILogger logger, IVacancyRepository repository, Settings settings)
        {
            _cache = cache;
            _logger = logger;
            _repository = repository;
            _settings = settings;
        }

        public async Task<IPagedList<Vacancy>> GetVacancies(int page = 1, int pageSize = 10)
        {
            var key = $"vacancy{page}_{pageSize}";

            var result = _cache.Get(key) as IPagedList<Vacancy>;

            if (result == null)
            {
                var items =  await  _repository.GetVacancies(page, pageSize);
                var totalItemsCount = await _repository.GetVacanciesCount();
                
                result = new StaticPagedList<Vacancy>(items, page, pageSize, totalItemsCount);
                _cache.Set(key, result, GetMemoryCacheEntryOptions());
            }

            return result;
        }

        public async Task<IReadOnlyCollection<Vacancy>> GetHotVacancies()
        {
            var key = $"hot_vacancies";
            const int size = 5;

            var result = _cache.Get(key) as IReadOnlyCollection<Vacancy>;

            if (result == null)
            {
                result = await _repository.GetHotVacancies(size);

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
                result = await _repository.GetVacancy(id);
                _cache.Set(key, result, GetMemoryCacheEntryOptions());
            }

            return result;
        }

        public async Task IncreaseViewCount(int id) => await _repository.IncreaseVacancyViewCount(id);

        public string GetVacancyImage()
        {
            var path = Path.Combine(_settings.WebRootPath, "images/vacancy");
            var file = Directory.GetFiles(path).OrderBy(o => Guid.NewGuid()).Select(Path.GetFileName).FirstOrDefault();
            
            return $"{_settings.WebSiteUrl}images/vacancy/{file}";
        }

        private static MemoryCacheEntryOptions GetMemoryCacheEntryOptions() => new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(1)
        };
    }
}
