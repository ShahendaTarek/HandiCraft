using HandiCraft.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HandiCraft.Application.DTOs.Orders
{
    public class PaymentResponseDto
    {
        public string PaymobOrderId { get; set; }
        public string PaymentUrl { get; set; }   
        public string PaymentKey {  get; set; }
        public string Status { get; set; }
    }
}
