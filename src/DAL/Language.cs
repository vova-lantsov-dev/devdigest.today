using System;
using System.Collections.Generic;

namespace DAL
{
    public partial class Language
    {
        public Language()
        {
            Vacancies = new HashSet<Vacancy>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        
        public virtual ICollection<Vacancy> Vacancies { get; set; }
    }
}
