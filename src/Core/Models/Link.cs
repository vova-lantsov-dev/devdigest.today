using System;

namespace Core.Models;

public record Link
{
    public Link(string title, Uri url)
    {
        Title = title;
        Url = url;
    }

    public Link(string title, string url)
        : this(title, new Uri(url))
    {
    }

    public string Title { get; init; }
    public Uri Url { get; init; }
}