using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Application.DTOs.Orders
{
    public class PaymentStatusResponseDto
    {
        public int PaymentId { get; set; }
        public string Status { get; set; }
        public bool IsPaid { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
    }
}
