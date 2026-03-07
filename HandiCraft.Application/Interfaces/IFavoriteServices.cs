using HandiCraft.Application.DTOs.Spec;
using HandiCraft.Application.DTOs.UserConnection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Application.Interfaces
{
    public interface IFavoriteServices
    {
        Task AddProductToFavoriteAsync(string userId, Guid productId);
        Task  RemoveProductFromFavoritesAsync(string userId, Guid productId);
        Task <bool>RemoveFromFavoritesAsync(int favoriteId,string userId);
        Task<List<FavoriteDto>> GetUserFavoritesAsync(string userId);
    }
}
