using System;

namespace Core.ViewModels
{
    public class NewPostRequest
    {
        public Uri Link { get; set; }
        public Guid Key { get; set; }
        public int CategoryId { get; set; }
        public string Comment { get; set; }
    }
}