﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend_Recruiting_Apply_App.Data.Entities
{
    [Table("_Notification")]
    public class Notification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }
        [Column("Name")]
        public string Name { get; set; } = string.Empty;
        [Column("Content")]
        public string Content { get; set; } = string.Empty;
        [Column("User_ID")]
        public int User_ID { get; set; } = 0;
        [Column("Time")]
        public DateTime Time { get; set; } = DateTime.UtcNow;
    }
}
