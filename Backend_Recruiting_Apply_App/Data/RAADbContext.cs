using Backend_Recruiting_Apply_App.Data.Entities;
using Backend_Recruiting_Apply_App.DTO;
using Microsoft.EntityFrameworkCore;


namespace SystemAPIdotnet.Data
{
    public class RAADbContext : DbContext
    {
        public RAADbContext(DbContextOptions<RAADbContext> options) : base(options) { }
        public DbSet<User> User { get; set; }
        public DbSet<Article> Article { get; set; }
        public DbSet<Message> Message { get; set; }
        public DbSet<Job> Job { get; set; }
        public DbSet<Applicant> Applicant { get; set; }
        public DbSet<Resume> Resume { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<Recruiter> Recruiter { get; set; }
        public DbSet<Apply> ApplicantJob { get; set; }
        public DbSet<Admin> Admin { get; set; }
        public DbSet<ApplyDto> JobDetails { get; set; }
        public DbSet<Field> Field { get; set; }
        public DbSet<JobName> JobName { get; set; }
        public DbSet<Province> Province { get; set; }
        public DbSet<Experience> Experience { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplyDto>().HasNoKey().ToView("ApplyView");
        }
    }
}
