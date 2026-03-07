using HandiCraft.Application.DTOs.Notification;
using HandiCraft.Domain.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Application.Interfaces
{
    public interface INotificationService
    {
        Task SendAsync(string userId, string message, string type, Dictionary<string, string> data = null);
        Task<List<NotificationDto>> GetUserNotificationsAsync(string userId);
        Task MarkAsReadAsync(int notificationId);
        Task RegisterDeviceTokenAsync(string userId, string deviceToken);
    }
}
