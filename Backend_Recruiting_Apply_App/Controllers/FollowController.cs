using Backend_Recruiting_Apply_App.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SystemAPIdotnet.Data;

namespace Backend_Recruiting_Apply_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FollowController : ControllerBase
    {
        private readonly RAADbContext _context;
        public FollowController(RAADbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Follow>>> GetFollow()
        {
            return await _context.Follow.ToListAsync();
        }
        [HttpGet("{applicantId}/{companyId}")]
        public async Task<ActionResult<Follow>> GetFollowByApplicantId(int applicantId, int companyId)
        {
            var follow = await _context.Follow
                .FirstOrDefaultAsync(j => j.ApplicantID == applicantId && j.CompanyID == companyId);

            return Ok(follow);
        }
        [HttpGet("applicant/{id}")]
        public async Task<ActionResult<IEnumerable<Follow>>> GetFollowedCompanyByApplicantId(int id)
        {
            var follow = await _context.Follow
                .Where(j => j.ApplicantID.Equals(id))
                .ToListAsync();
            return Ok(follow);
        }

        [HttpGet("companyList/applicant/{id}")]
        public async Task<ActionResult<IEnumerable<Company>>> GetFollowedCompanyListByApplicantId(int id)
        {
            var companies = await _context.Follow
                .Where(f => f.ApplicantID == id) // Lấy tất cả bản ghi Follow có ApplicantID khớp với id
                .Join(
                    _context.Company, // Bảng Company
                    follow => follow.CompanyID, // Khóa ngoại từ Follow
                    company => company.ID, // Khóa chính từ Company
                    (follow, company) => company // Chỉ lấy thông tin Company
                )
                .ToListAsync();

            return Ok(companies);
        }
        [HttpPost]
        public async Task<ActionResult<Follow>> CreateFollow(Follow follow)
        {
            _context.Follow.Add(follow);
            await _context.SaveChangesAsync();
            return Ok(follow);
        }
        [HttpDelete("{applicantId}/{companyId}")]
        public async Task<ActionResult<Follow>> DeleteFollow(int applicantId, int companyId)
        {
            var follow = await _context.Follow.FirstOrDefaultAsync(j => j.ApplicantID == applicantId && j.CompanyID == companyId);
            if (follow == null)
            {
                return Ok();
            }
            _context.Follow.Remove(follow);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
