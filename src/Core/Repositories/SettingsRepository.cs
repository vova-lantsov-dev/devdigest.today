using System.Threading.Tasks;
using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Core.Repositories;

public interface ISettingsRepository
{
    Task<DAL.Language> GetLanguage(string code);
}

public class SettingsRepository : RepositoryBase, ISettingsRepository
{
    private ILogger _logger;

    public SettingsRepository(DatabaseContext databaseContext, ILogger<SettingsRepository> logger) 
        : base(databaseContext)
    {
        _logger = logger;
    }
        
    public async Task<DAL.Language> GetLanguage(string code) =>
        await DatabaseContext.Languages.FirstOrDefaultAsync(o => o.Code == code);
}