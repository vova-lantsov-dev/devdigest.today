using System;

namespace Core.Models
{
    public class PageMetaData
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Keywords { get; set; }
        public Uri Url { get; set; }
        public Uri Image { get; set; }
    }
}