using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Domain.Orders
{
    public class ShippingAddress
    {
        public ShippingAddress() { }
        public ShippingAddress(string fullName, string street, string city, string country)
        {
            FullName = fullName;
            Street = street;
            City = city;
            Country = country;
        }

        public string FullName { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        
    }
}
