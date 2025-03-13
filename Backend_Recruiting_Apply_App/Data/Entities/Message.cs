using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend_Recruiting_Apply_App.Data.Entities
{
    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int Sender_ID { get; set; } = 0;
        public int Receiver_ID { get; set; } = 0;
        public string Content { get; set; } = string.Empty;
        public bool Status { get; set; } = false;
        public DateTime Time { get; set; } = DateTime.Now;
    }
}
