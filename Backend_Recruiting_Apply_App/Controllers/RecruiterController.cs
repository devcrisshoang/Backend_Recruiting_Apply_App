using Microsoft.AspNetCore.Mvc;
using Backend_Recruiting_Apply_App.Data.Entities;
using Microsoft.EntityFrameworkCore;
using TopCVSystemAPIdotnet.Data;

namespace Backend_Recruiting_Apply_App.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecruiterController : ControllerBase
    {
        private readonly RAADbContext _context;

        public RecruiterController(RAADbContext context)
        {
            _context = context;
        }

        // GET: api/Recruiter
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Recruiter>>> GetRecruiters()
        {
            return await _context.Set<Recruiter>().ToListAsync();
        }

        // GET: api/Recruiter/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Recruiter>> GetRecruiter(int id)
        {
            var recruiter = await _context.Set<Recruiter>().FindAsync(id);

            if (recruiter == null)
            {
                return NotFound();
            }

            return recruiter;
        }

        // POST: api/Recruiter
        [HttpPost]
        public async Task<ActionResult<Recruiter>> CreateRecruiter(Recruiter recruiter)
        {
            _context.Set<Recruiter>().Add(recruiter);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRecruiter), new { id = recruiter.id }, recruiter);
        }

        // PUT: api/Recruiter/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRecruiter(int id, [FromBody] Recruiter recruiter)
        {
            if (recruiter == null)
            {
                return BadRequest("Recruiter object is null.");
            }

            // Truy vấn recruiter từ database bằng id từ URL
            var existingRecruiter = await _context.Set<Recruiter>().FindAsync(id);
            if (existingRecruiter == null)
            {
                return NotFound($"Recruiter with ID {id} not found.");
            }

            // Cập nhật giá trị từ đối tượng mới (bỏ qua id)
            existingRecruiter.name = recruiter.name;
            existingRecruiter.phone_number = recruiter.phone_number;
            existingRecruiter.email = recruiter.email;
            existingRecruiter.front_image = recruiter.front_image;
            existingRecruiter.back_image = recruiter.back_image;

            // Đánh dấu đối tượng đã chỉnh sửa
            _context.Entry(existingRecruiter).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecruiterExists(id))
                {
                    return NotFound($"Recruiter with ID {id} no longer exists.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        // DELETE: api/Recruiter/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecruiter(int id)
        {
            var recruiter = await _context.Set<Recruiter>().FindAsync(id);
            if (recruiter == null)
            {
                return NotFound();
            }

            _context.Set<Recruiter>().Remove(recruiter);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RecruiterExists(int id)
        {
            return _context.Set<Recruiter>().Any(e => e.id == id);
        }
    }
}
