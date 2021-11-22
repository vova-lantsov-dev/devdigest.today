using System;
using System.Threading.Tasks;
using Core.Repositories;
using DAL;
using Microsoft.Extensions.Logging;

namespace Core.Services;

public interface IUserService
{
    Task<User> GetBySecretKey(Guid key);
}

public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly ILogger _logger;
        
    public UserService(IUserRepository repository, ILogger<UserService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<User> GetBySecretKey(Guid key) => await _repository.GetUserBySecretKey(key);
}