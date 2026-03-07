using HandiCraft.Application.DTOs.Services;
using HandiCraft.Application.DTOs.Settings;
using HandiCraft.Application.Interfaces;
using HandiCraft.Domain.UserAccount;
using HandiCraft.Infrastructure.Services;
using HandiCraft.Infrastructure.Services.UserConnections;
using HandiCraft.Presentation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HandiCraft.API.Controllers
{
    
    public class UserSettingsController : APIControllerBase
    {
        private readonly ISettingsServices _settingsService;

        public UserSettingsController(ISettingsServices settingService)
        {
            _settingsService=settingService;
        }
        [Authorize]
        [HttpPut("update-settings")]
        public async Task<IActionResult> UpdateSettings([FromBody] UpdateSettingsDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized(new Response(401));

            var success = await _settingsService.UpdateSettingsAsync(userId, dto);
            return success ? Ok("Settings updated") : BadRequest (new Response(400,"Failed to update settings"));
        }
        [Authorize]
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized(new Response(401));

            var updatedUser = await _settingsService.UpdateProfileAsync(userId, dto);

            if (updatedUser == null)
                return BadRequest("Failed to update profile");

            return Ok(updatedUser);
        }
        [Authorize]
        [HttpGet("blocked-users")]
        public async Task<IActionResult> GetBlockedUsers()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized(new Response(401));

            var blocked = await _settingsService.GetBlockedUsersAsync(userId);
            return Ok(blocked);
        }
        [Authorize]
        [HttpDelete("unblock/{userId}")]
        public async Task<IActionResult> UnblockUser(string userId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (currentUserId == null)
                return Unauthorized("User not authenticated");

            await _settingsService.UnblockUserAsync(currentUserId, userId);
            return Ok(new { message = "User unblocked successfully." });
        }
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            var success = await _settingsService.ForgotPasswordAsync(model);
            if (!success)
                return BadRequest("Email not found.");

            return Ok(new { message = "Password reset token has been sent to your email." });
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            var success = await _settingsService.ResetPasswordAsync(model);
            if (!success)
                return BadRequest("Invalid request. Check email, token, or password confirmation.");

            return Ok(new { message = "Password has been reset successfully." });
        }
    }
}
