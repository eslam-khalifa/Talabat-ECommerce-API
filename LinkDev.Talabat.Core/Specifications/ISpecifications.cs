using System.Linq.Expressions;

namespace LinkDev.Talabat.Core.Specifications
{
    public interface ISpecifications<T> where T : class
    {
        Expression<Func<T, bool>>? Criteria { get; set; }
        List<Expression<Func<T, object>>> Includes { get; set; }
        Expression<Func<T, object>>? OrderBy { get; set; }
        Expression<Func<T, object>>? OrderByDescending { get; set; }
        int Take { get; set; }
        int Skip { get; set; }
        bool IsPagingEnabled { get; set; }
    }
}
