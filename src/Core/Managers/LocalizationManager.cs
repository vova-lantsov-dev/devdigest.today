using System.Linq;
using Microsoft.Extensions.Caching.Memory;

namespace Core.Managers
{
    public class LocalizationManager : ManagerBase
    {
        public LocalizationManager(string connectionString, IMemoryCache cache = null) 
            : base(connectionString, cache)
        {
        }

        public int? GetLanguageId(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return null;
            }
            
            code = code.Trim().ToLower();

            var language = _database.Language.FirstOrDefault(o => o.Name == code);
            return language?.Id;
        }
    }
}