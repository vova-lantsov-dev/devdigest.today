namespace Core.ViewModels
{
    public class TelegramViewModel
    {
        private readonly string _name;

        public TelegramViewModel(string name) => _name = name;

        public string Title { get; set; }
        public string Description { get; set; }
        public string Logo { get; set; }
        public string Link => $"https://t.me/{_name.Replace("@", "")}";
    }
}