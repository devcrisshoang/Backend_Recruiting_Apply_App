using Microsoft.EntityFrameworkCore;
using Backend_Recruiting_Apply_App.Data.Entities;
using SystemAPIdotnet.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend_Recruiting_Apply_App.Services
{
    public interface IFollowService
    {
        Task<List<Follow>> GetAllFollowsAsync();
        Task<Follow?> GetFollowByApplicantAndCompanyAsync(int applicantId, int companyId);
        Task<List<Follow>> GetFollowedCompaniesByApplicantIdAsync(int applicantId);
        Task<List<Company>> GetFollowedCompanyListByApplicantIdAsync(int applicantId);
        Task<List<Applicant>> GetFollowedApplicantListByCompanyIdAsync(int companyId);
        Task<Follow> CreateFollowAsync(Follow follow);
        Task<bool> DeleteFollowAsync(int applicantId, int companyId);
    }

    public class FollowService : IFollowService
    {
        private readonly RAADbContext _dbContext;

        public FollowService(RAADbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Follow>> GetAllFollowsAsync()
        {
            return await _dbContext.Follow.ToListAsync();
        }

        public async Task<Follow?> GetFollowByApplicantAndCompanyAsync(int applicantId, int companyId)
        {
            return await _dbContext.Follow
                .FirstOrDefaultAsync(j => j.ApplicantID == applicantId && j.CompanyID == companyId);
        }

        public async Task<List<Follow>> GetFollowedCompaniesByApplicantIdAsync(int applicantId)
        {
            return await _dbContext.Follow
                .Where(j => j.ApplicantID == applicantId)
                .ToListAsync();
        }

        public async Task<List<Company>> GetFollowedCompanyListByApplicantIdAsync(int applicantId)
        {
            return await _dbContext.Follow
                .Where(f => f.ApplicantID == applicantId)
                .Join(
                    _dbContext.Company,
                    follow => follow.CompanyID,
                    company => company.ID,
                    (follow, company) => company
                )
                .ToListAsync();
        }

        public async Task<List<Applicant>> GetFollowedApplicantListByCompanyIdAsync(int companyId)
        {
            return await _dbContext.Follow
                .Where(f => f.CompanyID == companyId)
                .Join(
                    _dbContext.Applicant,
                    follow => follow.ApplicantID,
                    applicant => applicant.ID,
                    (follow, applicant) => applicant
                )
                .GroupBy(applicant => applicant.User_ID)
                .Select(group => group.First())
                .ToListAsync();
        }

        public async Task<Follow> CreateFollowAsync(Follow follow)
        {
            _dbContext.Follow.Add(follow);
            await _dbContext.SaveChangesAsync();
            return follow;
        }

        public async Task<bool> DeleteFollowAsync(int applicantId, int companyId)
        {
            var follow = await _dbContext.Follow
                .FirstOrDefaultAsync(j => j.ApplicantID == applicantId && j.CompanyID == companyId);
            if (follow == null)
            {
                return true; // Return true to match original behavior of returning Ok() when not found
            }

            _dbContext.Follow.Remove(follow);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}