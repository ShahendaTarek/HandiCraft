using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Domain.Orders
{
    public class Payment
    {
        public int Id { get; set; }

        public decimal Amount { get; set; }
        public string Currency { get; set; }

        public PaymentStatus Status { get; set; }

        public string? PaymobOrderId { get; set; } 
        public string? TransactionId { get; set; } 
        public string PaymentMethod { get; set; } 
        public string? PaymentKey { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsPaid { get; set; }
        public DateTime? PaidAt { get; set; }
        public int IntegrationId { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
    }
}
