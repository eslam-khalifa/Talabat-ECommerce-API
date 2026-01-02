using LinkDev.Talabat.Core.Entities.Common;
using LinkDev.Talabat.Core.Entities.Products;
using LinkDev.Talabat.Core.Repositories.Contracts;
using LinkDev.Talabat.Core.Specifications;
using Microsoft.EntityFrameworkCore;

namespace LinkDev.Talabat.Infrastructure.Data.GenericRepository
{
    public class StoreGenericRepository<T>(StoreDbContext storeDbContext) : IStoreGenericRepository<T> where T : BaseEntity
    {
        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            if(typeof(T) == typeof(Product))
            {
                return (IReadOnlyList<T>)await storeDbContext.Products.Include(p => p.Category).Include(p => p.Brand).ToListAsync();
            }
            return await storeDbContext.Set<T>().ToListAsync();
        }

        public async Task<T?> GetAsync(int id)
        {
            if(typeof(T) == typeof(Product))
            {
                return await storeDbContext.Products.Include(p => p.Category).Include(p => p.Brand).FirstOrDefaultAsync(p => p.Id == id) as T;
            }
            return await storeDbContext.Set<T>().FindAsync(id);
        }

        public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecifications<T> specifications, bool withTracking = false){
            if (!withTracking) return await ApplySpecifications(specifications).AsNoTracking().ToListAsync();
            else return await ApplySpecifications(specifications).ToListAsync();
        }

        public async Task<T?> GetWithSpecAsync(ISpecifications<T> specifications, bool withTracking = false)
        {
            if (!withTracking) return await ApplySpecifications(specifications).AsNoTracking().FirstOrDefaultAsync();
            else return await ApplySpecifications(specifications).FirstOrDefaultAsync();
        }

        public Task<int> GetCountAsync(ISpecifications<T> specifications)
        {
            return ApplySpecifications(specifications).CountAsync();
        }

        private IQueryable<T> ApplySpecifications(ISpecifications<T> specifications)
        {
            return SpecificationEvaluator<T>.GetQuery(storeDbContext.Set<T>(), specifications);
        }

        // AddAsync is needed when there is a sequence
        public void Add(T entity)
        {
            storeDbContext.Set<T>().Add(entity);
        }

        public void Update(T entity)
        {
            storeDbContext.Set<T>().Update(entity);
        }

        public void Delete(T entity)
        {
            storeDbContext.Set<T>().Remove(entity);
        }
    }
}
