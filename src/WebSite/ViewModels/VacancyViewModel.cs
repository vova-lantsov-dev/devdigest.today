using Core.Models;

namespace WebSite.ViewModels;

public record VacancyViewModel
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
		Date = vacancy.Date.ToDateTime(TimeOnly.MinValue);
		Url = string.IsNullOrWhiteSpace(vacancy.Url) ? null : new Uri(vacancy.Url);
		Company = vacancy.Company;
		ViewsCount = vacancy.Views;
		Location = vacancy.Location;
		Category = new CategoryViewModel
		{
			Id = vacancy.Category.Id,
			Name = vacancy.Category.Name
		};
	}

	public int ViewsCount { get; init; }
	public int Id { get; init; }
	public string Title { get; init; }
	public Uri Image { get; init; }
	public CategoryViewModel Category { get; init; }
	public DateTime Date { get; set; }
	public string Description { get; init; }
	public Uri ShareUrl => new($"{_websiteUrl}vacancy/{Id}");
	public string Content { get; init; }
	public string Contact { get; init; }
	public bool Active { get; init; }
	public Uri Url { get; init; }
	public string Company { get; init; }
	public string Location { get; init; }

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
			var contact = Contact?.Trim().ToLower() ?? string.Empty;

			if (contact.Contains("facebook.com"))
			{
				return ContactType.Facebook;
			}

			if (contact.Contains("linkedin.com"))
			{
				return ContactType.LinkedIn;
			}

			if (contact.Contains("http://") || contact.Contains("https://"))
			{
				return ContactType.WebSite;
			}

			if (contact.Contains("skype:"))
			{
				return ContactType.Skype;
			}

			if (contact.StartsWith("@"))
			{
				return ContactType.Telegram;
			}

			if (contact.Contains("@"))
			{
				return ContactType.Email;
			}

			if (contact.Contains("+38") || contact.Contains("+1") || contact.Contains("+7")) //Improve this logic
			{
				return ContactType.Phone;
			}

			return ContactType.Default;
		}
	}

	public string Place => string.IsNullOrWhiteSpace(Company) ? $"{Location}" : $"{Company}, {Location}";
}