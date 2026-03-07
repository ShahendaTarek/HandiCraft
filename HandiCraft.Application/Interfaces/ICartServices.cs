using HandiCraft.Application.DTOs.Orders;
using HandiCraft.Domain.Orders;

namespace HandiCraft.Application.Interfaces
{
    public interface ICartServices
    {
        Task<Cart?> GetCartAsync(string CartId);
        Task<CartDto?> UpdateCartAsync(string UserId,CartDto? cart);
        Task<bool> DeleteCartAsync(string CartId);
        Task AddToCartAsync(string userId, Guid productId, int quantity);
        Task RemoveFromCartAsync(string userId,Guid productId);
        Task<decimal> GetCartTotalAsync(string userId);

    }
}
