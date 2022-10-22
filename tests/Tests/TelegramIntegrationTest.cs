using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Services.Posting;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Tests;

public class TelegramIntegrationTest
{
    [Fact(Skip = "On demand")]
    public async Task CheckFormatting()
    {
        var token = "";
        var name = "";
        var logger = new NullLogger<TelegramPostingService>();
        var postingService = new TelegramPostingService(token, name, logger);

        var message1 = @"Ecma International одобрила шестую версию спецификации языка C# официально известную как ECMA-334|Ecma International одобрила шестую версию спецификации языка C#  официально известную как ECMA-334";
        var message2 = @"Ecma International одобрила шестую версию спецификации языка C# официально известную как ECMA-334";
        var link = new Uri("https://devdigest.today/goto/1980");

        await postingService.Send(
            "Ecma International одобрила шестую версию спецификации языка C#",
            message1,
            link);
        
        await postingService.Send(
            "Ecma International одобрила шестую версию спецификации языка C#",
            message2,
            link);
    }
}