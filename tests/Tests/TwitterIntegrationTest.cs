using System;
using System.Threading.Tasks;
using Core.Services.Posting;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Tests;

public class TwitterIntegrationTest
{
    [Fact(Skip = "On demand")]
    public async Task CheckSending()
    {
        var consumerKey = "xxx";
        var consumerSecret = "xxx";        
        var accessToken = "xxx";
        var accessSecret = "xxx";
        var name = "devdigest";

        var defaultTags = new[] { "#test", "#devdigest" };
        var logger = NullLogger<TwitterPostingService>.Instance;

        var postingService = new TwitterPostingService(
            consumerKey,
            consumerSecret,
            accessToken,
            accessSecret,
            name,
            defaultTags,
            logger);

        await postingService.Post("Test", "Test post", new Uri("http://example.com"));

        Assert.True(true);
    }
}
