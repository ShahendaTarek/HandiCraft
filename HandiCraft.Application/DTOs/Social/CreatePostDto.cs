using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Application.DTOs.Social
{
    public class CreatePostDto
    {
        public string? TextContent { get; set; }
        public  List<IFormFile>? MediaItems { get; set; }
    }
}
