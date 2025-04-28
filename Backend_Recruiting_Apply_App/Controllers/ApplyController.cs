using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend_Recruiting_Apply_App.Data.Entities;
using SystemAPIdotnet.Data;

namespace Backend_Recruiting_Apply_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplyController : ControllerBase
    {
        private readonly RAADbContext _context;

        public ApplyController(RAADbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Apply>>> GetApplicantJob()
        {
            return await _context.ApplicantJob.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Apply>> GetApplicantJob(int id)
        {
            var applicantJob = await _context.ApplicantJob.FindAsync(id);
            return Ok(applicantJob); // Trả về 200 OK với null nếu không tìm thấy
        }

        // Lấy danh sách bản ghi Apply với Is_Accepted = 1 dựa trên jobId
        [HttpGet("accepted-by-job/{jobId}")]
        public async Task<ActionResult<IEnumerable<Apply>>> GetAcceptedApplicantsByJobId(int jobId)
        {
            var applies = await _context.ApplicantJob
                .Where(a => a.Job_ID == jobId && a.Is_Accepted == 1)
                .ToListAsync();

            return Ok(applies ?? new List<Apply>());
        }

        // Lấy danh sách bản ghi Apply với Is_Accepted = 2 dựa trên jobId
        [HttpGet("pending-by-job/{jobId}")]
        public async Task<ActionResult<IEnumerable<Apply>>> GetPendingApplicantsByJobId(int jobId)
        {
            var applies = await _context.ApplicantJob
                .Where(a => a.Job_ID == jobId && a.Is_Accepted == 2)
                .ToListAsync();

            return Ok(applies ?? new List<Apply>());
        }

        // Lấy danh sách bản ghi Apply với Is_Accepted = 0 dựa trên jobId
        [HttpGet("rejected-by-job/{jobId}")]
        public async Task<ActionResult<IEnumerable<Apply>>> GetRejectedApplicantsByJobId(int jobId)
        {
            var applies = await _context.ApplicantJob
                .Where(a => a.Job_ID == jobId && a.Is_Accepted == 0)
                .ToListAsync();

            return Ok(applies ?? new List<Apply>());
        }

        [HttpPost]
        public async Task<ActionResult<Apply>> CreateApplicantJob(Apply applicantJob)
        {
            _context.ApplicantJob.Add(applicantJob);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetApplicantJob), new { id = applicantJob.ID }, applicantJob);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateApplicantJob(int id, Apply applicantJob)
        {
            if (id != applicantJob.ID)
            {
                return BadRequest();
            }

            _context.Entry(applicantJob).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.ApplicantJob.Any(e => e.ID == id))
                {
                    return Ok(); // Trả về 200 OK thay vì 404
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        [HttpPut("{id}/is-accepted")]
        public async Task<IActionResult> UpdateIsAccepted(int id, [FromBody] int isAccepted)
        {
            var applicantJob = await _context.ApplicantJob.FindAsync(id);
            if (applicantJob == null)
            {
                return Ok(); // Trả về 200 OK thay vì 404 để đồng bộ với các hàm khác
            }

            applicantJob.Is_Accepted = isAccepted;
            _context.Entry(applicantJob).Property(x => x.Is_Accepted).IsModified = true;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.ApplicantJob.Any(e => e.ID == id))
                {
                    return Ok(); // Trả về 200 OK thay vì 404
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApplicantJob(int id)
        {
            var applicantJob = await _context.ApplicantJob.FindAsync(id);
            if (applicantJob == null)
            {
                return Ok(); // Trả về 200 OK thay vì 404
            }

            _context.ApplicantJob.Remove(applicantJob);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Lấy thông tin job dựa trên applicantID
        [HttpGet("jobs-by-applicant/{applicantId}")]
        public async Task<ActionResult<IEnumerable<Job>>> GetJobsByApplicantId(int applicantId)
        {
            var applyRecords = await _context.ApplicantJob
                .Where(a => a.Applicant_ID == applicantId)
                .Select(a => a.Job_ID)
                .Distinct()
                .ToListAsync();

            var jobs = await _context.Job
                .Where(j => applyRecords.Contains(j.ID))
                .ToListAsync();

            return Ok(jobs ?? new List<Job>()); // Trả về 200 OK với mảng rỗng nếu không tìm thấy
        }

        // Lấy danh sách applicant ứng tuyển vào job dựa trên jobID
        [HttpGet("applicants-by-job/{jobId}")]
        public async Task<ActionResult<IEnumerable<Applicant>>> GetApplicantsByJobId(int jobId)
        {
            var applyRecords = await _context.ApplicantJob
                .Where(a => a.Job_ID == jobId)
                .Select(a => a.Applicant_ID)
                .Distinct()
                .ToListAsync();

            var applicants = await _context.Applicant
                .Where(a => applyRecords.Contains(a.ID))
                .ToListAsync();

            return Ok(applicants ?? new List<Applicant>()); // Trả về 200 OK với mảng rỗng nếu không tìm thấy
        }

        // Lấy Resume_ID dựa trên jobID và applicantID
        [HttpGet("resume/{jobId}/{applicantId}")]
        public async Task<ActionResult<int>> GetResumeIdByJobAndApplicant(int jobId, int applicantId)
        {
            // Kiểm tra đầu vào
            if (jobId <= 0 || applicantId <= 0)
            {
                return BadRequest("JobID và ApplicantID phải lớn hơn 0.");
            }

            // Tìm bản ghi Apply khớp với jobId và applicantId
            var applyRecord = await _context.ApplicantJob
                .Where(a => a.Job_ID == jobId && a.Applicant_ID == applicantId)
                .Select(a => a.Resume_ID)
                .FirstOrDefaultAsync();

            return Ok(applyRecord); // Trả về 200 OK với 0 nếu không tìm thấy
        }

        // Lấy ID của bản ghi Apply dựa trên jobId, resumeId, applicantId
        [HttpGet("apply-id/{jobId}/{resumeId}/{applicantId}")]
        public async Task<ActionResult<int>> GetApplyIdByJobResumeApplicant(int jobId, int resumeId, int applicantId)
        {
            // Kiểm tra đầu vào
            if (jobId <= 0 || resumeId <= 0 || applicantId <= 0)
            {
                return BadRequest("JobID, ResumeID và ApplicantID phải lớn hơn 0.");
            }

            // Tìm bản ghi Apply khớp với jobId, resumeId và applicantId
            var applyRecord = await _context.ApplicantJob
                .Where(a => a.Job_ID == jobId && a.Resume_ID == resumeId && a.Applicant_ID == applicantId)
                .Select(a => a.ID)
                .FirstOrDefaultAsync();

            return Ok(applyRecord); // Trả về 200 OK với ID hoặc 0 nếu không tìm thấy
        }
        public class ApplicantWithResumeDto
        {
            public int ApplicantId { get; set; }
            public string Name { get; set; } = string.Empty;
            public int ResumeId { get; set; }
        }
    }
}