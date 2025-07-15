// <summary>
// <copyright file="RedisService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.Redis
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using StackExchange.Redis;

    /// <summary>
    /// Get the redis data.
    /// </summary>
    public class RedisService : IRedisService
    {
        private readonly IConnectionMultiplexer redis;

        private readonly IDatabase database;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisService"/> class.
        /// </summary>
        /// <param name="redis">Redis Cache.</param>
        public RedisService(IConnectionMultiplexer redis)
        {
            this.redis = redis ?? throw new ArgumentNullException(nameof(redis));
            this.database = redis.GetDatabase();
        }

        /// <inheritdoc/>
        public async Task<string> GetRedisKey(string key)
        {
            var result = await this.database.StringGetAsync(key);
            return result.HasValue ? result.ToString() : string.Empty;
        }

        /// <inheritdoc/>
        public async Task<bool> WriteToRedis(string key, string value)
        {
            await this.database.KeyDeleteAsync(key);
            await this.database.StringSetAsync(key, value);
            return true;
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
        public async Task<bool> StoreListAsync<T>(string key, IEnumerable<T> items, TimeSpan timeToLive)
        {
            if (items == null || !items.Any())
            {
                return false;
            }

            // Convertir los objetos a RedisValue[]
            RedisValue[] redisValues = items
                .Select(item => (RedisValue)JsonConvert.SerializeObject(item))
                .ToArray();

            // Insertar todos los elementos en una sola operación
            await this.database.ListRightPushAsync(key, redisValues);

            // Establecer el tiempo de vida de la clave
            await this.database.KeyExpireAsync(key, timeToLive);
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
