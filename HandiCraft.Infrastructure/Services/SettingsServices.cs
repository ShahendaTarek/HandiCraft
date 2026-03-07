using AutoMapper;
using Azure;
using Google;
using HandiCraft.Application.DTOs.Services;
using HandiCraft.Application.DTOs.Settings;
using HandiCraft.Application.DTOs.User;
using HandiCraft.Application.Interfaces;
using HandiCraft.Application.Interfaces.Auth;
using HandiCraft.Domain.Identity;
using HandiCraft.Domain.UserAccount;
using HandiCraft.Infrastructure.EmailServices;
using HandiCraft.Presistance.context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Infrastructure.Services
{
    public class SettingsServices : ISettingsServices
    {
        private readonly HandiCraftDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        private readonly IAuthServices _authServices;
        private readonly IEmailService _emailService;
        public SettingsServices(HandiCraftDbContext context, UserManager<ApplicationUser> userManager, IMapper mapper, IWebHostEnvironment env, IAuthServices authServices, IEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
            _env = env;
            _authServices = authServices;
            _emailService = emailService;
        }
        public async Task<bool> UpdateSettingsAsync(string userId, UpdateSettingsDto dto)
        {
            var settings = await _context.Settings.FirstOrDefaultAsync(s => s.UserId == userId);

            if (settings == null)
                throw new ArgumentException("can't find settings for this user");

            _mapper.Map(dto, settings);
            settings.UpdatedAt = DateTime.UtcNow;

            _context.Settings.Update(settings);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<UserDto> UpdateProfileAsync(string userId, UpdateProfileDto model)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new ArgumentException("User not found");


            if (!string.IsNullOrEmpty(model.DisplayName))
            {
                var existingUser = await _userManager.FindByNameAsync(model.DisplayName);
                if (existingUser != null && existingUser.Id != user.Id)
                    throw new Exception("Username already taken");

                user.UserName = model.DisplayName;
            }

            if (model.ProfilePicture != null)
            {
                var fileName = $"{Guid.NewGuid()}_{model.ProfilePicture.FileName}";
                var folderPath = Path.Combine(_env.WebRootPath, "images/users");
                Directory.CreateDirectory(folderPath);
                var fullPath = Path.Combine(folderPath, fileName);

                using var stream = new FileStream(fullPath, FileMode.Create);
                await model.ProfilePicture.CopyToAsync(stream);

                user.ProfilePicture = $"/images/users/{fileName}";
            }
            _mapper.Map(model, user);

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new Exception("Failed to update user profile");

            var dto = _mapper.Map<UserDto>(user);
            dto.Token = await _authServices.CreateTokenAsync(user, _userManager);

            return dto;
        }
        public async Task UnblockUserAsync(string currentUserId, string targetUserId)
        {
            var block = await _context.UserBlocks
                .FirstOrDefaultAsync(b => b.BlockerId == currentUserId && b.BlockedId == targetUserId);

            if (block == null)
                throw new KeyNotFoundException("This user is not blocked.");

            _context.UserBlocks.Remove(block);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<UserProfileDto>> GetBlockedUsersAsync(string userId)
        {
            var blocked = await _context.UserBlocks
                .Where(x => x.BlockerId == userId)
                .Select(x => x.Blocked)
                .ToListAsync();

            return _mapper.Map<IEnumerable<UserProfileDto>>(blocked);
        }
        public async Task<bool> ForgotPasswordAsync(ForgotPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return false;

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var body = $@"
            <p>Hello,</p>
            <p>You requested to reset your password.</p>
            <p>Your reset token is:</p>
            <h3>{token}</h3>
            <p>Use this token in the app to reset your password.</p>
        ";

            await _emailService.SendEmailAsync(
                to: model.Email,
                subject: "Reset Your Password",
                body: body
            );

            return true;
        }
        public async Task<bool> ResetPasswordAsync(ResetPasswordDto model)
        {
            if (model.NewPassword != model.ConfirmPassword) return false;

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return false;

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            return result.Succeeded;
        }

        
    }
}
