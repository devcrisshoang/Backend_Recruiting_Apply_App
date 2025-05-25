using Microsoft.EntityFrameworkCore;
using Backend_Recruiting_Apply_App.Data.Entities;
using SystemAPIdotnet.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend_Recruiting_Apply_App.Services
{
    public interface INotificationService
    {
        Task<List<Notification>> GetAllNotificationsAsync();
        Task<Notification?> GetNotificationByIdAsync(int id);
        Task<Notification> CreateNotificationAsync(Notification notification);
        Task<bool> UpdateNotificationAsync(int id, Notification notification);
        Task<bool> DeleteNotificationAsync(int id);
        Task<List<Notification>> GetNotificationsByUserIdAsync(int userId);
    }

    public class NotificationService : INotificationService
    {
        private readonly RAADbContext _dbContext;

        public NotificationService(RAADbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Notification>> GetAllNotificationsAsync()
        {
            return await _dbContext.Notification.ToListAsync();
        }

        public async Task<Notification?> GetNotificationByIdAsync(int id)
        {
            return await _dbContext.Notification.FindAsync(id);
        }

        public async Task<Notification> CreateNotificationAsync(Notification notification)
        {
            if (string.IsNullOrEmpty(notification.Content) || notification.User_ID == 0)
            {
                throw new ArgumentException("Content và User_ID là bắt buộc.");
            }

            notification.Time = DateTime.Now;
            _dbContext.Notification.Add(notification);
            await _dbContext.SaveChangesAsync();
            return notification;
        }

        public async Task<bool> UpdateNotificationAsync(int id, Notification notification)
        {
            if (id != notification.ID)
            {
                return false;
            }

            if (string.IsNullOrEmpty(notification.Content) || notification.User_ID == 0)
            {
                throw new ArgumentException("Content và User_ID là bắt buộc.");
            }

            _dbContext.Entry(notification).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return !await _dbContext.Notification.AnyAsync(e => e.ID == id);
            }
        }

        public async Task<bool> DeleteNotificationAsync(int id)
        {
            var notification = await _dbContext.Notification.FindAsync(id);
            if (notification == null)
            {
                return false;
            }

            _dbContext.Notification.Remove(notification);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<Notification>> GetNotificationsByUserIdAsync(int userId)
        {
            return await _dbContext.Notification
                .Where(n => n.User_ID == userId)
                .OrderByDescending(n => n.Time)
                .ToListAsync();
        }
    }
}