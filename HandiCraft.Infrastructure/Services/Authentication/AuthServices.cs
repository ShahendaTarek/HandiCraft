using HandiCraft.Application.Interfaces.Auth;
using HandiCraft.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;


namespace HandiCraft.Infrastructure.Services.Authentication
{
    public class AuthServices : IAuthServices
    {
        private readonly IConfiguration _configuration;

        public AuthServices(IConfiguration configuration)
        {
            _configuration = configuration;
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
            };
            var UserRoles = await userManager.GetRolesAsync(user);
            authClaims.AddRange(UserRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            var token = new JwtSecurityToken(
               issuer: _configuration["JWT:Issuer"],
               audience: _configuration["JWT:Audience"],
               expires: DateTime.Now.AddDays(double.Parse(_configuration["JWT:DurationInDays"]!)),
               claims: authClaims,
               signingCredentials: new SigningCredentials(authKey, SecurityAlgorithms.HmacSha256)
           );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}
