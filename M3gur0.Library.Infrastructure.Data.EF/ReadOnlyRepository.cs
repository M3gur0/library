using M3gur0.Library.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace M3gur0.Library.Infrastructure.Data.EF
{
    public class ReadOnlyRepository<TEntity> : IReadOnlyRepository<TEntity> where TEntity : class, IEntity
    {
        protected readonly DbContext context;
        protected readonly DbSet<TEntity> set;

        public ReadOnlyRepository(DbContext dbContext)
        {
            context = dbContext;
            set = context.Set<TEntity>();
        }

        public async Task<IEnumerable<TEntity>> GetByFilter(Expression<Func<TEntity, bool>> predicate) => await set.Where(predicate).ToListAsync();

        public async Task<TEntity> GetSingleByFilter(Expression<Func<TEntity, bool>> predicate) => await set.SingleOrDefaultAsync(predicate);

        public async Task<TEntity> GetSingleById(params object[] keys) => await set.FindAsync(keys);
    }
}
