using Backend_Recruiting_Apply_App.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TopCVSystemAPIdotnet.Data;


namespace Backend_Recruiting_Apply_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicantController : ControllerBase
    {
        private readonly RAADbContext _context;

        public ApplicantController(RAADbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Applicant>>> GetApplicant()
        {
            return await _context.Applicant.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Applicant>> GetApplicant(int id)
        {
            var applicant = await _context.Applicant.FindAsync(id);

            if (applicant == null)
            {
                return NotFound(new { message = "Applicant not found" });
            }

            return applicant;
        }

        [HttpPost]
        public async Task<ActionResult<Applicant>> CreateApplicant(Applicant applicant)
        {
            _context.Applicant.Add(applicant);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetApplicant), new { id = applicant.ID }, applicant);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateApplicant(int id, Applicant applicant)
        {
            if (id != applicant.ID)
            {
                return BadRequest(new { message = "ID mismatch" });
            }

            _context.Entry(applicant).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApplicantExists(id))
                {
                    return NotFound(new { message = "Applicant not found" });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApplicant(int id)
        {
            var applicant = await _context.Applicant.FindAsync(id);
            if (applicant == null)
            {
                return NotFound(new { message = "Applicant not found" });
            }

            _context.Applicant.Remove(applicant);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ApplicantExists(int id)
        {
            return _context.Applicant.Any(e => e.ID == id);
        }
    }
}
