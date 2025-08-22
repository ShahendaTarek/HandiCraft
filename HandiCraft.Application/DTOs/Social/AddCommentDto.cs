using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Application.DTOs.Social
{
    public class AddCommentDto
    {
        public Guid PostId { get; set; }
        public string Content { get; set; }
        public int? ParentCommentId { get; set; }
    }
}
