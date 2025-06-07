using Microsoft.EntityFrameworkCore;
using Backend_Recruiting_Apply_App.Data.Entities;
using SystemAPIdotnet.Data;

namespace Backend_Recruiting_Apply_App.Services
{
    public interface IApplyService
    {
        Task<List<Apply>> GetAllAppliesAsync();
        Task<Apply?> GetApplyByIdAsync(int id);
        Task<List<Apply>> GetAcceptedApplicantsByJobIdAsync(int jobId);
        Task<List<Apply>> GetPendingApplicantsByJobIdAsync(int jobId);
        Task<List<Apply>> GetRejectedApplicantsByJobIdAsync(int jobId);
        Task<List<Job>> GetAppliedJobsByApplicantIdAsync(int applicantId);
        Task<Apply> CreateApplyAsync(Apply apply);
        Task<bool> UpdateApplyAsync(int id, Apply apply);
        Task<bool> UpdateIsAcceptedAsync(int id, int isAccepted);
        Task<bool> DeleteApplyAsync(int id);
        Task<List<Job>> GetJobsByApplicantIdAsync(int applicantId);
        Task<List<Applicant>> GetApplicantsByJobIdAsync(int jobId);
        Task<int> GetResumeIdByJobAndApplicantAsync(int jobId, int applicantId);
        Task<int> GetApplyIdByJobResumeApplicantAsync(int jobId, int resumeId, int applicantId);
        Task<bool> CheckExistingApplyAsync(int jobId, int applicantId, int resumeId);
        Task<List<Apply>> GetApplyByJobAndApplicantAsync(int applicantId, int jobId);
    }

    public class ApplicantWithResumeDto
    {
        public int ApplicantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ResumeId { get; set; }
    }

    public class ApplyService : IApplyService
    {
        private readonly RAADbContext _context;

        public ApplyService(RAADbContext context)
        {
            _context = context;
        }

        public async Task<List<Apply>> GetAllAppliesAsync()
        {
            return await _context.Apply.ToListAsync();
        }

        public async Task<Apply?> GetApplyByIdAsync(int id)
        {
            return await _context.Apply.FindAsync(id);
        }

        public async Task<List<Apply>> GetAcceptedApplicantsByJobIdAsync(int jobId)
        {
            return await _context.Apply
                .Where(a => a.Job_ID == jobId && a.Is_Accepted == 1)
                .ToListAsync();
        }

        public async Task<List<Apply>> GetPendingApplicantsByJobIdAsync(int jobId)
        {
            return await _context.Apply
                .Where(a => a.Job_ID == jobId && a.Is_Accepted == 2)
                .ToListAsync();
        }

        public async Task<List<Apply>> GetRejectedApplicantsByJobIdAsync(int jobId)
        {
            return await _context.Apply
                .Where(a => a.Job_ID == jobId && a.Is_Accepted == 0)
                .ToListAsync();
        }

        public async Task<List<Job>> GetAppliedJobsByApplicantIdAsync(int applicantId)
        {
            return await _context.Apply
                .Where(a => a.Applicant_ID == applicantId)
                .Join(
                    _context.Job,
                    apply => apply.Job_ID,
                    job => job.ID,
                    (apply, job) => job
                )
                .GroupBy(job => job.ID)
                .Select(group => group.First())
                .ToListAsync();
        }

        public async Task<Apply> CreateApplyAsync(Apply apply)
        {
            _context.Apply.Add(apply);
            await _context.SaveChangesAsync();
            return apply;
        }

        public async Task<bool> UpdateApplyAsync(int id, Apply apply)
        {
            if (id != apply.ID)
            {
                return false;
            }

            _context.Entry(apply).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return !await _context.Apply.AnyAsync(e => e.ID == id);
            }
        }

        public async Task<bool> UpdateIsAcceptedAsync(int id, int isAccepted)
        {
            var apply = await _context.Apply.FindAsync(id);
            if (apply == null)
            {
                return false;
            }

            apply.Is_Accepted = isAccepted;
            _context.Entry(apply).Property(x => x.Is_Accepted).IsModified = true;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return !await _context.Apply.AnyAsync(e => e.ID == id);
            }
        }

        public async Task<bool> DeleteApplyAsync(int id)
        {
            var apply = await _context.Apply.FindAsync(id);
            if (apply == null)
            {
                return false;
            }

            _context.Apply.Remove(apply);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Job>> GetJobsByApplicantIdAsync(int applicantId)
        {
            var applyRecords = await _context.Apply
                .Where(a => a.Applicant_ID == applicantId)
                .Select(a => a.Job_ID)
                .Distinct()
                .ToListAsync();

            return await _context.Job
                .Where(j => applyRecords.Contains(j.ID))
                .ToListAsync();
        }

        public async Task<List<Applicant>> GetApplicantsByJobIdAsync(int jobId)
        {
            var applyRecords = await _context.Apply
                .Where(a => a.Job_ID == jobId)
                .Select(a => a.Applicant_ID)
                .Distinct()
                .ToListAsync();

            return await _context.Applicant
                .Where(a => applyRecords.Contains(a.ID))
                .ToListAsync();
        }

        public async Task<int> GetResumeIdByJobAndApplicantAsync(int jobId, int applicantId)
        {
            if (jobId <= 0 || applicantId <= 0)
            {
                throw new ArgumentException("JobID and ApplicantID must be greater than 0.");
            }

            return await _context.Apply
                .Where(a => a.Job_ID == jobId && a.Applicant_ID == applicantId)
                .Select(a => a.Resume_ID)
                .FirstOrDefaultAsync();
        }

        public async Task<int> GetApplyIdByJobResumeApplicantAsync(int jobId, int resumeId, int applicantId)
        {
            if (jobId <= 0 || resumeId <= 0 || applicantId <= 0)
            {
                throw new ArgumentException("JobID, ResumeID, and ApplicantID must be greater than 0.");
            }

            return await _context.Apply
                .Where(a => a.Job_ID == jobId && a.Resume_ID == resumeId && a.Applicant_ID == applicantId)
                .Select(a => a.ID)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> CheckExistingApplyAsync(int jobId, int applicantId, int resumeId)
        {
            if (jobId <= 0 || applicantId <= 0 || resumeId <= 0)
            {
                throw new ArgumentException("JobID, ApplicantID, and ResumeID must be greater than 0.");
            }

            return await _context.Apply
                .AnyAsync(a => a.Job_ID == jobId && a.Applicant_ID == applicantId && a.Resume_ID == resumeId);
        }

        public async Task<List<Apply>> GetApplyByJobAndApplicantAsync(int applicantId, int jobId)
        {
            if (applicantId <= 0 || jobId <= 0)
            {
                throw new ArgumentException("ApplicantID and JobID must be greater than 0.");
            }
            return await _context.Apply
                .Where(a => a.Applicant_ID == applicantId && a.Job_ID == jobId)
                .ToListAsync();
        }
    }
}