using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using Microsoft.EntityFrameworkCore;

namespace Core.Repositories;

public interface IVacancyRepository
{
    Task<IReadOnlyCollection<Vacancy>> GetList(int page, int size);

    /// <summary>
    /// Return total count of active vacancies
    /// </summary>
    /// <returns></returns>
    Task<int> GetCount();

    /// <summary>
    /// Get list of hot vacancies
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    Task<IReadOnlyCollection<Vacancy>> GetHot(int size);

    /// <summary>
    /// Get vacancy
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<Vacancy> Get(int id);

    /// <summary>
    /// Increase view count of vacancy
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task IncreaseViewCount(int id);
}

public class VacancyRepository : RepositoryBase, IVacancyRepository 
{
    public VacancyRepository(DatabaseContext databaseContext)
        : base(databaseContext)
    {
    }

    public async Task<IReadOnlyCollection<Vacancy>> GetList(int page, int size) =>
        await DatabaseContext
            .Vacancies
            .Include(o => o.Category)
            .Where(o => o.Active == 1 && o.LanguageId == Language.EnglishId)
            .OrderByDescending(o => o.Id)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

    public Task<int> GetCount() =>
        DatabaseContext.Vacancies.Where(o => o.Active == 1).CountAsync();

    public async Task<IReadOnlyCollection<Vacancy>> GetHot(int size) =>
        await DatabaseContext
            .Vacancies
            .Include(o => o.Category)
            .Where(o => o.Active == 1 && o.LanguageId == Language.EnglishId)
            .OrderByDescending(o => o.Id)
            .Take(size)
            .ToListAsync();

    public Task<Vacancy> Get(int id) =>
        DatabaseContext.Vacancies.SingleOrDefaultAsync(o => o.Id == id);

    public async Task IncreaseViewCount(int id)
    {
        var vacancy = await DatabaseContext.Vacancies.SingleOrDefaultAsync(o => o.Id == id);
            
        if (vacancy != null)
        {
            vacancy.Views++;
                
            await DatabaseContext.SaveChangesAsync();
        }
    }
}