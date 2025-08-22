using HandiCraft.Domain.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Application.DTOs.Social
{
    public class MediaItemDto
    {
        public string Url { get; set; } = null!;
        public MediaType MediaType { get; set; }
    }
}
