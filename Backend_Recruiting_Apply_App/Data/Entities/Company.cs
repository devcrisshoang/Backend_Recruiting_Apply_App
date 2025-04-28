using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend_Recruiting_Apply_App.Data.Entities
{
    [Table("_Company")]
    public class Company
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }

        [Column("Name")]
        public string Name { get; set; } = string.Empty;

        [Column("Address")]
        public string Address { get; set; } = string.Empty;

        [Column("Phone")]
        public string Phone { get; set; } = string.Empty;

        [Column("Field")]
        public int Field { get; set; } = 0;

        [Column("Image")]
        public byte[]? Image { get; set; } // Cho phép NULL

        [Column("Badge")]
        public int Badge { get; set; } = 0;

        [Column("Website")]
        public string Website { get; set; } = string.Empty;

        [Column("Tax")]
        public int Tax { get; set; } = 0;

        [Column("Founded_Time")]
        public DateTime Founded_Time { get; set; } = DateTime.Now;
    }
}