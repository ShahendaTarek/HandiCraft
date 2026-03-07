using AutoMapper;
using HandiCraft.Application.DTOs.Services;
using HandiCraft.Application.DTOs.Settings;
using HandiCraft.Application.DTOs.User;
using HandiCraft.Application.Interfaces;
using HandiCraft.Application.Interfaces.Auth;
using HandiCraft.Domain.Identity;
using HandiCraft.Domain.UserAccount;
using HandiCraft.Infrastructure.EmailServices;
using HandiCraft.Infrastructure.Services.UserConnections;
using HandiCraft.Presentation;
using HandiCraft.Presistance.context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;

namespace HandiCraft.API.Controllers
{
    public class AccountController : APIControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthServices _AuthServics;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly HandiCraftDbContext _context;
        private readonly IFavoriteServices _favoriteServices;

        public AccountController(UserManager<ApplicationUser> userManager, IWebHostEnvironment env, IAuthServices authServics, IMapper mapper, IEmailService emailService, HandiCraftDbContext context, IFavoriteServices favoriteServices)
        {
            _userManager = userManager;
            _env = env;
            _AuthServics = authServics;
            _mapper = mapper;
            _emailService = emailService;
            _context = context;
            _favoriteServices = favoriteServices;
        }
        [HttpPost("Register")]
        public async Task<ActionResult<UserDto>> Register([FromForm] RegisterDto model)
        {
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return BadRequest(new Response(400, "Email is already in use."));
                   
            }
            if (model.Password != model.ConfirmPassword)
                return BadRequest(new Response(400,"Passwords do not match."));

            string? profileImageUrl = null;

            if (model.ProfilePicture != null)
            {
                var fileName = $"{Guid.NewGuid()}_{model.ProfilePicture.FileName}";
                var path = Path.Combine(_env.WebRootPath, "images/users");

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                var fullPath = Path.Combine(path, fileName);
                using var stream = new FileStream(fullPath, FileMode.Create);
                await model.ProfilePicture.CopyToAsync(stream);

                profileImageUrl = $"/images/users/{fileName}";
            }

            var user = new ApplicationUser
            {
                DisplayName = model.DisplayName,
                Email = model.Email,
                UserName = model.Email.Split("@")[0],
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserType = model.UserType,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                Gender = model.Gender,
                DateOfBirth = model.DateOfBirth,
                ProfilePicture = profileImageUrl
                
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);
            var settings = new UserSettings
            {
                UserId = user.Id,
                IsNotificationEnabled = true,
                IsDarkMode = false,
                PreferredLanguage = Language.English, 
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.Settings.AddAsync(settings);
            await _context.SaveChangesAsync();
            var ReturnedObject = _mapper.Map<UserDto>(user);
            ReturnedObject.Token = await _AuthServics.CreateTokenAsync(user, _userManager);

            

            return Ok(new
            {
                message = "User registered successfully",
                ReturnedObject
            });
        }
        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
        [HttpPost("Login")]
        public async Task<ActionResult>Login(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return Unauthorized(new Response(401, "Invalid email or password"));

            var token = await _AuthServics.CreateTokenAsync(user, _userManager);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            return Ok(new AuthResponseDto
            {
                AccessToken = token,
                RefreshToken = refreshToken
            });
        }
        [Authorize]
        [HttpGet("GetUser")]
        public async Task<ActionResult<UserDto>>GetCurrentUser()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            
            if (userEmail == null)
            {

                return Unauthorized(new Response(401,"Invalid token or not authenticated."));
            }
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
                return NotFound(new Response(400,"User not found."));

            var ReturnedObject = _mapper.Map<UserDto>(user);
            ReturnedObject.Token = await _AuthServics.CreateTokenAsync(user, _userManager);

            return Ok(ReturnedObject);
            
        }
      
      
        [HttpGet("AllUsers")]
        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            return _mapper.Map<List<UserDto>>(users);
        }
        [HttpDelete("remove-favorite/{id}")]
        [Authorize]
        public async Task<IActionResult> RemoveFromFavorite(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var result = await _favoriteServices.RemoveFromFavoritesAsync(id, userId);
            if (!result)
                return BadRequest("Failed to remove item from favorites.");

            return Ok("Removed from favorites successfully.");

      
        }
        [HttpPost("RefreshToken")]
        public async Task<ActionResult<AuthResponseDto>> RefreshToken([FromBody] RefreshRequestDto model)
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == model.RefreshToken);

            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return Unauthorized(new Response(401, "Invalid or expired refresh token"));

            var newAccessToken = await _AuthServics.CreateTokenAsync(user, _userManager);
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            return Ok(new AuthResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }

        [HttpGet("my-favorites")]
        [Authorize]
        public async Task<IActionResult> GetMyFavorites()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var result = await _favoriteServices.GetUserFavoritesAsync(userId);
            return Ok(result);
        }
        [Authorize]
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("User not found.");

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await _userManager.UpdateAsync(user);

            await _userManager.UpdateSecurityStampAsync(user);

            return Ok(new { message = "Logged out successfully." });
        }
    }
}
