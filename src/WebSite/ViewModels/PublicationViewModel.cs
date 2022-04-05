using DAL;
using X.Text;

namespace WebSite.ViewModels;

public record PublicationViewModel
{
    public int Id { get; set; }
    public string Type { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string Description { get; set; }
    public string Image { get; set; }
    public DateTime DateTime { get; set; }
    public string EmbeddedPlayerCode { get; set; }
    public Uri Url { get; set; }
    public Uri ShareUrl => new($"{_websiteUrl}post/{Id}");
    
    /// <summary>
    /// Url from which website redirect to original url 
    /// </summary>
    public Uri RedirectUrl => new($"{_websiteUrl}goto/{Id}");
    public string Keywords => TextHelper.GetKeywords(Description, 10);
    public CategoryViewModel Category { get; set; }
    public int ViewsCount { get; set; }
        
    private readonly Uri _websiteUrl;

    public PublicationViewModel(
        Publication publication, 
        Uri websiteUrl, 
        IReadOnlyCollection<Category> categories = null)
    {
        _websiteUrl = websiteUrl;

        Id = publication.Id;
        Title = publication.Title;
        Description = publication.Description;
        Image = publication.Image;
        Url = string.IsNullOrWhiteSpace(publication.Link) ? null : new Uri(publication.Link);
        DateTime = publication.DateTime;
        Type = publication.Type;
        Content = publication.Content;
        EmbeddedPlayerCode = publication.EmbededPlayerCode;
        ViewsCount = publication.Views;

        if (categories != null && categories.Any())
        {
            var categoryName = categories
                .Where(o => o.Id == publication.CategoryId)
                .Select(o => o.Name)
                .FirstOrDefault();

            Category = new CategoryViewModel
            {
                Id = publication.CategoryId,
                Name = categoryName
            };
        }

    }
        
    public override string ToString() => $"{Title}\r\n{GetShortDescription()}\r\n{Url}";

    public string GetShortDescription() => TextHelper.Substring(Description, 256, "...");
}