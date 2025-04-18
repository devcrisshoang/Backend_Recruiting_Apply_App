using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Backend_Recruiting_Apply_App.Data.Entities;
using Backend_Recruiting_Apply_App.Hubs;
using TopCVSystemAPIdotnet.Data;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Backend_Recruiting_Apply_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly RAADbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationController(RAADbContext context, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Notification>>> GetNotification()
        {
            return await _context.Notification.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Notification>> GetNotification(int id)
        {
            var notification = await _context.Notification.FindAsync(id);
            if (notification == null)
            {
                return NotFound();
            }
            return notification;
        }

        [HttpPost]
        public async Task<ActionResult<Notification>> CreateNotification(Notification notification)
        {
            if (string.IsNullOrEmpty(notification.Content) || notification.User_ID == 0)
            {
                Console.WriteLine("Dữ liệu thông báo không hợp lệ");
                return BadRequest("Content và User_ID là bắt buộc.");
            }

            notification.Time = DateTime.Now;
            _context.Notification.Add(notification);
            await _context.SaveChangesAsync();

            // Gửi thông báo qua SignalR
            await _hubContext.Clients.Group($"User_{notification.User_ID}")
                .SendAsync("ReceiveNotification", notification.User_ID, notification.Name, notification.Content, notification.Time.ToString("o"));
            Console.WriteLine($"Gửi thông báo tới User_{notification.User_ID}: {notification.Content}");

            return CreatedAtAction(nameof(GetNotification), new { id = notification.ID }, notification);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNotification(int id, Notification notification)
        {
            if (id != notification.ID)
            {
                return BadRequest("ID thông báo không khớp.");
            }

            if (string.IsNullOrEmpty(notification.Content) || notification.User_ID == 0)
            {
                return BadRequest("Content và User_ID là bắt buộc.");
            }

            _context.Entry(notification).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                // Gửi thông báo cập nhật qua SignalR
                await _hubContext.Clients.Group($"User_{notification.User_ID}")
                    .SendAsync("ReceiveNotification", notification.User_ID, notification.Name, notification.Content, notification.Time.ToString("o"));
                Console.WriteLine($"Cập nhật thông báo cho User_{notification.User_ID}: {notification.Content}");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Notification.Any(e => e.ID == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var notification = await _context.Notification.FindAsync(id);
            if (notification == null)
            {
                return NotFound();
            }

            _context.Notification.Remove(notification);
            await _context.SaveChangesAsync();

            // Gửi thông báo xóa qua SignalR
            await _hubContext.Clients.Group($"User_{notification.User_ID}")
                .SendAsync("ReceiveNotification", notification.User_ID, notification.Name, "Thông báo đã bị xóa", DateTime.UtcNow.ToString("o"));
            Console.WriteLine($"Xóa thông báo ID {id} cho User_{notification.User_ID}");

            return NoContent();
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Notification>>> GetNotificationByUserID(int userId)
        {
            var notifications = await _context.Notification
                .Where(n => n.User_ID == userId)
                .OrderByDescending(n => n.Time)
                .ToListAsync();

            return Ok(notifications);
        }
    }
}