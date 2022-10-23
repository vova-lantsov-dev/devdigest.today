using X.PagedList;

namespace WebSite.ViewModels;

public record PostListViewModel
{
    public IPagedList<PostViewModel> List { get; init; }
        
    public int? CategoryId { get; init; }

    public string CategoryName => CategoryId.HasValue
        ? List.Select(o => o.Category.Name).FirstOrDefault()?.ToLower()
        : string.Empty;
}