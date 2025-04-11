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

        [HttpGet("applicantID/{id}")]
        public async Task<ActionResult> GetResumeByApplicantID(int id)
        {
            var resumes = await _context.Resume
                .Where(r => r.Applicant_ID == id)
                .ToListAsync();

            if (resumes == null || resumes.Count == 0)
            {
                return NotFound(new { message = "No resumes found for the given applicant ID" });
            }

            return Ok(resumes);
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


        [HttpPut("{id}/image")]
        public async Task<IActionResult> UpdateResumeImage(int id, [FromBody] byte[] image)
        {
            var resume = await _context.Resume.FindAsync(id);
            if (resume == null)
            {
                return NotFound(new { message = "Resume not found" });
            }

            resume.Image = image;
            await _context.SaveChangesAsync();

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

        [HttpDelete("all")]
        public async Task<IActionResult> DeleteAllResumes()
        {
            try
            {
                var resumes = await _context.Resume.ToListAsync();
                if (resumes == null || resumes.Count == 0)
                {
                    return NotFound(new { message = "No resumes found to delete" });
                }

                _context.Resume.RemoveRange(resumes);
                await _context.SaveChangesAsync();

                return Ok(new { message = "All resumes have been deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting all resumes", error = ex.Message });
            }
        }
    }
    public class UpdateResumeImageDto
    {
        public byte[] Image { get; set; } = [];
    }
}
