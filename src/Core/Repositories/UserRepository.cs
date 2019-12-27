using System;
using System.Linq;
using System.Threading.Tasks;
using Core.Logging;
using DAL;
using Microsoft.EntityFrameworkCore;

namespace Core.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUserBySecretKey(Guid key);
    }

    public class UserRepository : IUserRepository
    {
        private readonly DatabaseContext _database;

        public UserRepository(DatabaseContext database, ILogger logger) => _database = database;

        public async Task<User> GetUserBySecretKey(Guid key) =>
            await _database.User.FirstOrDefaultAsync(o => o.Key == key.ToString());
    }
}