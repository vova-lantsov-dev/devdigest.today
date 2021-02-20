using System;
using System.Collections.Generic;

#nullable disable

namespace DAL
{
    public partial class Slack
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string WebHookUrl { get; set; }
    }
}
