﻿using System;
using Core.Models;

namespace WebSite.ViewModels
{
	public class VacancyViewModel
	{
		private readonly Uri _websiteUrl;

		public VacancyViewModel(DAL.Vacancy vacancy, Uri webSiteUrl, string fallbackImage = null)
		{
			_websiteUrl = webSiteUrl;
			var fallbackImageUrl = string.IsNullOrWhiteSpace(fallbackImage) ? null : new Uri(fallbackImage); 
			
			Id = vacancy.Id;
			Title = vacancy.Title;
			Image = string.IsNullOrEmpty(vacancy.Image) ? fallbackImageUrl : new Uri(vacancy.Image);
			Description = vacancy.Description;
			Content = vacancy.Content;
			Contact = vacancy.Contact;
			Active = vacancy.Active == 1;
			Date = vacancy.Date;
			Url = string.IsNullOrWhiteSpace(vacancy.Url) ? null : new Uri(vacancy.Url);
			Category = new CategoryViewModel();
			Company = vacancy.Company;
			ViewsCount = vacancy.Views;
			Location = vacancy.Location;

			if (vacancy.Category != null)
			{
				Category.Id = vacancy.Category.Id;
				Category.Name = vacancy.Category.Name;
			}
		}

		public int ViewsCount { get; set; }
		public int Id { get; set; }
		public string Title { get; set; }
		public Uri Image { get; set; }
		public CategoryViewModel Category { get; set; }
		public DateTime Date { get; set; }
		public string Description { get; set; }
		public Uri ShareUrl => new Uri($"{_websiteUrl}vacancy/{Id}");
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
				var contact = Contact?.Trim()?.ToLower() ?? string.Empty;

				if (contact.Contains("facebook.com"))
					return ContactType.Facebook;

				if (contact.Contains("linkedin.com"))
					return ContactType.LinkedIn;

				if (contact.Contains("http://") || contact.Contains("https://"))
					return ContactType.WebSite;

				if (contact.Contains("skype:"))
					return ContactType.Skype;
				
				if (contact.StartsWith("@"))
					return ContactType.Telegram;

				if (contact.Contains("@"))
					return ContactType.Email;

				if (contact.Contains("+38") || contact.Contains("+1") || contact.Contains("+7")) //Improve this logic
					return ContactType.Phone;

				return ContactType.Default;
			}
		}

		public string Place => string.IsNullOrWhiteSpace(Company) ? $"{Location}" : $"{Company}, {Location}";
	}
}
