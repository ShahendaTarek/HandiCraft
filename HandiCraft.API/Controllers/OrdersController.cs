using HandiCraft.Application.DTOs.Orders;
using HandiCraft.Application.Interfaces;
using HandiCraft.Presentation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HandiCraft.API.Controllers
{

    public class OrdersController : APIControllerBase
    {
        private readonly IOrderServices _orderService;

        public OrdersController(IOrderServices orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authorized");

            try
            {
                var order = await _orderService.CreateOrderAsync(userId, dto);
                return Ok(order);
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                return BadRequest(new Response(400,message));
            }
        }
        [Authorize]
        [HttpGet("my-orders")]
        public async Task<IActionResult> GetUserOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authorized");

            var orders = await _orderService.GetUserOrdersAsync(userId);

            if (orders == null || !orders.Any())
                return NotFound(new Response(404,"No orders found for this user."));

            return Ok(orders);
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new Response(401,"User not authorized"));

            var order = await _orderService.GetOrderByIdAsync(id, userId);

            if (order == null)
                return NotFound(new Response(404,$"Order with ID {id} not found."));

            return Ok(order);
        }
        [Authorize]
        [HttpPut("{orderId}/cancel")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new Response(401,"User not authorized"));

            var isCancelled = await _orderService.CancelOrderAsync(orderId, userId);

            if (!isCancelled)
                return BadRequest($"Failed to cancel order with ID {orderId}. It may not exist or cannot be cancelled.");

            return Ok(new { message = $"Order {orderId} cancelled successfully." });
        }
        [Authorize]
        [HttpGet("delivery-methods")]
        public async Task<IActionResult> GetAllDeliveryMethods()
        {
            var methods = await _orderService.GetAllDeliveryMethodsAsync();
            return Ok(methods);
        }
    }
}
