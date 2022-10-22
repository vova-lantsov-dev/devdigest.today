using System.Collections.Generic;
using X.PagedList;

namespace WebSite.ViewModels;

public class HomePageViewModel
{
    public StaticPagedList<PostViewModel> Publications { get; set; }
    public IReadOnlyCollection<PostViewModel> TopPublications { get; set; }
}