using LinkDev.Talabat.Core.Entities.Common;
using LinkDev.Talabat.Core.Entities.Order_Aggregate;
using LinkDev.Talabat.Core.Entities.Products;
using LinkDev.Talabat.Core.Repositories.Contracts;

namespace LinkDev.Talabat.Core
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IStoreGenericRepository<TEntity> Repository<TEntity>() where TEntity: BaseEntity;
        Task<int> CompleteAsync();
    }
}
