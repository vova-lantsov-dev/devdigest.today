using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Services
{
    public interface ICrossPostService
    {
        Task Send(int categoryId, string comment, string link, IReadOnlyCollection<string> tags);
    }
}