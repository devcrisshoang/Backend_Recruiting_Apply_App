using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend_Recruiting_Apply_App.Data.Entities
{
    public class Applicant
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Job { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Experience { get; set; } = string.Empty;
        public bool Is_Premium { get; set; } = false;
        public int User_ID { get; set; } = 0;
        
    }
}
