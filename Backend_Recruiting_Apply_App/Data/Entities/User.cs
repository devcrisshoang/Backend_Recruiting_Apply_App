using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend_Recruiting_Apply_App.Data.Entities
{
    [Table("Users")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }
        [Column("Username")]
        public string Username { get; set; } = string.Empty;
        [Column("Password")]
        public string Password { get; set; } = string.Empty;
        [Column("Background")]
        public string Background { get; set; } = string.Empty;
        [Column("Image")]
        public byte[] Image { get; set; } = [];
        [Column("Is_Applicant")]
        public int Is_Applicant { get; set; } = 0;
        [Column("Is_Recruiter")]
        public int Is_Recruiter { get; set; } = 0;
    }
}
