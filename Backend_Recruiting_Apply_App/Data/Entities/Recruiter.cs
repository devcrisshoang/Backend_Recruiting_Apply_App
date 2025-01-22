using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend_Recruiting_Apply_App.Data.Entities
{
    public class Recruiter
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string name { get; set; } = string.Empty;
        public string phone_number { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public byte[] front_image { get; set; } = new byte[0];
        public byte[] back_image { get; set; } = new byte[0];
        public int company_id { get; set; } = 0;
        public int user_id { get; set; } = 0;
    }
}
