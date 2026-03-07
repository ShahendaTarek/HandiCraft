using HandiCraft.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HandiCraft.Application.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;

        public ChatHub(IChatService chatService)
        {
            _chatService = chatService;
        }

        public async Task SendMessage(string recipientId, string message)
        {
            var senderId = Context.UserIdentifier; 

            if (string.IsNullOrEmpty(senderId))
            {
                throw new HubException("User is not authenticated.");
            }

            var chatMessage = await _chatService.SaveMessageAsync(senderId, recipientId, message);

            await Clients.User(recipientId).SendAsync("ReceiveMessage", senderId, chatMessage.Text, chatMessage.SentAt, chatMessage.Id);
            await Clients.User(senderId).SendAsync("ReceiveMessage", senderId, chatMessage.Text, chatMessage.SentAt, chatMessage.Id);
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
            }
            await base.OnDisconnectedAsync(exception);
        }
        public async Task DeleteMessage(Guid chatId, Guid messageId)
        {
            var userId = Context.UserIdentifier;
            if (string.IsNullOrEmpty(userId))
            {
                throw new HubException("User is not authenticated.");
            }

            await _chatService.DeleteMessageAsync(chatId, messageId, userId);
        }
    }
}

