// <summary>
// <copyright file="IRedisService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Services.Redis
{
    /// <summary>
    /// Class for IRedisService.
    /// </summary>
    public interface IRedisService
    {
        /// <summary>
        /// TryAcquireLockAsync.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="ttl">TTL.</param>
        /// <returns>True if exists.</returns>
        Task<bool> TryAcquireLockAsync(string key, TimeSpan ttl);

        /// <summary>
        /// ReleaseLockAsync.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ReleaseLockAsync(string key);
    }
}
