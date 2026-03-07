using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Domain.Orders
{
    public class CartItem
    {
        public Guid Id {  get; set; }
        public string Name {  get; set; }
        public string PictureUrl {  get; set; }
        public decimal price { get; set; }
        public int Quantity {  get; set; }
    }
}
