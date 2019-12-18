using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace M3gur0.Library.Domain
{
    public interface IReadOnlyRepository<TEntity> where TEntity : IEntity
    {
        Task<IEnumerable<TEntity>> GetByFilter(Expression<Func<TEntity, bool>> predicate);

        Task<TEntity> GetSingleByFilter(Expression<Func<TEntity, bool>> predicate);

        Task<TEntity> GetSingleById(params object[] keys);
    }
}