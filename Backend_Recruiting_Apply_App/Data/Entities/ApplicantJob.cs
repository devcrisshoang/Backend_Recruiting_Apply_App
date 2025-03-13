using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend_Recruiting_Apply_App.Data.Entities
{
    public class ApplicantJob
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public bool Is_Accepted { get; set; } = false;
        public bool Is_Rejected { get; set; } = false;
        public DateTime Time { get; set; } = DateTime.Now;
        public int Job_ID { get; set; } = 0;
        public int Applicant_ID { get; set; } = 0;
        public int Resume_ID { get; set; } = 0;
    }
}
