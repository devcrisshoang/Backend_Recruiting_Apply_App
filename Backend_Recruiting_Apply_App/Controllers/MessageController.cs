using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Backend_Recruiting_Apply_App.Data.Entities;
using Backend_Recruiting_Apply_App.Hubs;
using Backend_Recruiting_Apply_App.Services;

namespace Backend_Recruiting_Apply_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IHubContext<MessageHub> _hubContext;

        public MessageController(IMessageService messageService, IHubContext<MessageHub> hubContext)
        {
            _messageService = messageService;
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Message>>> GetMessage()
        {
            Console.WriteLine("Fetching all messages");
            var messages = await _messageService.GetAllMessagesAsync();
            return Ok(messages);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Message>> GetMessage(int id)
        {
            Console.WriteLine($"Fetching message with ID: {id}");
            var message = await _messageService.GetMessageByIdAsync(id);
            if (message == null)
            {
                Console.WriteLine($"Message not found: {id}");
                return NotFound();
            }
            return Ok(message);
        }

        [HttpPost]
        public async Task<ActionResult<Message>> SendMessage(Message message)
        {
            Console.WriteLine($"Saving message: SenderId={message.Sender_ID}, ReceiverId={message.Receiver_ID}, Content={message.Content}");
            var createdMessage = await _messageService.SendMessageAsync(message);

            var conversationKey = message.Sender_ID < message.Receiver_ID
                ? $"{message.Sender_ID}_{message.Receiver_ID}"
                : $"{message.Receiver_ID}_{message.Sender_ID}";

            Console.WriteLine($"Sending ReceiveMessage to group: {conversationKey}, MessageId: {createdMessage.ID}, SenderId: {createdMessage.Sender_ID}, ReceiverId: {createdMessage.Receiver_ID}");
            await _hubContext.Clients.Group(conversationKey).SendAsync(
                "ReceiveMessage",
                createdMessage.ID,
                createdMessage.Sender_ID,
                createdMessage.Receiver_ID,
                createdMessage.Content,
                createdMessage.Time
            );

            return CreatedAtAction(nameof(GetMessage), new { id = createdMessage.ID }, createdMessage);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMessage(int id, Message message)
        {
            Console.WriteLine($"Updating message: ID={id}, Content={message.Content}");
            var success = await _messageService.UpdateMessageAsync(id, message);
            if (!success)
            {
                Console.WriteLine(id != message.ID ? $"Invalid message ID: {id} does not match {message.ID}" : $"Message not found for update: {id}");
                return id != message.ID ? BadRequest() : NotFound();
            }

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

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            Console.WriteLine($"Deleting message: ID={id}");
            var message = await _messageService.GetMessageByIdAsync(id);
            if (message == null)
            {
                Console.WriteLine($"Message not found: {id}");
                return NotFound();
            }

            await _messageService.DeleteMessageAsync(id);

            var conversationKey = message.Sender_ID < message.Receiver_ID
                ? $"{message.Sender_ID}_{message.Receiver_ID}"
                : $"{message.Receiver_ID}_{message.Sender_ID}";

            Console.WriteLine($"Sending DeleteMessage to group: {conversationKey}, MessageId: {id}");
            await _hubContext.Clients.Group(conversationKey).SendAsync(
                "DeleteMessage",
                id
            );

            return NoContent();
        }

        [HttpDelete("all")]
        public async Task<IActionResult> DeleteAllMessages()
        {
            try
            {
                Console.WriteLine("Deleting all messages");
                var success = await _messageService.DeleteAllMessagesAsync();
                if (!success)
                {
                    Console.WriteLine("No messages found to delete");
                    return NotFound(new { Message = "No messages found to delete." });
                }

                var messages = await _messageService.GetAllMessagesAsync();
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
                var messages = await _messageService.GetMessagesBySenderAndReceiverAsync(senderId, receiverId, skip, take);
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
                var conversations = await _messageService.GetConversationsAsync(userId);
                if (conversations == null || !conversations.Any())
                {
                    Console.WriteLine($"No conversations found for user: {userId}");
                    return NotFound(new { Message = $"No conversations found for user {userId}." });
                }

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