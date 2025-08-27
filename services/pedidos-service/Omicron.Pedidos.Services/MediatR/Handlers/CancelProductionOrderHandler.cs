// <summary>
// <copyright file="CancelProductionOrderHandler.cs" company="Axity">
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
    using Omicron.Pedidos.Entities.Model.Db;
    using Omicron.Pedidos.Services.Constants;
    using Omicron.Pedidos.Services.MediatR.Commands;
    using Omicron.Pedidos.Services.MediatR.Services;
    using Omicron.Pedidos.Services.OrderHistory;
    using Omicron.Pedidos.Services.Redis;
    using Omicron.Pedidos.Services.SapServiceLayerAdapter;
    using Omicron.Pedidos.Services.Utils;
    using Serilog;

    /// <summary>
    /// CancelProductionOrderHandler.
    /// </summary>
    public class CancelProductionOrderHandler : IRequestHandler<CancelProductionOrderCommand, bool>
    {
        private readonly IPedidosDao pedidosDao;

        private readonly ISapServiceLayerAdapterService serviceLayerAdapterService;

        private readonly ILogger logger;

        private readonly IBackgroundTaskQueue backgroundTaskQueue;

        private readonly IRedisService redisService;

        private readonly IOrderHistoryHelper orderHistoryHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelProductionOrderHandler"/> class.
        /// </summary>
        /// <param name="pedidosDao">Pedidos Dao.</param>
        /// <param name="serviceLayerAdapterService">Service Layer Service.</param>
        /// <param name="backgroundTaskQueue">backgroundTaskQueue.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="redisService">redisService.</param>
        /// <param name="orderHistoryHelper">Order History Helper.</param>
        public CancelProductionOrderHandler(
            IPedidosDao pedidosDao,
            ISapServiceLayerAdapterService serviceLayerAdapterService,
            IBackgroundTaskQueue backgroundTaskQueue,
            ILogger logger,
            IRedisService redisService,
            IOrderHistoryHelper orderHistoryHelper)
        {
            this.pedidosDao = pedidosDao.ThrowIfNull(nameof(pedidosDao));
            this.serviceLayerAdapterService = serviceLayerAdapterService.ThrowIfNull(nameof(serviceLayerAdapterService));
            this.logger = logger.ThrowIfNull(nameof(logger));
            this.backgroundTaskQueue = backgroundTaskQueue.ThrowIfNull(nameof(backgroundTaskQueue));
            this.redisService = redisService.ThrowIfNull(nameof(redisService));
            this.orderHistoryHelper = orderHistoryHelper.ThrowIfNull(nameof(orderHistoryHelper));
        }

        /// <inheritdoc/>
        public async Task<bool> Handle(CancelProductionOrderCommand request, CancellationToken cancellationToken)
        {
            var logBase = string.Format(LogsConstants.SeparateProductionOrderLogBase, request.SeparationId, request.ProductionOrderId);
            this.logger.Information(LogsConstants.SeparateProductionOrderStart, logBase, request.Pieces);
            try
            {
                var productionOrder =
                    (await this.pedidosDao.GetUserOrderByProducionOrder([request.ProductionOrderId.ToString()]))
                    .FirstOrDefault() ?? throw new Exception(LogsConstants.ProductionOrderNotFound);

                await this.ExecuteCancellationStepAsync(productionOrder, request, logBase);

                this.backgroundTaskQueue.QueueBackgroundWorkItem(async (services, token) =>
                {
                    var mediator = services.GetRequiredService<IMediator>();
                    var createChildOrdersCommand = new CreateChildOrdersSapCommand(
                        request.ProductionOrderId,
                        request.Pieces,
                        request.SeparationId,
                        request.UserId,
                        request.DxpOrder,
                        request.SapOrder,
                        request.TotalPieces,
                        request.LastStep);
                    await mediator.Send(createChildOrdersCommand, token);
                });

                this.logger.Information(LogsConstants.SeparateProductionOrderEndSuccessfuly, logBase);
                return true;
            }
            catch (Exception ex)
            {
                var error = string.Format(LogsConstants.SeparateProductionOrderEndWithError, logBase);
                this.logger.Error(ex, error);
                var productionOrderSeparationLog = await this.pedidosDao.GetProductionOrderSeparationDetailLogById(request.SeparationId);

                if (productionOrderSeparationLog != null)
                {
                    productionOrderSeparationLog.ErrorMessage = ex.Message;
                    productionOrderSeparationLog.LastStep = request.LastStep;
                    productionOrderSeparationLog.LastUpdated = DateTime.Now;

                    await this.pedidosDao.UpdateProductionOrderSeparationDetailLog(productionOrderSeparationLog);
                }
                else
                {
                    var processWithError = new ProductionOrderSeparationDetailLogsModel
                    {
                        Id = request.SeparationId,
                        ParentProductionOrderId = request.ProductionOrderId,
                        LastStep = request.LastStep,
                        IsSuccessful = false,
                        ErrorMessage = ex.Message,
                        ChildProductionOrderId = null,
                        Payload = JsonConvert.SerializeObject(request),
                        CreatedAt = DateTime.Now,
                        LastUpdated = DateTime.Now,
                    };

                    await this.pedidosDao.InsertProductionOrderSeparationDetailLogById(processWithError);
                }

                var redisKey = string.Format(ServiceConstants.ProductionOrderSeparationProcessKey, request.ProductionOrderId);
                await this.redisService.DeleteKey(redisKey);
                return false;
            }
        }

        private async Task ExecuteCancellationStepAsync(UserOrderModel productionOrder, CancelProductionOrderCommand request, string logBase)
        {
            switch (request.LastStep?.Trim())
            {
                case null:
                case ServiceConstants.EmptyValue:
                case ServiceConstants.StartStep:
                case ServiceConstants.UpdateCancelParentOrderStep:
                    await this.ExecuteFullCancellationFlow(productionOrder, request, logBase);
                    break;
                case ServiceConstants.CancelSapStep:
                    await this.ExecuteCancellationFromPostgresStep(productionOrder, request, logBase);
                    break;
                case ServiceConstants.CancelPostgresStep:
                    await this.ExecuteCancellationFromHistoryStep(productionOrder, request, logBase);
                    break;
                default:
                    this.logger.Error(LogsConstants.StepNotRecognized, logBase, request.LastStep);
                    break;
            }
        }

        private async Task ExecuteFullCancellationFlow(UserOrderModel productionOrder, CancelProductionOrderCommand request, string logBase)
        {
            await this.CancelProductionOrderProcess(productionOrder, request, logBase);
            request.LastStep = ServiceConstants.CancelSapStep;

            await this.CancelProductionOrderOnPostgresqlAsync(productionOrder, logBase);
            request.LastStep = ServiceConstants.CancelPostgresStep;

            await this.SaveHistoryOrdersFab(productionOrder, request, logBase);
            request.LastStep = ServiceConstants.SaveHistoryStep;
        }

        private async Task ExecuteCancellationFromPostgresStep(UserOrderModel productionOrder, CancelProductionOrderCommand request, string logBase)
        {
            await this.CancelProductionOrderOnPostgresqlAsync(productionOrder, logBase);
            request.LastStep = ServiceConstants.CancelPostgresStep;

            await this.SaveHistoryOrdersFab(productionOrder, request, logBase);
            request.LastStep = ServiceConstants.SaveHistoryStep;
        }

        private async Task ExecuteCancellationFromHistoryStep(UserOrderModel productionOrder, CancelProductionOrderCommand request, string logBase)
        {
            await this.SaveHistoryOrdersFab(productionOrder, request, logBase);
            request.LastStep = ServiceConstants.SaveHistoryStep;
        }

        private async Task CancelProductionOrderProcess(UserOrderModel productionOrder, CancelProductionOrderCommand request, string logBase)
        {
            if (productionOrder.Status == ServiceConstants.Cancelled)
            {
                this.logger.Information(LogsConstants.ProductionOrderIsAlreadyCancelled, logBase);
                return;
            }

            await this.CancelProductionOrderOnSapAsync(request, logBase);
            this.logger.Information(LogsConstants.ProductionOrderCancelledSuccessfully, logBase);
        }

        private async Task CancelProductionOrderOnSapAsync(CancelProductionOrderCommand request, string logBase)
        {
            this.logger.Information(LogsConstants.CancellingProductionOrderInSAP, logBase);
            var result = await this.serviceLayerAdapterService.PostAsync(
                new CancelProductionOrderDto
                {
                    ProductionOrderId = request.ProductionOrderId,
                    SeparationId = request.SeparationId,
                    LastStep = request.LastStep,
                },
                ServiceConstants.SeparationProcessCancelProductionOrderEndPoint,
                string.Format(LogsConstants.FailedToCancelProductionOrderInSAP, logBase));

            if (!result.Success)
            {
                this.logger.Error(string.Format(LogsConstants.FailedToCancelProductionOrderInSAP, logBase));
                request.LastStep = ServiceConstants.StartStep;
                throw new Exception(result.ExceptionMessage);
            }

            var response = JsonConvert.DeserializeObject<CancelProductionOrderDto>(result.Response.ToString());

            if (!string.IsNullOrEmpty(response.ErrorMessage))
            {
                this.logger.Error(string.Format(LogsConstants.FailedToCancelProductionOrderInSAP, logBase));
                request.LastStep = response.LastStep;
                throw new Exception(response.ErrorMessage);
            }
        }

        private async Task CancelProductionOrderOnPostgresqlAsync(UserOrderModel userOrderToCancel, string logBase)
        {
            this.logger.Information(LogsConstants.CancellingProductionOrderInPostgreSQL, logBase);
            userOrderToCancel.Status = ServiceConstants.Cancelled;
            await this.pedidosDao.UpdateUserOrders([userOrderToCancel]);
        }

        private async Task SaveHistoryOrdersFab(UserOrderModel productionOrder, CancelProductionOrderCommand request, string logBase)
        {
            await this.orderHistoryHelper.SaveHistoryParentOrdersFab(request);
        }
    }
}
