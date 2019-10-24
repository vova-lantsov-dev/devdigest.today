using System.Threading.Tasks;
using Core.Logging;
using Serilog.Events;

namespace Core.Managers.Crosspost
{
    public class FakeCrosspostManager : ICrossPostManager
    {
        private readonly ILogger _logger;

        public FakeCrosspostManager(ILogger logger) => _logger = logger;

        public async Task Send(int categoryId, string comment, string link)
        {
            _logger.Write(LogEventLevel.Information, $"{comment} {link} {categoryId}");
        }
    }
}