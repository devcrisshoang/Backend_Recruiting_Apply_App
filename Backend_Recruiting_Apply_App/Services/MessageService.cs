using Microsoft.EntityFrameworkCore;
using Backend_Recruiting_Apply_App.Data.Entities;
using SystemAPIdotnet.Data;

namespace Backend_Recruiting_Apply_App.Services
{
    public interface IMessageService
    {
        Task<List<Message>> GetAllMessagesAsync();
        Task<Message?> GetMessageByIdAsync(int id);
        Task<Message> SendMessageAsync(Message message);
        Task<bool> UpdateMessageAsync(int id, Message message);
        Task<bool> DeleteMessageAsync(int id);
        Task<bool> DeleteAllMessagesAsync();
        Task<List<Message>> GetMessagesBySenderAndReceiverAsync(int senderId, int receiverId, int skip, int take);
        Task<List<object>> GetConversationsAsync(int userId);
    }

    public class MessageService : IMessageService
    {
        private readonly RAADbContext _dbContext;

        public MessageService(RAADbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Message>> GetAllMessagesAsync()
        {
            return await _dbContext.Message.ToListAsync();
        }

        public async Task<Message?> GetMessageByIdAsync(int id)
        {
            return await _dbContext.Message.FindAsync(id);
        }

        public async Task<Message> SendMessageAsync(Message message)
        {
            var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");                                                                            
            message.Time = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
            _dbContext.Message.Add(message);
            await _dbContext.SaveChangesAsync();
            return message;
        }

        public async Task<bool> UpdateMessageAsync(int id, Message message)
        {
            if (id != message.ID)
            {
                return false;
            }

            _dbContext.Entry(message).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return !await _dbContext.Message.AnyAsync(e => e.ID == id);
            }
        }

        public async Task<bool> DeleteMessageAsync(int id)
        {
            var message = await _dbContext.Message.FindAsync(id);
            if (message == null)
            {
                return false;
            }

            _dbContext.Message.Remove(message);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAllMessagesAsync()
        {
            var messages = await _dbContext.Message.ToListAsync();
            if (messages == null || !messages.Any())
            {
                return false;
            }

            _dbContext.Message.RemoveRange(messages);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<Message>> GetMessagesBySenderAndReceiverAsync(int senderId, int receiverId, int skip, int take)
        {
            return await _dbContext.Message
                .Where(m => (m.Sender_ID == senderId && m.Receiver_ID == receiverId) ||
                            (m.Sender_ID == receiverId && m.Receiver_ID == senderId))
                .OrderByDescending(m => m.Time)
                .Skip(skip)
                .Take(take)
                .OrderBy(m => m.Time)
                .ToListAsync();
        }

        public async Task<List<object>> GetConversationsAsync(int userId)
        {
            var messages = await _dbContext.Message
                .Where(m => m.Sender_ID == userId || m.Receiver_ID == userId)
                .OrderBy(m => m.Time)
                .ToListAsync();

            if (messages == null || !messages.Any())
            {
                return new List<object>();
            }

            return messages
                .GroupBy(m => m.Sender_ID < m.Receiver_ID
                    ? $"{m.Sender_ID}_{m.Receiver_ID}"
                    : $"{m.Receiver_ID}_{m.Sender_ID}")
                .Select(group =>
                {
                    var lastMessage = group.OrderByDescending(m => m.Time).First();
                    return new
                    {
                        conversationKey = group.Key,
                        lastMessage = lastMessage.Content,
                        otherUserId = lastMessage.Sender_ID == userId ? lastMessage.Receiver_ID : lastMessage.Sender_ID,
                        timestamp = lastMessage.Time
                    };
                })
                .OrderByDescending(c => c.timestamp)
                .ToList<object>();
        }
    }
}