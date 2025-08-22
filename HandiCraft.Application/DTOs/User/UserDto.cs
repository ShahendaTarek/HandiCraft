using HandiCraft.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Application.DTOs.User
{
    public class UserDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public UserType UserType { get; set; }
        public string Token { get; set; }
    }
    
}
