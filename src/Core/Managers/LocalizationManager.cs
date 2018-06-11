using System.Linq;
using Core.Logging;
using DAL;
using Microsoft.Extensions.Caching.Memory;

namespace Core.Managers
{
    public interface ILocalizationManager
    {
        int? GetLanguageId(string code);
    }

    public class LocalizationManager : IManager, ILocalizationManager
    {
        private readonly ILogger _logger;
        private readonly DatabaseContext _database;

        public LocalizationManager(DAL.DatabaseContext database, ILogger logger)
        {
            _database = database;
            _logger = logger;
        }

        public int? GetLanguageId(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return null;
            }
            
            code = code.Trim().ToLower();

            var language = _database.Language.FirstOrDefault(o => o.Code == code);
            return language?.Id;
        }
    }
}