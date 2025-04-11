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


        [HttpGet("check-applicant/{userId}")]
        public async Task<IActionResult> CheckApplicant(int userId)
        {
            var applicant = await _context.Applicant.FirstOrDefaultAsync(a => a.User_ID == userId);

            if (applicant == null)
            {
                return NotFound(new { message = "Applicant profile not found" });
            }

            return Ok(applicant);
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

        [HttpPut("{id}/experience")]
        public async Task<IActionResult> UpdateExperience(int id, [FromBody] string experience)
        {
            var applicant = await _context.Applicant.FindAsync(id);
            if (applicant == null)
            {
                return NotFound(new { message = "Applicant not found" });
            }

            applicant.Experience = experience;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Applicant.Any(e => e.ID == id))
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

        [HttpPut("{id}/job")]
        public async Task<IActionResult> UpdateJob(int id, [FromBody] string job)
        {
            var applicant = await _context.Applicant.FindAsync(id);
            if (applicant == null)
            {
                return NotFound(new { message = "Applicant not found" });
            }

            applicant.Job = job;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Applicant.Any(e => e.ID == id))
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

        [HttpPut("{id}/location")]
        public async Task<IActionResult> UpdateLocation(int id, [FromBody] string location)
        {
            var applicant = await _context.Applicant.FindAsync(id);
            if (applicant == null)
            {
                return NotFound(new { message = "Applicant not found" });
            }

            applicant.Location = location;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Applicant.Any(e => e.ID == id))
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
