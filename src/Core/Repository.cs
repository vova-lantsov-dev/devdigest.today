using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using X.PagedList;

namespace Core
{
    public interface IRepository
    {
        Task<IReadOnlyCollection<FacebookPage>> GetFacebookPages(int categoryId);
        Task<IReadOnlyCollection<Channel>> GetTelegramChannels(int categoryId);
        Task<IReadOnlyCollection<Channel>> GetTelegramChannels();
        Task<IReadOnlyCollection<FacebookPage>> GetFacebookPages();
    }
    
    public class Repository : IRepository 
    {
        private readonly DatabaseContext _database;

        public Repository(DatabaseContext database) => _database = database;

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