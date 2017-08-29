using System;
using System.Collections.Generic;
using System.Linq;
using DAL;

namespace Core.ViewModels
{
    public class PublicationViewModel : DAL.Publication
    {
        private string _websiteUrl;

        public PublicationViewModel()
        {

        }

        public PublicationViewModel(DAL.Publication publication, string websiteUrl, IEnumerable<DAL.Category> categories = null)
        {
            _websiteUrl = websiteUrl;

            Id = publication.Id;
            Title = publication.Title;
            Description = publication.Description;
            Image = publication.Image;
            Link = publication.Link;
            DateTime = publication.DateTime;
            Type = publication.Type;
            Content = publication.Content;

            if (publication.CategoryId.HasValue && categories != null && categories.Any())
            {
                var categoryName = categories
                    .Where(o => o.Id == publication.CategoryId)
                    .Select(o => o.Name)
                    .FirstOrDefault();

                Category = new CategoryViewModel
                {
                    Id = publication.CategoryId.Value,
                    Name = categoryName
                };
            }

        }

        public Uri Url => new Uri(this.Link);

        public string ShareUrl => $"{_websiteUrl}post/{Id}";

        public string Keywords => X.Text.TextHelper.GetKeywords(Description, 10);

        public CategoryViewModel Category { get; set; }

        public override string ToString()
        {
            return $"{Title}\r\n{Description}\r\n{Url}";
        }
    }
}