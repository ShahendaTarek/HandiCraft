using HandiCraft.Application.Interfaces;
using HandiCraft.Domain.UserConnections;
using HandiCraft.Infrastructure.Services.UserConnections;
using HandiCraft.Presentation;
using HandiCraft.Presistance.context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HandiCraft.API.Controllers
{
    [Authorize]
    public class UserConnectionController : APIControllerBase
    {

        private readonly IUserConnectionsServices _userConnectionService;
        public UserConnectionController(IUserConnectionsServices userConnectionService)
        {
            _userConnectionService = userConnectionService;
        }
        [Authorize]
        [HttpPost("follow/{userId}")]
        public async Task<IActionResult> FollowUser(string userId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == null)
                return BadRequest(new Response(400,"User not authenticated."));

            if (currentUserId == userId)
                return BadRequest(new Response(400,"You cannot follow yourself."));

            try
            {
                var result = await _userConnectionService.FollowUserAsync(currentUserId, userId);
                return Ok(new { message = "Followed successfully." });

            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }


            
        }
        
        [HttpGet("followers/{userId}")]

        public async Task<IActionResult> GetFollowers(string userId)
        {
            var followers = await _userConnectionService.GetFollowersAsync(userId);

            if (followers == null || !followers.Any())
                return NotFound(new Response(404,"No followers found."));

            return Ok(followers);
        }
        [HttpGet("following/{userId}")]
        public async Task<IActionResult> GetFollowing(string userId)
        {
            var following = await _userConnectionService.GetFollowingAsync(userId);

            if (following == null || !following.Any())
                return NotFound("This user is not following anyone yet.");

            return Ok(following);
        }

        [HttpDelete("unfollow/{userId}")]
        public async Task<IActionResult> UnfollowUser(string userId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (currentUserId == null)
                return Unauthorized("User not authenticated");

            var follow = await _userConnectionService.UnfollowUserAsync(currentUserId, userId);

            if (follow == null)
                return NotFound(new Response(404,"You are not following this user"));

            return Ok("Unfollowed successfully");
        }


      
        [HttpPost("block/{userId}")]
        public async Task<IActionResult> BlockUser(string userId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (currentUserId == null)
                return Unauthorized("User not authenticated");


            try
            {
                await _userConnectionService.BlockUserAsync(currentUserId, userId);
                return Ok(new { message = "User blocked successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        
    }
}
