using Microsoft.AspNetCore.Mvc;
using Backend_Recruiting_Apply_App.Data.Entities;
using Backend_Recruiting_Apply_App.Services;

namespace Backend_Recruiting_Apply_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobNameController : ControllerBase
    {
        private readonly IJobNameService _jobNameService;

        public JobNameController(IJobNameService jobNameService)
        {
            _jobNameService = jobNameService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobName>>> GetJobName()
        {
            var jobNames = await _jobNameService.GetAllJobNamesAsync();
            return Ok(jobNames);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<JobName>> GetJobName(int id)
        {
            var jobName = await _jobNameService.GetJobNameByIdAsync(id);
            if (jobName == null)
            {
                return NotFound();
            }
            return Ok(jobName);
        }

        [HttpGet("by-field/{fieldId}")]
        public async Task<ActionResult<IEnumerable<string>>> GetJobNamesByFieldId(int fieldId)
        {
            var jobNames = await _jobNameService.GetJobNamesByFieldIdAsync(fieldId);
            if (jobNames == null || !jobNames.Any())
            {
                return NotFound($"No job names found for Field_ID {fieldId}");
            }
            return Ok(jobNames);
        }

        [HttpPost]
        public async Task<ActionResult<JobName>> CreateJobName(JobName jobName)
        {
            var createdJobName = await _jobNameService.CreateJobNameAsync(jobName);
            return CreatedAtAction(nameof(GetJobName), new { id = createdJobName.ID }, createdJobName);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJobName(int id, JobName jobName)
        {
            var success = await _jobNameService.UpdateJobNameAsync(id, jobName);
            if (!success)
            {
                return id != jobName.ID ? BadRequest() : NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJobName(int id)
        {
            var success = await _jobNameService.DeleteJobNameAsync(id);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}