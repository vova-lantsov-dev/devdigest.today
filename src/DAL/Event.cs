using System;
using System.Collections.Generic;

namespace DAL
{
    public partial class Event
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? CategoryId { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public string Image { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }

        public Category Category { get; set; }
        public User User { get; set; }
    }
}
