using HandiCraft.Application.DTOs.Notification;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Application.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task SendNotification(string userId, NotificationDto notification)
        {
            await Clients.User(userId).SendAsync("ReceiveNotification", notification);
        }
    }
}
