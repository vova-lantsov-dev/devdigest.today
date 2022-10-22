using System;
using System.Collections.Generic;

namespace DAL
{
    public partial class Post
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Link { get; set; }
        public DateTime DateTime { get; set; }
        public string Header { get; set; }
        public string Comment { get; set; }
        public string HeaderUa { get; set; }
        public string CommentUa { get; set; }
        public string EmbededPlayerCode { get; set; }
        public int? LanguageId { get; set; }
        public int Views { get; set; }
        public ulong Visible { get; set; }
    }
}
