// <summary>
// <copyright file="IRedisService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.OrderDivisionProcess.Batch.Services.Redis
{
    /// <summary>
    /// IRedisService.
    /// </summary>
    public interface IRedisService
    {
        /// <summary>
        /// Gets the redis key.
        /// </summary>
        /// <param name="key">the key.</param>
        /// <returns>the data.</returns>
        Task<string> GetRedisKey(string key);

        /// <summary>
        /// Writes to Redis.
        /// </summary>
        /// <param name="key">the key.</param>
        /// <param name="value">the value.</param>
        /// <param name="timeToLive">Time to live.</param>
        /// <returns>the data.</returns>
        Task<bool> WriteToRedis(string key, string value, TimeSpan timeToLive);

        /// <summary>
        /// Deletes a key from redis.
        /// </summary>
        /// <param name="key">the key.</param>
        /// <returns>the data.</returns>
        Task<bool> DeleteKey(string key);

        /// <summary>
        /// Read Redis List by skip and take.
        /// </summary>
        /// <typeparam name="T">Type of list.</typeparam>
        /// <param name="redisKey">Redis key.</param>
        /// <param name="skip">Skip tolist.</param>
        /// <param name="take">Take list.</param>
        /// <returns>List.</returns>
        Task<List<T>> ReadListAsync<T>(string redisKey, int skip, int take);
    }
}
