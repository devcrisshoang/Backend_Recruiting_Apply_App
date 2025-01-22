using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend_Recruiting_Apply_App.Data.Entities
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string username { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
        public string background { get; set; } = string.Empty;
        public byte[] image { get; set; } = new byte[0];
        public bool is_applicant { get; set; } = false;
        public bool is_recruiter { get; set; } = false;
    }
}
