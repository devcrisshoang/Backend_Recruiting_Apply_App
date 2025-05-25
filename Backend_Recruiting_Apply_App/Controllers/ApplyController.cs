using Microsoft.AspNetCore.Mvc;
using Backend_Recruiting_Apply_App.Data.Entities;
using Backend_Recruiting_Apply_App.Services;

namespace Backend_Recruiting_Apply_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplyController : ControllerBase
    {
        private readonly IApplyService _applyService;

        public ApplyController(IApplyService applyService)
        {
            _applyService = applyService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Apply>>> GetApply()
        {
            var applies = await _applyService.GetAllAppliesAsync();
            return Ok(applies);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Apply>> GetApply(int id)
        {
            var apply = await _applyService.GetApplyByIdAsync(id);
            return Ok(apply); // Returns 200 OK with null if not found
        }

        [HttpGet("accepted-by-job/{jobId}")]
        public async Task<ActionResult<IEnumerable<Apply>>> GetAcceptedApplicantsByJobId(int jobId)
        {
            var applies = await _applyService.GetAcceptedApplicantsByJobIdAsync(jobId);
            return Ok(applies ?? new List<Apply>());
        }

        [HttpGet("pending-by-job/{jobId}")]
        public async Task<ActionResult<IEnumerable<Apply>>> GetPendingApplicantsByJobId(int jobId)
        {
            var applies = await _applyService.GetPendingApplicantsByJobIdAsync(jobId);
            return Ok(applies ?? new List<Apply>());
        }

        [HttpGet("rejected-by-job/{jobId}")]
        public async Task<ActionResult<IEnumerable<Apply>>> GetRejectedApplicantsByJobId(int jobId)
        {
            var applies = await _applyService.GetRejectedApplicantsByJobIdAsync(jobId);
            return Ok(applies ?? new List<Apply>());
        }

        [HttpGet("appliedJob/applicant/{id}")]
        public async Task<ActionResult<IEnumerable<Job>>> GetAppliedJobByApplicantId(int id)
        {
            var jobs = await _applyService.GetAppliedJobsByApplicantIdAsync(id);
            return Ok(jobs);
        }

        [HttpPost]
        public async Task<ActionResult<Apply>> CreateApply(Apply apply)
        {
            var createdApply = await _applyService.CreateApplyAsync(apply);
            return CreatedAtAction(nameof(GetApply), new { id = createdApply.ID }, createdApply);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateApply(int id, Apply apply)
        {
            var success = await _applyService.UpdateApplyAsync(id, apply);
            if (!success)
            {
                return BadRequest();
            }
            return NoContent();
        }

        [HttpPut("{id}/is-accepted")]
        public async Task<IActionResult> UpdateIsAccepted(int id, [FromBody] int isAccepted)
        {
            var success = await _applyService.UpdateIsAcceptedAsync(id, isAccepted);
            if (!success)
            {
                return Ok(); // Returns 200 OK instead of 404 for consistency
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApply(int id)
        {
            var success = await _applyService.DeleteApplyAsync(id);
            if (!success)
            {
                return Ok(); // Returns 200 OK instead of 404 for consistency
            }
            return NoContent();
        }

        [HttpGet("jobs-by-applicant/{applicantId}")]
        public async Task<ActionResult<IEnumerable<Job>>> GetJobsByApplicantId(int applicantId)
        {
            var jobs = await _applyService.GetJobsByApplicantIdAsync(applicantId);
            return Ok(jobs ?? new List<Job>());
        }

        [HttpGet("applicants-by-job/{jobId}")]
        public async Task<ActionResult<IEnumerable<Applicant>>> GetApplicantsByJobId(int jobId)
        {
            var applicants = await _applyService.GetApplicantsByJobIdAsync(jobId);
            return Ok(applicants ?? new List<Applicant>());
        }

        [HttpGet("resume/{jobId}/{applicantId}")]
        public async Task<ActionResult<int>> GetResumeIdByJobAndApplicant(int jobId, int applicantId)
        {
            try
            {
                var resumeId = await _applyService.GetResumeIdByJobAndApplicantAsync(jobId, applicantId);
                return Ok(resumeId); // Returns 200 OK with 0 if not found
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("apply-id/{jobId}/{resumeId}/{applicantId}")]
        public async Task<ActionResult<int>> GetApplyIdByJobResumeApplicant(int jobId, int resumeId, int applicantId)
        {
            try
            {
                var applyId = await _applyService.GetApplyIdByJobResumeApplicantAsync(jobId, resumeId, applicantId);
                return Ok(applyId); // Returns 200 OK with 0 if not found
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public class ApplicantWithResumeDto
        {
            public int ApplicantId { get; set; }
            public string Name { get; set; } = string.Empty;
            public int ResumeId { get; set; }
        }
    }
}