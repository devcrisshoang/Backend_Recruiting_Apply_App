using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Backend_Recruiting_Apply_App.Data.Entities
{
    public class Job
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Experience { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Skill { get; set; } = string.Empty;
        public string Benefit { get; set; } = "";
        public int Gender { get; set; } = 0;
        public string Time { get; set; } = string.Empty;
        public int Method { get; set; } = 0;
        public int Position { get; set; } = 0;
        public int Salary { get; set; } = 0;
        public DateTime Create_Time { get; set; } = DateTime.Now;
        public int Status { get; set; } = 0;
        public int Recruiter_ID { get; set; } = 0;
    }
}
