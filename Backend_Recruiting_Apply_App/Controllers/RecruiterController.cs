using Backend_Recruiting_Apply_App.Data;
using Backend_Recruiting_Apply_App.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TopCVSystemAPIdotnet.Data;

namespace Backend_Recruiting_Apply_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecruiterController : ControllerBase
    {
        private readonly RAADbContext _context;

        public RecruiterController(RAADbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Recruiter>>> GetRecruiter()
        {
            return await _context.Recruiter.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Recruiter>> GetRecruiter(int id)
        {
            var recruiter = await _context.Recruiter.FindAsync(id);

            if (recruiter == null)
            {
                return NotFound(new { message = "Recruiter not found" });
            }

            return recruiter;
        }

        [HttpPost]
        public async Task<ActionResult<Recruiter>> CreateRecruiter(Recruiter recruiter)
        {
            _context.Recruiter.Add(recruiter);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRecruiter), new { id = recruiter.ID }, recruiter);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRecruiter(int id, Recruiter recruiter)
        {
            if (id != recruiter.ID)
            {
                return BadRequest(new { message = "ID mismatch" });
            }

            _context.Entry(recruiter).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecruiterExists(id))
                {
                    return NotFound(new { message = "Recruiter not found" });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecruiter(int id)
        {
            var recruiter = await _context.Recruiter.FindAsync(id);
            if (recruiter == null)
            {
                return NotFound(new { message = "Recruiter not found" });
            }

            _context.Recruiter.Remove(recruiter);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RecruiterExists(int id)
        {
            return _context.Recruiter.Any(e => e.ID == id);
        }
    }
}
