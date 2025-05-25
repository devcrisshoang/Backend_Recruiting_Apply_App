using Microsoft.AspNetCore.Mvc;
using Backend_Recruiting_Apply_App.Data.Entities;
using Backend_Recruiting_Apply_App.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend_Recruiting_Apply_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly IJobService _jobService;

        public JobController(IJobService jobService)
        {
            _jobService = jobService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Job>>> GetJob()
        {
            var jobs = await _jobService.GetAllJobsAsync();
            return Ok(jobs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Job>> GetJob(int id)
        {
            var job = await _jobService.GetJobByIdAsync(id);
            return Ok(job); // Return 200 OK with null if not found, per original behavior
        }

        [HttpGet("recruiter/{recruiterId}")]
        public async Task<ActionResult<IEnumerable<Job>>> GetJobsByRecruiterId(int recruiterId)
        {
            var jobs = await _jobService.GetJobsByRecruiterIdAsync(recruiterId);
            return Ok(jobs ?? new List<Job>()); // Return 200 OK with empty list if not found
        }

        [HttpGet("company/{id}")]
        public async Task<ActionResult<IEnumerable<Job>>> GetJobByCompanyId(int id)
        {
            var jobs = await _jobService.GetJobsByCompanyIdAsync(id);
            return Ok(jobs ?? new List<Job>()); // Return 200 OK with empty list if not found
        }

        [HttpPost]
        public async Task<ActionResult<Job>> CreateJob(Job job)
        {
            var createdJob = await _jobService.CreateJobAsync(job);
            return CreatedAtAction(nameof(GetJob), new { id = createdJob.ID }, createdJob);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJob(int id, Job job)
        {
            var success = await _jobService.UpdateJobAsync(id, job);
            if (!success)
            {
                return id != job.ID ? BadRequest() : Ok(); // Return 200 OK if not found, per original behavior
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            var success = await _jobService.DeleteJobAsync(id);
            return success ? NoContent() : Ok(); // Return 200 OK if not found, per original behavior
        }
    }
}