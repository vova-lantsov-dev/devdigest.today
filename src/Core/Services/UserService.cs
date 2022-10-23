using System;
using System.Threading.Tasks;
using Core.Repositories;
using Microsoft.Extensions.Logging;

namespace Core.Services;

public interface IUserService
{
    /// <summary>
    /// Return user ID if user key exist
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    Task<int?> GetUserId(Guid key);
}

public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly ILogger _logger;

    public UserService(IUserRepository repository, ILogger<UserService> logger)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<int?> GetUserId(Guid key)
    {
        try
        {
            var user = await _repository.Get(key);

            return user?.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            
            return null;
        }
    }
}