using DAL;

namespace Core
{
    public abstract class ManagerBase
    {
        protected readonly DAL.DatabaseContext _database;

        protected ManagerBase(string connectionString)
        {
            _database = new DatabaseContext(connectionString);
        }
    }
}