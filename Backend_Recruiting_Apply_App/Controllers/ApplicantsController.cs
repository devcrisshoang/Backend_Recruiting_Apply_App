using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend_Recruiting_Apply_App.Data.Entities;
using TopCVSystemAPIdotnet.Data;

namespace Backend_Recruiting_Apply_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicantController : ControllerBase
    {
        private readonly RRADbContext _context;

        public ApplicantController(RRADbContext context)
        {
            _context = context;
        }

        // GET: api/Applicant
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Applicant>>> GetApplicant()
        {
            return await _context.Applicant.ToListAsync();
        }

        // GET: api/Applicant/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Applicant>> GetApplicant(int id)
        {
            var applicant = await _context.Applicant.FindAsync(id);

            if (applicant == null)
            {
                return NotFound();
            }

            return applicant;
        }

        // POST: api/Applicant
        [HttpPost]
        public async Task<ActionResult<Applicant>> PostApplicant(Applicant applicant)
        {
            _context.Applicant.Add(applicant);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetApplicant), new { id = applicant.id }, applicant);
        }

        // PUT: api/Applicant/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutApplicant(int id, Applicant applicant)
        {
            if (id != applicant.id)
            {
                return BadRequest();
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
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Applicant/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApplicant(int id)
        {
            var applicant = await _context.Applicant.FindAsync(id);
            if (applicant == null)
            {
                return NotFound();
            }

            _context.Applicant.Remove(applicant);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ApplicantExists(int id)
        {
            return _context.Applicant.Any(e => e.id == id);
        }
    }
}
