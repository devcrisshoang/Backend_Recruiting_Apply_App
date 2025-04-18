using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Backend_Recruiting_Apply_App.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task JoinUserNotifications(int userId)
        {
            var groupName = $"User_{userId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            Console.WriteLine($"Người dùng {userId} đã tham gia nhóm {groupName} với ConnectionId {Context.ConnectionId}");
        }

        public async Task SendNotificationToUser(int userId, string name, string content, DateTime time)
        {
            var groupName = $"User_{userId}";
            await Clients.Group(groupName).SendAsync("ReceiveNotification", userId, name, content, time.ToString("o"));
            Console.WriteLine($"Gửi thông báo tới nhóm {groupName}: {content}");
        }

        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"Client đã kết nối tới NotificationHub: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine($"Client ngắt kết nối khỏi NotificationHub: {Context.ConnectionId}, Lỗi: {exception?.Message}");
            await base.OnDisconnectedAsync(exception);
        }
    }
}