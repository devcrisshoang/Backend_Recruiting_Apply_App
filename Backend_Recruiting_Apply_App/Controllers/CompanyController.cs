using Microsoft.AspNetCore.Mvc;
using Backend_Recruiting_Apply_App.Data.Entities;
using Backend_Recruiting_Apply_App.Services;
using SystemAPIdotnet.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Backend_Recruiting_Apply_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;
        private readonly RAADbContext _dbContext;

        public CompanyController(ICompanyService companyService, RAADbContext dbContext)
        {
            _companyService = companyService;
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Company>>> GetCompany()
        {
            var companies = await _companyService.GetAllCompaniesAsync();
            return Ok(companies);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Company>> GetCompany(int id)
        {
            var company = await _companyService.GetCompanyByIdAsync(id);
            if (company == null)
            {
                return NotFound(new { message = "Công ty không tồn tại" });
            }
            return Ok(company);
        }

        [HttpGet("recruiter/{recruiterId}")]
        public async Task<ActionResult<Company>> GetCompanyByRecruiterId(int recruiterId)
        {
            var company = await _companyService.GetCompanyByRecruiterIdAsync(recruiterId);
            if (company == null)
            {
                // Check if recruiter exists
                var recruiterExists = await _dbContext.Recruiter.AnyAsync(r => r.ID == recruiterId);
                if (!recruiterExists)
                {
                    return NotFound(new { message = "Nhà tuyển dụng không tồn tại" });
                }
                return NotFound(new { message = "Nhà tuyển dụng không được liên kết với công ty nào" });
            }
            return Ok(company);
        }

        [HttpPost]
        public async Task<ActionResult<Company>> CreateCompany(Company company)
        {
            var createdCompany = await _companyService.CreateCompanyAsync(company);
            return CreatedAtAction(nameof(GetCompany), new { id = createdCompany.ID }, new { message = "Tạo công ty thành công", data = createdCompany });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCompany(int id, Company company)
        {
            var success = await _companyService.UpdateCompanyAsync(id, company);
            if (!success)
            {
                return id != company.ID
                    ? BadRequest(new { message = "ID không khớp" })
                    : NotFound(new { message = "Công ty không tồn tại" });
            }
            return Ok(new { message = "Cập nhật công ty thành công" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            var success = await _companyService.DeleteCompanyAsync(id);
            if (!success)
            {
                return NotFound(new { message = "Công ty không tồn tại" });
            }
            return Ok(new { message = "Xóa công ty thành công" });
        }
    }
}