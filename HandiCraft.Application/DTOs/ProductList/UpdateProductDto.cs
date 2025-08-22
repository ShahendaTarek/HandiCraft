using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Application.DTOs.ProductList
{
    public class UpdateProductDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Material { get; set; }
        public string Color { get; set; }
        public int ShippingTimeInDays { get; set; }
        public string CategoryName { get; set; }
    }
}
