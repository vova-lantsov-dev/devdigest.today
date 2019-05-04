using System.Threading.Tasks;
using Core.Logging;

namespace Core.Managers.Crosspost
{
    public class FakeCrosspostManager : ICrossPostManager
    {
        private readonly ILogger _logger;

        public FakeCrosspostManager(ILogger logger) => _logger = logger;

        public async Task<bool> Send(int categoryId, string comment, string link)
        {
            _logger.Write(LogLevel.Info, $"{comment} {link} {categoryId}");
            
            return await Task.FromResult(true);
        }
    }
}