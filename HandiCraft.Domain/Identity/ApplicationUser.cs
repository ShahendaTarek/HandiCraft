using HandiCraft.Domain.Posts;
using HandiCraft.Domain.ProductList;
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



        public ICollection<Product> Products { get; set; }
        public ICollection<Post> Posts { get; set; }
    }
    
    
}
