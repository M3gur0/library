using M3gur0.Library.Domain;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace M3gur0.Library.Infrastructure.Data.EF
{
    public class ReadWriteRepository<TEntity> : ReadOnlyRepository<TEntity>, IReadWriteRepository<TEntity> where TEntity : class, IEntity
    {
        public ReadWriteRepository(DbContext dbContext) : base(dbContext) { }

        public async Task Add(TEntity entity) => await set.AddAsync(entity);

        public void Update(TEntity entity) => context.Entry(entity).State = EntityState.Modified;

        public async Task Delete(params object[] keys) => set.Remove(await set.FindAsync(keys));

        public async Task Save() => await context.SaveChangesAsync();
    }
}
