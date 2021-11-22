using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Core.Services.Posting
{
    public class FakePostingService : IPostingService
    {
        private readonly ILogger _logger;

        public FakePostingService(ILogger<FakePostingService> logger) =>
            _logger = logger;

        public Task Send(string message, Uri link, IReadOnlyCollection<string> tags)
        {
            _logger.LogInformation($"{message} {link}");
            
            Console.WriteLine($"{message} {link}");

            return Task.CompletedTask;
        }
    }
}