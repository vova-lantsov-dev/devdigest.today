using System;
using System.Collections.Generic;

namespace DAL
{
    public partial class User
    {
        public User()
        {
            Event = new HashSet<Event>();
            Publication = new HashSet<Publication>();
            Vacancy = new HashSet<Vacancy>();
        }

        public int Id { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Event> Event { get; set; }
        public virtual ICollection<Publication> Publication { get; set; }
        public virtual ICollection<Vacancy> Vacancy { get; set; }
    }
}
