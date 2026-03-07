using HandiCraft.Application.Interfaces.Auth;
using HandiCraft.Domain.Identity;
using HandiCraft.Presistance.context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;


namespace HandiCraft.Infrastructure.Services.Authentication.IdentityServices
{
    public static class IdentityServicesExtention
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection Services, IConfiguration Configuration)
        {
            Services.AddScoped<IAuthServices, AuthServices>();

            Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<HandiCraftDbContext>()
                .AddDefaultTokenProviders();

            Services.AddAuthentication(options=>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["JWT:Issuer"], 
                        ValidAudience = Configuration["JWT:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(Configuration["JWT:Key"]!)),
                        ClockSkew = TimeSpan.Zero 
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = async context =>
                        {
                            var userManager = context.HttpContext.RequestServices
                                .GetRequiredService<UserManager<ApplicationUser>>();

                            var userId = context.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
                            var securityStampClaim = context.Principal.FindFirstValue("SecurityStamp");

                            var user = await userManager.FindByIdAsync(userId);

                            if (user == null || user.SecurityStamp != securityStampClaim)
                            {
                                context.Fail("Token is no longer valid.");
                            }
                        }
                    };

                });

            //Services.AddAuthorization();

            return Services;

           
        }
        

    }
}
