// <summary>
// <copyright file="SeparateProductionOrderHandler.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.MediatR.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using global::MediatR;
    using Microsoft.Extensions.DependencyInjection;
    using Omicron.Pedidos.DataAccess.DAO.Pedidos;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Services.Constants;
    using Omicron.Pedidos.Services.MediatR.Commands;
    using Omicron.Pedidos.Services.MediatR.Services;
    using Omicron.Pedidos.Services.SapServiceLayerAdapter;
    using Omicron.Pedidos.Services.Utils;
    using Serilog;

    /// <summary>
    /// SeparateProductionOrderHandler.
    /// </summary>
    public class SeparateProductionOrderHandler : IRequestHandler<SeparateProductionOrderCommand, bool>
    {
        private readonly IPedidosDao pedidosDao;

        private readonly ISapServiceLayerAdapterService serviceLayerAdapterService;

        private readonly ILogger logger;

        private readonly IBackgroundTaskQueue backgroundTaskQueue;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeparateProductionOrderHandler"/> class.
        /// </summary>
        /// <param name="pedidosDao">Pedidos Dao.</param>
        /// <param name="serviceLayerAdapterService">Service Layer Service.</param>
        /// <param name="backgroundTaskQueue">backgroundTaskQueue.</param>
        /// <param name="logger">Logger.</param>
        public SeparateProductionOrderHandler(
            IPedidosDao pedidosDao,
            ISapServiceLayerAdapterService serviceLayerAdapterService,
            IBackgroundTaskQueue backgroundTaskQueue,
            ILogger logger)
        {
            this.pedidosDao = pedidosDao.ThrowIfNull(nameof(pedidosDao));
            this.serviceLayerAdapterService = serviceLayerAdapterService.ThrowIfNull(nameof(serviceLayerAdapterService));
            this.logger = logger.ThrowIfNull(nameof(logger));
            this.backgroundTaskQueue = backgroundTaskQueue.ThrowIfNull(nameof(backgroundTaskQueue));
        }

        /// <inheritdoc/>
        public async Task<bool> Handle(SeparateProductionOrderCommand request, CancellationToken cancellationToken)
        {
            var logBase = string.Format(LogsConstants.SeparateProductionOrderLogBase, request.SeparationId, request.ProductionOrderId);
            this.logger.Information(LogsConstants.SeparateProductionOrderStart, logBase, request.Pieces, request.RetryCount + 1);
            try
            {
                var productionOrder =
                    (await this.pedidosDao.GetUserOrderByProducionOrder([request.ProductionOrderId.ToString()]))
                    .FirstOrDefault() ?? throw new Exception(LogsConstants.ProductionOrderNotFound);

                await this.CancelProductionOrderProcess(productionOrder, request.ProductionOrderId, logBase);

                this.logger.Information(LogsConstants.SeparateProductionOrderEndSuccessfuly, logBase);
                return true;
            }
            catch (Exception ex)
            {
                var error = string.Format(LogsConstants.SeparateProductionOrderEndWithError, logBase);
                this.logger.Error(ex, error);
                if (request.RetryCount < request.MaxRetries)
                {
                    this.ScheduleRetry(request, logBase);
                    return false;
                }
                else
                {
                    var finalError = string.Format(LogsConstants.MaximumNumberOfRetriesReached, logBase);
                    this.logger.Error(ex, finalError);
                    throw;
                }
            }
        }

        private async Task CancelProductionOrderProcess(UserOrderModel productionOrder, int productionOrderId, string logBase)
        {
            if (productionOrder.Status == ServiceConstants.Cancelled)
            {
                this.logger.Information(LogsConstants.ProductionOrderIsAlreadyCancelled, logBase);
                return;
            }

            await this.CancelProductionOrderOnSapAsync(productionOrderId, logBase);
            await this.CancelProductionOrderOnPostgresqlAsync(productionOrder, logBase);
            this.logger.Information(LogsConstants.ProductionOrderCancelledSuccessfully, logBase);
        }

        private async Task CancelProductionOrderOnSapAsync(int productionOrderId, string logBase)
        {
            this.logger.Information(LogsConstants.CancellingProductionOrderInSAP, logBase);
            var result = await this.serviceLayerAdapterService.PostAsync(
                new List<int> { productionOrderId },
                "endpointcancelar",
                string.Format(LogsConstants.FailedToCancelProductionOrderInSAP, logBase)); // ServiceConstants.SapFinalizeProductionOrdersEndpoint

            if (!result.Success)
            {
                this.logger.Error(string.Format(LogsConstants.FailedToCancelProductionOrderInSAP, logBase));
                throw new Exception(result.ExceptionMessage);
            }
        }

        private async Task CancelProductionOrderOnPostgresqlAsync(UserOrderModel userOrderToCancel, string logBase)
        {
            this.logger.Information(LogsConstants.CancellingProductionOrderInPostgreSQL, logBase);
            userOrderToCancel.Status = ServiceConstants.Cancelled;
            await this.pedidosDao.UpdateUserOrders([userOrderToCancel]);
        }

        private void ScheduleRetry(SeparateProductionOrderCommand request, string logBase)
        {
            var retryCommand = new SeparateProductionOrderCommand(request.ProductionOrderId, request.Pieces, request.SeparationId)
            {
                RetryCount = request.RetryCount + 1,
                MaxRetries = request.MaxRetries,
            };

            this.backgroundTaskQueue.QueueBackgroundWorkItem(async (services, token) =>
            {
                await Task.Delay(TimeSpan.FromMinutes(ServiceConstants.MinutesToRetrySeparationProductionOrder), token);
                var mediator = services.GetRequiredService<IMediator>();
                await mediator.Send(retryCommand, token);
            });

            this.logger.Information(LogsConstants.RetryScheduledLog, logBase, ServiceConstants.MinutesToRetrySeparationProductionOrder, request.RetryCount + 2);
        }
    }
}
