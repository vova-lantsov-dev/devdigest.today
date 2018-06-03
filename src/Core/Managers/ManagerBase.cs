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
        protected readonly IMemoryCache _cache;

        protected ManagerBase(IMemoryCache cache = null)
        {
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