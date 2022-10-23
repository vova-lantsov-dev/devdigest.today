using X.PagedList;

namespace WebSite.ViewModels;

public record VacancyListViewModel
{
    public IPagedList<VacancyViewModel> List { get; init; }
    public int? CategoryId { get; init; }
}