using DAL;

namespace Core.Repositories;

public abstract class RepositoryBase
{
    protected readonly DatabaseContext DatabaseContext;

    protected RepositoryBase(DatabaseContext databaseContext)
    {
        DatabaseContext = databaseContext;
    }
}