using LinkDev.Talabat.Core.Repositories.Contracts;
using LinkDev.Talabat.Core.Specifications;
using Microsoft.EntityFrameworkCore;

namespace LinkDev.Talabat.Infrastructure.Identity.IdentityGenericRepository
{
    public class IdentityGenericRepository<T>(ApplicationIdentityDbContext applicationIdentityDbContext) : IIdentityGenericRepository<T> where T : class
    {
        public void Add(T entity)
        {
            applicationIdentityDbContext.Set<T>().Add(entity);
        }

        private IQueryable<T> ApplySpecifications(ISpecifications<T> specifications)
        {
            return SpecificationEvaluator<T>.GetQuery(applicationIdentityDbContext.Set<T>(), specifications);
        }

        public void Delete(T entity)
        {
            applicationIdentityDbContext.Set<T>().Remove(entity);
        }
        
        public async Task<int> DeleteRangeAsync(ISpecifications<T> spec, CancellationToken cancellationToken = default)
        {
            // Use ExecuteDeleteAsync for efficient bulk delete (EF Core 7+)
            // This executes a single DELETE SQL statement without loading entities into memory
            return await ApplySpecifications(spec).ExecuteDeleteAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecifications<T> specifications, bool withTracking = false, CancellationToken cancellationToken = default)
        {
            if (!withTracking) return await ApplySpecifications(specifications).AsNoTracking().ToListAsync(cancellationToken);
            else return await ApplySpecifications(specifications).ToListAsync(cancellationToken);
        }

        public async Task<T?> GetWithSpecAsync(ISpecifications<T> specifications, bool withTracking = false, CancellationToken cancellationToken = default)
        {
            if (!withTracking) return await ApplySpecifications(specifications).AsNoTracking().FirstOrDefaultAsync();
            else return await ApplySpecifications(specifications).FirstOrDefaultAsync();
        }

        public async Task<int> GetCountAsync(ISpecifications<T> specifications, CancellationToken cancellationToken = default)
        {
            return await ApplySpecifications(specifications).CountAsync(cancellationToken);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await applicationIdentityDbContext.SaveChangesAsync(cancellationToken);
        }

        public void Update(T entity)
        {
            applicationIdentityDbContext.Set<T>().Update(entity);
        }
    }
}
