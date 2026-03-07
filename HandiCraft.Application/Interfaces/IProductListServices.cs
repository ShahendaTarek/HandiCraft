using HandiCraft.Application.DTOs.ProductList;
using HandiCraft.Application.DTOs.Spec;
using HandiCraft.Application.Specificatoins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Application.Interfaces
{
    public  interface IProductListServices
    {
        Task<ProductResponseDto>AddProductAsync(string UserId,AddProductDto product);
        Task<bool> AddProductReactionAsync(string userId, Guid productId);
        Task<bool> RemoveProductReactionAsync(string userId, Guid productId);
        Task<Pagination<ProductResponseDto>> GetAllProductsAsync(ProductSpecParams productParams);
        Task<ProductResponseDto> GetProductByIdAsync(Guid id);
        Task<ProductResponseDto> UpdateProductAsync(string userId,Guid id, UpdateProductDto dto);
        Task<bool> DeleteProductAsync(string userId,Guid id);
    }
}
