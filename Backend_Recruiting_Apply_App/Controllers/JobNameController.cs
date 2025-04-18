using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend_Recruiting_Apply_App.Data.Entities;
using TopCVSystemAPIdotnet.Data;

namespace Backend_Recruiting_Apply_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobNameController : ControllerBase
    {
        private readonly RAADbContext _context;

        public JobNameController(RAADbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobName>>> GetJobName()
        {
            return await _context.JobName.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<JobName>> GetJobName(int id)
        {
            var jobName = await _context.JobName.FindAsync(id);

            if (jobName == null)
                return NotFound();

            return jobName;
        }

        [HttpPost]
        public async Task<ActionResult<JobName>> CreateJobName(JobName jobName)
        {
            _context.JobName.Add(jobName);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetJobName), new { id = jobName.ID }, jobName);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJobName(int id, JobName jobName)
        {
            if (id != jobName.ID)
                return BadRequest();

            _context.Entry(jobName).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJobName(int id)
        {
            var jobName = await _context.JobName.FindAsync(id);
            if (jobName == null)
                return NotFound();

            _context.JobName.Remove(jobName);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
