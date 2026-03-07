using HandiCraft.Application.DTOs.Chat;
using HandiCraft.Domain.UserConnections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Application.Interfaces
{
    public interface IChatService
    {
        Task<ChatMessageDto> SaveMessageAsync(string senderId, string recipientId, string message);
        Task<Chat> GetOrCreateChatAsync(string userId1, string userId2);
        Task<List<ChatMessageDto>> GetChatMessagesAsync(Guid chatId, string userId);
        Task MarkMessagesAsReadAsync(Guid chatId, string userId);
        Task<List<ChatDto>> GetUserChatsAsync(string userId);
        Task DeleteMessageAsync(Guid chatId, Guid messageId, string userId);
    }
}
    

