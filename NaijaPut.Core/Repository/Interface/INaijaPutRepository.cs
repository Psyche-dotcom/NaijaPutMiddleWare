using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaijaPut.Core.Repository.Interface
{
    public interface INaijaPutRepository<TEntity>
    {
        Task<TEntity?> GetByIdAsync(string id);
        IQueryable<TEntity> GetQueryable();
        Task<TEntity> Add(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        Task<int> SaveChanges();
        Task<IDbContextTransaction> BeginTransaction();
    }
}
