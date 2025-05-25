using Microsoft.AspNetCore.Mvc;
using Backend_Recruiting_Apply_App.Data.Entities;
using Backend_Recruiting_Apply_App.Services;

namespace Backend_Recruiting_Apply_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategory()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound(new { Message = $"Category with ID {id} not found." });
            }
            return Ok(category);
        }

        [HttpGet("resume/{id}")]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategoriesByResume(int id)
        {
            var categories = await _categoryService.GetCategoriesByResumeIdAsync(id);
            if (categories == null || !categories.Any())
            {
                return NotFound(new { Message = $"No categories found for resume with ID {id}." });
            }
            return Ok(categories);
        }

        [HttpPost]
        public async Task<ActionResult<Category>> CreateCategory([FromBody] Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdCategory = await _categoryService.CreateCategoryAsync(category);
            return CreatedAtAction(nameof(GetCategory), new { id = createdCategory.ID }, createdCategory);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] Category category)
        {
            var success = await _categoryService.UpdateCategoryAsync(id, category);
            if (!success)
            {
                return id != category.ID
                    ? BadRequest(new { Message = "ID in URL does not match ID in body." })
                    : NotFound(new { Message = $"Category with ID {id} not found." });
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var success = await _categoryService.DeleteCategoryAsync(id);
            if (!success)
            {
                return NotFound(new { Message = $"Category with ID {id} not found." });
            }
            return NoContent();
        }

        [HttpDelete("resume/{resumeId}")]
        public async Task<IActionResult> DeleteCategoriesByResume(int resumeId)
        {
            try
            {
                var success = await _categoryService.DeleteCategoriesByResumeIdAsync(resumeId);
                if (!success)
                {
                    return NotFound(new { Message = $"No categories found for resume with ID {resumeId}." });
                }
                return Ok(new { Message = $"All categories for resume with ID {resumeId} have been deleted." });
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
                var success = await _categoryService.DeleteAllCategoriesAsync();
                if (!success)
                {
                    return NotFound(new { Message = "No categories found to delete." });
                }
                return Ok(new { Message = "All categories have been deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while deleting all categories.", Error = ex.Message });
            }
        }
    }
}