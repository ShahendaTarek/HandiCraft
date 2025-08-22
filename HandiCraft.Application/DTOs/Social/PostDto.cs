using HandiCraft.Domain.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Application.DTOs.Social
{
    public class PostDto
    {
        public Guid Id { get; set; }
        public string? TextContent { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public string ContentSnippet { get; set; }
        public string UserId { get; set; }

        public List<MediaItemDto> MediaItems { get; set; } = new();
        public List<CommentDto> Comments { get; set; }
        public List<ReactDto> Reactions { get; set; }
    }
}
