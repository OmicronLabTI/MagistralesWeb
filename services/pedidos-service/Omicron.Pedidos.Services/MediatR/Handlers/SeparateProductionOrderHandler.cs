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
            try
            {
                var productionOrder =
                    (await this.pedidosDao.GetUserOrderByProducionOrder([request.ProductionOrderId.ToString()]))
                    .FirstOrDefault() ?? throw new Exception("No existe la orden de producción.");

                await this.CancelProductionOrderProcess(productionOrder, request.ProductionOrderId);

                return true;
            }
            catch (Exception ex)
            {
                this.logger.Error(ex, $"Error en intento {request.RetryCount + 1} - Production Order Id: {request.ProductionOrderId}");
                if (request.RetryCount < request.MaxRetries)
                {
                    // Programar reintento en 3 minutos
                    this.ScheduleRetry(request);
                    return false; // Indica que se programó un reintento
                }
                else
                {
                    this.logger.Error($"Máximo número de reintentos alcanzado - Production Order Id: {request.ProductionOrderId}");
                    throw;
                }
            }
        }

        private async Task CancelProductionOrderProcess(UserOrderModel productionOrder, int productionOrderId)
        {
            if (productionOrder.Status == ServiceConstants.Cancelled)
            {
                this.logger.Information("Orden de producción ya se encuentra cancelada");
                return;
            }

            await this.CancelProductionOrderOnSapAsync(productionOrderId);
            await this.CancelProductionOrderOnPostgresqlAsync(productionOrder);
            this.logger.Information("Actualización finalizada correctamente.");
        }

        private async Task CancelProductionOrderOnSapAsync(int productionOrderId)
        {
            var result = await this.serviceLayerAdapterService.PostAsync(
                new List<int> { productionOrderId },
                "endpointcancelar",
                "Error al cxancelar en sap"); // ServiceConstants.SapFinalizeProductionOrdersEndpoint

            if (!result.Success)
            {
                throw new Exception(result.ExceptionMessage);
            }
        }

        private async Task CancelProductionOrderOnPostgresqlAsync(UserOrderModel userOrderToCancel)
        {
            userOrderToCancel.Status = ServiceConstants.Cancelled;
            await this.pedidosDao.UpdateUserOrders([userOrderToCancel]);
        }

        private void ScheduleRetry(SeparateProductionOrderCommand request)
        {
            var retryCommand = new SeparateProductionOrderCommand(request.ProductionOrderId, request.Pieces, request.SeparationId)
            {
                RetryCount = request.RetryCount + 1,
                MaxRetries = request.MaxRetries,
            };

            this.backgroundTaskQueue.QueueBackgroundWorkItem(async (services, token) =>
            {
                await Task.Delay(TimeSpan.FromMinutes(3), token); // Esperar 3 minutos
                var mediator = services.GetRequiredService<IMediator>();
                await mediator.Send(retryCommand, token);
            });

            this.logger.Information($"Reintento programado para 3 minutos - Production Order Id: {request.ProductionOrderId}, Intento: {request.RetryCount + 2}");
        }
    }
}
