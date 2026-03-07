using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Application.DTOs.Orders
{
    public class CreateOrderDto
    {
        public int DeliveryMethodId { get; set; }
        public AddressDto ShippingAddress { get; set; }
        public string BuyerEmail {  get; set; }
    }
}
