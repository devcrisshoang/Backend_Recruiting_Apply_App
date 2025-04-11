using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Backend_Recruiting_Apply_App.Hubs
{
    public class MessageHub : Hub
    {
        // Gửi tin nhắn đến một nhóm (conversation) cụ thể
        public async Task SendMessageToConversation(int senderId, int receiverId, string content)
        {
            // Tạo một conversation key (ví dụ: "1_2")
            var conversationKey = senderId < receiverId ? $"{senderId}_{receiverId}" : $"{receiverId}_{senderId}";

            // Gửi tin nhắn đến tất cả client trong nhóm conversation
            await Clients.Group(conversationKey).SendAsync("ReceiveMessage", senderId, receiverId, content, DateTime.UtcNow);
        }

        // Thêm client vào nhóm conversation khi kết nối
        public async Task JoinConversation(int senderId, int receiverId)
        {
            var conversationKey = senderId < receiverId ? $"{senderId}_{receiverId}" : $"{receiverId}_{senderId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, conversationKey);
        }

        // Xóa client khỏi nhóm khi ngắt kết nối
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // Bạn có thể thêm logic để xóa client khỏi nhóm nếu cần
            await base.OnDisconnectedAsync(exception);
        }
    }
}