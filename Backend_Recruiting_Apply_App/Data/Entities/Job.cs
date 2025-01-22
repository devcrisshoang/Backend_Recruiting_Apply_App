using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend_Recruiting_Apply_App.Data.Entities
{
    public class Job
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string name { get; set; } = string.Empty;
        public string experience { get; set; } = string.Empty;
        public string address { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public string skills { get; set; } = string.Empty;
        public string benefit { get; set; } = string.Empty;
        public int gender { get; set; } = 0;
        public string working_time { get; set; } = string.Empty;
        public int working_method { get; set; } = 0;
        public int position { get; set; } = 0;
        public int salary { get; set; } = 0;
        public DateTime create_time { get; set; } = DateTime.Now;
        public int status { get; set; } = 0;
        public int recruiter_id { get; set; } = 0;
    }
}
