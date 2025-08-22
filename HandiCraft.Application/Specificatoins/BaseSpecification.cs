using HandiCraft.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Application.Specificatoins
{
    public class BaseSpecification <T>: ISpecification<T> where T :class
    {
        public Expression<Func<T, bool>> Criteria { get; set; }
        public List<Expression<Func<T, object>>> Include { get; set; } = new List<Expression<Func<T, object>>>();
        public Expression<Func<T, object>> OrderBy { get; set; }
        public Expression<Func<T, object>> OrderByDescending { get; set; }
        public int Take { get; set; }
        public int Skip { get; set; }
        public bool IsPaginationEnable { get; set; }

        public BaseSpecification() { }
        public BaseSpecification(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }
        protected void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Include.Add(includeExpression);
        }

        public void AddOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
        }
        public void AddOrderByDesc(Expression<Func<T, object>> OrderByDescExpression)
        {
            OrderByDescending = OrderByDescExpression;
        }
        public void ApplyPagination(int skip, int take)
        {
            IsPaginationEnable = true;
            Take = take;
            Skip = skip;
        }

    }
}
