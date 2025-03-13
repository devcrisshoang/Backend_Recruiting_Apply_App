using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend_Recruiting_Apply_App.Data.Entities
{
    [Table("Messages")]
    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }
        [Column("Sender_ID")]
        public int Sender_ID { get; set; } = 0;
        [Column("Receiver_ID")]
        public int Receiver_ID { get; set; } = 0;
        [Column("Content")]
        public string Content { get; set; } = string.Empty;
        [Column("Status")]
        public int Status { get; set; } = 0;
        [Column("Time")]
        public DateTime Time { get; set; } = DateTime.Now;
    }
}
