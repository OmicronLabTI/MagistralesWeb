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
    using Omicron.Pedidos.Services.Redis;
    using Omicron.Pedidos.Services.SapServiceLayerAdapter;
    using Omicron.Pedidos.Services.Utils;
    using Serilog;
    using StackExchange.Redis;

    /// <summary>
    /// CreateChildOrdersSapHandler.
    /// </summary>
    public class CreateChildOrdersSapHandler : IRequestHandler<CreateChildOrdersSapCommand, bool>
    {
        private readonly IPedidosDao pedidosDao;

        private readonly ISapServiceLayerAdapterService serviceLayerAdapterService;

        private readonly ILogger logger;

        private readonly IRedisService redisService;

        private readonly IMediator mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateChildOrdersSapHandler"/> class.
        /// </summary>
        /// <param name="pedidosDao">Pedidos Dao.</param>
        /// <param name="serviceLayerAdapterService">Service Layer Service.</param>
        /// <param name="backgroundTaskQueue">backgroundTaskQueue.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="redisService">redisService.</param>
        /// <param name="orderHistoryHelper">Order History Helper.</param>
        /// <param name="mediator">Mediator.</param>
        public CreateChildOrdersSapHandler(
            IPedidosDao pedidosDao,
            ISapServiceLayerAdapterService serviceLayerAdapterService,
            ILogger logger,
            IRedisService redisService,
            IMediator mediator)
        {
            this.pedidosDao = pedidosDao.ThrowIfNull(nameof(pedidosDao));
            this.serviceLayerAdapterService = serviceLayerAdapterService.ThrowIfNull(nameof(serviceLayerAdapterService));
            this.logger = logger.ThrowIfNull(nameof(logger));
            this.redisService = redisService.ThrowIfNull(nameof(redisService));
            this.mediator = mediator;
        }

        /// <inheritdoc/>
        public async Task<bool> Handle(CreateChildOrdersSapCommand request, CancellationToken cancellationToken)
        {
            this.logger.Information(LogsConstants.CreateChildOrdersSapLogBase, request.ProductionOrderId);
            var logBase = string.Format(LogsConstants.CreateChildOrdersSapLogBase, request.ProductionOrderId);
            var (isSucces, error, childOrderId) = await this.CreateChild(request);
            Console.WriteLine(isSucces);

            if (!isSucces)
            {
                Console.WriteLine("ENTRO A LA EXCEPCION");
                var productionOrderSeparationLog = await this.pedidosDao.GetProductionOrderSeparationDetailLogById(request.SeparationId);

                if (productionOrderSeparationLog != null)
                {
                    productionOrderSeparationLog.ErrorMessage = error;
                    productionOrderSeparationLog.LastStep = ServiceConstants.StepSuccessfullyCloseInSapStep;
                    productionOrderSeparationLog.LastUpdated = DateTime.Now;

                    await this.pedidosDao.UpdateProductionOrderSeparationDetailLog(productionOrderSeparationLog);
                }

                var processWithError = new ProductionOrderSeparationDetailLogsModel
                {
                    Id = request.SeparationId,
                    ParentProductionOrderId = request.ProductionOrderId,
                    LastStep = ServiceConstants.StepSuccessfullyCloseInSapStep,
                    IsSuccessful = false,
                    ErrorMessage = error,
                    ChildProductionOrderId = null,
                    Payload = JsonConvert.SerializeObject(request),
                    CreatedAt = DateTime.Now,
                    LastUpdated = DateTime.Now,
                };

                await this.pedidosDao.InsertProductionOrderSeparationDetailLogById(processWithError);

                var redisKey = string.Format(ServiceConstants.ProductionOrderSeparationProcessKey, request.ProductionOrderId);
                await this.redisService.DeleteKey(redisKey);

                return false;
            }

            return true;
        }

        private async Task<(bool, string, int)> CreateChild(CreateChildOrdersSapCommand request)
        {
            try
            {
                var productionOrder =
                    (await this.pedidosDao.GetUserOrderByProducionOrder([request.ProductionOrderId.ToString()]))
                    .FirstOrDefault() ?? throw new Exception(LogsConstants.ProductionOrderNotFound);
                var childOrderId = await this.CreateChildOrdersProcess(productionOrder, request.ProductionOrderId, request.Pieces, request.SeparationId);

                return (true, string.Empty, childOrderId);
            }
            catch (Exception ex)
            {
                var error = string.Format(LogsConstants.SeparateProductionOrderEndWithError);
                this.logger.Error(ex, error);
                return (false, ex.Message, 0);
            }
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
            return newFoId;
        }
    }
}
