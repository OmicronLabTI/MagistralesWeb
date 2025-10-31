// <summary>
// <copyright file="BackgroundTaskQueue.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Services.BackgroundTaskQueue.Impl
{
    /// <summary>
    /// BackgroundTaskQueue.
    /// </summary>
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly ConcurrentQueue<Func<CancellationToken, Task>> workItems = new ();
        private readonly SemaphoreSlim signal = new (0);

        /// <inheritdoc/>
        public async Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken)
        {
            await this.signal.WaitAsync(cancellationToken);
            this.workItems.TryDequeue(out var workItem);
            return workItem;
        }

        /// <inheritdoc/>
        public void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem)
        {
            ArgumentNullException.ThrowIfNull(workItem);
            this.workItems.Enqueue(workItem);
            this.signal.Release();
        }
    }
}
