// <summary>
// <copyright file="CreateChildOrdersSapHandler.cs" company="Axity">
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
    using Newtonsoft.Json;
    using Omicron.Pedidos.DataAccess.DAO.Pedidos;
    using Omicron.Pedidos.Dtos.Models;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Entities.Model.Db;
    using Omicron.Pedidos.Services.Constants;
    using Omicron.Pedidos.Services.MediatR.Commands;
    using Omicron.Pedidos.Services.OrderHistory;
    using Omicron.Pedidos.Services.Redis;
    using Omicron.Pedidos.Services.SapServiceLayerAdapter;
    using Omicron.Pedidos.Services.Utils;
    using Serilog;

    /// <summary>
    /// CreateChildOrdersSapHandler.
    /// </summary>
    public class CreateChildOrdersSapHandler : IRequestHandler<CreateChildOrdersSapCommand>
    {
        private readonly IPedidosDao pedidosDao;

        private readonly ISapServiceLayerAdapterService serviceLayerAdapterService;

        private readonly ILogger logger;

        private readonly IRedisService redisService;

        private readonly IOrderHistoryHelper orderHistoryHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateChildOrdersSapHandler"/> class.
        /// </summary>
        /// <param name="pedidosDao">Pedidos Dao.</param>
        /// <param name="serviceLayerAdapterService">Service Layer Service.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="redisService">redisService.</param>
        /// <param name="orderHistoryHelper">Order History Helper.</param>
        public CreateChildOrdersSapHandler(
            IPedidosDao pedidosDao,
            ISapServiceLayerAdapterService serviceLayerAdapterService,
            ILogger logger,
            IRedisService redisService,
            IOrderHistoryHelper orderHistoryHelper)
        {
            this.pedidosDao = pedidosDao.ThrowIfNull(nameof(pedidosDao));
            this.serviceLayerAdapterService = serviceLayerAdapterService.ThrowIfNull(nameof(serviceLayerAdapterService));
            this.logger = logger.ThrowIfNull(nameof(logger));
            this.redisService = redisService.ThrowIfNull(nameof(redisService));
            this.orderHistoryHelper = orderHistoryHelper.ThrowIfNull(nameof(orderHistoryHelper));
        }

        /// <inheritdoc/>
        public async Task Handle(CreateChildOrdersSapCommand request, CancellationToken cancellationToken)
        {
            this.logger.Information(LogsConstants.CreateChildOrdersLogBase, request.ProductionOrderId);
            var logBase = string.Format(LogsConstants.CreateChildOrdersLogBaseStart, LogsConstants.CreateChildOrdersLogBase, request.Pieces, request.SeparationId);

            try
            {
                var productionOrder = (await this.pedidosDao.GetUserOrderByProducionOrder([request.ProductionOrderId.ToString()]))
                                      .FirstOrDefault() ?? throw new Exception(LogsConstants.ProductionOrderNotFound);

                await this.ExecuteCreateChildOrderStep(productionOrder, request, logBase);

                this.logger.Information($"{logBase} - Proceso de division de orden ejecutado correctamente (Orden de produccion hija: {request.ProductionOrderChildId})");

                var redisKey = string.Format(ServiceConstants.ProductionOrderSeparationProcessKey, request.ProductionOrderId);
                await this.redisService.DeleteKey(redisKey);
            }
            catch (Exception ex)
            {
                this.logger.Error(ex, $"{logBase} - Error en el proceso de division (Step: {request.LastStep} ProductionOrder: {request.ProductionOrderId})");

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
                        ChildProductionOrderId = request.ProductionOrderChildId > 0 ? request.ProductionOrderChildId : null,
                        Payload = JsonConvert.SerializeObject(request),
                        CreatedAt = DateTime.Now,
                        LastUpdated = DateTime.Now,
                    };

                    await this.pedidosDao.InsertProductionOrderSeparationDetailLogById(processWithError);
                }

                var redisKey = string.Format(ServiceConstants.ProductionOrderSeparationProcessKey, request.ProductionOrderId);
                await this.redisService.DeleteKey(redisKey);
            }
        }

        private async Task ExecuteCreateChildOrderStep(UserOrderModel productionOrder, CreateChildOrdersSapCommand request, string logBase)
        {
            switch (request.LastStep?.Trim())
            {
                case null:
                case ServiceConstants.EmptyValue:
                case ServiceConstants.SaveHistoryStep:
                case ServiceConstants.StepCreateChildOrderSap:
                    await this.ExecuteFullCreationFlow(productionOrder, request, logBase);
                    break;
                case ServiceConstants.StepCreateChildOrderWithComponentsSap:
                    await this.ExecuteCreationFromPostgresStep(productionOrder, request, logBase);
                    break;
                case ServiceConstants.StepCreateChildOrderPostgres:
                    await this.ExecuteCreationFromHistoryStep(request, logBase);
                    break;
                default:
                    this.logger.Error(LogsConstants.StepNotRecognized, logBase, request.LastStep);
                    break;
            }
        }

        private async Task ExecuteFullCreationFlow(UserOrderModel productionOrder, CreateChildOrdersSapCommand request, string logBase)
        {
            this.logger.Information($"{logBase} - Iniciando flujo completo de creación de orden hija (Step: {request.LastStep})");
            int productionOrderChild;
            productionOrderChild = await this.CreateChildOrderProcess(request.ProductionOrderId, request.Pieces, request.SeparationId, request);
            request.ProductionOrderChildId = productionOrderChild;
            request.LastStep = ServiceConstants.StepCreateChildOrderWithComponentsSap;
            await this.CreateChildOrderOnPostgres(productionOrder, request.ProductionOrderId, request.Pieces, request.SeparationId, request.ProductionOrderChildId);
            request.LastStep = ServiceConstants.StepCreateChildOrderPostgres;
            await this.orderHistoryHelper.SaveHistoryOrdersFab(request.ProductionOrderChildId, request);
            request.LastStep = ServiceConstants.StepSaveChildOrderHistory;

            var productionOrderSeparationLog = await this.pedidosDao.GetProductionOrderSeparationDetailLogById(request.SeparationId);
            if (productionOrderSeparationLog != null)
            {
                productionOrderSeparationLog.LastStep = request.LastStep;
                productionOrderSeparationLog.LastUpdated = DateTime.Now;
                productionOrderSeparationLog.IsSuccessful = true;

                await this.pedidosDao.UpdateProductionOrderSeparationDetailLog(productionOrderSeparationLog);
            }
        }

        private async Task ExecuteCreationFromPostgresStep(UserOrderModel productionOrder, CreateChildOrdersSapCommand request, string logBase)
        {
            this.logger.Information($"{logBase} - Proceso de reintento desde Postgres (Step: {request.LastStep})");
            await this.CreateChildOrderOnPostgres(productionOrder, request.ProductionOrderId, request.Pieces, request.SeparationId, request.ProductionOrderChildId);
            request.LastStep = ServiceConstants.StepCreateChildOrderPostgres;
            await this.orderHistoryHelper.SaveHistoryOrdersFab(request.ProductionOrderChildId, request);
            request.LastStep = ServiceConstants.StepSaveChildOrderHistory;

            var productionOrderSeparationLog = await this.pedidosDao.GetProductionOrderSeparationDetailLogById(request.SeparationId);
            if (productionOrderSeparationLog != null)
            {
                productionOrderSeparationLog.LastStep = request.LastStep;
                productionOrderSeparationLog.LastUpdated = DateTime.Now;
                productionOrderSeparationLog.IsSuccessful = true;

                await this.pedidosDao.UpdateProductionOrderSeparationDetailLog(productionOrderSeparationLog);
            }
        }

        private async Task ExecuteCreationFromHistoryStep(CreateChildOrdersSapCommand request, string logBase)
        {
            this.logger.Information($"{logBase} - Proceso de reintento desde historial (Step: {request.LastStep})");
            await this.orderHistoryHelper.SaveHistoryOrdersFab(request.ProductionOrderChildId, request);
            request.LastStep = ServiceConstants.StepSaveChildOrderHistory;

            var productionOrderSeparationLog = await this.pedidosDao.GetProductionOrderSeparationDetailLogById(request.SeparationId);
            if (productionOrderSeparationLog != null)
            {
                productionOrderSeparationLog.LastStep = request.LastStep;
                productionOrderSeparationLog.LastUpdated = DateTime.Now;
                productionOrderSeparationLog.IsSuccessful = true;

                await this.pedidosDao.UpdateProductionOrderSeparationDetailLog(productionOrderSeparationLog);
            }
        }

        private async Task<int> CreateChildOrderProcess(int productionOrderId, int pieces, string separationId, CreateChildOrdersSapCommand request)
        {
            var separationOrders = await this.pedidosDao.GetOrdersBySeparationId(separationId);
            if (separationOrders.Any())
            {
                this.logger.Information($"separationId-{separationId}: La orden ya fue creada anteriormente, omitiendo creación");
                return separationOrders.First().Id;
            }

            this.logger.Information($"separationId-{separationId}: Inicia proceso de creación");

            var requestSap = new CreateChildProductionOrdersDto()
            {
                OrderId = productionOrderId,
                Pieces = pieces,
                LastStep = request.LastStep,
                ProductionOrderChildId = request.ProductionOrderChildId,
            };

            this.logger.Information($"separationId-{separationId}: Enviando al service layer para creación");
            var serviceLResult = await this.serviceLayerAdapterService.PostAsync(requestSap, ServiceConstants.CreateChildOrderSapUrl);

            if (!serviceLResult.Success)
            {
                throw new Exception(ServiceConstants.ErrorOccurredWhileCommunicatingWithServiceLayerAdapter);
            }

            var result = JsonConvert.DeserializeObject<CreateChildOrderResultDto>(serviceLResult.Response.ToString());

            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                this.logger.Error($"separationId-{separationId}: Error service layer. ErrorMessage='{result.ErrorMessage}', ProductionOrderChildId={result.ProductionOrderChildId}, LastStep='{result.LastStep}'");
                request.ProductionOrderChildId = result.ProductionOrderChildId > 0 ? result.ProductionOrderChildId : request.ProductionOrderChildId;
                request.LastStep = result.LastStep;
                throw new Exception(result.ErrorMessage);
            }

            var newFoId = result.ProductionOrderChildId;
            this.logger.Information($"separationId-{separationId}: Se creó la Orden de fabricación {newFoId}");
            this.logger.Information($"separationId-{separationId}: Finaliza proceso de creación");

            return newFoId;
        }

        private async Task CreateChildOrderOnPostgres(UserOrderModel productionOrder, int productionOrderId, int pieces, string separationId, int newFoId)
        {
            this.logger.Information($"separationId-{separationId}: Inicio guardado de orden de fabricacion {newFoId} en Postgres");
            var qr = new MagistralQrModel();
            if (!string.IsNullOrEmpty(productionOrder.MagistralQr))
            {
                qr = JsonConvert.DeserializeObject<MagistralQrModel>(productionOrder.MagistralQr);
                qr.ProductionOrder = newFoId;
                qr.Quantity = pieces;
            }

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
                MagistralQr = ServiceShared.CalculateTernary(!string.IsNullOrEmpty(productionOrder.MagistralQr), JsonConvert.SerializeObject(qr), null),
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
            await this.pedidosDao.InsertUserOrder(new List<UserOrderModel>() { newProductionOrder });
            this.logger.Information($"separationId-{separationId}: Se guardó la orden de fabricacion {newFoId} en Postgres correctamente");
        }
    }
}
