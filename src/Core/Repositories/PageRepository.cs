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

public class PageRepository : IPageRepository
{
    private readonly DatabaseContext _database;
    private readonly ILogger _logger;

    public PageRepository(DatabaseContext database, ILogger<PostRepository> logger)
    {
        _database = database;
        _logger = logger;
    }

    public async Task<Page> GetPage(string name)
    {
        name = name?.Trim().ToLower();

        return await _database
            .Pages
            .Where(o => o.Name == name)
            .SingleOrDefaultAsync();
    }

}