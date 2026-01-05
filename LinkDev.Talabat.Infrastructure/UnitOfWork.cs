using LinkDev.Talabat.Core;
using LinkDev.Talabat.Core.Entities.Common;
using LinkDev.Talabat.Core.Repositories.Contracts;
using LinkDev.Talabat.Infrastructure.Data;
using LinkDev.Talabat.Infrastructure.Data.GenericRepository;
using System.Collections;

namespace LinkDev.Talabat.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private StoreDbContext _storeDbContext;
        private Hashtable _repositories;

        public UnitOfWork(StoreDbContext storeDbContext)
        {
            this._storeDbContext = storeDbContext;
            this._repositories = new Hashtable();
        }

        public async Task<int> CompleteAsync()
        {
            return await _storeDbContext.SaveChangesAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await _storeDbContext.DisposeAsync();
        }

        // it is like the scooped lifetime in DI container
        public IStoreGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            var key = typeof(TEntity).Name;
            if (!_repositories.ContainsKey(key))
            {
                var repository = new StoreGenericRepository<TEntity>(_storeDbContext);
                _repositories.Add(key, repository);
            }
            return _repositories[key] as IStoreGenericRepository<TEntity>;
        }
    }
}
