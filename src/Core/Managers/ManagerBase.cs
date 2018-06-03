using System;
using DAL;
using Microsoft.Extensions.Caching.Memory;

namespace Core.Managers
{
    public interface IManager
    {
    }

//    public abstract class ManagerBase : IManager
//    {
//        
//        protected ManagerBase()
//        {
//            _cache = cache;
//        }
//
//        protected static MemoryCacheEntryOptions GetMemoryCacheEntryOptions()
//        {
//            return new MemoryCacheEntryOptions
//            {
//                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(1)
//            };
//        }
//    }
}