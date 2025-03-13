using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend_Recruiting_Apply_App.Data.Entities
{
    [Table("Resumes")]
    public class Resume
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }
        [Column("Name")]
        public string Name { get; set; } = string.Empty;
        [Column("Email")]
        public string Email { get; set; } = string.Empty;
        [Column("Phone")]
        public string Phone { get; set; } = string.Empty;
        [Column("Education")]
        public string Education { get; set; } = string.Empty;
        [Column("Skill")]
        public string Skill { get; set; } = string.Empty;
        [Column("Certification")]
        public string Certification { get; set; } = string.Empty;
        [Column("Job")]
        public string Job { get; set; } = string.Empty;
        [Column("Introduction")]
        public string Introduction { get; set; } = string.Empty;
        [Column("Image")]
        public byte[] Image { get; set; } = [];
        [Column("Experience")]
        public string Experience { get; set; } = string.Empty;
        [Column("Create_Time")]
        public DateTime Create_Time {  get; set; } = DateTime.Now;
        [Column("Applicant_ID")]
        public int Applicant_ID { get; set; } = 0;
    }
}
