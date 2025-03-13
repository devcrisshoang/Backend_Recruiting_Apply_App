using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend_Recruiting_Apply_App.Data.Entities
{
    public class Company
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Field { get; set; } = string.Empty;
        public byte[] Image { get; set; } = [];
        public bool Badge { get; set; } = false;
        public string Website { get; set; } = string.Empty;
        public int Tax { get; set; } = 0;
        public DateTime Founded_Time { get; set; } = DateTime.Now;
    }
}
