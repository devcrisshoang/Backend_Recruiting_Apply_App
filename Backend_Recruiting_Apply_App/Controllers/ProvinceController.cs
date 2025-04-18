using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend_Recruiting_Apply_App.Data.Entities;
using SystemAPIdotnet.Data;

namespace Backend_Recruiting_Apply_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProvinceController : ControllerBase
    {
        private readonly RAADbContext _context;

        public ProvinceController(RAADbContext context)
        {
            _context = context;
        }

        // GET: api/Province
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Province>>> GetProvince()
        {
            return await _context.Province.ToListAsync();
        }

        // GET: api/Province/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Province>> GetProvince(int id)
        {
            var province = await _context.Province.FindAsync(id);

            if (province == null)
                return NotFound();

            return province;
        }

        // POST: api/Province
        [HttpPost]
        public async Task<ActionResult<Province>> CreateProvince(Province province)
        {
            _context.Province.Add(province);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProvince), new { id = province.ID }, province);
        }

        // PUT: api/Province/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProvince(int id, Province province)
        {
            if (id != province.ID)
                return BadRequest();

            _context.Entry(province).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProvinceExists(id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Province/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProvince(int id)
        {
            var province = await _context.Province.FindAsync(id);
            if (province == null)
                return NotFound();

            _context.Province.Remove(province);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProvinceExists(int id)
        {
            return _context.Province.Any(e => e.ID == id);
        }
    }
}