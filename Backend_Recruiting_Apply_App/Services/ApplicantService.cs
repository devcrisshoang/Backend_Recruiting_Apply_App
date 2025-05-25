using Microsoft.EntityFrameworkCore;
using Backend_Recruiting_Apply_App.Data.Entities;
using SystemAPIdotnet.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend_Recruiting_Apply_App.Services
{
    public interface IApplicantService
    {
        Task<List<Applicant>> GetAllApplicantsAsync();
        Task<Applicant?> GetApplicantByIdAsync(int id);
        Task<Applicant?> GetApplicantByUserIdAsync(int userId);
        Task<Applicant> CreateApplicantAsync(Applicant applicant);
        Task<bool> UpdateApplicantAsync(int id, Applicant applicant);
        Task<bool> UpdateExperienceAsync(int id, string experience);
        Task<bool> UpdateJobAsync(int id, string job);
        Task<bool> UpdateLocationAsync(int id, string location);
        Task<bool> DeleteApplicantAsync(int id);
    }

    public class ApplicantService : IApplicantService
    {
        private readonly RAADbContext _context;

        public ApplicantService(RAADbContext context)
        {
            _context = context;
        }

        public async Task<List<Applicant>> GetAllApplicantsAsync()
        {
            return await _context.Applicant.ToListAsync();
        }

        public async Task<Applicant?> GetApplicantByIdAsync(int id)
        {
            return await _context.Applicant.FindAsync(id);
        }

        public async Task<Applicant?> GetApplicantByUserIdAsync(int userId)
        {
            return await _context.Applicant.FirstOrDefaultAsync(a => a.User_ID == userId);
        }

        public async Task<Applicant> CreateApplicantAsync(Applicant applicant)
        {
            _context.Applicant.Add(applicant);
            await _context.SaveChangesAsync();
            return applicant;
        }

        public async Task<bool> UpdateApplicantAsync(int id, Applicant applicant)
        {
            if (id != applicant.ID)
            {
                return false;
            }

            _context.Entry(applicant).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return !await _context.Applicant.AnyAsync(e => e.ID == id);
            }
        }

        public async Task<bool> UpdateExperienceAsync(int id, string experience)
        {
            var applicant = await _context.Applicant.FindAsync(id);
            if (applicant == null)
            {
                return false;
            }

            applicant.Experience = experience;
            _context.Entry(applicant).Property(x => x.Experience).IsModified = true;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return !await _context.Applicant.AnyAsync(e => e.ID == id);
            }
        }

        public async Task<bool> UpdateJobAsync(int id, string job)
        {
            var applicant = await _context.Applicant.FindAsync(id);
            if (applicant == null)
            {
                return false;
            }

            applicant.Job = job;
            _context.Entry(applicant).Property(x => x.Job).IsModified = true;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return !await _context.Applicant.AnyAsync(e => e.ID == id);
            }
        }

        public async Task<bool> UpdateLocationAsync(int id, string location)
        {
            var applicant = await _context.Applicant.FindAsync(id);
            if (applicant == null)
            {
                return false;
            }

            applicant.Location = location;
            _context.Entry(applicant).Property(x => x.Location).IsModified = true;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return !await _context.Applicant.AnyAsync(e => e.ID == id);
            }
        }

        public async Task<bool> DeleteApplicantAsync(int id)
        {
            var applicant = await _context.Applicant.FindAsync(id);
            if (applicant == null)
            {
                return false;
            }

            _context.Applicant.Remove(applicant);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}