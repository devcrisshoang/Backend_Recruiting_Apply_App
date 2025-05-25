using Microsoft.EntityFrameworkCore;
using Backend_Recruiting_Apply_App.Data.Entities;
using SystemAPIdotnet.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend_Recruiting_Apply_App.Services
{
    public interface IExperienceService
    {
        Task<List<Experience>> GetAllExperiencesAsync();
        Task<Experience?> GetExperienceByIdAsync(int id);
        Task<Experience> CreateExperienceAsync(Experience experience);
        Task<bool> UpdateExperienceAsync(int id, Experience experience);
        Task<bool> DeleteExperienceAsync(int id);
    }

    public class ExperienceService : IExperienceService
    {
        private readonly RAADbContext _dbContext;

        public ExperienceService(RAADbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Experience>> GetAllExperiencesAsync()
        {
            return await _dbContext.Experience.ToListAsync();
        }

        public async Task<Experience?> GetExperienceByIdAsync(int id)
        {
            return await _dbContext.Experience.FindAsync(id);
        }

        public async Task<Experience> CreateExperienceAsync(Experience experience)
        {
            _dbContext.Experience.Add(experience);
            await _dbContext.SaveChangesAsync();
            return experience;
        }

        public async Task<bool> UpdateExperienceAsync(int id, Experience experience)
        {
            if (id != experience.ID)
            {
                return false;
            }

            _dbContext.Entry(experience).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return !await _dbContext.Experience.AnyAsync(e => e.ID == id);
            }
        }

        public async Task<bool> DeleteExperienceAsync(int id)
        {
            var experience = await _dbContext.Experience.FindAsync(id);
            if (experience == null)
            {
                return false;
            }

            _dbContext.Experience.Remove(experience);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}