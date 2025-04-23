using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend_Recruiting_Apply_App.Data.Entities
{
    [Table("_Payment")]
    public class Payment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }
        [Column("Order_Code")]
        public int Order_Code { get; set; } = 0;
        [Column("Amount")]
        public int Amount { get; set; } = 0; // Số tiền (VNĐ)
        [Column("Description")]
        public string Description { get; set; } = string.Empty; // Mô tả giao dịch
        [Column("Status")]
        public string Status { get; set; } = string.Empty; // PENDING, PAID, CANCELLED
        [Column("Checkout_Url")]
        public string Checkout_Url { get; set; } = string.Empty; // Link thanh toán
        [Column("Qr_Code")]
        public string Qr_Code { get; set; } = string.Empty; // Mã QR
        [Column("Created_Time")]
        public DateTime Created_Time { get; set; } = DateTime.UtcNow;
        [Column("User_ID")]
        public int User_ID { get; set; }
    }
}