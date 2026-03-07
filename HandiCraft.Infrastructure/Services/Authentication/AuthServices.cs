using HandiCraft.Application.DTOs.User;
using HandiCraft.Application.Interfaces.Auth;
using HandiCraft.Domain.Identity;
using HandiCraft.Presistance.context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;


namespace HandiCraft.Infrastructure.Services.Authentication
{
    public class AuthServices : IAuthServices
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly HandiCraftDbContext _context;

        public AuthServices(IConfiguration configuration, UserManager<ApplicationUser> userManager, HandiCraftDbContext context)
        {
            _configuration = configuration;
            _userManager = userManager;
            _context = context;
        }

        public async Task<string> CreateTokenAsync(ApplicationUser user, UserManager<ApplicationUser> userManager)
        {
            var key = _configuration["JWT:Key"];
            if (string.IsNullOrEmpty(key) || key.Length < 32)
                throw new ArgumentException("Invalid JWT Key configuration");


            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("SecurityStamp", user.SecurityStamp ?? string.Empty)
            };
            var UserRoles = await userManager.GetRolesAsync(user);
            authClaims.AddRange(UserRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            var token = new JwtSecurityToken(
               issuer: _configuration["JWT:Issuer"],
               audience: _configuration["JWT:Audience"],
               expires: DateTime.Now.AddMinutes(double.Parse(_configuration["JWT:DurationInMinutes"]!)),
               claims: authClaims,
               signingCredentials: new SigningCredentials(authKey, SecurityAlgorithms.HmacSha256)
           );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public async Task<string> GenerateRefreshTokenAsync(ApplicationUser user)
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            var refreshToken = Convert.ToBase64String(randomNumber);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); 

            await _userManager.UpdateAsync(user);

            return refreshToken;
        }
        public async Task<AuthResponseDto> RefreshTokenAsync(RefreshRequestDto request)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken);

            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                throw new SecurityTokenException("Invalid or expired refresh token");

            var newJwtToken = await CreateTokenAsync(user, _userManager);
            var newRefreshToken = await GenerateRefreshTokenAsync(user);

            return new AuthResponseDto
            {
                AccessToken = newJwtToken,
                RefreshToken = newRefreshToken
            };
        }
        public async Task<bool> RevokeRefreshTokenAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;

            await _userManager.UpdateAsync(user);
            return true;
        }


    }
}
