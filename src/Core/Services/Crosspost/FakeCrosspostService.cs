using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Logging;
using Serilog.Events;

namespace Core.Services.Crosspost
{
    public class FakeCrosspostService : ICrossPostService
    {
        private readonly ILogger _logger;

        public FakeCrosspostService(ILogger logger) => _logger = logger;

        public async Task Send(int categoryId, string comment, string link, IReadOnlyCollection<string> tags)
        {
            _logger.Write(LogEventLevel.Information, $"{comment} {link} {categoryId}");
        }
    }
}