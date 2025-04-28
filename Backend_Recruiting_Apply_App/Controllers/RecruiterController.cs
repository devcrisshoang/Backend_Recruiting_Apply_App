using Backend_Recruiting_Apply_App.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SystemAPIdotnet.Data;

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
            var recruiters = await _context.Recruiter.ToListAsync();
            return Ok(recruiters ?? new List<Recruiter>());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Recruiter>> GetRecruiter(int id)
        {
            var recruiter = await _context.Recruiter.FindAsync(id);
            return Ok(recruiter ?? null);
        }

        [HttpGet("check-recruiter/{userId}")]
        public async Task<IActionResult> CheckRecruiter(int userId)
        {
            var recruiter = await _context.Recruiter.FirstOrDefaultAsync(r => r.User_ID == userId);
            return Ok(recruiter ?? null);
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

            var existingRecruiter = await _context.Recruiter.FindAsync(id);
            if (existingRecruiter == null)
            {
                return Ok(null); // Trả về 200 rỗng thay vì 404
            }

            _context.Entry(existingRecruiter).CurrentValues.SetValues(recruiter);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecruiterExists(id))
                {
                    return Ok(null); // Trả về 200 rỗng thay vì 404
                }
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecruiter(int id)
        {
            var recruiter = await _context.Recruiter.FindAsync(id);
            if (recruiter == null)
            {
                return Ok(null); // Trả về 200 rỗng thay vì 404
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