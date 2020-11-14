using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Core.Repositories
{
    public interface IPublicationRepository
    {
        Task<IReadOnlyCollection<Publication>> GetPublications(int? categoryId, int languageId, int page, int pageSize);
        Task<int> GetPublicationsCount(int? categoryId, int languageId);
        Task<IReadOnlyCollection<Category>> GetCategories();
        Task<Publication> GetPublication(int id);
        Task<Publication> Save(Publication publication);
        Task IncreasePublicationViewCount(int id);
        Task<Publication> GetPublication(Uri uri);
        Task<IReadOnlyCollection<string>> GetCategoryTags(int categoryId);
        Task<IReadOnlyCollection<Publication>> GetTopPublications(int languageId);
        Task<IReadOnlyCollection<Publication>> FindPublications(params string[] keywords);
    }

    public class PublicationRepository : IPublicationRepository
    {
        private readonly DatabaseContext _database;
        private readonly ILogger _logger;

        public PublicationRepository(DatabaseContext database, ILogger<PublicationRepository> logger)
        {
            _database = database;
            _logger = logger;
        }

        public async Task<IReadOnlyCollection<Publication>> GetPublications(int? categoryId, int languageId, int page, int pageSize)
        {
            var skip = (page - 1) * pageSize;

            return await _database
                .Publications
                .Where(o => o.CategoryId == categoryId || categoryId == null)
                .Where(o => o.LanguageId == languageId)
                .OrderByDescending(o => o.DateTime)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();
        }

        public Task<int> GetPublicationsCount(int? categoryId, int languageId) =>
            _database.Publications
                .Where(o => o.CategoryId == categoryId || categoryId == null)
                .Where(o => o.LanguageId == languageId)
                .CountAsync();

        public async Task<IReadOnlyCollection<Category>> GetCategories() =>
            await _database.Categories.ToListAsync();

        public Task<Publication> GetPublication(int id)
        {
            return _database.Publications.SingleOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Publication> Save(Publication publication)
        {
            _database.Add(publication);
            
            await _database.SaveChangesAsync();

            publication = await _database.Publications.OrderBy(o => o.DateTime).LastOrDefaultAsync();
            
            _logger.LogInformation($"Publication `{publication.Title}`  was saved. Id: {publication.Id}");

            return publication;
        }

        public async Task IncreasePublicationViewCount(int id)
        {
            var publication = _database.Publications.SingleOrDefault(o => o.Id == id);

            if (publication != null)
            {
                publication.Views++;
                await _database.SaveChangesAsync();
            }
        }

        public Task<Publication> GetPublication(Uri uri) =>
            _database.Publications.SingleOrDefaultAsync(o => o.Link.ToLower() == uri.ToString().ToLower());

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

        public async Task<IReadOnlyCollection<Publication>> GetTopPublications(int languageId)
        {
           var publications = await _database.Publications
                .Where(p => p.LanguageId == languageId)
                .Where(p => p.DateTime > DateTime.Now.AddDays(-30))
                .ToListAsync();

           return publications
               .GroupBy(p => p.CategoryId)
               .Select(g => g.OrderByDescending(o => o.DateTime).FirstOrDefault())
               .ToImmutableList();
        }

        public async Task<IReadOnlyCollection<Publication>> FindPublications(params string[] keywords)
        {
            var result = new List<Publication>();
            
            foreach (var keyword in keywords)
            {
                var items = await _database.Publications
                    .Where(p =>
                        EF.Functions.Like(p.Title, $"%{keyword}%") ||
                        EF.Functions.Like(p.Description, $"%{keyword}%") ||
                        EF.Functions.Like(p.Content, $"%{keyword}%") ||
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
}