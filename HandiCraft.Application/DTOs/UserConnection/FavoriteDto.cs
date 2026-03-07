using HandiCraft.Application.DTOs.ProductList;
using HandiCraft.Application.DTOs.Social;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Application.DTOs.UserConnection
{
    public class FavoriteDto
    {
        public int Id { get; set; }

        public string UserId { get; set; } = null!;
        public string Type { get; set; } = null!;
        public ProductResponseDto? Product { get; set; }
    }
}
