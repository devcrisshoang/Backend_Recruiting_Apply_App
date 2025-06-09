using Microsoft.AspNetCore.Mvc;
using Backend_Recruiting_Apply_App.Data.Entities;
using Backend_Recruiting_Apply_App.Services;

namespace Backend_Recruiting_Apply_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResumeController : ControllerBase
    {
        private readonly IResumeService _resumeService;

        public ResumeController(IResumeService resumeService)
        {
            _resumeService = resumeService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Resume>>> GetResume()
        {
            var resumes = await _resumeService.GetAllResumesAsync();

            return Ok(resumes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Resume>> GetResume(int id)
        {
            var resume = await _resumeService.GetResumeByIdAsync(id);
            if (resume == null)
            {
                return NotFound();
            }
            return Ok(resume);
        }

        [HttpGet("applicantID/{id}")]
        public async Task<ActionResult> GetResumeByApplicantID(int id)
        {
            var resumes = await _resumeService.GetResumesByApplicantIdAsync(id);
            if (resumes == null || !resumes.Any())
            {
                return NotFound(new { message = "No resumes found for the given applicant ID" });
            }
            return Ok(resumes);
        }

        [HttpPost]
        public async Task<ActionResult<Resume>> CreateResume(Resume resume)
        {
            var createdResume = await _resumeService.CreateResumeAsync(resume);
            return CreatedAtAction(nameof(GetResume), new { id = createdResume.ID }, createdResume);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateResume(int id, Resume resume)
        {
            if (id != resume.ID)
            {
                return BadRequest();
            }

            var result = await _resumeService.UpdateResumeAsync(id, resume);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPut("{id}/image")]
        public async Task<IActionResult> UpdateResumeImage(int id, [FromBody] UpdateResumeImageDto dto)
        {
            var result = await _resumeService.UpdateResumeImageAsync(id, dto);
            if (!result)
            {
                return NotFound(new { message = "Resume not found" });
            }
            return NoContent();
        }

        [HttpPut("{id}/is-delete")]
        public async Task<IActionResult> UpdateResumeIsDelete(int id, [FromBody] UpdateResumeIsDeleteDto dto)
        {
            var result = await _resumeService.UpdateResumeIsDeleteAsync(id, dto);
            if (!result)
            {
                return NotFound(new { message = "Resume not found" });
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteResume(int id)
        {
            var result = await _resumeService.DeleteResumeAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("all")]
        public async Task<IActionResult> DeleteAllResumes()
        {
            try
            {
                var result = await _resumeService.DeleteAllResumesAsync();
                if (!result)
                {
                    return NotFound(new { message = "No resumes found to delete" });
                }
                return Ok(new { message = "All resumes have been deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting all resumes", error = ex.Message });
            }
        }
    }
}