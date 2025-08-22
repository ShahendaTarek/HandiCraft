using HandiCraft.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Domain.UserConnections
{
    public class UserBlock
    {
        public string BlockerId { get; set; }
        public ApplicationUser Blocker { get; set; }

        public string BlockedId { get; set; }
        public ApplicationUser Blocked { get; set; }

        public DateTime BlockedAt { get; set; } = DateTime.UtcNow;
    }
}
