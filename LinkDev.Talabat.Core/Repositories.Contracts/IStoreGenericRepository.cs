using LinkDev.Talabat.Core.Entities.Common;
using LinkDev.Talabat.Core.Specifications;

namespace LinkDev.Talabat.Core.Repositories.Contracts
{
    public interface IStoreGenericRepository <T> where T : BaseEntity
    {
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<T?> GetAsync(int id);
        Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecifications<T> specifications, bool withTracking = false);
        Task<T?> GetWithSpecAsync(ISpecifications<T> specifications, bool withTracking = false);
        Task<int> GetCountAsync(ISpecifications<T> specifications);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
