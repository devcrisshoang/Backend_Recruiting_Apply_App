using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend_Recruiting_Apply_App.Data.Entities
{
    public class Article
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string name { get; set; } = string.Empty;
        public string content { get; set; } = string.Empty;
        public string time { get; set; } = string.Empty;
        public byte[] image { get; set; } = new byte[0];
        public int recruiter_id { get; set; } = 0;
    }
}
