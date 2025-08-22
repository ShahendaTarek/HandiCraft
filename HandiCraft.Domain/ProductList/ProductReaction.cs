using HandiCraft.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Domain.ProductList
{
    public class ProductReaction
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public Guid ProductId { get; set; }
        public Product Product { get; set; }

        public DateTime CreatedAT { get; set; } = DateTime.UtcNow;

    }
}
