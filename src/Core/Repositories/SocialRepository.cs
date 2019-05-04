using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Logging;
using DAL;
using Microsoft.EntityFrameworkCore;

namespace Core.Repositories
{
    public interface ISocialRepository
    {
        Task<IReadOnlyCollection<FacebookPage>> GetFacebookPages(int categoryId);
        Task<IReadOnlyCollection<FacebookPage>> GetFacebookPages();
        Task<IReadOnlyCollection<Channel>> GetTelegramChannels(int categoryId);
        Task<IReadOnlyCollection<Channel>> GetTelegramChannels();
    }

    public class SocialRepository : ISocialRepository
    {
        private readonly DatabaseContext _database;
        private ILogger _logger;

        public SocialRepository(DatabaseContext database, ILogger logger)
        {
            _database = database;
            _logger = logger;
        }

        public async Task<IReadOnlyCollection<FacebookPage>> GetFacebookPages(int categoryId) =>
            await _database.FacebookPage.Where(o => o.CategoryId == categoryId).ToListAsync();

        public async Task<IReadOnlyCollection<Channel>> GetTelegramChannels(int categoryId) =>
            await _database.Channel.Where(o => o.CategoryId == categoryId).ToListAsync();

        public async Task<IReadOnlyCollection<Channel>> GetTelegramChannels() =>
            await _database.Channel.ToListAsync();

        public async Task<IReadOnlyCollection<FacebookPage>> GetFacebookPages() =>
            await _database.FacebookPage.ToListAsync();
    }
}