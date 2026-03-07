using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Domain.UserConnections
{
    public  class Chat
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public ICollection<ChatParticipant> Participants { get; set; } = new List<ChatParticipant>();
        public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
