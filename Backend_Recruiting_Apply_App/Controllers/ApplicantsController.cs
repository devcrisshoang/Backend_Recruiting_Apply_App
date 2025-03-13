using Microsoft.AspNetCore.Mvc;
using Backend_Recruiting_Apply_App.Data.Entities;
using Microsoft.EntityFrameworkCore;
using TopCVSystemAPIdotnet.Data;

namespace Backend_Recruiting_Apply_App.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicantController : ControllerBase
    {
        private readonly RAADbContext _context;

        public ApplicantController(RAADbContext context)
        {
            _context = context;
        }

        // GET: api/Applicant
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Applicant>>> GetApplicants()
        {
            return await _context.Set<Applicant>().ToListAsync();
        }

        // GET: api/Applicant/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Applicant>> GetApplicant(int id)
        {
            var applicant = await _context.Set<Applicant>().FindAsync(id);

            if (applicant == null)
            {
                return NotFound();
            }

            return applicant;
        }

        // POST: api/Applicant
        [HttpPost]
        public async Task<ActionResult<Applicant>> CreateApplicant(Applicant applicant)
        {
            _context.Set<Applicant>().Add(applicant);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetApplicant), new { id = applicant.id }, applicant);
        }

        // PUT: api/Applicant/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateApplicant(int id, [FromBody] Applicant applicant)
        {
            if (applicant == null)
            {
                return BadRequest("Applicant object is null.");
            }

            // Truy vấn ứng viên từ database bằng id từ URL
            var existingApplicant = await _context.Set<Applicant>().FindAsync(id);
            if (existingApplicant == null)
            {
                return NotFound($"Applicant with ID {id} not found.");
            }

            // Cập nhật giá trị từ đối tượng mới (bỏ qua id)
            existingApplicant.name = applicant.name;
            existingApplicant.email = applicant.email;
            existingApplicant.phone_number = applicant.phone_number;
            existingApplicant.job = applicant.job;
            existingApplicant.working_location = applicant.working_location;
            existingApplicant.experience = applicant.experience;
            existingApplicant.premium = applicant.premium;

            // Đánh dấu đối tượng đã chỉnh sửa
            _context.Entry(existingApplicant).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApplicantExists(id))
                {
                    return NotFound($"Applicant with ID {id} no longer exists.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        // DELETE: api/Applicant/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApplicant(int id)
        {
            var applicant = await _context.Set<Applicant>().FindAsync(id);
            if (applicant == null)
            {
                return NotFound();
            }

            _context.Set<Applicant>().Remove(applicant);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ApplicantExists(int id)
        {
            return _context.Set<Applicant>().Any(e => e.id == id);
        }
    }
}
