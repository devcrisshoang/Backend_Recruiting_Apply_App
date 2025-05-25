using Microsoft.EntityFrameworkCore;
using Backend_Recruiting_Apply_App.Data.Entities;
using SystemAPIdotnet.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend_Recruiting_Apply_App.Services
{
    public interface IJobNameService
    {
        Task<List<JobName>> GetAllJobNamesAsync();
        Task<JobName?> GetJobNameByIdAsync(int id);
        Task<List<string>> GetJobNamesByFieldIdAsync(int fieldId);
        Task<JobName> CreateJobNameAsync(JobName jobName);
        Task<bool> UpdateJobNameAsync(int id, JobName jobName);
        Task<bool> DeleteJobNameAsync(int id);
    }

    public class JobNameService : IJobNameService
    {
        private readonly RAADbContext _dbContext;

        public JobNameService(RAADbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<JobName>> GetAllJobNamesAsync()
        {
            return await _dbContext.JobName.ToListAsync();
        }

        public async Task<JobName?> GetJobNameByIdAsync(int id)
        {
            return await _dbContext.JobName.FindAsync(id);
        }

        public async Task<List<string>> GetJobNamesByFieldIdAsync(int fieldId)
        {
            return await _dbContext.JobName
                .Where(j => j.Field_ID == fieldId)
                .Select(j => j.Name)
                .ToListAsync();
        }

        public async Task<JobName> CreateJobNameAsync(JobName jobName)
        {
            _dbContext.JobName.Add(jobName);
            await _dbContext.SaveChangesAsync();
            return jobName;
        }

        public async Task<bool> UpdateJobNameAsync(int id, JobName jobName)
        {
            if (id != jobName.ID)
            {
                return false;
            }

            _dbContext.Entry(jobName).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return !await _dbContext.JobName.AnyAsync(e => e.ID == id);
            }
        }

        public async Task<bool> DeleteJobNameAsync(int id)
        {
            var jobName = await _dbContext.JobName.FindAsync(id);
            if (jobName == null)
            {
                return false;
            }

            _dbContext.JobName.Remove(jobName);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}