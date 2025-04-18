using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend_Recruiting_Apply_App.Data.Entities
{
    [Table("_JobName")]
    public class JobName
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }
        [Column("Name")]
        public string Name { get; set; } = string.Empty;
        [Column("Field_ID")]
        public int Field_ID { get; set; } = 0;
    }
}
