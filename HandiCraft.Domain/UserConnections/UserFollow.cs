using HandiCraft.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Domain.UserConnections
{
    public class UserFollow
    {
        public string FollowerId { get; set; }
        public ApplicationUser Follower { get; set; }

        public string FollowedId { get; set; }
        public ApplicationUser Followed { get; set; }

        public DateTime FollowedAt { get; set; } = DateTime.UtcNow;
    }
}
