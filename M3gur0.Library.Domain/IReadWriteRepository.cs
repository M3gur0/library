using System.Threading.Tasks;

namespace M3gur0.Library.Domain
{
    public interface IReadWriteRepository<TEntity> : IReadOnlyRepository<TEntity> where TEntity : IEntity
    {
        Task Add(TEntity entity);

        void Update(TEntity entity);

        Task Delete(params object[] keys);

        Task Save();
    }
}
