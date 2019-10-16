using System.Linq;
using System.Threading.Tasks;
using Core.Logging;
using Core.Repositories;
using DAL;
using Microsoft.Extensions.Caching.Memory;

namespace Core.Managers
{
    public interface ILocalizationManager
    {
        Task<int?> GetLanguageId(string code);
    }

    public class LocalizationManager : ILocalizationManager
    {
        private readonly ISettingsRepository _settingsRepository;
        private readonly ILogger _logger;

        public LocalizationManager(ISettingsRepository settingsRepository, ILogger logger)
        {
            _settingsRepository = settingsRepository;
            _logger = logger;
        }

        public async Task<int?> GetLanguageId(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return null;
            }

            code = code.Trim().ToLower();

            var language = await _settingsRepository.GetLanguage(code);
            return language?.Id;
        }
    }
}