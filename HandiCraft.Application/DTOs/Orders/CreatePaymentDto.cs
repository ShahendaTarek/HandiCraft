using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Application.DTOs.Orders
{
    public class CreatePaymentDto
    {
        public int OrderId { get; set; }    
        public string Currency { get; set; } = "EGP";
        public string PaymentMethod { get; set; } 
        public string? PhoneNumber { get; set; }  
    }
}
