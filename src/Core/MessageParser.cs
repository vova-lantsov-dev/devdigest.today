using System;
using System.Collections.Generic;
using System.Linq;

namespace Core;

public static class MessageParser
{
    public static (string title, string body) Split(string message)
    {
        if (message.Contains('|'))
        {
            var separatorIndex = message.IndexOf("|", StringComparison.Ordinal);
            var title = message.Substring(0, separatorIndex);
            var body = message.Substring(separatorIndex + 1, message.Length - separatorIndex - 1);

            return (title, body);
        }

        return (message, string.Empty);
    }

    public static string Glue(string title, string body)
    {
        if (string.IsNullOrWhiteSpace(title) && string.IsNullOrWhiteSpace(body))
        {
            return string.Empty;            
        }

        if (!string.IsNullOrWhiteSpace(title))
        {
            var point = EndsWith(title.Trim(), new [] {".", "?", "!", "..."}) ? string.Empty : ".";
            var result = $"{title.Trim()}{point} {body}".Trim();
            
            return result;
        }

        return body ?? string.Empty;
    }

    private static bool EndsWith(string text, IEnumerable<string> symbols) => 
        symbols.Any(text.EndsWith);
}