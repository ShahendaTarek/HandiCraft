using AutoMapper;
using HandiCraft.Application.DTOs.Chat;
using HandiCraft.Application.Interfaces;
using HandiCraft.Presentation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HandiCraft.API.Controllers
{
  
    public class ChatController : APIControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }
        [HttpPost("start/{recipientId}")]
        public async Task<IActionResult> StartChat(string recipientId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new Response(401,"User is not authenticated."));
            }

            if (string.IsNullOrEmpty(recipientId))
            {
                return BadRequest(new Response(400,"Recipient ID is required."));
            }

            try
            {
                var chat = await _chatService.GetOrCreateChatAsync(userId, recipientId);
                return Ok(chat);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("{recipientId}/send")]
        public async Task<IActionResult> SendMessage(string recipientId, [FromBody] string message)
        {
            var senderId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(senderId))
            {
                return Unauthorized("User is not authenticated.");
            }

            if (string.IsNullOrEmpty(message))
            {
                return BadRequest("Message cannot be empty.");
            }

            var chatMessage = await _chatService.SaveMessageAsync(senderId, recipientId, message);
            return Ok(chatMessage);
        }
        [HttpGet("{chatId}/messages")]
        public async Task<IActionResult> GetChatMessages(Guid chatId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new Response(401,"User is not authenticated."));
            }

            try
            {
                var messages = await _chatService.GetChatMessagesAsync(chatId, userId);
                return Ok(messages);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }
        [HttpPost("{chatId}/mark-read")]
        public async Task<IActionResult> MarkMessagesAsRead(Guid chatId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new Response(401,"User is not authenticated."));
            }

            try
            {
                await _chatService.MarkMessagesAsReadAsync(chatId, userId);
                return Ok();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }
        [HttpGet("my-chats")]
        public async Task<IActionResult> GetMyChats()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated.");
            }

            try
            {
                var chats = await _chatService.GetUserChatsAsync(userId);
                return Ok(chats);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("{chatId}/messages/{messageId}")]
        public async Task<IActionResult> DeleteMessage(Guid chatId, Guid messageId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated.");
            }

            try
            {
                await _chatService.DeleteMessageAsync(chatId, messageId, userId);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }


    }
}

