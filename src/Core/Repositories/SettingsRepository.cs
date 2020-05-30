using System.Threading.Tasks;
using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Core.Repositories
{
    public interface ISettingsRepository
    {
        Task<DAL.Language> GetLanguage(string code);
    }

    public class SettingsRepository : ISettingsRepository
    {
        private readonly DatabaseContext _database;
        private ILogger _logger;

        public SettingsRepository(DatabaseContext database, ILogger<SettingsRepository> logger)
        {
            _database = database;
            _logger = logger;
        }
        
        public async Task<DAL.Language> GetLanguage(string code) =>
            await _database.Language.FirstOrDefaultAsync(o => o.Code == code);
    }
}