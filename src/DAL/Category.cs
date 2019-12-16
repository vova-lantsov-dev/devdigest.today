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
        public string Description { get; set; }
        public string Tags { get; set; }

        public virtual ICollection<Channel> Channel { get; set; }
        public virtual ICollection<Event> Event { get; set; }
        public virtual ICollection<FacebookPage> FacebookPage { get; set; }
        public virtual ICollection<Publication> Publication { get; set; }
        public virtual ICollection<Vacancy> Vacancy { get; set; }
    }
}
