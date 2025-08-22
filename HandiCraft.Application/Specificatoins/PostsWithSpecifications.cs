using HandiCraft.Domain.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Application.Specificatoins
{
    public class PostsWithSpecifications: BaseSpecification<Post>
    {
        public PostsWithSpecifications(string search, int pageIndex, int pageSize)
          : base(p => !string.IsNullOrEmpty(search) &&
              (p.TextContent.ToLower().Contains(search.ToLower())))
               
        {
            if (pageSize <= 0) throw new ArgumentException("PageSize must be greater than zero.");
            if (pageIndex <= 0) throw new ArgumentException("PageIndex must be greater than zero.");

            ApplyPagination(pageSize * (pageIndex - 1), pageSize);

            AddOrderByDesc(p => p.CreatedAt); // Default sort by newest
        }
    }
}
