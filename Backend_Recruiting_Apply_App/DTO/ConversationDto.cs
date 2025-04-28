namespace Backend_Recruiting_Apply_App.Data.Dtos
{
    public class ConversationDto
    {
        public string ConversationKey { get; set; }
        public string LastMessage { get; set; }
        public int OtherUserId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}