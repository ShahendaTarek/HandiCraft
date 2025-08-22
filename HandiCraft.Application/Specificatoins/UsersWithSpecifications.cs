using HandiCraft.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Application.Specificatoins
{
    public class UsersWithSpecifications : BaseSpecification<ApplicationUser>
    {
        public UsersWithSpecifications(string search, int pageIndex, int pageSize)
           : base(u => !string.IsNullOrEmpty(search) &&
               (u.DisplayName.ToLower().Contains(search.ToLower()) ||
                u.UserName.ToLower().Contains(search.ToLower())))
        {
            if (pageSize <= 0) throw new ArgumentException("PageSize must be greater than zero.");
            if (pageIndex <= 0) throw new ArgumentException("PageIndex must be greater than zero.");

            ApplyPagination(pageSize * (pageIndex - 1), pageSize);

            AddOrderBy(u => u.DisplayName);
        }
    }
}
