using Microsoft.EntityFrameworkCore.Storage;
using NaijaPut.Core.Context;
using NaijaPut.Core.Repository.Interface;

namespace Inventrify.Core.Repository.Implementation
{
    public class NaijaPutRepository<TEntity> : INaijaPutRepository<TEntity> where TEntity : class
    {
        private readonly NaijaPutContext _context;

        public NaijaPutRepository(NaijaPutContext context)
        {
            _context = context;
        }
        public async Task<TEntity> Add(TEntity entity)
        {
            var result = await _context.Set<TEntity>().AddAsync(entity);
            return result.Entity;
        }

        public void Delete(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
        }

        public async Task<TEntity?> GetByIdAsync(string id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }


        public IQueryable<TEntity> GetQueryable()
        {
            return _context.Set<TEntity>();
        }

        public async Task<int> SaveChanges()
        {
            return await _context.SaveChangesAsync();
        }

        public void Update(TEntity entity)
        {
            _context.Set<TEntity>().Update(entity);
        }
        public async Task<IDbContextTransaction> BeginTransaction()
        {
           return await _context.Database.BeginTransactionAsync();
        }
    }
}
