using HandiCraft.Domain.ProductList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Application.Specificatoins
{
    public class ProductsWithSpecifications : BaseSpecification<Product>
    {
        public ProductsWithSpecifications(ProductSpecParams productParams)
        : base(p =>
              (string.IsNullOrEmpty(productParams.Search) ||
            p.Title.ToLower().Contains(productParams.Search.ToLower()) ||
            p.Type.ToLower().Contains(productParams.Search.ToLower()) ||
            p.Material.ToLower().Contains(productParams.Search.ToLower()) ||
            p.Category.Name.ToLower().Contains(productParams.Search.ToLower())) &&
            (string.IsNullOrEmpty(productParams.CategoryName) || p.Category.Name == productParams.CategoryName) &&
            (string.IsNullOrEmpty(productParams.Material) || p.Material == productParams.Material) &&
            (string.IsNullOrEmpty(productParams.Type) || p.Type == productParams.Type)
        )
        {
            AddInclude(p => p.Category);
            AddInclude(p => p.User);
            ApplyPagination(productParams.PageSize * (productParams.PageIndex - 1), productParams.PageSize);

            if (!string.IsNullOrEmpty(productParams.Sort))
            {
                switch (productParams.Sort.ToLower())
                {
                    case "oldest":
                        AddOrderBy(p => p.CreatedAt);
                        break;
                    case "newest":
                        AddOrderByDesc(p => p.CreatedAt);
                        break;
                    
                    default:
                        AddOrderBy(p => p.Title);
                        break;
                }
            }
            else
            {
                AddOrderBy(p => p.Title);
            }

        }
    }
}
