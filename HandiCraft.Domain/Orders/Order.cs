using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Domain.Orders
{
    public class Order
    {
        public Order() { }
        public Order(string userId, ShippingAddress address, DeliveryMethod deliveryMethod, ICollection<OrderItem> items)
        {
            UserId = userId;
            Address = address;
            DeliveryMethod = deliveryMethod;
            Items = items;
        }

        public int Id { get; set; }
        public string UserId { get; set; }
        public string BuyerEmail {  get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTime.UtcNow;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public ShippingAddress Address { get; set; }
        public int DeliveryMethodId { get; set; }
        public DeliveryMethod DeliveryMethod { get; set; }
        public bool IsPaid { get; set; }
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
        public decimal SubTotal { get; set; }
        public decimal Total() => SubTotal + DeliveryMethod.Price;
        public int? PaymentId { get; set; }
        public Payment Payment { get; set; }
    }
}
