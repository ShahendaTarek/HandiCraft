using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Domain.Orders
{
    public class Cart
    {
        public string Id { get; set; }
        public ICollection<CartItem>Items { get; set; }=new List<CartItem>();
        public Cart(string id)
        {
            Id = id;
        }
    }
}
