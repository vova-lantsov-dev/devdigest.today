using System;

namespace Core.ViewModels
{
    public class VacancyViewModel
    {
        private readonly string _websiteUrl;

        public VacancyViewModel(DAL.Vacancy vacancy, string webSiteUrl)
        {
            _websiteUrl = webSiteUrl;
            Id = vacancy.Id;
            Title = vacancy.Title;
            Image = vacancy.Image;
            Description = vacancy.Description;
            Content = vacancy.Content;
            Url = string.IsNullOrWhiteSpace(vacancy.Url) ? null : new Uri(vacancy.Url);
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string ShareUrl => $"{_websiteUrl}vacancy/{Id}";
        public string Content { get; set; }
        public Uri Url { get; set; }
    }
}
