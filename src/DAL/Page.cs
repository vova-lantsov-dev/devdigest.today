using System;
using System.Collections.Generic;

namespace DAL
{
    public partial class Page
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
    }
}
