using System;

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
            return $"{title.Trim()} {body}".Trim();
        }

        return body ?? string.Empty;
    }
}