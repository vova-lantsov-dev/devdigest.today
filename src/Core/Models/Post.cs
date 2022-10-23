using System;

namespace Core.Models;

public record Post
{
    public Post()
    {
    }
    
    public int Id { get; init; }
    public string Title { get; init; }
    public string Description { get; init; }
    public string Image { get; init; }
    public DateTime DateTime { get; init; }
    public string EmbeddedPlayerCode { get; init; }
    public Uri Url { get; init; }
    public int Views { get; init; }
    public int CategoryId { get; init; }
}