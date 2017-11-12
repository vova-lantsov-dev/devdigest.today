using System;
using System.Collections.Generic;

namespace DAL
{
    public partial class FacebookPage
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Token { get; set; }

        public Category Category { get; set; }
    }
}
