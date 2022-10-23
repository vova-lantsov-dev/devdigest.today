using System;
using System.Threading.Tasks;
using DAL;
using Microsoft.EntityFrameworkCore;

namespace Core.Repositories;

public interface IUserRepository
{
    Task<User> Get(Guid key);
}

public class UserRepository : RepositoryBase, IUserRepository
{
    public UserRepository(DatabaseContext databaseContext) 
        : base(databaseContext)
    {
    }

    public Task<User> Get(Guid key) =>
        DatabaseContext.Users.FirstOrDefaultAsync(o => o.Key == key.ToString());
}