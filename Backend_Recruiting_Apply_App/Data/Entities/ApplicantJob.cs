using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend_Recruiting_Apply_App.Data.Entities
{
    public class ApplicantJob
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public bool is_accepted { get; set; } = false;
        public bool is_rejected { get; set; } = false;
        public DateTime time { get; set; } = DateTime.Now;
        public int job_id { get; set; } = 0;
        public int applicant_id { get; set; } = 0;
        public int resume_id { get; set; } = 0;
    }
}
