using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Backend_Recruiting_Apply_App.Hubs
{
    public class MessageHub : Hub
    {
        public async Task JoinConversation(int senderId, int receiverId)
        {
            var conversationKey = senderId < receiverId ? $"{senderId}_{receiverId}" : $"{receiverId}_{senderId}";
            Console.WriteLine($"Client joined conversation: {conversationKey}, ConnectionId: {Context.ConnectionId}, SenderId: {senderId}, ReceiverId: {receiverId}");
            await Groups.AddToGroupAsync(Context.ConnectionId, conversationKey);
        }

        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"Client connected: ConnectionId: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"Client disconnected: ConnectionId: {Context.ConnectionId}, Exception: {exception?.Message ?? "None"}");
            await base.OnDisconnectedAsync(exception);
        }
    }
}