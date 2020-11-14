using System;
using System.Collections.Generic;

#nullable disable

namespace DAL
{
    public partial class Language
    {
        public Language()
        {
            Publications = new HashSet<Publication>();
            Vacancies = new HashSet<Vacancy>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }

        public virtual ICollection<Publication> Publications { get; set; }
        public virtual ICollection<Vacancy> Vacancies { get; set; }
    }
}
