namespace Backend_Recruiting_Apply_App.DTO
{
    public class ApplyDto
    {
        public int ID { get; set; }
        public string Name { get; set; } = "";
        public string Experience { get; set; } = "";
        public string Address { get; set; } = "";
        public string Description { get; set; } = "";
        public string Skill { get; set; } = "";
        public string Benefit { get; set; } = "";
        public int Gender { get; set; } = 0;
        public string Time { get; set; } = string.Empty;
        public int Method { get; set; } = 0;
        public int Position { get; set; } = 0;
        public int Salary { get; set; } = 0;
        public DateTime Create_Time { get; set; } = DateTime.Now;
        public int Status { get; set; } = 0;
        public int Recruiter_ID { get; set; } = 0;
        public byte[] Image { get; set; } = [];
        public int Badge { get; set; } = 0;
    }
}
