using AutoMapper;
using AutoMapper.QueryableExtensions;
using HandiCraft.Application.DTOs.Orders;
using HandiCraft.Application.Interfaces;
using HandiCraft.Domain.Orders;
using HandiCraft.Presistance.context;
using Microsoft.EntityFrameworkCore;

namespace HandiCraft.Infrastructure.Services.Order
{
    public class OrderServices : IOrderServices
    {
        private readonly HandiCraftDbContext _context;
        private readonly ICartServices _cartService;
        private readonly IMapper _mapper;

        public OrderServices(HandiCraftDbContext context, ICartServices cartService, IMapper mapper)
        {
            _context = context;
            _cartService = cartService;
            _mapper = mapper;
        }
        public async Task<OrderDto> CreateOrderAsync(string userId,  CreateOrderDto dto)
        {
            var cart = await _cartService.GetCartAsync(userId);
            if (cart == null || !cart.Items.Any())
                throw new Exception("Your cart is empty.");

            var deliveryMethod = await _context.DeliveryMethods
                .FirstOrDefaultAsync(dm => dm.Id == dto.DeliveryMethodId);
            if (deliveryMethod == null)
                throw new Exception("Invalid delivery method.");

            var address = _mapper.Map<ShippingAddress>(dto.ShippingAddress);

            var orderItems = new List<OrderItem>();
            foreach (var item in cart.Items)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == item.Id);
                if (product == null) continue;

                var orderItem = new OrderItem(product.Id, product.Title, product.ProductImageUrl, product.Price, item.Quantity);
                orderItems.Add(orderItem);
            }

            if (!orderItems.Any())
                throw new Exception("No valid products found in the cart.");

            var subtotal = orderItems.Sum(i => i.Price * i.Quantity);

            var order = new Domain.Orders.Order
            {
                UserId = userId,
                BuyerEmail = dto.BuyerEmail,
                Status = OrderStatus.Pending,
                Address = address,
                DeliveryMethod = deliveryMethod,
                SubTotal = subtotal,
                CreatedAt = DateTime.UtcNow,
                Items = orderItems
            };

            _context.Orders.Add(order);
            var result = await _context.SaveChangesAsync();

            if (result <= 0)
                throw new Exception("Error creating the order.");

            await _cartService.DeleteCartAsync(userId);

            return _mapper.Map<OrderDto>(order);
        }
        public async Task<List<OrderDto>> GetUserOrdersAsync(string userId)
        {
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.Items)
                .Include(o => o.DeliveryMethod)
                .ProjectTo<OrderDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return orders;
        }
        public async Task<OrderDto?> GetOrderByIdAsync(int id, string userId)
        {
            var order = await _context.Orders
                .Where(o => o.Id == id && o.UserId == userId)
                .Include(o => o.Items)
                .Include(o => o.DeliveryMethod)
                .ProjectTo<OrderDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            return order;
        }
        public async Task<bool> CancelOrderAsync(int orderId, string userId)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

            if (order == null)
                throw new Exception("Order not found.");

            if (order.Status != OrderStatus.Pending)
                throw new Exception("Only pending orders can be cancelled.");

            order.Status = OrderStatus.Cancelled;
            _context.Orders.Update(order);

            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
        public async Task<List<DeliveryMethodDto>> GetAllDeliveryMethodsAsync()
        {
            var deliveryMethods = await _context.DeliveryMethods.ToListAsync();
            return _mapper.Map<List<DeliveryMethodDto>>(deliveryMethods);
        }
    }
}

