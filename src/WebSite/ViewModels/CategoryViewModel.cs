namespace WebSite.ViewModels
{
    public class CategoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CssClass => $"category-{Id}";
    }
}