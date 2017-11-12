using System;
using System.Collections.Generic;

namespace DAL
{
    public partial class Language
    {
        public Language()
        {
            Publication = new HashSet<Publication>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }

        public ICollection<Publication> Publication { get; set; }
    }
}
