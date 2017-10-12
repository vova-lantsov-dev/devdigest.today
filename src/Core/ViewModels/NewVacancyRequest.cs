using System;

namespace Core.ViewModels
{
    public class NewVacancyRequest
    {
        public Guid Key { get; set; }
        public int CategoryId { get; set; }
        public string Comment { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Contact { get; set; }
    }
}
