using System.Collections.Generic;
using System.Linq;
using DAL;

namespace Core
{
    public interface IRepository
    {
        IReadOnlyCollection<FacebookPage> GetFacebookPages(int categoryId);
        IReadOnlyCollection<Channel> GetTelegramChannels(int categoryId);
        IReadOnlyCollection<Channel> GetTelegramChannels();
    }
    
    public class Repository : IRepository 
    {
        private readonly DatabaseContext _database;

        public Repository(DatabaseContext database)
        {
            _database = database;
        }

        public IReadOnlyCollection<FacebookPage> GetFacebookPages(int categoryId)
        {
            return _database.FacebookPage.Where(o => o.CategoryId == categoryId).ToList();
        }

        public IReadOnlyCollection<Channel> GetTelegramChannels(int categoryId)
        {
            return _database.Channel.Where(o => o.CategoryId == categoryId).ToList();
        }

        public IReadOnlyCollection<Channel> GetTelegramChannels()
        {
            return _database.Channel.ToList();
        }
    }
}