using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend_Recruiting_Apply_App.Data.Entities
{
    [Table("_Resume")]
    public class Resume
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }
        [Column("Name")]
        public string Name { get; set; } = string.Empty;
        [Column("Image")]
        public byte[] Image { get; set; } = [];
        [Column("Create_Time")]
        public DateTime Create_Time {  get; set; } = DateTime.Now;
        [Column("Applicant_ID")]
        public int Applicant_ID { get; set; } = 0;
    }
}
