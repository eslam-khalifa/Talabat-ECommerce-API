using LinkDev.Talabat.Core.Specifications;

namespace LinkDev.Talabat.Core.Repositories.Contracts
{
    public interface IIdentityGenericRepository<T> where T : class
    {
        void Add(T entity);
        void Delete(T entity);
        Task<int> DeleteRangeAsync(ISpecifications<T> spec, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecifications<T> specifications, bool withTracking = false, CancellationToken cancellationToken = default);
        Task<T?> GetWithSpecAsync(ISpecifications<T> spec, bool withTracking = false, CancellationToken cancellationToken = default);
        Task<int> GetCountAsync(ISpecifications<T> specifications, CancellationToken cancellationToken = default);
        // any async method that performs I/O operations like database operations or external API calls should allow cancellation token.
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        void Update(T entity);
    }
}
