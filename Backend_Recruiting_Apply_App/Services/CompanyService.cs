using Microsoft.EntityFrameworkCore;
using Backend_Recruiting_Apply_App.Data.Entities;
using SystemAPIdotnet.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend_Recruiting_Apply_App.Services
{
    public interface ICompanyService
    {
        Task<List<Company>> GetAllCompaniesAsync();
        Task<Company?> GetCompanyByIdAsync(int id);
        Task<Company?> GetCompanyByRecruiterIdAsync(int recruiterId);
        Task<Company> CreateCompanyAsync(Company company);
        Task<bool> UpdateCompanyAsync(int id, Company company);
        Task<bool> DeleteCompanyAsync(int id);
    }

    public class CompanyService : ICompanyService
    {
        private readonly RAADbContext _dbContext;

        public CompanyService(RAADbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Company>> GetAllCompaniesAsync()
        {
            return await _dbContext.Company.ToListAsync();
        }

        public async Task<Company?> GetCompanyByIdAsync(int id)
        {
            return await _dbContext.Company.FindAsync(id);
        }

        public async Task<Company?> GetCompanyByRecruiterIdAsync(int recruiterId)
        {
            var recruiter = await _dbContext.Recruiter
                .FirstOrDefaultAsync(r => r.ID == recruiterId);

            if (recruiter == null || recruiter.Company_ID == 0)
            {
                return null;
            }

            return await _dbContext.Company
                .FirstOrDefaultAsync(c => c.ID == recruiter.Company_ID);
        }

        public async Task<Company> CreateCompanyAsync(Company company)
        {
            _dbContext.Company.Add(company);
            await _dbContext.SaveChangesAsync();
            return company;
        }

        public async Task<bool> UpdateCompanyAsync(int id, Company company)
        {
            if (id != company.ID)
            {
                return false;
            }

            _dbContext.Entry(company).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return !await _dbContext.Company.AnyAsync(e => e.ID == id);
            }
        }

        public async Task<bool> DeleteCompanyAsync(int id)
        {
            var company = await _dbContext.Company.FindAsync(id);
            if (company == null)
            {
                return false;
            }

            _dbContext.Company.Remove(company);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}