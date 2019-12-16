using M3gur0.Library.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace M3gur0.Library.Infrastructure.Data.EF
{
    public class GenericRepository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
    {
        private readonly DbContext context;
        private readonly DbSet<TEntity> set;

        public GenericRepository(DbContext dbContext)
        {
            context = dbContext;
            set = context.Set<TEntity>();
        }

        public async Task<IEnumerable<TEntity>> GetByFilter(Expression<Func<TEntity, bool>> predicate) => await set.Where(predicate).ToListAsync();

        public async Task<TEntity> GetSingleById(params object[] keys) => await set.FindAsync(keys);

        public async Task Add(TEntity entity) => await set.AddAsync(entity);

        public void Update(TEntity entity) => context.Entry(entity).State = EntityState.Modified;

        public async Task Delete(params object[] keys) => set.Remove(await set.FindAsync(keys));

        public async Task Save() => await context.SaveChangesAsync();
    }
}
