using System;
using System.Linq;
using Core.Logging;
using DAL;

namespace Core.Managers
{
    public interface IUserManager
    {
        User GetBySecretKey(Guid key);
    }

    public class UserManager : IManager, IUserManager
    {
        private readonly ILogger _logger;
        private readonly DatabaseContext _database;

        public UserManager(DatabaseContext database, ILogger logger)
        {
            _database = database;
            _logger = logger;
        }

        public User GetBySecretKey(Guid key) => _database.User.FirstOrDefault( o => o.Key == key.ToString());
    }
}