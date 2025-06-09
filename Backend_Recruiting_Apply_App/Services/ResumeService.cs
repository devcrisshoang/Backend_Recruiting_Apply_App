using Backend_Recruiting_Apply_App.Data.Entities;
using SystemAPIdotnet.Data;
using Microsoft.EntityFrameworkCore;

namespace Backend_Recruiting_Apply_App.Services
{
    public class UpdateResumeImageDto
    {
        public byte[] Image { get; set; } = [];
    }

    public class UpdateResumeIsDeleteDto
    {
        public int Is_delete { get; set; }
    }

    public interface IResumeService
    {
        Task<IEnumerable<Resume>> GetAllResumesAsync();
        Task<Resume> GetResumeByIdAsync(int id);
        Task<IEnumerable<Resume>> GetResumesByApplicantIdAsync(int applicantId);
        Task<Resume> CreateResumeAsync(Resume resume);
        Task<bool> UpdateResumeAsync(int id, Resume resume);
        Task<bool> UpdateResumeImageAsync(int id, UpdateResumeImageDto dto);
        Task<bool> UpdateResumeIsDeleteAsync(int id, UpdateResumeIsDeleteDto dto);
        Task<bool> DeleteResumeAsync(int id);
        Task<bool> DeleteAllResumesAsync();
    }

    public class ResumeService : IResumeService
    {
        private readonly RAADbContext _context;

        public ResumeService(RAADbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Resume>> GetAllResumesAsync()
        {
            return await _context.Resume.ToListAsync();
        }

        public async Task<Resume> GetResumeByIdAsync(int id)
        {
            return await _context.Resume.FindAsync(id);
        }

        public async Task<IEnumerable<Resume>> GetResumesByApplicantIdAsync(int applicantId)
        {
            return await _context.Resume
                .Where(r => r.Applicant_ID == applicantId)
                .ToListAsync();
        }

        public async Task<Resume> CreateResumeAsync(Resume resume)
        {
            _context.Resume.Add(resume);
            await _context.SaveChangesAsync();
            return resume;
        }

        public async Task<bool> UpdateResumeAsync(int id, Resume resume)
        {
            if (id != resume.ID)
                return false;

            _context.Entry(resume).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ResumeExistsAsync(id))
                    return false;
                throw;
            }
        }

        public async Task<bool> UpdateResumeImageAsync(int id, UpdateResumeImageDto dto)
        {
            var resume = await _context.Resume.FindAsync(id);
            if (resume == null)
                return false;

            resume.Image = dto.Image;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateResumeIsDeleteAsync(int id, UpdateResumeIsDeleteDto dto)
        {
            var resume = await _context.Resume.FindAsync(id);
            if (resume == null)
                return false;

            resume.Is_Delete = dto.Is_delete;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteResumeAsync(int id)
        {
            var resume = await _context.Resume.FindAsync(id);
            if (resume == null)
                return false;

            _context.Resume.Remove(resume);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAllResumesAsync()
        {
            var resumes = await _context.Resume.ToListAsync();
            if (resumes == null || resumes.Count == 0)
                return false;

            _context.Resume.RemoveRange(resumes);
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<bool> ResumeExistsAsync(int id)
        {
            return await _context.Resume.AnyAsync(e => e.ID == id);
        }
    }
}