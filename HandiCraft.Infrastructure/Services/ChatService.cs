using AutoMapper;
using HandiCraft.Application.DTOs.Chat;
using HandiCraft.Application.Hubs;
using HandiCraft.Application.Interfaces;
using HandiCraft.Domain.UserConnections;
using HandiCraft.Presistance.context;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HandiCraft.Infrastructure.Services
{
    public class ChatService : IChatService
    {
        private readonly HandiCraftDbContext _context;
        private readonly IHubContext<ChatHub> _chatHubContext;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;
        private readonly ILogger<ChatService> _logger;


        public ChatService(HandiCraftDbContext context, IHubContext<ChatHub> chatHubContext, INotificationService notificationService, IMapper mapper, ILogger<ChatService> logger)
        {
            _context = context;
            _chatHubContext = chatHubContext;
            _notificationService = notificationService;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task <ChatMessageDto> SaveMessageAsync(string senderId, string recipientId, string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentException("Message cannot be empty.");
            }

            var chat = await GetOrCreateChatAsync(senderId, recipientId);

            var chatMessage = new ChatMessage
            {
                ChatId = chat.Id,
                SenderId = senderId,
                Text = message,
                SentAt = DateTime.UtcNow,
                IsRead = false
            };

            _context.Messages.Add(chatMessage); 
            await _context.SaveChangesAsync();

            await _notificationService.SendAsync(
                recipientId,
                $"New message: {message}",
                "ChatMessage",
            new Dictionary<string, string> { { "chatId", chat.Id.ToString() } });

            return _mapper.Map<ChatMessageDto>(chatMessage);
        }
        public async Task<Chat>GetOrCreateChatAsync(string userId1, string userId2)
        {
            if (userId1 == userId2)
            {
                throw new ArgumentException("Cannot create a chat with the same user.");
            }

            var chat = await _context.Chats
                .Include(c => c.Participants)
                .FirstOrDefaultAsync(c =>
                    c.Participants.Count == 2 &&
                    c.Participants.Any(p => p.UserId == userId1) &&
                    c.Participants.Any(p => p.UserId == userId2));

            if (chat == null)
            {
                chat = new Chat
                {
                    CreatedAt = DateTime.UtcNow
                };

                chat.Participants.Add(new ChatParticipant { UserId = userId1 });
                chat.Participants.Add(new ChatParticipant { UserId = userId2 });

                _context.Chats.Add(chat);
                await _context.SaveChangesAsync();
            }

            return chat; 
        }
        public async Task<List<ChatMessageDto>> GetChatMessagesAsync(Guid chatId, string userId)
        {
            var chat = await _context.Chats
                 .Include(c => c.Participants)
                 .FirstOrDefaultAsync(c => c.Id == chatId && c.Participants.Any(p => p.UserId == userId));

            if (chat == null)
            {
                throw new UnauthorizedAccessException("User is not a participant in this chat.");
            }

            var messages = await _context.Messages
                .Where(m => m.ChatId == chatId && !m.IsDeleted)
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            return _mapper.Map<List<ChatMessageDto>>(messages);
        }



        public async Task MarkMessagesAsReadAsync(Guid chatId, string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("User ID cannot be empty.");
            }

            var chat = await _context.Chats
                .Include(c => c.Participants)
                .FirstOrDefaultAsync(c => c.Id == chatId && c.Participants.Any(p => p.UserId == userId));

            if (chat == null)
            {
                throw new UnauthorizedAccessException("User is not a participant in this chat.");
            }

            var unreadMessages = await _context.Messages
                .Where(m => m.ChatId == chatId && m.SenderId != userId && !m.IsRead)
                .ToListAsync();

            _logger.LogInformation("Found {Count} unread messages for chat {ChatId} and user {UserId}", unreadMessages.Count, chatId, userId);

            if (!unreadMessages.Any())
            {
                _logger.LogWarning("No unread messages found for chat {ChatId} and user {UserId}", chatId, userId);
            }

            foreach (var message in unreadMessages)
            {
                message.IsRead = true;
                message.ReadAt = DateTime.UtcNow;
                _logger.LogInformation("Marked message {MessageId} as read for chat {ChatId}", message.Id, chatId);
            }

            var changes = await _context.SaveChangesAsync();
            _logger.LogInformation("Saved {Changes} changes to the database for chat {ChatId}", changes, chatId);

            foreach (var message in unreadMessages)
            {
                await _chatHubContext.Clients.User(message.SenderId)
                    .SendAsync("MessageRead", message.Id, userId, message.ReadAt);
                _logger.LogInformation("Notified sender {SenderId} of message {MessageId} read status", message.SenderId, message.Id);
            }
        }
        public async Task<List<ChatDto>> GetUserChatsAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("User ID cannot be empty.");
            }

            var chats = await _context.Chats
                .Include(c => c.Participants)
                .Where(c => c.Participants.Any(p => p.UserId == userId))
                .ToListAsync();

            return _mapper.Map<List<ChatDto>>(chats);
        }
        public async Task DeleteMessageAsync(Guid chatId, Guid messageId, string userId)
        {
            var chat = await _context.Chats
                .Include(c => c.Participants)
                .FirstOrDefaultAsync(c => c.Id == chatId && c.Participants.Any(p => p.UserId == userId));

            if (chat == null)
            {
                throw new UnauthorizedAccessException("User is not a participant in this chat.");
            }

            var message = await _context.Messages
                .FirstOrDefaultAsync(m => m.Id == messageId && m.ChatId == chatId && m.SenderId == userId && !m.IsDeleted);

            if (message == null)
            {
                throw new KeyNotFoundException("Message not found or already deleted.");
            }

            message.IsDeleted = true;
            await _context.SaveChangesAsync();

            foreach (var participant in chat.Participants)
            {
                await _chatHubContext.Clients.User(participant.UserId)
                    .SendAsync("MessageDeleted", chatId, messageId);
            }
        }

    }
}
