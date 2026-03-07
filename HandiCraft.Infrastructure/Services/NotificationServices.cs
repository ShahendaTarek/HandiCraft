using AutoMapper;
using Google.Apis.Auth.OAuth2;
using HandiCraft.Application.DTOs.Notification;
using HandiCraft.Application.Hubs;
using HandiCraft.Application.Interfaces;
using HandiCraft.Domain.Notifications;
using HandiCraft.Presistance.context;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HandiCraft.Infrastructure.Services
{
    public class NotificationServices : INotificationService
    {
        private readonly HandiCraftDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly string _serviceAccountFile;
        private readonly string _fcmProjectId;
        private readonly ILogger<NotificationServices> _logger;
        private readonly IMapper _mapper;

        public NotificationServices(HandiCraftDbContext context, IHubContext<NotificationHub> hubContext, IConfiguration configuration, ILogger<NotificationServices> logger, IMapper mapper)
        {
            _context = context;
            _hubContext = hubContext;
            _logger = logger;
            _serviceAccountFile = configuration["FCM:ServiceAccountFile"] ?? throw new ArgumentNullException("FCM:ServiceAccountFile is missing in configuration.");
            _fcmProjectId = configuration["FCM:ProjectId"] ?? throw new ArgumentNullException("FCM:ProjectId is missing in configuration.");
            _mapper = mapper;
        }
        public async Task SendAsync(string userId, string message, string type, Dictionary<string, string> data = null)
        {

            var notification = new Notification
            {
                UserId = userId,
                Message = message,
                Type = type,
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                SentViaFCM = false
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            var notificationDto = _mapper.Map<NotificationDto>(notification);
            if (data != null)
            {
                notificationDto.Data = data;
            }

            await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", notificationDto);

            var device = await _context.UserDevices
                .Where(d => d.UserId == userId)
                .OrderByDescending(d => d.LastUpdated)
                .FirstOrDefaultAsync();

            if (device == null)
                return;

            try
            {
                var credential = GoogleCredential.FromFile(_serviceAccountFile)
                    .CreateScoped("https://www.googleapis.com/auth/firebase.messaging");

                var accessToken = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();

                var fcmData = new Dictionary<string, string>
                {
                    { "type", type },
                    { "notificationId", notification.Id.ToString() }
                };

                if (data != null)
                {
                    foreach (var kvp in data)
                    {
                        fcmData[kvp.Key] = kvp.Value;
                    }
                }

                var fcmMessage = new
                {
                    message = new
                    {
                        token = device.DeviceToken,
                        notification = new
                        {
                            title = type == "ChatMessage" ? "New Message" : "New Notification",
                            body = message
                        },
                        data = fcmData
                    }
                };

                var json = JsonSerializer.Serialize(
                    fcmMessage,
                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
                );

                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(
                    $"https://fcm.googleapis.com/v1/projects/{_fcmProjectId}/messages:send",
                    content
                );

                if (response.IsSuccessStatusCode)
                {
                    notification.SentViaFCM = true;
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("FCM v1 notification sent successfully for user {UserId}", userId);
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    _logger.LogError("FCM v1 Error for user {UserId}: {Error}", userId, errorResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while sending FCM v1 notification for user {UserId}", userId);
            }
        }
        public async Task<List<NotificationDto>> GetUserNotificationsAsync(string userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return _mapper.Map<List<NotificationDto>>(notifications);
        }
        public async Task MarkAsReadAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }
        public async Task RegisterDeviceTokenAsync(string userId, string deviceToken)
        {
            var existingDevice = await _context.UserDevices
                .FirstOrDefaultAsync(d => d.UserId == userId && d.DeviceToken == deviceToken);

            if (existingDevice == null)
            {
                var device = new UserDevice
                {
                    UserId = userId,
                    DeviceToken = deviceToken,
                    LastUpdated = DateTime.UtcNow
                };
                _context.UserDevices.Add(device);
            }
            else
            {
                existingDevice.LastUpdated = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }
    }
    
    
}
