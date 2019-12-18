using M3gur0.Library.Domain;
using M3gur0.Library.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using ServiceStack.Redis;
using ServiceStack.Redis.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace M3gur0.Library.Infrastructure.Data.Redis
{
    public class RedisRepository<TEntity> : IReadWriteRepository<TEntity> where TEntity : class, IEntity
    {
        private readonly IRedisTypedClient<TEntity> redis;
        private readonly ILogger<RedisRepository<TEntity>> logger;

        public RedisRepository(IRedisClient redisClient, ILogger<RedisRepository<TEntity>> appLogger)
        {
            redis = redisClient.As<TEntity>();
            logger = appLogger;
        }

        public Task Add(TEntity entity)
        {
            var result = redis.Store(entity);
            return Task.FromResult(result);
        }

        public Task Delete(params object[] keys)
        {
            if (keys.Count() > 1) throw new CompositeKeyinCacheStoreException();

            redis.ExpireIn(keys.Single(), new TimeSpan(0));
            return Task.FromResult(true);
        }

        public Task<IEnumerable<TEntity>> GetByFilter(Expression<Func<TEntity, bool>> predicate)
        {
            var fn = predicate.Compile(); 
            return Task.FromResult(redis.GetAll().Where(fn));
        }

        public async Task<TEntity> GetSingleByFilter(Expression<Func<TEntity, bool>> predicate)
        {
            var fn = predicate.Compile();
            return await Task.FromResult(redis.GetAll().SingleOrDefault(fn));
        }

        public Task<TEntity> GetSingleById(params object[] keys)
        {
            if (keys.Count() > 1) throw new CompositeKeyinCacheStoreException();

            return Task.FromResult(redis.GetById(keys.Single()));
        }

        public Task Save()
        {
            throw new NotImplementedException();
        }

        public void Update(TEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
