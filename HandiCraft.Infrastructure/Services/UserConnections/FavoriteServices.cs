using AutoMapper;
using HandiCraft.Application.DTOs.Spec;
using HandiCraft.Application.DTOs.UserConnection;
using HandiCraft.Application.Interfaces;
using HandiCraft.Domain.UserConnections;
using HandiCraft.Presistance.context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Infrastructure.Services.UserConnections
{
    public class FavoriteServices:IFavoriteServices
    {
        private readonly HandiCraftDbContext _context;
        private readonly IMapper _mapper;

        public FavoriteServices(HandiCraftDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        
        public async Task AddProductToFavoriteAsync(string userId, Guid productId)
        {
            var exists = await _context.FavoriteLists
                 .AnyAsync(f => f.UserId == userId && f.ProductId == productId);

            if (exists)
                throw new InvalidOperationException("This product is already in favorites.");

            var favorite = new Favorite
            {
                UserId = userId,
                ProductId = productId
            };

            _context.FavoriteLists.Add(favorite);
            await _context.SaveChangesAsync();
        }
        public async Task RemoveProductFromFavoritesAsync(string userId, Guid productId)
        {
            var favorite = await _context.FavoriteLists
                .FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == productId);

            if (favorite == null)
                throw new InvalidOperationException("Favorite not found for this product.");

            _context.FavoriteLists.Remove(favorite);
            await _context.SaveChangesAsync();
            
        }
       
        public async Task<bool> RemoveFromFavoritesAsync(int favoriteId, string userId)
        {
            var favorite = await _context.FavoriteLists
                .FirstOrDefaultAsync(f => f.Id == favoriteId && f.UserId == userId);

            if (favorite == null)
                throw new InvalidOperationException("The favorite list is empty");

            _context.FavoriteLists.Remove(favorite);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<FavoriteDto>>GetUserFavoritesAsync(string userId)
        {
            var favorites = await _context.FavoriteLists
              .Where(f => f.UserId == userId)
              .Include(f => f.Product)
              .ToListAsync();

            favorites = favorites
                .Where(f =>  f.Product != null)
                .ToList();

            var favoriteDtos = _mapper.Map<List<FavoriteDto>>(favorites);

            return favoriteDtos;
        }

        

      
    }
}
