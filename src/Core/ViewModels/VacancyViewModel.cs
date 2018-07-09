using System;
using Core.Models;

namespace Core.ViewModels
{
	public class VacancyViewModel
	{
		private readonly string _websiteUrl;

		public VacancyViewModel(DAL.Vacancy vacancy, string webSiteUrl, string fallbackImage = null)
		{
			_websiteUrl = webSiteUrl;
			
			Id = vacancy.Id;
			Title = vacancy.Title;
			Image = vacancy.Image ?? fallbackImage;
			Description = vacancy.Description;
			Content = vacancy.Content;
			Contact = vacancy.Contact;
			Active = vacancy.Active;
			Date = vacancy.Date;
			Url = string.IsNullOrWhiteSpace(vacancy.Url) ? null : new Uri(vacancy.Url);
			Category = new CategoryViewModel();
			Company = vacancy.Company;
			ViewsCount = vacancy.Views;
			Location = vacancy.Location;
			
			//ViewsCount = new Random((int)DateTime.Now.Ticks).Next(10, 915);

			if (vacancy.Category != null)
			{
				Category.Id = vacancy.Category.Id;
				Category.Name = vacancy.Category.Name;
			}
		}

		public int ViewsCount { get; set; }
		public int Id { get; set; }
		public string Title { get; set; }
		public string Image { get; set; }
		public CategoryViewModel Category { get; set; }
		public DateTime Date { get; set; }
		public string Description { get; set; }
		public string ShareUrl => $"{_websiteUrl}vacancy/{Id}";
		public string Content { get; set; }
		public string Contact { get; set; }
		public bool Active { get; set; }
		public Uri Url { get; set; }
		public string Company { get; set; }
		public string Location { get; set; }

		/// <summary>
		/// Vacancy posted today
		/// </summary>
		public bool Fresh => Date.Date == DateTime.Now.Date;

		/// <summary>
		/// Vacancy posted on this week
		/// </summary>
		public bool Hot => Date.Date.AddDays(7) > DateTime.Now.Date;

		public ContactType ContactType
		{
			get
			{
				var contact = Contact.Trim().ToLower();

				if (contact.Contains("facebook.com"))
					return ContactType.Facebook;

				if (contact.Contains("linkedin.com"))
					return ContactType.LinkedIn;

				if (contact.Contains("http://") || contact.Contains("https://"))
					return ContactType.WebSite;

				if (contact.Contains("skype:"))
					return ContactType.Skype;

				if (contact.Contains("@"))
					return ContactType.Email;

				if (contact.Contains("+38") || contact.Contains("+1") || contact.Contains("+7")) //Imprve this logic
					return ContactType.Phone;

				return ContactType.Default;
			}
		}
	}
}
