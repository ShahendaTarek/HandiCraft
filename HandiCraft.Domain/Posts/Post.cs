using HandiCraft.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace HandiCraft.Domain.Posts
{
    public class Post
    {
        public Guid Id { get; set; }
        public string? TextContent { get; set; }
        public string? MediaUrl { get; set; } 
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; }


        public ApplicationUser User { get; set; }
        public List<Media> MediaItems { get; set; } = new();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();
    }
}
