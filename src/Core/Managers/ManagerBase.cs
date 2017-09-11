using DAL;

namespace Core
{
    public interface IManager
    {

    }
    
    public abstract class ManagerBase : IManager
    {
        protected readonly DAL.DatabaseContext _database;

        protected ManagerBase(string connectionString)
        {
            _database = new DatabaseContext(connectionString);
        }
    }
}