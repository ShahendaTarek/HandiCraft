using Azure;
using HandiCraft.Application.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Application.Interfaces
{
    public interface IUserConnectionsServices
    {
        Task <bool>FollowUserAsync(string currentUserId, string targetUserId);
        Task<List<UserProfileDto>> GetFollowersAsync(string userId);
        Task<List<UserProfileDto>> GetFollowingAsync(string userId);
        Task <bool>UnfollowUserAsync(string currentUserId, string targetUserId);
        Task <bool>BlockUserAsync(string currentUserId, string targetUserId);
        
    }
}
