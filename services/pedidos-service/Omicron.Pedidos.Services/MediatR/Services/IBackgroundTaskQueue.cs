// <summary>
// <copyright file="IBackgroundTaskQueue.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.MediatR.Services
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// IBackgroundTaskQueue.
    /// </summary>
    public interface IBackgroundTaskQueue
    {
        /// <summary>
        /// QueueBackgroundWorkItem.
        /// </summary>
        /// <param name="workItem">workItem.</param>
        void QueueBackgroundWorkItem(Func<IServiceProvider, CancellationToken, Task> workItem);

        /// <summary>
        /// DequeueAsync.
        /// </summary>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns>Process.</returns>
        Task<Func<IServiceProvider, CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
    }
}
