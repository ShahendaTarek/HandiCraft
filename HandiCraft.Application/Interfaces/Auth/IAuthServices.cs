using HandiCraft.Domain.Identity;
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
    }
}
