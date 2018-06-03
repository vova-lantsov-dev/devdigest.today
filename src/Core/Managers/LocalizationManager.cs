using System.Linq;
using DAL;
using Microsoft.Extensions.Caching.Memory;

namespace Core.Managers
{
    public interface ILocalizationManager
    {
        int? GetLanguageId(string code);
    }

    public class LocalizationManager : ManagerBase, ILocalizationManager
    {
        private readonly DatabaseContext _database;

        public LocalizationManager(DAL.DatabaseContext database, IMemoryCache cache = null) 
            : base(cache)
        {
            this._database = database;
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