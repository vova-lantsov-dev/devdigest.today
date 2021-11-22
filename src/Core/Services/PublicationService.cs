using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Repositories;
using DAL;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using X.PagedList;
using X.Web.MetaExtractor;

namespace Core.Services
{
    public interface IPublicationService
    {
        Task<IPagedList<Publication>> GetPublications(
            int? categoryId = null, 
            int page = 1,
            int pageSize = 10, 
            int languageId = Language.EnglishId);
        
        Task<IReadOnlyCollection<Publication>> GetTopPublications(int languageId = Language.EnglishId);
        
        Task<IReadOnlyCollection<Category>> GetCategories();
        
        Task<Publication> Get(int id);
        
        Task IncreaseViewCount(int id);
        
        Task<Publication> Get(Uri uri);
        
        Task<Publication> CreatePublication(
            Metadata metadata,
            int userId,
            int languageId,
            string playerCode, 
            int categoryId, 
            string comment);

        Task<IReadOnlyCollection<string>> GetCategoryTags(int categoryId);
        Task<IReadOnlyCollection<Publication>> FindPublications(params string[] keywords);
    }

    public class PublicationService : IPublicationService
    {
        private readonly ILogger _logger;
        private readonly IMemoryCache _cache;
        private readonly IPublicationRepository _repository;
        
        public PublicationService(
            IMemoryCache cache,
            IPublicationRepository repository,
            ILogger<PublicationService> logger)
        {
            _cache = cache;
            _logger = logger;
            _repository = repository;
        }

        public async Task<IPagedList<Publication>> GetPublications(
            int? categoryId = null, 
            int page = 1,
            int pageSize = 10, 
            int languageId = Language.EnglishId)
        {
            var key = $"page_{page}_{pageSize}_{categoryId}";

            var result = _cache.Get(key) as IPagedList<Publication>;

            if (result == null)
            {
                var items = await _repository.GetPublications(categoryId, languageId, page, pageSize);
                var totalItemsCount = await _repository.GetPublicationsCount(categoryId, languageId);

                result = new StaticPagedList<Publication>(items, page, pageSize, totalItemsCount);
                _cache.Set(key, result, GetMemoryCacheEntryOptions());
            }

            return result;
        }

        public async Task<IReadOnlyCollection<Category>> GetCategories() => await _repository.GetCategories();

        public async Task<Publication> Get(int id)
        {
            var key = $"publication_{id}";

            var result = _cache.Get(key) as Publication;

            if (result == null)
            {
                result = await _repository.GetPublication(id);
                _cache.Set(key, result, GetMemoryCacheEntryOptions());
            }

            return await Task.FromResult(result);
        }
        
        public async Task IncreaseViewCount(int id) => await _repository.IncreasePublicationViewCount(id);

        public async Task<Publication> Get(Uri uri) => await _repository.GetPublication(uri);

        public async Task<Publication> CreatePublication(
            Metadata metadata,
            int userId,
            int languageId,
            string playerCode, 
            int categoryId,
            string comment)
        {
            var image = metadata.Images.FirstOrDefault();
            
            var publication = new DAL.Publication
            {
                Title = metadata.Title,
                Description = X.Text.TextHelper.Substring(metadata?.Description, 4500, "..."),
                Link = metadata.Url,
                Image = string.IsNullOrWhiteSpace(image) || image.Length > 250 ? string.Empty : image,
                Type = "article",
                DateTime = DateTime.Now,
                UserId = userId,
                CategoryId = categoryId,
                Comment = comment,
                LanguageId = languageId,
                EmbededPlayerCode = playerCode
            };

            return await _repository.Save(publication);
        }

        public Task<IReadOnlyCollection<string>> GetCategoryTags(int categoryId)
        {
            return _repository.GetCategoryTags(categoryId);
        }

        public Task<IReadOnlyCollection<Publication>> FindPublications(params string[] keywords)
        {
            return _repository.FindPublications(keywords);
        }

        private static MemoryCacheEntryOptions GetMemoryCacheEntryOptions() => new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(1)
        };

        public Task<IReadOnlyCollection<Publication>> GetTopPublications(int languageId = Language.EnglishId)
        {
            return _repository.GetTopPublications(languageId);
        }
    }
}