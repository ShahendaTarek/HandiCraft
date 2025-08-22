using HandiCraft.Domain.UserConnections;
using HandiCraft.Presentation;
using HandiCraft.Presistance.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HandiCraft.API.Controllers
{
    public class UserConnectionController : APIControllerBase
    {

        private readonly HandiCraftDbContext _context;
        public UserConnectionController(HandiCraftDbContext context)
        {
            _context = context;
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

            var existingFollow = await _context.UserFollows
                .FirstOrDefaultAsync(x => x.FollowerId == currentUserId && x.FollowedId == userId);

            if (existingFollow != null)
                return BadRequest(new Response(400,"You already follow this user."));

            var follow = new UserFollow
            {
                FollowerId = currentUserId,
                FollowedId = userId
            };

            _context.UserFollows.Add(follow);
            await _context.SaveChangesAsync();

            return Ok("Followed user successfully.");
        }
        [Authorize]
        [HttpGet("followers")]
        public async Task<IActionResult> GetFollowers()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var followers = await _context.UserFollows
                .Where(x => x.FollowedId == userId)
                .Select(x => x.Follower.DisplayName)
                .ToListAsync();

            return Ok(followers);
        }
        [Authorize]
        [HttpDelete("unfollow/{userId}")]
        public async Task<IActionResult> UnfollowUser(string userId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (currentUserId == null)
                return Unauthorized("User not authenticated");

            var follow = await _context.UserFollows
                .FirstOrDefaultAsync(f => f.FollowerId == currentUserId && f.FollowedId == userId);

            if (follow == null)
                return NotFound(new Response(404,"You are not following this user"));

            _context.UserFollows.Remove(follow);
            await _context.SaveChangesAsync();

            return Ok("Unfollowed successfully");
        }


        [Authorize]
        [HttpPost("block/{userId}")]
        public async Task<IActionResult> BlockUser(string userId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (currentUserId == userId)
                return BadRequest(new Response(400,"You cannot block yourself."));

            var exists = await _context.UserBlocks
                .AnyAsync(x => x.BlockerId == currentUserId && x.BlockedId == userId);

            if (exists)
                return BadRequest("You already blocked this user.");

            _context.UserBlocks.Add(new UserBlock
            {
                BlockerId = currentUserId,
                BlockedId = userId
            });

            var follow = await _context.UserFollows
                .FirstOrDefaultAsync(x => x.FollowerId == userId && x.FollowedId == currentUserId);

            if (follow != null)
                _context.UserFollows.Remove(follow);

            await _context.SaveChangesAsync();

            return Ok("User blocked.");
        }
    }
}
