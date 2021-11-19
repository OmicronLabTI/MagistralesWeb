// <summary>
// <copyright file="RedisService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Services.Redis
{
    using System;
    using System.Threading.Tasks;
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
        /// <param name="logger">Catalogos Facade.</param>
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
        public async Task<bool> WriteToRedis(string key, string value, TimeSpan timeToLive)
        {
            await this.database.KeyDeleteAsync(key);
            await this.database.StringSetAsync(key, value, timeToLive);
            return true;
        }

        /// <inheritdoc/>
        public bool IsConnectedRedis()
        {
            return this.redis.IsConnected;
        }
    }
}
