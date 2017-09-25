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
            EmbededPlayerCode = publication.EmbededPlayerCode;

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

        public Uri Url => string.IsNullOrWhiteSpace(this.Link) ? null : new Uri(this.Link);

        public string ShareUrl => $"{_websiteUrl}post/{Id}";

        public string Keywords => X.Text.TextHelper.GetKeywords(Description, 10);

        public string EmbededPlayerCode { get; set; }

        public CategoryViewModel Category { get; set; }

        public override string ToString()
        {
            return $"{Title}\r\n{Description}\r\n{Url}";
        }
    }
}