using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend_Recruiting_Apply_App.Data.Entities
{
    [Table("_Apply")]
    public class Apply
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }
        [Column("Is_Accepted")]
        public int Is_Accepted { get; set; } = 0;
        [Column("Is_Rejected")]
        public int Is_Rejected { get; set; } = 0;
        [Column("Time")]
        public DateTime Time { get; set; } = DateTime.Now;
        [Column("Job_ID")]
        public int Job_ID { get; set; } = 0;
        [Column("Applicant_ID")]
        public int Applicant_ID { get; set; } = 0;
        [Column("Resume_ID")]
        public int Resume_ID { get; set; } = 0;
    }
}
