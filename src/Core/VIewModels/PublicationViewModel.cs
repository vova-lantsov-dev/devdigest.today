using System;

namespace Core.ViewModels
{
    public class PublicationViewModel : DAL.Publication
    {
        private string _websiteUrl;
        
        public PublicationViewModel()
        {

        }

        public PublicationViewModel(DAL.Publication publication, string websiteUrl)
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
        }

        public Uri Url => new Uri(this.Link);

        public string ShareUrl => $"{_websiteUrl}post/{Id}";

        public string Keywords => X.Text.TextHelper.GetKeywords(Description, 10);

        public override string ToString()
        {
            return $"{Title}\r\n{Description}\r\n{Url}";
        }
    }
}