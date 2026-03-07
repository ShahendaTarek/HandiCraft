using HandiCraft.Application.DTOs.Services;
using HandiCraft.Application.DTOs.Settings;
using HandiCraft.Application.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Application.Interfaces
{
    public interface ISettingsServices
    {
        Task<UserDto> UpdateProfileAsync(string userId, UpdateProfileDto dto);
        Task<bool> UpdateSettingsAsync(string userId, UpdateSettingsDto dto);
        Task<IEnumerable<UserProfileDto>> GetBlockedUsersAsync(string userId);
        Task UnblockUserAsync(string userId, string blockedUserId);
        Task<bool> ForgotPasswordAsync(ForgotPasswordDto model);
        Task<bool> ResetPasswordAsync(ResetPasswordDto model);
    }
}
