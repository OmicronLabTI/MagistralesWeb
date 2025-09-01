// <summary>
// <copyright file="RedisService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.OrderDivisionProcess.Batch.Services.Redis.Impl
{
    /// <summary>
    /// RedisService.
    /// </summary>
    public class RedisService : IRedisService
    {
        private readonly IDatabase database;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisService"/> class.
        /// </summary>
        /// <param name="redis">Redis Cache.</param>
        public RedisService(IConnectionMultiplexer redis)
        {
            this.database = redis.GetDatabase();
        }

        /// <inheritdoc/>
        public async Task<string> GetRedisKey(string key)
        {
            var result = await this.database.StringGetAsync(key);
            return result.HasValue ? result.ToString() : string.Empty;
        }

        /// <inheritdoc/>
        public async Task<bool> WriteToRedis(string key, string value, TimeSpan timeToLive)
        {
            await this.database.KeyDeleteAsync(key);
            await this.database.StringSetAsync(key, value, timeToLive);
            return true;
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteKey(string key)
        {
            await this.database.KeyDeleteAsync(key);
            return true;
        }

        /// <inheritdoc/>
        public async Task<List<T>> ReadListAsync<T>(string redisKey, int skip, int take)
        {
            // Obtener los elementos de Redis con paginación
            RedisValue[] redisValues = await this.database.ListRangeAsync(redisKey, skip, skip + take - 1);

            // Convertir los elementos de Redis a objetos de tipo T
            return redisValues
                .Where(value => !value.IsNullOrEmpty)
                .Select(value => JsonConvert.DeserializeObject<T>(value.ToString()))
                .ToList();
        }
    }
}
