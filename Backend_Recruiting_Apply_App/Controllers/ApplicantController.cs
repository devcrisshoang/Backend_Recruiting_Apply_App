using Microsoft.AspNetCore.Mvc;
using Backend_Recruiting_Apply_App.Data.Entities;
using Backend_Recruiting_Apply_App.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend_Recruiting_Apply_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicantController : ControllerBase
    {
        private readonly IApplicantService _applicantService;

        public ApplicantController(IApplicantService applicantService)
        {
            _applicantService = applicantService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Applicant>>> GetApplicant()
        {
            var applicants = await _applicantService.GetAllApplicantsAsync();
            return Ok(applicants);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Applicant>> GetApplicant(int id)
        {
            var applicant = await _applicantService.GetApplicantByIdAsync(id);
            if (applicant == null)
            {
                return NotFound(new { message = "Applicant not found" });
            }
            return Ok(applicant);
        }

        [HttpGet("check-applicant/{userId}")]
        public async Task<IActionResult> CheckApplicant(int userId)
        {
            var applicant = await _applicantService.GetApplicantByUserIdAsync(userId);
            if (applicant == null)
            {
                return NotFound(new { message = "Applicant profile not found" });
            }
            return Ok(applicant);
        }

        [HttpPost]
        public async Task<ActionResult<Applicant>> CreateApplicant(Applicant applicant)
        {
            var createdApplicant = await _applicantService.CreateApplicantAsync(applicant);
            return CreatedAtAction(nameof(GetApplicant), new { id = createdApplicant.ID }, createdApplicant);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateApplicant(int id, Applicant applicant)
        {
            var success = await _applicantService.UpdateApplicantAsync(id, applicant);
            if (!success)
            {
                return BadRequest(new { message = "ID mismatch" });
            }
            return NoContent();
        }

        [HttpPut("{id}/experience")]
        public async Task<IActionResult> UpdateExperience(int id, [FromBody] string experience)
        {
            var success = await _applicantService.UpdateExperienceAsync(id, experience);
            if (!success)
            {
                return NotFound(new { message = "Applicant not found" });
            }
            return NoContent();
        }

        [HttpPut("{id}/job")]
        public async Task<IActionResult> UpdateJob(int id, [FromBody] string job)
        {
            var success = await _applicantService.UpdateJobAsync(id, job);
            if (!success)
            {
                return NotFound(new { message = "Applicant not found" });
            }
            return NoContent();
        }

        [HttpPut("{id}/location")]
        public async Task<IActionResult> UpdateLocation(int id, [FromBody] string location)
        {
            var success = await _applicantService.UpdateLocationAsync(id, location);
            if (!success)
            {
                return NotFound(new { message = "Applicant not found" });
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApplicant(int id)
        {
            var success = await _applicantService.DeleteApplicantAsync(id);
            if (!success)
            {
                return NotFound(new { message = "Applicant not found" });
            }
            return NoContent();
        }
    }
}