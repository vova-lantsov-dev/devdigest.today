using System;
using System.Collections.Generic;

namespace DAL
{
    public partial class Vacancy
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Contact { get; set; }
        public bool Active { get; set; }
        public DateTime Date { get; set; }
        public string Content { get; set; }
        public string Image { get; set; }
        public string Url { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }

        public Category Category { get; set; }
        public User User { get; set; }
    }
}
