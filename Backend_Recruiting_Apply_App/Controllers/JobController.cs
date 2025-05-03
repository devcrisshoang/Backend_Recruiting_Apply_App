using Backend_Recruiting_Apply_App.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SystemAPIdotnet.Data;

namespace Backend_Recruiting_Apply_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly RAADbContext _context;

        public JobController(RAADbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Job>>> GetJob()
        {
            return await _context.Job.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Job>> GetJob(int id)
        {
            var job = await _context.Job.FindAsync(id);

            if (job == null)
                return Ok(null); // Trả về 200 OK với null thay vì 404

            return Ok(job);
        }

        [HttpGet("recruiter/{recruiterId}")]
        public async Task<ActionResult<IEnumerable<Job>>> GetJobsByRecruiterId(int recruiterId)
        {
            var jobs = await _context.Job
                .Where(j => j.Recruiter_ID == recruiterId)
                .ToListAsync();

            return Ok(jobs ?? new List<Job>()); // Trả về 200 OK với mảng rỗng nếu không tìm thấy
        }

        [HttpGet("company/{id}")]
        public async Task<ActionResult<IEnumerable<Job>>> GetJobByCompanyId(int id)
        {
            var jobs = await _context.Recruiter
                .Where(j => j.Company_ID == id)
                .Join(
                _context.Job,
                recruiter => recruiter.ID,
                job => job.Recruiter_ID,
                (recruiter, job) => job
                ).ToListAsync();

            return Ok(jobs ?? new List<Job>());
        }

        [HttpPost]
        public async Task<ActionResult<Job>> CreateJob(Job job)
        {
            _context.Job.Add(job);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetJob), new { id = job.ID }, job);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJob(int id, Job job)
        {
            if (id != job.ID)
                return BadRequest();

            _context.Entry(job).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JobExists(id))
                    return Ok(); // Trả về 200 OK thay vì 404
                else
                    throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            var job = await _context.Job.FindAsync(id);
            if (job == null)
                return Ok(); // Trả về 200 OK thay vì 404

            _context.Job.Remove(job);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool JobExists(int id)
        {
            return _context.Job.Any(e => e.ID == id);
        }
    }
}