using System.Linq;

namespace Core;

public static class MessageParser
{
    public static string Glue(string title, string body)
    {
        if (string.IsNullOrWhiteSpace(title) && string.IsNullOrWhiteSpace(body))
        {
            return string.Empty;            
        }

        if (!string.IsNullOrWhiteSpace(title))
        {
            var symbols = new[] { ".", "?", "!", "..." };
            var point = symbols.Any(title.EndsWith) ? string.Empty : ".";
            
            return $"{title.Trim()}{point} {body}".Trim();
        }

        return body ?? string.Empty;
    }
}