using LinkDev.Talabat.Core.Specifications;
using Microsoft.EntityFrameworkCore;

namespace LinkDev.Talabat.Infrastructure
{
    internal class SpecificationEvaluator<TEntity> where TEntity : class
    {
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery, ISpecifications<TEntity> specifications)
        {
            var query = inputQuery;
            if(specifications.Criteria is not null)
            {
                query = query.Where(specifications.Criteria);
            }

            if(specifications.OrderBy is not null)
            {
                query = query.OrderBy(specifications.OrderBy);
            }
            else if(specifications.OrderByDescending is not null)
            {
                query = query.OrderByDescending(specifications.OrderByDescending);
            }

            if(specifications.IsPagingEnabled)
            {
                query = query.Skip(specifications.Skip).Take(specifications.Take);
            }

            query = specifications.Includes.Aggregate(query, (current, include) => current.Include(include));
            return query;
        }
    }
}
