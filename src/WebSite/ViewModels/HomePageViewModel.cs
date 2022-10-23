using X.PagedList;

namespace WebSite.ViewModels;

public class HomePageViewModel
{
    public StaticPagedList<PostViewModel> Publications { get; init; }
    public IReadOnlyCollection<PostViewModel> TopPublications { get; init; }
}