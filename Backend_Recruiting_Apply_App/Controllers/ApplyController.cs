using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend_Recruiting_Apply_App.Data.Entities;
using SystemAPIdotnet.Data;

namespace Backend_Recruiting_Apply_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplyController : ControllerBase
    {
        private readonly RAADbContext _context;

        public ApplyController(RAADbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Apply>>> GetApplicantJob()
        {
            return await _context.ApplicantJob.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Apply>> GetApplicantJob(int id)
        {
            var applicantJob = await _context.ApplicantJob.FindAsync(id);
            if (applicantJob == null)
            {
                return NotFound();
            }
            return applicantJob;
        }

        [HttpPost]
        public async Task<ActionResult<Apply>> CreateApplicantJob(Apply applicantJob)
        {
            _context.ApplicantJob.Add(applicantJob);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetApplicantJob), new { id = applicantJob.ID }, applicantJob);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateApplicantJob(int id, Apply applicantJob)
        {
            if (id != applicantJob.ID)
            {
                return BadRequest();
            }

            _context.Entry(applicantJob).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.ApplicantJob.Any(e => e.ID == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApplicantJob(int id)
        {
            var applicantJob = await _context.ApplicantJob.FindAsync(id);
            if (applicantJob == null)
            {
                return NotFound();
            }

            _context.ApplicantJob.Remove(applicantJob);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
