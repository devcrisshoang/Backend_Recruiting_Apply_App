using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend_Recruiting_Apply_App.Data.Entities;
using SystemAPIdotnet.Data;

namespace Backend_Recruiting_Apply_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FieldController : ControllerBase
    {
        private readonly RAADbContext _context;

        public FieldController(RAADbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Field>>> GetField()
        {
            return await _context.Field.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Field>> GetField(int id)
        {
            var field = await _context.Field.FindAsync(id);

            if (field == null)
                return NotFound();

            return field;
        }

        [HttpPost]
        public async Task<ActionResult<Field>> CreateField(Field field)
        {
            _context.Field.Add(field);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetField), new { id = field.ID }, field);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateField(int id, Field field)
        {
            if (id != field.ID)
                return BadRequest();

            _context.Entry(field).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteField(int id)
        {
            var field = await _context.Field.FindAsync(id);
            if (field == null)
                return NotFound();

            _context.Field.Remove(field);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
