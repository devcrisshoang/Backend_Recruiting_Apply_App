using Backend_Recruiting_Apply_App.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TopCVSystemAPIdotnet.Data;

namespace Backend_Recruiting_Apply_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly RAADbContext _context;

        // Constructor nhận DbContext qua Dependency Injection
        public CategoryController(RAADbContext context)
        {
            _context = context;
        }

        // GET: api/Category
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategory()
        {
            var Category = await _context.Category.ToListAsync();
            return Ok(Category);
        }

        // GET: api/Category/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _context.Category.FindAsync(id);

            if (category == null)
            {
                return NotFound(new { Message = $"Category with ID {id} not found." });
            }

            return Ok(category);
        }

        // GET: api/Category/resume/5
        [HttpGet("resume/{id}")]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategoriesByResume(int id)
        {
            var categories = await _context.Category
                .Where(a => a.Resume_ID == id)
                .ToListAsync();

            if (categories == null || !categories.Any())
            {
                return NotFound(new { Message = $"No categories found for resume with ID {id}." });
            }

            return Ok(categories);
        }

        // POST: api/Category
        [HttpPost]
        public async Task<ActionResult<Category>> CreateCategory([FromBody] Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Category.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategory), new { id = category.ID }, category);
        }

        // PUT: api/Category/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] Category category)
        {
            if (id != category.ID)
            {
                return BadRequest(new { Message = "ID in URL does not match ID in body." });
            }

            var existingCategory = await _context.Category.FindAsync(id);
            if (existingCategory == null)
            {
                return NotFound(new { Message = $"Category with ID {id} not found." });
            }

            // Cập nhật các thuộc tính
            existingCategory.Name = category.Name;
            existingCategory.Description = category.Description;
            existingCategory.Resume_ID = category.Resume_ID;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
                {
                    return NotFound(new { Message = $"Category with ID {id} no longer exists." });
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Category/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Category.FindAsync(id);
            if (category == null)
            {
                return NotFound(new { Message = $"Category with ID {id} not found." });
            }

            _context.Category.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Hàm hỗ trợ kiểm tra sự tồn tại của Category
        private bool CategoryExists(int id)
        {
            return _context.Category.Any(e => e.ID == id);
        }

        [HttpDelete("resume/{resumeID}")]
        public async Task<IActionResult> DeleteCategoriesByResume(int resumeID)
        {
            try
            {
                // Tìm tất cả các Category có Resume_ID khớp
                var categories = await _context.Category
                    .Where(c => c.Resume_ID == resumeID)
                    .ToListAsync();

                if (categories == null || !categories.Any())
                {
                    return NotFound(new { Message = $"No categories found for resume with ID {resumeID}." });
                }

                // Xóa tất cả các Category tìm được
                _context.Category.RemoveRange(categories);
                await _context.SaveChangesAsync();

                return Ok(new { Message = $"All categories for resume with ID {resumeID} have been deleted." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while deleting categories.", Error = ex.Message });
            }
        }

        [HttpDelete("all")]
        public async Task<IActionResult> DeleteAllCategory()
        {
            try
            {
                var categories = await _context.Category.ToListAsync();
                if (categories == null || categories.Count == 0)
                {
                    return NotFound(new { message = "No categories found to delete" });
                }

                _context.Category.RemoveRange(categories);
                await _context.SaveChangesAsync();

                return Ok(new { message = "All categories have been deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting all categories", error = ex.Message });
            }
        }
    }
}