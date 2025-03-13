using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend_Recruiting_Apply_App.Data.Entities
{
    public class Resume
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Education { get; set; } = string.Empty;
        public string Skill { get; set; } = string.Empty;
        public string Certification { get; set; } = string.Empty;
        public string Job { get; set; } = string.Empty;
        public string Introduction { get; set; } = string.Empty;
        public byte[] Image { get; set; } = []; 
        public string Experience { get; set; } = string.Empty;
        public DateTime Create_Time {  get; set; } = DateTime.Now;
        public int Applicant_ID { get; set; } = 0;
    }
}
