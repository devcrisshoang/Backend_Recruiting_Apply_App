using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend_Recruiting_Apply_App.Data.Entities
{
    [Table("_Job")]
    public class Job
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }
        [Column("Name")]
        public string Name { get; set; } = string.Empty;
        [Column("Experience")]
        public string Experience { get; set; } = string.Empty;
        [Column("Address")]
        public string Address { get; set; } = string.Empty;
        [Column("Description")]
        public string Description { get; set; } = string.Empty;
        [Column("Skill")]
        public string Skill { get; set; } = string.Empty;
        [Column("Benefit")]
        public string Benefit { get; set; } = "";
        [Column("Gender")]
        public int Gender { get; set; } = 0;
        [Column("Time")]
        public string Time { get; set; } = string.Empty;
        [Column("Method")]
        public int Method { get; set; } = 0;
        [Column("Position")]
        public int Position { get; set; } = 0;
        [Column("Salary")]
        public int Salary { get; set; } = 0;
        [Column("Quantity")]
        public int Quantity { get; set; } = 0;
        [Column("Create_Time")]
        public DateTime Create_Time { get; set; } = DateTime.UtcNow;
        [Column("Status")]
        public int Status { get; set; } = 0;
        [Column("Recruiter_ID")]
        public int Recruiter_ID { get; set; } = 0;
    }
}
