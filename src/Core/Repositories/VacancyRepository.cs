using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using Microsoft.EntityFrameworkCore;

namespace Core.Repositories
{
    public interface IVacancyRepository
    {   Task<IReadOnlyCollection<Vacancy>> GetVacancies(int page, int size);
        Task<int> GetVacanciesCount();
        Task<IReadOnlyCollection<Vacancy>> GetHotVacancies(int size);
        Task<Vacancy> GetVacancy(int id);
        Task IncreaseVacancyViewCount(int id);
    }
    
    public class VacancyRepository : IVacancyRepository 
    {
        private readonly DatabaseContext _database;

        public VacancyRepository(DatabaseContext database) => _database = database;

        public async Task<IReadOnlyCollection<Vacancy>> GetVacancies(int page, int size) =>
            await _database
                .Vacancy
                .Include(o => o.Category)
                .Where(o => o.Active == 1 && o.LanguageId == Language.EnglishId)
                .OrderByDescending(o => o.Id)
                .Skip((page - 1) * size)
                .Take(size).ToListAsync();

        public async Task<int> GetVacanciesCount() =>
            await _database.Vacancy.Where(o => o.Active == 1).CountAsync();

        public async Task<IReadOnlyCollection<Vacancy>> GetHotVacancies(int size) =>
            await _database
                .Vacancy
                .Include(o => o.Category)
                .Where(o => o.Active == 1 && o.LanguageId == Language.EnglishId)
                .OrderByDescending(o => o.Id)
                .Take(size)
                .ToListAsync();

        public async Task<Vacancy> GetVacancy(int id) => 
            await _database.Vacancy.SingleOrDefaultAsync(o => o.Id == id);

        public async Task IncreaseVacancyViewCount(int id)
        {
            
            var vacancy = await _database.Vacancy.SingleOrDefaultAsync(o => o.Id == id);
            
            if (vacancy != null)
            {
                vacancy.Views++;
                
                await _database.SaveChangesAsync();
            }
        }
    }
}