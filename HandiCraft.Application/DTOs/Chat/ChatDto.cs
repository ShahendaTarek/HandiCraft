using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Application.DTOs.Chat
{
    public class ChatDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ChatParticipantDto> Participants { get; set; } = new List<ChatParticipantDto>();
    }
}
