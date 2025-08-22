using HandiCraft.Application.DTOs.ProductList;
using HandiCraft.Application.DTOs.Social;
using HandiCraft.Application.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Application.DTOs.Spec
{
    public class GlobalSearchResultDto
    {
        public Pagination<ProductResponseDto> Products { get; set; } = new Pagination<ProductResponseDto>();
        public Pagination<PostDto> Posts { get; set; } = new Pagination<PostDto>();
        public Pagination<UserDto> Users { get; set; } = new Pagination<UserDto>();
    }
}
