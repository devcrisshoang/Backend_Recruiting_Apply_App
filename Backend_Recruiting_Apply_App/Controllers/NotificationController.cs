using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Backend_Recruiting_Apply_App.Data.Entities;
using Backend_Recruiting_Apply_App.Hubs;
using Backend_Recruiting_Apply_App.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend_Recruiting_Apply_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationController(INotificationService notificationService, IHubContext<NotificationHub> hubContext)
        {
            _notificationService = notificationService;
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Notification>>> GetNotification()
        {
            var notifications = await _notificationService.GetAllNotificationsAsync();
            return Ok(notifications);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Notification>> GetNotification(int id)
        {
            var notification = await _notificationService.GetNotificationByIdAsync(id);
            if (notification == null)
            {
                return NotFound();
            }
            return Ok(notification);
        }

        [HttpPost]
        public async Task<ActionResult<Notification>> CreateNotification(Notification notification)
        {
            try
            {
                var createdNotification = await _notificationService.CreateNotificationAsync(notification);

                await _hubContext.Clients.Group($"User_{createdNotification.User_ID}")
                    .SendAsync("ReceiveNotification", createdNotification.User_ID, createdNotification.Name, createdNotification.Content, createdNotification.Time.ToString("o"));
                Console.WriteLine($"Gửi thông báo tới User_{createdNotification.User_ID}: {createdNotification.Content}");

                return CreatedAtAction(nameof(GetNotification), new { id = createdNotification.ID }, createdNotification);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Dữ liệu thông báo không hợp lệ");
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNotification(int id, Notification notification)
        {
            try
            {
                var success = await _notificationService.UpdateNotificationAsync(id, notification);
                if (!success)
                {
                    return id != notification.ID ? BadRequest("ID thông báo không khớp.") : NotFound();
                }

                await _hubContext.Clients.Group($"User_{notification.User_ID}")
                    .SendAsync("ReceiveNotification", notification.User_ID, notification.Name, notification.Content, notification.Time.ToString("o"));
                Console.WriteLine($"Cập nhật thông báo cho User_{notification.User_ID}: {notification.Content}");

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var notification = await _notificationService.GetNotificationByIdAsync(id);
            if (notification == null)
            {
                return NotFound();
            }

            await _notificationService.DeleteNotificationAsync(id);

            await _hubContext.Clients.Group($"User_{notification.User_ID}")
                .SendAsync("ReceiveNotification", notification.User_ID, notification.Name, "Thông báo đã bị xóa", DateTime.UtcNow.ToString("o"));
            Console.WriteLine($"Xóa thông báo ID {id} cho User_{notification.User_ID}");

            return NoContent();
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Notification>>> GetNotificationByUserID(int userId)
        {
            var notifications = await _notificationService.GetNotificationsByUserIdAsync(userId);
            return Ok(notifications);
        }
    }
}