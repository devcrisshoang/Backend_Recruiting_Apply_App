using Microsoft.EntityFrameworkCore;
using Backend_Recruiting_Apply_App.Data.Entities;
using SystemAPIdotnet.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend_Recruiting_Apply_App.Services
{
    public interface IJobService
    {
        Task<List<Job>> GetAllJobsAsync();
        Task<Job?> GetJobByIdAsync(int id);
        Task<List<Job>> GetJobsByRecruiterIdAsync(int recruiterId);
        Task<List<Job>> GetJobsByCompanyIdAsync(int companyId);
        Task<Job> CreateJobAsync(Job job);
        Task<bool> UpdateJobAsync(int id, Job job);
        Task<bool> DeleteJobAsync(int id);
    }

    public class JobService : IJobService
    {
        private readonly RAADbContext _dbContext;

        public JobService(RAADbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Job>> GetAllJobsAsync()
        {
            return await _dbContext.Job.ToListAsync();
        }

        public async Task<Job?> GetJobByIdAsync(int id)
        {
            return await _dbContext.Job.FindAsync(id);
        }

        public async Task<List<Job>> GetJobsByRecruiterIdAsync(int recruiterId)
        {
            return await _dbContext.Job
                .Where(j => j.Recruiter_ID == recruiterId)
                .ToListAsync();
        }

        public async Task<List<Job>> GetJobsByCompanyIdAsync(int companyId)
        {
            return await _dbContext.Recruiter
                .Where(r => r.Company_ID == companyId)
                .Join(
                    _dbContext.Job,
                    recruiter => recruiter.ID,
                    job => job.Recruiter_ID,
                    (recruiter, job) => job
                )
                .ToListAsync();
        }

        public async Task<Job> CreateJobAsync(Job job)
        {
            _dbContext.Job.Add(job);
            await _dbContext.SaveChangesAsync();
            return job;
        }

        public async Task<bool> UpdateJobAsync(int id, Job job)
        {
            if (id != job.ID)
            {
                return false;
            }

            _dbContext.Entry(job).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return !await _dbContext.Job.AnyAsync(e => e.ID == id);
            }
        }

        public async Task<bool> DeleteJobAsync(int id)
        {
            var job = await _dbContext.Job.FindAsync(id);
            if (job == null)
            {
                return true; // Return true to match original behavior of returning Ok() when not found
            }

            _dbContext.Job.Remove(job);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}