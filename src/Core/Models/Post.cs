using System;

namespace Core.Models;

public record Post
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Image { get; set; }
    public DateTime DateTime { get; set; }
    public string EmbeddedPlayerCode { get; set; }
    public Uri Url { get; set; }
    public int Views { get; set; }
}