using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Domain.Orders
{
    public class DeliveryMethod
    {
        public DeliveryMethod() { } 
        public DeliveryMethod( string name, string description, decimal price, string estimatedDays)
        {
            Name = name;
            Description = description;
            Price = price;
            EstimatedDays = estimatedDays;
        }

        public int Id { get; set; }
        public string Name { get; set; }  
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string EstimatedDays { get; set; }
    }
}
