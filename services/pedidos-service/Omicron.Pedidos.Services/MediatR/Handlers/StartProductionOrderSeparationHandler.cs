// <summary>
// <copyright file="StartProductionOrderSeparationHandler.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.MediatR.Handlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using global::MediatR;
    using Microsoft.Extensions.DependencyInjection;
    using Omicron.Pedidos.Services.Constants;
    using Omicron.Pedidos.Services.MediatR.Commands;
    using Omicron.Pedidos.Services.MediatR.Services;
    using Omicron.Pedidos.Services.Utils;
    using Serilog;

    /// <summary>
    /// StartProductionOrderSeparationHandler.
    /// </summary>
    public class StartProductionOrderSeparationHandler : IRequestHandler<StartProductionOrderSeparationCommand>
    {
        private readonly IBackgroundTaskQueue backgroundTaskQueue;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="StartProductionOrderSeparationHandler"/> class.
        /// </summary>
        /// <param name="backgroundTaskQueue">backgroundTaskQueue.</param>
        /// <param name="logger">logger.</param>
        public StartProductionOrderSeparationHandler(
            IBackgroundTaskQueue backgroundTaskQueue,
            ILogger logger)
        {
            this.backgroundTaskQueue = backgroundTaskQueue;
            this.logger = logger.ThrowIfNull(nameof(logger));
        }

        /// <inheritdoc/>
        public Task Handle(StartProductionOrderSeparationCommand request, CancellationToken cancellationToken)
        {
            this.logger.Information(LogsConstants.StartingBackgroundProductionOrderSplit, request.SeparationId, request.ProductionOrderId);
            this.backgroundTaskQueue.QueueBackgroundWorkItem(async (services, token) =>
            {
                var mediator = services.GetRequiredService<IMediator>();
                var updateCommand = new SeparateProductionOrderCommand(
                    request.ProductionOrderId,
                    request.Pieces,
                    request.SeparationId,
                    request.UserId,
                    request.DxpOrder,
                    request.SapOrder,
                    request.TotalPieces);
                await mediator.Send(updateCommand, token);
            });

            return Task.FromResult(Unit.Value);
        }
    }
}
