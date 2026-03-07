using AutoMapper;
using HandiCraft.Application.DTOs.Orders;
using HandiCraft.Application.Interfaces;
using HandiCraft.Domain.Orders;
using HandiCraft.Presistance.context;
using Org.BouncyCastle.Bcpg;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HandiCraft.Infrastructure.Services.Order
{
    public class CartServices : ICartServices
    {
        private readonly IDatabase _database;
        private readonly HandiCraftDbContext _dbContext;
        private readonly IMapper _mapper;
        public CartServices(IConnectionMultiplexer redis, HandiCraftDbContext dbContext, IMapper mapper)
        {
            _database = redis.GetDatabase();
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<Cart?> GetCartAsync(string userId)
        {
            var data = await _database.StringGetAsync(userId);
            if (data.IsNullOrEmpty) return new Cart(userId);
            return JsonSerializer.Deserialize<Cart>(data!) ?? new Cart(userId);
        }

        public async Task<CartDto?> UpdateCartAsync(string userId, CartDto? updatedCartDto)
        {
            if (updatedCartDto == null)
                throw new ArgumentNullException(nameof(updatedCartDto));

            var existingCart = await GetCartAsync(userId);

            if (existingCart == null)
            {
                existingCart = new Cart(userId)
                {
                    Items = new List<CartItem>()
                };
            }

            if (updatedCartDto.Items != null && updatedCartDto.Items.Any())
            {
                foreach (var updatedItemDto in updatedCartDto.Items)
                {
                    var existingItem = existingCart.Items.FirstOrDefault(i => i.Id == updatedItemDto.Id);

                    if (existingItem != null)
                    {
                        existingItem.Quantity = updatedItemDto.Quantity;
                    }
                    else
                    {
                        var newItem = _mapper.Map<CartItem>(updatedItemDto);
                        existingCart.Items.Add(newItem);
                    }
                }
            }

            var data = JsonSerializer.Serialize(existingCart);
            var success = await _database.StringSetAsync(userId, data, TimeSpan.FromDays(30));

            if (!success) return null;

            var savedCart = await GetCartAsync(userId);

            return _mapper.Map<CartDto>(savedCart);
        }
        public async Task AddToCartAsync(string userId, Guid productId, int quantity)
        {
            var product = await _dbContext.Products.FindAsync(productId);
            if (product == null) throw new Exception("Product not found.");

            var cart = await GetCartAsync(userId);

            var existingItem = cart.Items.FirstOrDefault(i => i.Id == productId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    Id = product.Id,
                    Name = product.Title,
                    PictureUrl = product.ProductImageUrl,
                    price = product.Price,
                    Quantity = quantity
                });
            }
            var cartDto = _mapper.Map<CartDto>(cart);

            await UpdateCartAsync(userId, cartDto);
        }
        public async Task RemoveFromCartAsync(string userId, Guid productId)
        {
            var cart = await GetCartAsync(userId);
            cart.Items = cart.Items.Where(i => i.Id != productId).ToList();
            var cartDto = _mapper.Map<CartDto>(cart);
            await UpdateCartAsync(userId, cartDto);
        }
        
        public async Task<decimal> GetCartTotalAsync(string userId)
        {
            var cart = await GetCartAsync(userId);
            return cart.Items.Sum(i => i.price * i.Quantity);
        }
        public async Task<bool> DeleteCartAsync(string userId)
        {
            return await _database.KeyDeleteAsync(userId);
        }

      
    }
}
