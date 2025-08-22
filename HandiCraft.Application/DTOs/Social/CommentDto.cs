using HandiCraft.Domain.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Application.DTOs.Social
{
    public class CommentDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; }
        public ICollection<CommentDto> Replies { get; set; } = new List<CommentDto>();
    }
}
