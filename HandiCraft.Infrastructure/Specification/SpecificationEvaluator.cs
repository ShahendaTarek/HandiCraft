using HandiCraft.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Infrastructure.Specification
{
    public class SpecificationEvaluator<T> where T :class
    {
        public static IQueryable<T> GetQuery(IQueryable<T> InputQuery, ISpecification<T> Spec)
        {
            var Querey = InputQuery;
            if (Spec.Criteria != null)
            {
                Querey = Querey.Where(Spec.Criteria);
            }
            if (Spec.OrderBy != null)
            {
                Querey = Querey.OrderBy(Spec.OrderBy);
            }
            if (Spec.OrderByDescending != null)
            {
                Querey = Querey.OrderByDescending(Spec.OrderByDescending);
            }
            if (Spec.IsPaginationEnable)
            {
                Querey = Querey.Skip(Spec.Skip).Take(Spec.Take);
            }
            Querey = Spec.Include.Aggregate(Querey, (CurrentQuerey, IncludeExpression) => CurrentQuerey.Include(IncludeExpression));

            return Querey;
        }
    }
}
