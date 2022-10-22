using Core.Repositories;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Core.Services;

public interface ILanguageService
{
    Task<int?> GetLanguageId(string code);
}

public class LanguageService : ILanguageService
{
    private readonly ISettingsRepository _settingsRepository;
    private readonly ILogger _logger;

    public LanguageService(ISettingsRepository settingsRepository, ILogger<LanguageService> logger)
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