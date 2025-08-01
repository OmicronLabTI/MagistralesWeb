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
    using Newtonsoft.Json;
    using Omicron.Pedidos.DataAccess.DAO.Pedidos;
    using Omicron.Pedidos.Dtos.Models;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Services.Constants;
    using Omicron.Pedidos.Services.MediatR.Commands;
    using Omicron.Pedidos.Services.MediatR.Services;
    using Omicron.Pedidos.Services.Redis;
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

        private readonly IRedisService redisService;

        private readonly OrderHistoryHelper orderHistoryHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeparateProductionOrderHandler"/> class.
        /// </summary>
        /// <param name="pedidosDao">Pedidos Dao.</param>
        /// <param name="serviceLayerAdapterService">Service Layer Service.</param>
        /// <param name="backgroundTaskQueue">backgroundTaskQueue.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="redisService">redisService.</param>
        /// <param name="orderHistoryHelper">Order History Helper.</param>
        public SeparateProductionOrderHandler(
            IPedidosDao pedidosDao,
            ISapServiceLayerAdapterService serviceLayerAdapterService,
            IBackgroundTaskQueue backgroundTaskQueue,
            ILogger logger,
            IRedisService redisService,
            OrderHistoryHelper orderHistoryHelper)
        {
            this.pedidosDao = pedidosDao.ThrowIfNull(nameof(pedidosDao));
            this.serviceLayerAdapterService = serviceLayerAdapterService.ThrowIfNull(nameof(serviceLayerAdapterService));
            this.logger = logger.ThrowIfNull(nameof(logger));
            this.backgroundTaskQueue = backgroundTaskQueue.ThrowIfNull(nameof(backgroundTaskQueue));
            this.redisService = redisService.ThrowIfNull(nameof(redisService));
            this.orderHistoryHelper = orderHistoryHelper.ThrowIfNull(nameof(orderHistoryHelper));
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

                await this.CancelProductionOrderProcess(productionOrder, request, logBase);
                var childOrderId = await this.CreateChildOrdersProcess(productionOrder, request.ProductionOrderId, request.Pieces, request.SeparationId);

                await this.orderHistoryHelper.SaveHistoryOrdersFab(
                    childOrderId,
                    request.ProductionOrderId,
                    request.UserId,
                    request.DxpOrder,
                    request.SapOrder,
                    request.Pieces,
                    request.TotalPieces);

                var redisKey = string.Format(ServiceConstants.ProductionOrderSeparationProcessKey, request.ProductionOrderId);
                await this.redisService.DeleteKey(redisKey);
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

        private async Task CancelProductionOrderProcess(UserOrderModel productionOrder, SeparateProductionOrderCommand request, string logBase)
        {
            if (productionOrder.Status == ServiceConstants.Cancelled)
            {
                this.logger.Information(LogsConstants.ProductionOrderIsAlreadyCancelled, logBase);
                return;
            }

            await this.CancelProductionOrderOnSapAsync(request, logBase);
            await this.CancelProductionOrderOnPostgresqlAsync(productionOrder, logBase);
            this.logger.Information(LogsConstants.ProductionOrderCancelledSuccessfully, logBase);
        }

        private async Task CancelProductionOrderOnSapAsync(SeparateProductionOrderCommand request, string logBase)
        {
            this.logger.Information(LogsConstants.CancellingProductionOrderInSAP, logBase);
            var result = await this.serviceLayerAdapterService.PostAsync(
                new CancelProductionOrderDto
                {
                    ProductionOrderId = request.ProductionOrderId,
                    SeparationId = request.SeparationId,
                },
                ServiceConstants.SeparationProcessCancelProductionOrderEndPoint,
                string.Format(LogsConstants.FailedToCancelProductionOrderInSAP, logBase));

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
            var retryCommand = new SeparateProductionOrderCommand(
                request.ProductionOrderId,
                request.Pieces,
                request.SeparationId,
                request.UserId,
                request.DxpOrder,
                request.SapOrder,
                request.TotalPieces)
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

        private async Task<int> CreateChildOrdersProcess(UserOrderModel productionOrder, int productionOrderId, int pieces, string separationId)
        {
            this.logger.Information($"separationId-{separationId}: Validación orden creada");
            var separationOrders = await this.pedidosDao.GetOrdersBySeparationId(separationId);
            if (!separationOrders.Any())
            {
                this.logger.Information($"separationId-{separationId}: Inicia proceso de creación");
                var newFoId = await this.CreateChildOrders(productionOrder, productionOrderId, pieces, separationId);
                this.logger.Information($"separationId-{separationId}: Finaliza proceso de creación");
                return newFoId;
            }

            this.logger.Information($"separationId-{separationId}: La orden ya fue creada anteriormente, omitiendo creación");
            return separationOrders.First().Id;
        }

        private async Task<int> CreateChildOrders(UserOrderModel productionOrder, int productionOrderId, int pieces, string separationId)
        {
            var request = new CreateChildProductionOrdersDto() { OrderId = productionOrderId, Pieces = pieces };
            this.logger.Information($"separationId-{separationId}: Enviando al service layer para creación");
            var serviceLResult = await this.serviceLayerAdapterService.PostAsync(request, ServiceConstants.CreateChildOrderSapUrl);
            var newFoId = JsonConvert.DeserializeObject<int>(serviceLResult.Response.ToString());
            this.logger.Information($"separationId-{separationId}: Se creó la Orden de fabricación {newFoId}");

            var qr = JsonConvert.DeserializeObject<MagistralQrModel>(productionOrder.MagistralQr);
            qr.ProductionOrder = newFoId;
            qr.Quantity = pieces;
            var newStatus = productionOrder.ReassignmentDate.HasValue ? ServiceConstants.Reasignado : ServiceConstants.Proceso;
            var newProductionOrder = new UserOrderModel()
            {
                Userid = productionOrder.Userid,
                Salesorderid = productionOrder.Salesorderid,
                Productionorderid = newFoId.ToString(),
                Status = newStatus,
                PlanningDate = productionOrder.PlanningDate,
                Comments = productionOrder.Comments,
                FinishDate = productionOrder.FinishDate,
                CreationDate = productionOrder.CreationDate,
                CreatorUserId = productionOrder.CreatorUserId,
                CloseDate = productionOrder.CloseDate,
                CloseUserId = productionOrder.CloseUserId,
                FinishedLabel = productionOrder.FinishedLabel,
                MagistralQr = JsonConvert.SerializeObject(qr),
                FinalizedDate = productionOrder.FinalizedDate,
                StatusAlmacen = productionOrder.StatusAlmacen,
                UserCheckIn = productionOrder.UserCheckIn,
                DateTimeCheckIn = productionOrder.DateTimeCheckIn,
                RemisionQr = productionOrder.RemisionQr,
                DeliveryId = productionOrder.DeliveryId,
                StatusInvoice = productionOrder.StatusInvoice,
                UserInvoiceStored = productionOrder.UserInvoiceStored,
                InvoiceStoreDate = productionOrder.InvoiceStoreDate,
                InvoiceQr = productionOrder.InvoiceQr,
                InvoiceId = productionOrder.InvoiceId,
                InvoiceType = productionOrder.InvoiceType,
                TypeOrder = productionOrder.BatchFinalized,
                Quantity = pieces,
                BatchFinalized = productionOrder.BatchFinalized,
                AreBatchesComplete = 0,
                TecnicId = productionOrder.TecnicId,
                StatusForTecnic = productionOrder.StatusForTecnic,
                AssignmentDate = productionOrder.AssignmentDate,
                PackingDate = productionOrder.PackingDate,
                InvoiceLineNum = productionOrder.InvoiceLineNum,
                ReassignmentDate = productionOrder.ReassignmentDate,
                CloseSampleOrderId = productionOrder.CloseSampleOrderId,
                SeparationId = separationId,
            };
            this.logger.Information($"separationId-{separationId}: Inicio guardado de orden de fabricacion {newFoId} en Postgres");
            await this.pedidosDao.InsertUserOrder(new List<UserOrderModel>() { newProductionOrder });
            this.logger.Information($"separationId-{separationId}: Se guardó la orden de fabricacion {newFoId} en Postgres correctamente");
            return newFoId;
        }
    }
}
