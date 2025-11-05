// <summary>
// <copyright file="IBackgroundTaskQueue.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Services.BackgroundTaskQueue
{
    /// <summary>
    /// Class to connect to kafka.
    /// </summary>
    public interface IBackgroundTaskQueue
    {
        /// <summary>
        /// QueueBackgroundWorkItem.
        /// </summary>
        /// <param name="workItem">workItem.</param>
        void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem);

        /// <summary>
        /// DequeueAsync.
        /// </summary>
        /// <param name="cancellationToken">cancellationToken.</param>
        /// <returns>Function.</returns>
        Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
    }
}
