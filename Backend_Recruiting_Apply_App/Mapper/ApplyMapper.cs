using Backend_Recruiting_Apply_App.Data.Entities;
using Backend_Recruiting_Apply_App.DTO;

namespace Backend_Recruiting_Apply_App.Mapper
{
    public static class ApplyMapper
    {
        public static ApplyDto MapToDto(Company company, Job job)
        {
            return new ApplyDto
            {
                ID = job.ID,
                Name = job.Name,
                Experience = job.Experience,
                Address = job.Address,
                Description = job.Description,
                Skill = job.Skill,
                Benefit = job.Benefit,
                Gender = job.Gender,
                Method = job.Method,
                Position = job.Position,
                Salary = job.Salary,
                Create_Time = job.Create_Time,
                Status = job.Status,
                Recruiter_ID = job.Recruiter_ID,
                Image = company.Image,
                Badge = company.Badge,
            };
        }
    }
}
