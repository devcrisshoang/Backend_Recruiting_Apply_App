using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend_Recruiting_Apply_App.Data.Entities;
using TopCVSystemAPIdotnet.Data;

namespace Backend_Recruiting_Apply_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResumeController : ControllerBase
    {
        private readonly RAADbContext _context;

        public ResumeController(RAADbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Resume>>> GetResume()
        {
            return await _context.Resume.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Resume>> GetResume(int id)
        {
            var resume = await _context.Resume.FindAsync(id);
            if (resume == null)
            {
                return NotFound();
            }
            return resume;
        }

        [HttpPost]
        public async Task<ActionResult<Resume>> CreateResume(Resume resume)
        {
            _context.Resume.Add(resume);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetResume), new { id = resume.ID }, resume);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateResume(int id, Resume resume)
        {
            if (id != resume.ID)
            {
                return BadRequest();
            }

            _context.Entry(resume).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Resume.Any(e => e.ID == id))
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
        public async Task<IActionResult> DeleteResume(int id)
        {
            var resume = await _context.Resume.FindAsync(id);
            if (resume == null)
            {
                return NotFound();
            }

            _context.Resume.Remove(resume);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
