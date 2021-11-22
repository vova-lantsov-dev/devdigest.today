using X.PagedList;

namespace WebSite.ViewModels;

public class VacancyListViewModel
{
    public IPagedList<VacancyViewModel> List { get; set; }
    public int? CategoryId { get; set; }
}