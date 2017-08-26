using System;
using System.Collections.Generic;

namespace DAL
{
    public partial class Category
    {
        public Category()
        {
            Event = new HashSet<Event>();
            Publication = new HashSet<Publication>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Event> Event { get; set; }
        public ICollection<Publication> Publication { get; set; }
    }
}
