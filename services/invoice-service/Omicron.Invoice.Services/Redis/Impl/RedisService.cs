// <summary>
// <copyright file="RedisService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Services.Redis.Impl
{
    /// <summary>
    /// Class RedisService.
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
        public async Task<bool> DeleteKey(string key)
        {
            await this.database.KeyDeleteAsync(key);
            return true;
        }

        /// <inheritdoc/>
        public async Task<List<T>> ReadListAsync<T>(string redisKey, int skip, int take)
        {
            RedisValue[] redisValues = await this.database.ListRangeAsync(redisKey, skip, skip + take - 1);

            return redisValues
                .Where(value => !value.IsNullOrEmpty)
                .Select(value => JsonConvert.DeserializeObject<T>(value.ToString()))
                .ToList();
        }

        /// <inheritdoc/>
        public async Task<bool> StoreListAsync<T>(string key, IEnumerable<T> items, TimeSpan timeToLive)
        {
            await this.database.KeyDeleteAsync(key);

            RedisValue[] redisValues = items
                .Select(item => (RedisValue)JsonConvert.SerializeObject(item))
                .ToArray();

            await this.database.ListRightPushAsync(key, redisValues);
            await this.database.KeyExpireAsync(key, timeToLive);
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
        public async Task<bool> SetKeyIfNotExists(string key, string value, TimeSpan timeToLive)
        {
            return await this.database.StringSetAsync(key, value, timeToLive, When.NotExists);
        }

        /// <inheritdoc/>
        public async Task<string> GetRedisKey(string key)
        {
            var result = await this.database.StringGetAsync(key);
            return result.HasValue ? result.ToString() : string.Empty;
        }
    }
}
