using HandiCraft.Application.DTOs.User;
using HandiCraft.Domain.Identity;
using HandiCraft.Domain.UserAccount;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Application.Interfaces.Auth
{
    public interface IAuthServices
    {
        Task<string> CreateTokenAsync(ApplicationUser user, UserManager<ApplicationUser> userManger);
        Task<string> GenerateRefreshTokenAsync(ApplicationUser user);

        Task<AuthResponseDto> RefreshTokenAsync(RefreshRequestDto request);

        Task<bool> RevokeRefreshTokenAsync(string refreshToken);

    }
}
