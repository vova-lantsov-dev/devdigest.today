using System;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Core.Repositories;

public interface IPageRepository
{
    Task<Page> GetPage(string name);
}

public class PageRepository : RepositoryBase, IPageRepository
{
    private readonly ILogger _logger;

    public PageRepository(DatabaseContext databaseContext, ILogger<PostRepository> logger)
        : base(databaseContext) => _logger = logger;

    public async Task<Page> GetPage(string name)
    {
        try
        {
            name = name?.Trim().ToLower();

            return await DatabaseContext
                .Pages
                .Where(o => o.Name == name)
                .SingleOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            throw;
        }
    }
}