using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend_Recruiting_Apply_App.Data.Entities
{
    public class Company
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string name { get; set; } = string.Empty;
        public string address { get; set; } = string.Empty;
        public string phone_number { get; set; } = string.Empty;
        public string field { get; set; } = string.Empty;
        public byte[] image { get; set; } = new byte[0];
        public bool green_badge { get; set; } = false;
        public string website_link { get; set; } = string.Empty;
        public int tax_id { get; set; } = 0;
        public DateTime date_founded { get; set; } = DateTime.Now;
    }
}
