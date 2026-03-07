using HandiCraft.Application.DTOs.Orders;
using HandiCraft.Application.Interfaces;
using HandiCraft.Domain.Orders;
using HandiCraft.Infrastructure.Services.Order;
using HandiCraft.Presentation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HandiCraft.API.Controllers
{
    [Authorize]
    public class CartController : APIControllerBase
    {
        private readonly ICartServices _cartServices;

        public CartController(ICartServices cart)
        {
            _cartServices =cart;
        }
        private string? GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
        [HttpGet]
        public async Task<ActionResult<Cart>> GetCart()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authorized");

            var cart = await _cartServices.GetCartAsync(userId);
            return cart == null ? NotFound("Cart not found") : Ok(cart);

        }
        [HttpPost("update")]
        public async Task<ActionResult<Cart>> UpdateCart([FromBody]CartDto cart)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authorized");

            var updatedCart = await _cartServices.UpdateCartAsync(userId, cart);
            return updatedCart == null ? BadRequest("Failed to update cart") : Ok(updatedCart);
        }
        [HttpPost("add/{productId}/{quantity}")]
        public async Task<ActionResult> AddToCart(Guid productId, int quantity = 1)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authorized");

            await _cartServices.AddToCartAsync(userId, productId, quantity);
            return Ok("Item added to cart");
        }
        [HttpDelete("remove/{productId}")]
        public async Task<ActionResult> RemoveFromCart(Guid productId)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authorized");

            await _cartServices.RemoveFromCartAsync(userId, productId);
            return Ok("Item removed from cart");
        }

       
      
        [HttpGet("total")]
        public async Task<ActionResult<decimal>> GetCartTotal()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authorized");

            var total = await _cartServices.GetCartTotalAsync(userId);
            return Ok(total);
        }
        [HttpDelete]
        public async Task<ActionResult<bool>> DeleteCart()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authorized");

            var deleted = await _cartServices.DeleteCartAsync(userId);
            return deleted ? Ok("Cart cleared") : NotFound("Cart not found");
        }
    }
}
