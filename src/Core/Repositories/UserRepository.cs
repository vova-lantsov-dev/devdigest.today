using System;
using System.Threading.Tasks;
using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Core.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUserBySecretKey(Guid key);
    }

    public class UserRepository : IUserRepository
    {
        private readonly DatabaseContext _database;

        public UserRepository(DatabaseContext database, ILogger<UserRepository> logger) =>
            _database = database;

        public async Task<User> GetUserBySecretKey(Guid key) =>
            await _database.User.FirstOrDefaultAsync(o => o.Key == key.ToString());
    }
}