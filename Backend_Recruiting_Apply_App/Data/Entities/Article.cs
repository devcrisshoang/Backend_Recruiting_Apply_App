using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend_Recruiting_Apply_App.Data.Entities
{
    [Table("Articles")]
    public class Article
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }
        [Column("Name")]
        public string Name { get; set; } = string.Empty;
        [Column("Content")]
        public string Content { get; set; } = string.Empty;
        [Column("Time")]
        public string Time { get; set; } = string.Empty;
        [Column("Image")]
        public byte[] Image { get; set; } = [];
        [Column("Recruiter_ID")]
        public int Recruiter_ID { get; set; } = 0;
    }
}
