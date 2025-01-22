using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend_Recruiting_Apply_App.Data.Entities
{
    public class Resume
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string name { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string phone_number { get; set; } = string.Empty;
        public string education { get; set; } = string.Empty;
        public string skills { get; set; } = string.Empty;
        public string certification { get; set; } = string.Empty;
        public string job_applying { get; set; } = string.Empty;
        public string introduction { get; set; } = string.Empty;
        public byte[] image { get; set; } = new byte[0]; 
        public string experience { get; set; } = string.Empty;
        public DateTime create_time {  get; set; } = DateTime.Now;
        public int applicant_id { get; set; } = 0;
    }
}
