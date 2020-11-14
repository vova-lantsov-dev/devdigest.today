using System;
using System.Collections.Generic;

#nullable disable

namespace DAL
{
    public partial class User
    {
        public User()
        {
            Events = new HashSet<Event>();
            Publications = new HashSet<Publication>();
            Vacancies = new HashSet<Vacancy>();
        }

        public int Id { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Event> Events { get; set; }
        public virtual ICollection<Publication> Publications { get; set; }
        public virtual ICollection<Vacancy> Vacancies { get; set; }
    }
}
