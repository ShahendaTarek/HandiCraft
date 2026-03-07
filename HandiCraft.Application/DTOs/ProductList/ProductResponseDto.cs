using HandiCraft.Application.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Application.DTOs.ProductList
{
    public class ProductResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Material { get; set; }
        public string Color { get; set; }
        public int ShippingTimeInDays { get; set; }
        public string ProductImageUrl { get; set; }
        public decimal Price {  get; set; }
        public DateTime CreatedAt {  get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        public UserProfileDto User { get; set; }
    }
}
