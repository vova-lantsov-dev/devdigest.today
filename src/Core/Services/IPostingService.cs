using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Core.Services;

public interface IPostingService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <param name="link"></param>
    /// <param name="tags">
    ///     Tags which should be added to all all posts 
    /// </param>
    /// <returns></returns>
    Task Send(string message, Uri link, [NotNull] IReadOnlyCollection<string> tags);
}