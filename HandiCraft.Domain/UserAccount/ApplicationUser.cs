using HandiCraft.Domain.Orders;
using HandiCraft.Domain.Posts;
using HandiCraft.Domain.ProductList;
using HandiCraft.Domain.UserAccount;
using HandiCraft.Domain.UserConnections;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Domain.Identity
{
    public  class ApplicationUser : IdentityUser
    {
        public string DisplayName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public UserType UserType { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? ProfilePicture { get; set; }
        public UserSettings Settings { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }


        public ICollection<Product> Products { get; set; } = new List<Product>();
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<UserFollow> Followers { get; set; } = new List<UserFollow>();
        public ICollection<UserFollow> Followings { get; set; } = new List<UserFollow>();
        public ICollection<UserBlock> BlockedUsers { get; set; } = new List<UserBlock>();
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();


    }
    
    
}
