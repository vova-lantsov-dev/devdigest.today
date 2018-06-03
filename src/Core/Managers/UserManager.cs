using System;
using System.Linq;
using DAL;

namespace Core.Managers
{
    public interface IUserManager
    {
        User GetBySecretKey(Guid key);
    }

    public class UserManager : IManager, IUserManager
    {
        private readonly DAL.DatabaseContext _database;

        public UserManager(DatabaseContext database)
        {
            _database = database;
        }

        public User GetBySecretKey(Guid key)
        {
            var secret = key.ToString();
            return _database.User.FirstOrDefault( o => o.Key == secret);
        }
    }
}