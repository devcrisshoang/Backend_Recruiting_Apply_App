using Microsoft.AspNetCore.Mvc;
using Backend_Recruiting_Apply_App.Data.Entities;
using Backend_Recruiting_Apply_App.Services;

namespace Backend_Recruiting_Apply_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FieldController : ControllerBase
    {
        private readonly IFieldService _fieldService;

        public FieldController(IFieldService fieldService)
        {
            _fieldService = fieldService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Field>>> GetField()
        {
            var fields = await _fieldService.GetAllFieldsAsync();
            return Ok(fields);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Field>> GetField(int id)
        {
            var field = await _fieldService.GetFieldByIdAsync(id);
            if (field == null)
            {
                return NotFound();
            }
            return Ok(field);
        }

        [HttpPost]
        public async Task<ActionResult<Field>> CreateField(Field field)
        {
            var createdField = await _fieldService.CreateFieldAsync(field);
            return CreatedAtAction(nameof(GetField), new { id = createdField.ID }, createdField);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateField(int id, Field field)
        {
            var success = await _fieldService.UpdateFieldAsync(id, field);
            if (!success)
            {
                return id != field.ID ? BadRequest() : NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteField(int id)
        {
            var success = await _fieldService.DeleteFieldAsync(id);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}