using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using Microsoft.EntityFrameworkCore;

namespace Core.Repositories;

public interface ISocialRepository
{
    Task<IReadOnlyCollection<FacebookPage>> GetFacebookPages(int categoryId);
    Task<IReadOnlyCollection<FacebookPage>> GetFacebookPages();
    Task<IReadOnlyCollection<Channel>> GetTelegramChannels(int categoryId);
    Task<IReadOnlyCollection<Channel>> GetTelegramChannels();
    Task<IReadOnlyCollection<TwitterAccount>> GetTwitterAccounts();
    Task<IReadOnlyCollection<DAL.Slack>> GetSlackApplications();
}

public class SocialRepository : RepositoryBase, ISocialRepository
{
    public SocialRepository(DatabaseContext databaseContext) 
        : base(databaseContext)
    {
    }

    public async Task<IReadOnlyCollection<FacebookPage>> GetFacebookPages(int categoryId) =>
        await DatabaseContext.FacebookPages.Where(o => o.CategoryId == categoryId).ToListAsync();

    public async Task<IReadOnlyCollection<Channel>> GetTelegramChannels(int categoryId) =>
        await DatabaseContext.Channels.Where(o => o.CategoryId == categoryId).ToListAsync();

    public async Task<IReadOnlyCollection<Channel>> GetTelegramChannels() =>
        await DatabaseContext.Channels.ToListAsync();

    public async Task<IReadOnlyCollection<TwitterAccount>> GetTwitterAccounts() =>
        await DatabaseContext.TwitterAccounts.ToListAsync();

    public async Task<IReadOnlyCollection<DAL.Slack>> GetSlackApplications() => 
        await DatabaseContext.Slacks.ToListAsync();

    public async Task<IReadOnlyCollection<FacebookPage>> GetFacebookPages() =>
        await DatabaseContext.FacebookPages.ToListAsync();
}