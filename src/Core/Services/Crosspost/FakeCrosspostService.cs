using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Core.Services.Crosspost
{
    public class FakeCrosspostService : ICrossPostService
    {
        private readonly ILogger _logger;

        public FakeCrosspostService(ILogger<FakeCrosspostService> logger) =>
            _logger = logger;

        public async Task Send(string message, Uri link, IReadOnlyCollection<string> tags)
        {
            _logger.LogInformation($"{message} {link}");
            
            Console.WriteLine($"{message} {link}");
        }
    }
}