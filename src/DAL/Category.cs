using System;
using System.Collections.Generic;

namespace DAL
{
    public partial class Category
    {
        public Category()
        {
            Channel = new HashSet<Channel>();
            Event = new HashSet<Event>();
            FacebookPage = new HashSet<FacebookPage>();
            Publication = new HashSet<Publication>();
            Vacancy = new HashSet<Vacancy>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Channel> Channel { get; set; }
        public ICollection<Event> Event { get; set; }
        public ICollection<FacebookPage> FacebookPage { get; set; }
        public ICollection<Publication> Publication { get; set; }
        public ICollection<Vacancy> Vacancy { get; set; }
    }
}
