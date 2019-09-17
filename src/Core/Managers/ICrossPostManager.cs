using System.Threading.Tasks;

namespace Core.Managers
{
    public interface ICrossPostManager
    {
        Task Send(int categoryId, string comment, string link);
    }
}