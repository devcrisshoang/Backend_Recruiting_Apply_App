using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend_Recruiting_Apply_App.Data.Entities
{
    [Table("_Field")]
    public class Field
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }
        [Column("Name")]
        public string Name { get; set; } = string.Empty;
    }
}
