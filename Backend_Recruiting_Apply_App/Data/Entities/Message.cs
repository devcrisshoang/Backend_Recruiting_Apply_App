using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend_Recruiting_Apply_App.Data.Entities
{
    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int sender_id { get; set; } = 0;
        public int receiver_id { get; set; } = 0;
        public string content { get; set; } = string.Empty;
        public bool status { get; set; } = false;
        public DateTime time { get; set; } = DateTime.Now;
    }
}
