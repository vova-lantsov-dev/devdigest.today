using Core.Logging;
using Core.Repositories;
using System.Threading.Tasks;

namespace Core.Services
{
    public interface ILocalizationService
    {
        Task<int?> GetLanguageId(string code);
    }

    public class LocalizationService : ILocalizationService
    {
        private readonly ISettingsRepository _settingsRepository;
        private readonly ILogger _logger;

        public LocalizationService(ISettingsRepository settingsRepository, ILogger logger)
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