using HandiCraft.Application.DTOs.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Application.Interfaces
{
    public interface IOrderServices
    {
        Task<OrderDto?> CreateOrderAsync(string userId, CreateOrderDto dto);
        Task<List<OrderDto>> GetUserOrdersAsync(string userId);
        Task<OrderDto?> GetOrderByIdAsync(int id, string userId);
        Task<bool> CancelOrderAsync(int orderId, string userId);
        Task<List<DeliveryMethodDto>> GetAllDeliveryMethodsAsync();
    }
}
