using System;

namespace WebSite.ViewModels
{
    public class NewPostRequest
    {
        public Uri Link { get; set; }
        public Guid Key { get; set; }
        public int CategoryId { get; set; }
        public string Comment { get; set; }
        public string Tags { get; set; }
    }
}