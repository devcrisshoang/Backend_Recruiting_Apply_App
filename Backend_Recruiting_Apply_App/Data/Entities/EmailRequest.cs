namespace Backend_Recruiting_Apply_App.Data.Entities
{
    public class EmailRequest
    {
        public string To { get; set; }      // Địa chỉ người nhận
        public string Subject { get; set; } // Tiêu đề
        public string Compose { get; set; } // Nội dung email
    }
}
