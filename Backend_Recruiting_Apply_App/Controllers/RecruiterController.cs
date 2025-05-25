using Microsoft.AspNetCore.Mvc;
using Backend_Recruiting_Apply_App.Data.Entities;
using Backend_Recruiting_Apply_App.Services;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Backend_Recruiting_Apply_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecruiterController : ControllerBase
    {
        private readonly IRecruiterService _recruiterService;

        public RecruiterController(IRecruiterService recruiterService)
        {
            _recruiterService = recruiterService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Recruiter>>> GetRecruiter()
        {
            var recruiters = await _recruiterService.GetAllRecruitersAsync();
            return Ok(recruiters);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Recruiter>> GetRecruiter(int id)
        {
            var recruiter = await _recruiterService.GetRecruiterByIdAsync(id);
            return Ok(recruiter);
        }

        [HttpGet("check-recruiter/{userId}")]
        public async Task<IActionResult> CheckRecruiter(int userId)
        {
            var recruiter = await _recruiterService.GetRecruiterByUserIdAsync(userId);
            return Ok(recruiter);
        }

        [HttpPost]
        public async Task<ActionResult<Recruiter>> CreateRecruiter(Recruiter recruiter)
        {
            var createdRecruiter = await _recruiterService.CreateRecruiterAsync(recruiter);
            return CreatedAtAction(nameof(GetRecruiter), new { id = createdRecruiter.ID }, createdRecruiter);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRecruiter(int id, Recruiter recruiter)
        {
            if (id != recruiter.ID)
            {
                return BadRequest(new { message = "ID mismatch" });
            }

            var result = await _recruiterService.UpdateRecruiterAsync(id, recruiter);
            if (!result)
            {
                return Ok(null);
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecruiter(int id)
        {
            var result = await _recruiterService.DeleteRecruiterAsync(id);
            if (!result)
            {
                return Ok(null);
            }

            return NoContent();
        }
    }
}