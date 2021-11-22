using System.Collections.Generic;
using X.PagedList;

namespace WebSite.ViewModels;

public class HomePageViewModel
{
    public StaticPagedList<PublicationViewModel> Publications { get; set; }
    public IReadOnlyCollection<PublicationViewModel> TopPublications { get; set; }
}