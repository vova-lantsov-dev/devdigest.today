using System;
using DAL;
using Microsoft.Extensions.Caching.Memory;

namespace Core.Managers
{
    public interface IManager
    {
    }

    public abstract class ManagerBase : IManager
    {
        protected readonly DatabaseContext _database;
        protected readonly IMemoryCache _cache;

        protected ManagerBase(string connectionString, IMemoryCache cache = null)
        {
            _database = new DatabaseContext(connectionString);
            _cache = cache;
        }

        protected static MemoryCacheEntryOptions GetMemoryCacheEntryOptions()
        {
            return new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(1)
            };
        }
    }
}