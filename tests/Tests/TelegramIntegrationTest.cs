using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Services.Posting;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Tests;

public class TelegramIntegrationTest
{
    [Fact]
    public async Task CheckFormatting()
    {

        var token = "";
        var name = "";
        var logger = new NullLogger<TelegramPostingService>();
        var postingService = new TelegramPostingService(token, name, logger);

        string message1 = @"Ecma International одобрила шестую версию спецификации языка C# официально известную как ECMA-334|Ecma International одобрила шестую версию спецификации языка C#  официально известную как ECMA-334";
        string message2 = @"Ecma International одобрила шестую версию спецификации языка C# официально известную как ECMA-334";
        Uri link = new Uri("https://devdigest.today/goto/1980");
        IReadOnlyCollection<string> tags = new[] { "#microsoft", "#ecma", "#ecma334"};
        
        await postingService.Send(message2, link, tags);
    }
}