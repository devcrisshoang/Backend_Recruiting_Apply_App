using Backend_Recruiting_Apply_App.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SystemAPIdotnet.Data;

namespace Backend_Recruiting_Apply_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly RAADbContext _context;

        public CompanyController(RAADbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Company>>> GetCompany()
        {
            var companies = await _context.Company.ToListAsync();
            return Ok(companies);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Company>> GetCompany(int id)
        {
            var company = await _context.Company.FindAsync(id);

            if (company == null)
            {
                return NotFound(new { message = "Công ty không tồn tại" });
            }

            return Ok(company);
        }

        [HttpGet("recruiter/{recruiterId}")]
        public async Task<ActionResult<Company>> GetCompanyByRecruiterId(int recruiterId)
        {
            // Tìm Recruiter theo recruiterId
            var recruiter = await _context.Recruiter
                .FirstOrDefaultAsync(r => r.ID == recruiterId);

            if (recruiter == null)
            {
                return NotFound(new { message = "Nhà tuyển dụng không tồn tại" });
            }

            // Kiểm tra Company_ID có phải là 0 không
            if (recruiter.Company_ID == 0)
            {
                return NotFound(new { message = "Nhà tuyển dụng không được liên kết với công ty nào" });
            }

            // Tìm Company theo CompanyId từ Recruiter
            var company = await _context.Company
                .FirstOrDefaultAsync(c => c.ID == recruiter.Company_ID);

            if (company == null)
            {
                return NotFound(new { message = "Công ty không tồn tại" });
            }

            return Ok(company);
        }

        [HttpPost]
        public async Task<ActionResult<Company>> CreateCompany(Company company)
        {
            _context.Company.Add(company);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCompany), new { id = company.ID }, new { message = "Tạo công ty thành công", data = company });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCompany(int id, Company company)
        {
            if (id != company.ID)
            {
                return BadRequest(new { message = "ID không khớp" });
            }

            _context.Entry(company).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyExists(id))
                {
                    return NotFound(new { message = "Công ty không tồn tại" });
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { message = "Cập nhật công ty thành công" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            var company = await _context.Company.FindAsync(id);
            if (company == null)
            {
                return NotFound(new { message = "Công ty không tồn tại" });
            }

            _context.Company.Remove(company);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa công ty thành công" });
        }

        private bool CompanyExists(int id)
        {
            return _context.Company.Any(e => e.ID == id);
        }
    }
}