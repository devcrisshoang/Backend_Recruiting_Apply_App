using Backend_Recruiting_Apply_App.Data.Entities;
using SystemAPIdotnet.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend_Recruiting_Apply_App.Services
{
    public interface IRecruiterService
    {
        Task<IEnumerable<Recruiter>> GetAllRecruitersAsync();
        Task<Recruiter> GetRecruiterByIdAsync(int id);
        Task<Recruiter> GetRecruiterByUserIdAsync(int userId);
        Task<Recruiter> CreateRecruiterAsync(Recruiter recruiter);
        Task<bool> UpdateRecruiterAsync(int id, Recruiter recruiter);
        Task<bool> DeleteRecruiterAsync(int id);
        Task<bool> RecruiterExistsAsync(int id);
    }

    public class RecruiterService : IRecruiterService
    {
        private readonly RAADbContext _context;

        public RecruiterService(RAADbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Recruiter>> GetAllRecruitersAsync()
        {
            var recruiters = await _context.Recruiter.ToListAsync();
            return recruiters ?? new List<Recruiter>();
        }

        public async Task<Recruiter> GetRecruiterByIdAsync(int id)
        {
            return await _context.Recruiter.FindAsync(id);
        }

        public async Task<Recruiter> GetRecruiterByUserIdAsync(int userId)
        {
            return await _context.Recruiter.FirstOrDefaultAsync(r => r.User_ID == userId);
        }

        public async Task<Recruiter> CreateRecruiterAsync(Recruiter recruiter)
        {
            _context.Recruiter.Add(recruiter);
            await _context.SaveChangesAsync();
            return recruiter;
        }

        public async Task<bool> UpdateRecruiterAsync(int id, Recruiter recruiter)
        {
            if (id != recruiter.ID)
                return false;

            var existingRecruiter = await _context.Recruiter.FindAsync(id);
            if (existingRecruiter == null)
                return false;

            _context.Entry(existingRecruiter).CurrentValues.SetValues(recruiter);

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await RecruiterExistsAsync(id))
                    return false;
                throw;
            }
        }

        public async Task<bool> DeleteRecruiterAsync(int id)
        {
            var recruiter = await _context.Recruiter.FindAsync(id);
            if (recruiter == null)
                return false;

            _context.Recruiter.Remove(recruiter);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RecruiterExistsAsync(int id)
        {
            return await _context.Recruiter.AnyAsync(e => e.ID == id);
        }
    }
}