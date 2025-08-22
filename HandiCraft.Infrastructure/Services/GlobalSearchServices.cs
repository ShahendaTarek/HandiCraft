using AutoMapper;
using HandiCraft.Application.DTOs.ProductList;
using HandiCraft.Application.DTOs.Social;
using HandiCraft.Application.DTOs.Spec;
using HandiCraft.Application.DTOs.User;
using HandiCraft.Application.Specificatoins;
using HandiCraft.Domain.Identity;
using HandiCraft.Domain.Posts;
using HandiCraft.Domain.ProductList;
using HandiCraft.Infrastructure.Specification;
using HandiCraft.Presistance.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Infrastructure.Services
{
    public  class GlobalSearchServices
    {
        private readonly HandiCraftDbContext _context;
        private readonly IMapper _mapper;

        public GlobalSearchServices(HandiCraftDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<GlobalSearchResultDto> GlobalSearchAsync(GlobalSearchParams searchParams)
        {
            if (string.IsNullOrEmpty(searchParams.Query))
            {
                throw new ArgumentException("Search query cannot be empty.", nameof(searchParams.Query));
            }

            // Enforce pagination
            searchParams.PageIndex = searchParams.PageIndex <= 0 ? 1 : searchParams.PageIndex;
            searchParams.PageSize = searchParams.PageSize <= 0 ? 10 : Math.Min(searchParams.PageSize, 50);

            try
            {
                // Products
                var productParams = new ProductSpecParams { Search = searchParams.Query, PageIndex = searchParams.PageIndex, PageSize = searchParams.PageSize };
                var productSpec = new ProductsWithSpecifications(productParams);
                var productCount = await _context.Products.Where(productSpec.Criteria).CountAsync();
                var products = await SpecificationEvaluator<Product>.GetQuery(_context.Products.Include(p => p.Category).Include(p => p.User).AsQueryable(), productSpec).ToListAsync();
                var productDtos = _mapper.Map<List<ProductResponseDto>>(products);

                // Posts
                var postSpec = new PostsWithSpecifications(searchParams.Query, searchParams.PageIndex, searchParams.PageSize);
                var postCount = await _context.Posts.Where(postSpec.Criteria).CountAsync();
                var posts = await SpecificationEvaluator<Post>.GetQuery(_context.Posts.Include(p => p.User).AsQueryable(), postSpec).ToListAsync();
                var postDtos = _mapper.Map<List<PostDto>>(posts);

                // Users
                var userSpec = new UsersWithSpecifications(searchParams.Query, searchParams.PageIndex, searchParams.PageSize);
                var userCount = await _context.Users.Where(userSpec.Criteria).CountAsync();
                var users = await SpecificationEvaluator<ApplicationUser>.GetQuery(_context.Users.AsQueryable(), userSpec).ToListAsync();
                var userDtos = _mapper.Map<List<UserDto>>(users);

                return new GlobalSearchResultDto
                {
                    Products = new Pagination<ProductResponseDto> { Data = productDtos, Count = productCount, PageIndex = searchParams.PageIndex, PageSize = searchParams.PageSize },
                    Posts = new Pagination<PostDto> { Data = postDtos, Count = postCount, PageIndex = searchParams.PageIndex, PageSize = searchParams.PageSize },
                    Users = new Pagination<UserDto> { Data = userDtos, Count = userCount, PageIndex = searchParams.PageIndex, PageSize = searchParams.PageSize }
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Global search failed: {ex.Message}", ex);
            }
        }
    }
}
