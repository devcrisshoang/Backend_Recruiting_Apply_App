using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend_Recruiting_Apply_App.Data.Entities
{
    [Table("_Applicant")]
    public class Applicant
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }
        [Column("Job")]
        public string Job { get; set; } = string.Empty;
        [Column("Location")]
        public string Location { get; set; } = string.Empty;
        [Column("Experience")]
        public string Experience { get; set; } = string.Empty;
        [Column("Is_Premium")]
        public int Is_Premium { get; set; } = 0;
        [Column("User_ID")]
        public int User_ID { get; set; } = 0;
        
    }
}
