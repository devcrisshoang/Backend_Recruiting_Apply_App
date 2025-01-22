using Backend_Recruiting_Apply_App.Data.Entities;
using Microsoft.EntityFrameworkCore;


namespace TopCVSystemAPIdotnet.Data
{
    public class RRADbContext : DbContext
    {
        public RRADbContext(DbContextOptions<RRADbContext> options) : base(options) { }
        public DbSet<User> User { get; set; }
        public DbSet<Article> Article { get; set; }
        public DbSet<Message> Message { get; set; }
        public DbSet<Job> Job { get; set; }
        public DbSet<Applicant> Applicant { get; set; }
        public DbSet<Resume> Resume { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<Recruiter> Recruiter { get; set; }
        public DbSet<ApplicantJob> ApplicantJob { get; set; }
        public DbSet<Admin> Admin { get; set; }

    }
}
