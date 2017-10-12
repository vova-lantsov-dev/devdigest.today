using System;
using System.Linq;
using DAL;

namespace Core.Managers
{
    public class UserManager : ManagerBase
    {
        public UserManager(string connectionString)
            : base(connectionString)
        {
        }

        public User GetBySecretKey(Guid key)
        {
            var secret = key.ToString();
            return _database.User.FirstOrDefault( o => o.Key == secret);
        }
    }
}