// <summary>
// <copyright file="QueuedHostedService.cs" company="Axity">
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
    using Microsoft.Extensions.Hosting;
    using Omicron.Pedidos.Services.Utils;
    using Serilog;

    /// <summary>
    /// QueuedHostedService.
    /// </summary>
    public class QueuedHostedService : BackgroundService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IBackgroundTaskQueue taskQueue;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueuedHostedService"/> class.
        /// </summary>
        /// <param name="taskQueue">taskQueue.</param>
        /// <param name="logger">logger.</param>
        /// <param name="serviceProvider">serviceProvider.</param>
        public QueuedHostedService(
            IBackgroundTaskQueue taskQueue,
            ILogger logger,
            IServiceProvider serviceProvider)
        {
            this.taskQueue = taskQueue;
            this.logger = logger.ThrowIfNull(nameof(logger));
            this.serviceProvider = serviceProvider;
        }

        /// <inheritdoc/>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this.logger.Information("Servicio de procesamiento de actualizaciones iniciado");
            await this.BackgroundProcessing(stoppingToken);
        }

        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var workItem = await this.taskQueue.DequeueAsync(stoppingToken);

                    if (workItem != null)
                    {
                        using var scope = this.serviceProvider.CreateScope();
                        await workItem(scope.ServiceProvider, stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    this.logger.Error(ex, "Error procesando tarea de actualización");
                }
            }
        }
    }
}
