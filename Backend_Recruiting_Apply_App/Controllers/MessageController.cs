using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend_Recruiting_Apply_App.Data.Entities;
using SystemAPIdotnet.Data;
using Microsoft.AspNetCore.SignalR;
using Backend_Recruiting_Apply_App.Hubs;

namespace Backend_Recruiting_Apply_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly RAADbContext _context;
        private readonly IHubContext<MessageHub> _hubContext;

        public MessageController(RAADbContext context, IHubContext<MessageHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Message>>> GetMessage()
        {
            Console.WriteLine("Fetching all messages");
            return await _context.Message.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Message>> GetMessage(int id)
        {
            Console.WriteLine($"Fetching message with ID: {id}");
            var message = await _context.Message.FindAsync(id);
            if (message == null)
            {
                Console.WriteLine($"Message not found: {id}");
                return NotFound();
            }
            return message;
        }

        [HttpPost]
        public async Task<ActionResult<Message>> SendMessage(Message message)
        {
            message.Time = DateTime.UtcNow;
            Console.WriteLine($"Saving message: SenderId={message.Sender_ID}, ReceiverId={message.Receiver_ID}, Content={message.Content}");
            _context.Message.Add(message);
            await _context.SaveChangesAsync();

            var conversationKey = message.Sender_ID < message.Receiver_ID
                ? $"{message.Sender_ID}_{message.Receiver_ID}"
                : $"{message.Receiver_ID}_{message.Sender_ID}";

            Console.WriteLine($"Sending ReceiveMessage to group: {conversationKey}, MessageId: {message.ID}, SenderId: {message.Sender_ID}, ReceiverId: {message.Receiver_ID}");
            await _hubContext.Clients.Group(conversationKey).SendAsync(
                "ReceiveMessage",
                message.ID, // Add message.ID
                message.Sender_ID,
                message.Receiver_ID,
                message.Content,
                message.Time
            );

            return CreatedAtAction(nameof(GetMessage), new { id = message.ID }, message);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMessage(int id, Message message)
        {
            if (id != message.ID)
            {
                Console.WriteLine($"Invalid message ID: {id} does not match {message.ID}");
                return BadRequest();
            }

            _context.Entry(message).State = EntityState.Modified;

            try
            {
                Console.WriteLine($"Updating message: ID={id}, Content={message.Content}");
                await _context.SaveChangesAsync();

                var conversationKey = message.Sender_ID < message.Receiver_ID
                    ? $"{message.Sender_ID}_{message.Receiver_ID}"
                    : $"{message.Receiver_ID}_{message.Sender_ID}";

                Console.WriteLine($"Sending UpdateMessage to group: {conversationKey}, MessageId: {message.ID}");
                await _hubContext.Clients.Group(conversationKey).SendAsync(
                    "UpdateMessage",
                    message.ID,
                    message.Content,
                    message.Time
                );
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Message.Any(e => e.ID == id))
                {
                    Console.WriteLine($"Message not found for update: {id}");
                    return NotFound();
                }
                throw;
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            Console.WriteLine($"Deleting message: ID={id}");
            var message = await _context.Message.FindAsync(id);
            if (message == null)
            {
                Console.WriteLine($"Message not found: {id}");
                return NotFound();
            }

            _context.Message.Remove(message);
            await _context.SaveChangesAsync();

            var conversationKey = message.Sender_ID < message.Receiver_ID
                ? $"{message.Sender_ID}_{message.Receiver_ID}"
                : $"{message.Receiver_ID}_{message.Sender_ID}";

            Console.WriteLine($"Sending DeleteMessage to group: {conversationKey}, MessageId: {id}");
            await _hubContext.Clients.Group(conversationKey).SendAsync(
                "DeleteMessage",
                message.ID
            );

            return NoContent();
        }

        [HttpDelete("all")]
        public async Task<IActionResult> DeleteAllMessages()
        {
            try
            {
                Console.WriteLine("Deleting all messages");
                var messages = await _context.Message.ToListAsync();
                if (messages == null || !messages.Any())
                {
                    Console.WriteLine("No messages found to delete");
                    return NotFound(new { Message = "No messages found to delete." });
                }

                _context.Message.RemoveRange(messages);
                await _context.SaveChangesAsync();

                foreach (var message in messages)
                {
                    var conversationKey = message.Sender_ID < message.Receiver_ID
                        ? $"{message.Sender_ID}_{message.Receiver_ID}"
                        : $"{message.Receiver_ID}_{message.Sender_ID}";
                    Console.WriteLine($"Sending DeleteAllMessages to group: {conversationKey}");
                    await _hubContext.Clients.Group(conversationKey).SendAsync("DeleteAllMessages");
                }

                return Ok(new { Message = "All messages have been deleted successfully." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting all messages: {ex.Message}");
                return StatusCode(500, new { Message = "An error occurred while deleting all messages.", Error = ex.Message });
            }
        }

        [HttpGet("conversation/{senderId}/{receiverId}")]
        public async Task<ActionResult<IEnumerable<Message>>> GetMessagesBySenderAndReceiver(
            int senderId,
            int receiverId,
            [FromQuery] int skip = 0,
            [FromQuery] int take = 10)
        {
            try
            {
                Console.WriteLine($"Fetching messages: SenderId={senderId}, ReceiverId={receiverId}, Skip={skip}, Take={take}");
                var messages = await _context.Message
                    .Where(m => (m.Sender_ID == senderId && m.Receiver_ID == receiverId) ||
                                (m.Sender_ID == receiverId && m.Receiver_ID == senderId))
                    .OrderByDescending(m => m.Time)
                    .Skip(skip)
                    .Take(take)
                    .OrderBy(m => m.Time)
                    .ToListAsync();

                if (messages == null || !messages.Any())
                {
                    Console.WriteLine($"No messages found between sender {senderId} and receiver {receiverId}");
                    return NotFound(new { Message = $"No messages found between sender {senderId} and receiver {receiverId}." });
                }

                return Ok(messages);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving messages: {ex.Message}");
                return StatusCode(500, new { Message = "An error occurred while retrieving messages.", Error = ex.Message });
            }
        }

        [HttpGet("conversations/{userId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetConversations(int userId)
        {
            try
            {
                Console.WriteLine($"Fetching conversations for user: {userId}");
                var messages = await _context.Message
                    .Where(m => m.Sender_ID == userId || m.Receiver_ID == userId)
                    .OrderBy(m => m.Time)
                    .ToListAsync();

                if (messages == null || !messages.Any())
                {
                    Console.WriteLine($"No conversations found for user: {userId}");
                    return NotFound(new { Message = $"No conversations found for user {userId}." });
                }

                var conversations = messages
                    .GroupBy(m => m.Sender_ID < m.Receiver_ID
                        ? $"{m.Sender_ID}_{m.Receiver_ID}"
                        : $"{m.Receiver_ID}_{m.Sender_ID}")
                    .Select(group => {
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
                    .ToList();

                return Ok(conversations);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving conversations: {ex.Message}");
                return StatusCode(500, new { Message = "An error occurred while retrieving conversations.", Error = ex.Message });
            }
        }
    }
}