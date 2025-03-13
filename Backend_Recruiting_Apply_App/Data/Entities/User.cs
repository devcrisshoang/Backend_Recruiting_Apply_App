using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend_Recruiting_Apply_App.Data.Entities
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Background { get; set; } = string.Empty;
        public byte[] Image { get; set; } = [];
        public int Is_Applicant { get; set; } = 0;
        public int Is_Recruiter { get; set; } = 0;
    }
}
