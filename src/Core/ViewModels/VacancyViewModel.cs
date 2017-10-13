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
            Contact = vacancy.Contact;
            Active = vacancy.Active;
            Date = vacancy.Date;
            Url = string.IsNullOrWhiteSpace(vacancy.Url) ? null : new Uri(vacancy.Url);
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string ShareUrl => $"{_websiteUrl}vacancy/{Id}";
        public string Content { get; set; }
        public string Contact { get; set; }
        public bool Active { get; set; }
        public Uri Url { get; set; }

        public ContactType ContactType
        {
            get
            {
                var contact = Contact.Trim().ToLower();

                if (contact.Contains("facbook.com"))
                    return ContactType.Facebook;

                if (contact.Contains("linkedin.com"))
                    return ContactType.LinkedIn;

                if (contact.Contains("http://") || contact.Contains("https://"))
                    return ContactType.WebSite;

                if (contact.Contains("@"))
                    return ContactType.Email;

                if (contact.Contains("@"))
                    return ContactType.Email;

                if (contact.Contains("+38") || contact.Contains("+1") || contact.Contains("+7")) //Imprve this logic
                    return ContactType.Phone;

                return ContactType.Default;
            }
        }
    }
}
