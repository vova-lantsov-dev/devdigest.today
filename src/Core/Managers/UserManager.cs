using System;
using System.Linq;
using DAL;

namespace Core.Managers
{
    public interface IUserManager
    {
        User GetBySecretKey(Guid key);
    }

    public class UserManager : ManagerBase, IUserManager
    {
        private readonly DAL.DatabaseContext _database;

        public UserManager(DatabaseContext database)
            : base()
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