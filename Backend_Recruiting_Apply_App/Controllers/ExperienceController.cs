using Microsoft.AspNetCore.Mvc;
using Backend_Recruiting_Apply_App.Data.Entities;
using Backend_Recruiting_Apply_App.Services;

namespace Backend_Recruiting_Apply_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExperienceController : ControllerBase
    {
        private readonly IExperienceService _experienceService;

        public ExperienceController(IExperienceService experienceService)
        {
            _experienceService = experienceService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Experience>>> GetExperience()
        {
            var experiences = await _experienceService.GetAllExperiencesAsync();
            return Ok(experiences);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Experience>> GetExperience(int id)
        {
            var experience = await _experienceService.GetExperienceByIdAsync(id);
            if (experience == null)
            {
                return NotFound();
            }
            return Ok(experience);
        }

        [HttpPost]
        public async Task<ActionResult<Experience>> CreateExperience(Experience experience)
        {
            var createdExperience = await _experienceService.CreateExperienceAsync(experience);
            return CreatedAtAction(nameof(GetExperience), new { id = createdExperience.ID }, createdExperience);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExperience(int id, Experience experience)
        {
            var success = await _experienceService.UpdateExperienceAsync(id, experience);
            if (!success)
            {
                return id != experience.ID ? BadRequest() : NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExperience(int id)
        {
            var success = await _experienceService.DeleteExperienceAsync(id);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}