using HandiCraft.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Domain.Posts
{
    public class Reaction
    {
        public int Id { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid PostId { get; set; }
        public Post Post { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
