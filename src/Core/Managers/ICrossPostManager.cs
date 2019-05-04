using System.Threading.Tasks;

namespace Core.Managers
{
    public interface ICrossPostManager
    {
        Task<bool> Send(int categoryId, string comment, string link);
    }
}