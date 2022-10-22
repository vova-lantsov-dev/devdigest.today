using System;
using System.Threading.Tasks;

namespace Core.Services;

public interface IPostingService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="title">Message title</param>
    /// <param name="body">Message body</param>
    /// <param name="link"></param>
    /// <returns></returns>
    Task Send(string title, string body, Uri link);
}