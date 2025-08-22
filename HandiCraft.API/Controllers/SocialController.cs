using HandiCraft.Application.DTOs.Social;
using HandiCraft.Application.Interfaces;
using HandiCraft.Presentation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HandiCraft.API.Controllers
{
   
    public class SocialController :APIControllerBase
    {
        private readonly IPostServices _postService;
        public SocialController(IPostServices postService)
        {
            _postService = postService;
        }


        [Authorize]
        [HttpPost("Post")]
        public async Task<IActionResult> CreatePost([FromForm] CreatePostDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized(new Response(401,"not allowed to add posts"));

            var result = await _postService.CreatePostAsync(userId, dto);
            return Ok(result);
        }
        [Authorize]
        [HttpPost("comment")]
        public async Task<IActionResult> AddComment([FromBody] AddCommentDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _postService.AddCommentAsync(userId, dto);
            return Ok(result);
        }
        [Authorize]
        [HttpPost("reaction")]
        public async Task<IActionResult> AddReaction([FromBody] AddReactDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _postService.AddReactionAsync(userId, dto);
            return Ok(result);
        }
        [Authorize]
        [HttpDelete("{postId:guid}")]
        public async Task<IActionResult> DeletePost(Guid postId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _postService.DeletePostAsync(userId, postId);
            return Ok(result);
        }
        [Authorize]
        [HttpDelete("comment/{commentId:int}")]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _postService.DeleteCommentAsync(userId, commentId);
            return Ok(result);
        }
        [Authorize]
        [HttpDelete("reaction/{reactionId:int}")]
        public async Task<IActionResult> DeleteReaction(int reactionId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _postService.DeleteReactionAsync(userId, reactionId);
            return Ok(result);
        }
        [Authorize]
        [HttpPut("{postId:guid}")]
        public async Task<IActionResult> EditPost(Guid postId, [FromForm] string? newTextContent)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _postService.EditPostAsync(userId, postId, newTextContent);
            return Ok(result);
        }
        [Authorize]
        [HttpPut("comment/{commentId:int}")]
        public async Task<IActionResult> EditComment(int commentId, [FromForm] string newText)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _postService.EditCommentAsync(userId, commentId, newText);
            return Ok(result);
        }
        [Authorize]
        [HttpGet("user")]
        public async Task<IActionResult> GetPostsByUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _postService.GetAllPostsByUserIdAsync(userId);
            return Ok(result);
        }

        [HttpGet("{postId:guid}")]
        public async Task<IActionResult> GetPostById(Guid postId)
        {
            var result = await _postService.GetPostByIdAsync(postId);
            return Ok(result);
        }

        [HttpGet("{postId:guid}/comments")]
        public async Task<IActionResult> GetCommentsForPost(Guid postId)
        {
            var result = await _postService.GetAllCommentsAsync(postId);
            return Ok(result);
        }

        [HttpGet("{postId:guid}/reactions")]
        public async Task<IActionResult> GetReactionsForPost(Guid postId)
        {
            var result = await _postService.GetAllReactsAsync(postId);
            return Ok(result);
        }


    }
}
