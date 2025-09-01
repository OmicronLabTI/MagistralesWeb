// <summary>
// <copyright file="IRedisService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.Redis
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Class for redis.
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
        /// Writes to Reddis.
        /// </summary>
        /// <param name="key">the key.</param>
        /// <param name="value">the value.</param>
        /// <returns>the data.</returns>
        Task<bool> WriteToRedis(string key, string value);

        /// <summary>
        /// Writes to Reddis.
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
        /// Store List Async.
        /// </summary>
        /// <typeparam name="T">Type of list.</typeparam>
        /// <param name="key">Redis Key.</param>
        /// <param name="items">Items to save.</param>
        /// <param name="timeToLive">time to live.</param>
        /// <returns>true if save ok, otherwise false.</returns>
        Task<bool> StoreListAsync<T>(string key, IEnumerable<T> items, TimeSpan timeToLive);

        /// <summary>
        /// Read Redis List by skip and take.
        /// </summary>
        /// <typeparam name="T">Type of list.</typeparam>
        /// <param name="redisKey">Redis key.</param>
        /// <param name="skip">Skip tolist.</param>
        /// <param name="take">Take list.</param>
        /// <returns>List.</returns>
        Task<List<T>> ReadListAsync<T>(string redisKey, int skip, int take);

        /// <summary>
        /// Gets the redis keys.
        /// </summary>
        /// <param name="keys">the keys.</param>
        /// <returns>the data.</returns>
        Task<List<string>> GetRedisKeys(List<string> keys);
    }
}
