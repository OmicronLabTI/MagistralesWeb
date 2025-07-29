// <summary>
// <copyright file="BackgroundTaskQueue.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.MediatR.Services
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;
    using Omicron.Pedidos.Services.Constants;
    using Omicron.Pedidos.Services.Utils;
    using Serilog;

    /// <summary>
    /// BackgroundTaskQueue.
    /// </summary>
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly ConcurrentQueue<Func<IServiceProvider, CancellationToken, Task>> workItems = new ConcurrentQueue<Func<IServiceProvider, CancellationToken, Task>>();
        private readonly SemaphoreSlim signal = new SemaphoreSlim(0);
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundTaskQueue"/> class.
        /// </summary>
        /// <param name="logger">Logger.</param>
        public BackgroundTaskQueue(ILogger logger)
        {
            this.logger = logger.ThrowIfNull(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<Func<IServiceProvider, CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken)
        {
            await this.signal.WaitAsync(cancellationToken);
            this.workItems.TryDequeue(out var workItem);
            return workItem;
        }

        /// <inheritdoc/>
        public void QueueBackgroundWorkItem(Func<IServiceProvider, CancellationToken, Task> workItem)
        {
            ArgumentNullException.ThrowIfNull(workItem);
            this.workItems.Enqueue(workItem);
            this.signal.Release();
            this.logger.Debug(LogsConstants.UpdateTaskAddedToQueue);
        }
    }
}
