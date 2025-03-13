using Backend_Recruiting_Apply_App.DTO;
using Backend_Recruiting_Apply_App.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TopCVSystemAPIdotnet.Data;

namespace Backend_Recruiting_Apply_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplyViewController : ControllerBase
    {
        private readonly RAADbContext _context;

        public ApplyViewController(RAADbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApplyDto>>> GetJob()
        {
            var Job = await _context.Job.ToListAsync();
            var Company = await _context.Company.ToListAsync();

            var result = Job.Join(Company,
                job => job.Recruiter_ID,
                company => company.ID,
                (job, company) => new ApplyDto
                {
                    ID = job.ID,
                    Name = job.Name,
                    Experience = job.Experience,
                    Address = job.Address,
                    Description = job.Description,
                    Skill = job.Skill,
                    Benefit = job.Benefit,
                    Gender = job.Gender,
                    Time = job.Time,
                    Method = job.Method,
                    Position = job.Position,
                    Salary = job.Salary,
                    Create_Time = job.Create_Time,
                    Status = job.Status,
                    Recruiter_ID = job.Recruiter_ID,
                    Image = company.Image,
                    Badge = company.Badge
                }).ToList();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApplyDto>> GetJob(int id)
        {
            var job = await _context.Job.FindAsync(id);
            if (job == null)
                return NotFound();

            var company = await _context.Company.FirstOrDefaultAsync(c => c.ID == job.Recruiter_ID);
            if (company == null)
                return NotFound();

            var workDto = new ApplyDto
            {
                ID = job.ID,
                Name = job.Name,
                Experience = job.Experience,
                Address = job.Address,
                Description = job.Description,
                Skill = job.Skill,
                Benefit = job.Benefit,
                Gender = job.Gender,
                Time = job.Time,
                Method = job.Method,
                Position = job.Position,
                Salary = job.Salary,
                Create_Time = job.Create_Time,
                Status = job.Status,
                Recruiter_ID = job.Recruiter_ID,
                Image = company.Image,
                Badge = company.Badge
            };

            return Ok(workDto);
        }

        [HttpPost]
        public async Task<ActionResult<ApplyDto>> CreateJob(ApplyDto workDto)
        {
            var job = new Job
            {
                Name = workDto.Name,
                Experience = workDto.Experience,
                Address = workDto.Address,
                Description = workDto.Description,
                Skill = workDto.Skill,
                Benefit = workDto.Benefit,
                Gender = workDto.Gender,
                Time = workDto.Time,
                Method = workDto.Method,
                Position = workDto.Position,
                Salary = workDto.Salary,
                Create_Time = workDto.Create_Time,
                Status = workDto.Status,
                Recruiter_ID = workDto.Recruiter_ID
            };

            _context.Job.Add(job);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetJob), new { id = job.ID }, workDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJob(int id, ApplyDto workDto)
        {
            var job = await _context.Job.FindAsync(id);
            if (job == null)
                return NotFound();

            job.Name = workDto.Name;
            job.Experience = workDto.Experience;
            job.Address = workDto.Address;
            job.Description = workDto.Description;
            job.Skill = workDto.Skill;
            job.Benefit = workDto.Benefit;
            job.Gender = workDto.Gender;
            job.Time = workDto.Time;
            job.Method = workDto.Method;
            job.Position = workDto.Position;
            job.Salary = workDto.Salary;
            job.Create_Time = workDto.Create_Time;
            job.Status = workDto.Status;
            job.Recruiter_ID = workDto.Recruiter_ID;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            var job = await _context.Job.FindAsync(id);
            if (job == null)
                return NotFound();

            _context.Job.Remove(job);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
