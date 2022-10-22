using System;
using System.Collections.Generic;

namespace DAL
{
    public partial class User
    {
        public User()
        {
            Events = new HashSet<Event>();
            Vacancies = new HashSet<Vacancy>();
        }

        public int Id { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Event> Events { get; set; }
        public virtual ICollection<Vacancy> Vacancies { get; set; }
    }
}
