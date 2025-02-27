﻿using System;
using System.Collections.Generic;

namespace DAL
{
    public partial class Category
    {
        public Category()
        {
            Channels = new HashSet<Channel>();
            Events = new HashSet<Event>();
            FacebookPages = new HashSet<FacebookPage>();
            Vacancies = new HashSet<Vacancy>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Tags { get; set; }

        public virtual ICollection<Channel> Channels { get; set; }
        public virtual ICollection<Event> Events { get; set; }
        public virtual ICollection<FacebookPage> FacebookPages { get; set; }
        public virtual ICollection<Vacancy> Vacancies { get; set; }
    }
}
