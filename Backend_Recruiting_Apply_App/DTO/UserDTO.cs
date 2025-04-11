namespace Backend_Recruiting_Apply_App.Data.DTOs
{
    public class UserDTO
    {
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public byte[] Image { get; set; } = [];
        public int Type { get; set; } = 0;
    }
}
