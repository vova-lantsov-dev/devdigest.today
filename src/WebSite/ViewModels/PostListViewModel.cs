using System.Linq;
using X.PagedList;

namespace WebSite.ViewModels;

public class PostListViewModel
{
    public IPagedList<PostViewModel> List { get; set; }
        
    public int? CategoryId { get; set; }

    public string CategoryName => CategoryId.HasValue
        ? List.Select(o => o.Category.Name).FirstOrDefault()?.ToLower()
        : string.Empty;
}