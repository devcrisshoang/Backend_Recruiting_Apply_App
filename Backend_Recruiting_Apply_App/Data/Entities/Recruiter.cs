using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend_Recruiting_Apply_App.Data.Entities
{
    [Table("Recruiters")]
    public class Recruiter
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }
        [Column("Name")]
        public string Name { get; set; } = string.Empty;
        [Column("Phone")]
        public string Phone { get; set; } = string.Empty;
        [Column("Email")]
        public string Email { get; set; } = string.Empty;
        [Column("Front_Image")]
        public byte[] Front_Image { get; set; } = [];
        [Column("Back_Image")]
        public byte[] Back_Image { get; set; } = [];
        [Column("Company_ID")]
        public int Company_ID { get; set; } = 0;
        [Column("User_ID")]
        public int User_ID { get; set; } = 0;
    }
}
