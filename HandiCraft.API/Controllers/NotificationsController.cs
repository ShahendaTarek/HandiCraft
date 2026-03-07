using HandiCraft.Application.DTOs.Notification;
using HandiCraft.Application.Interfaces;
using HandiCraft.Presentation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HandiCraft.API.Controllers
{
    [Authorize]
    public class NotificationsController : APIControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        
        [HttpPost("send")]
        public async Task<IActionResult> SendNotification([FromBody] NotificationDto dto)
        {
            try
            {
                
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return  Unauthorized(new Response(401,"User ID not found in token."));
                }

                if (string.IsNullOrWhiteSpace(dto.UserId) || string.IsNullOrWhiteSpace(dto.Message) || string.IsNullOrWhiteSpace(dto.Type))
                {
                    return BadRequest(new Response(400,"UserId, Message, and Type are required."));
                }

                await _notificationService.SendAsync(dto.UserId, dto.Message, dto.Type);
                return Ok(new { Message = "Notification sent successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while sending the notification.");
            }
        }
        [HttpPost("register-device")]
        public async Task<IActionResult> RegisterDevice([FromBody] RegisterDeviceRequestDto dto)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return Unauthorized(new Response(401,"User ID not found in token."));
                }

                if (string.IsNullOrWhiteSpace(dto.DeviceToken))
                {
                    return BadRequest(new Response(400,"Device token is required."));
                }

                await _notificationService.RegisterDeviceTokenAsync(currentUserId, dto.DeviceToken);
                return Ok(new { Message = "Device token registered successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while registering the device token.");
            }
        }
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserNotifications(string userId)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (currentUserId != userId)
                {
                    return Unauthorized("You can only access your own notifications.");
                }

                var notifications = await _notificationService.GetUserNotificationsAsync(userId);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving notifications.");
            }
        }
        [HttpPut("{notificationId}/mark-as-read")]
        public async Task<IActionResult> MarkAsRead(int notificationId)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return Unauthorized("User ID not found in token.");
                }

                await _notificationService.MarkAsReadAsync(notificationId);
                return Ok(new { Message = "Notification marked as read." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while marking the notification as read.");
            }
        }

    }
}
