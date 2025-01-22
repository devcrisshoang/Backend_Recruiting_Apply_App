using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend_Recruiting_Apply_App.Data.Entities
{
    public class Applicant
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string name { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string phone_number { get; set; } = string.Empty;
        public string job { get; set; } = string.Empty;
        public string working_location { get; set; } = string.Empty;
        public string experience { get; set; } = string.Empty;
        public bool premium { get; set; } = false;
        public int user_id { get; set; } = 0;
        
    }
}
