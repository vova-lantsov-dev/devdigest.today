using System;
using System.Collections.Generic;

namespace DAL
{
    public partial class Channel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Token { get; set; }
        public int? CategoryId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Logo { get; set; }

        public virtual Category Category { get; set; }
    }
}
