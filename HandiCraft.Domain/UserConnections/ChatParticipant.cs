using HandiCraft.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Domain.UserConnections
{
    public class ChatParticipant
    {

        public Guid ChatId { get; set; }
        public Chat Chat { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }
}

