// <summary>
// <copyright file="RedisService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Services.Redis.Impl
{
    using StackExchange.Redis;

    /// <summary>
    /// RedisService.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="RedisService"/> class.
    /// </remarks>
    /// <param name="redis">Redis Cache.</param>
    public class RedisService(IConnectionMultiplexer redis)
        : IRedisService
    {
        private readonly IDatabase database = redis.GetDatabase();

        /// <inheritdoc/>
        public async Task ReleaseLockAsync(string key)
        {
            await this.database.KeyDeleteAsync(key);
        }

        /// <inheritdoc/>
        public async Task<bool> TryAcquireLockAsync(string key, TimeSpan ttl)
        {
            return await this.database.StringSetAsync(key, "locked", ttl, When.NotExists);
        }
    }
}
