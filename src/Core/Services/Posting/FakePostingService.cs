using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Core.Services.Posting;

public class FakePostingService : SocialNetworkPostingService
{
    private readonly ILogger _logger;

    public FakePostingService(ILogger<FakePostingService> logger) : base(logger) =>
        _logger = logger;

    protected override Task PostImplementation(string title, string body, Uri link)
    {
        var message = MergeMessage(title, body);
        
        _logger.LogInformation($"{message} {link}");
            
        Console.WriteLine($"{message} {link}");

        return Task.CompletedTask;
    }
}