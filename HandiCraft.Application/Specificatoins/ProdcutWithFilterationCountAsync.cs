using HandiCraft.Domain.ProductList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Application.Specificatoins
{
    public class ProdcutWithFilterationCountAsync: BaseSpecification<Product>
    {
        public ProdcutWithFilterationCountAsync(ProductSpecParams productParams)
       : base(p =>
            (string.IsNullOrEmpty(productParams.Search) ||
            p.Title.ToLower().Contains(productParams.Search.ToLower()) ||
            p.Type.ToLower().Contains(productParams.Search.ToLower()) ||
            p.Material.ToLower().Contains(productParams.Search.ToLower())||
            p.Category.Name.ToLower().Contains(productParams.Search.ToLower())) &&
           (string.IsNullOrEmpty(productParams.CategoryName) || p.Category.Name == productParams.CategoryName) &&
           (string.IsNullOrEmpty(productParams.Material) || p.Material == productParams.Material) &&
           (string.IsNullOrEmpty(productParams.Type) || p.Type == productParams.Type)
       )
        {
        }
    }
}
