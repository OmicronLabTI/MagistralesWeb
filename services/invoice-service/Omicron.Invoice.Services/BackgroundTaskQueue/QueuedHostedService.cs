// <copyright file="QueuedHostedService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>

namespace Omicron.Invoice.Services.BackgroundTaskQueue
{
    /// <summary>
    /// QueuedHostedService.
    /// </summary>
    public class QueuedHostedService : BackgroundService
    {
        private readonly IBackgroundTaskQueue taskQueue;
        private readonly Serilog.ILogger loggerSeriLog;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueuedHostedService"/> class.
        /// </summary>
        /// <param name="taskQueue">taskQueue.</param>
        /// <param name="loggerSeriLog">loggerSeriLog.</param>
        public QueuedHostedService(IBackgroundTaskQueue taskQueue, Serilog.ILogger loggerSeriLog)
        {
            this.taskQueue = taskQueue;
            this.loggerSeriLog = loggerSeriLog;
        }

        /// <inheritdoc/>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this.loggerSeriLog.Information(LogsConstants.QueuedHostedServiceRunning);
            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem = await this.taskQueue.DequeueAsync(stoppingToken);

                try
                {
                    await workItem(stoppingToken);
                }
                catch (Exception ex)
                {
                    this.loggerSeriLog.Error(string.Format(LogsConstants.ErrorOccurredExecutingBackgroundTask, ex.Message));
                }
            }
        }
    }
}
