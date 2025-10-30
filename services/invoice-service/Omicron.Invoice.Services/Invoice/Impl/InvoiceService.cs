// <summary>
// <copyright file="InvoiceService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Services.Invoice.Impl
{
    /// <summary>
    /// InvoiceService class.
    /// </summary>
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceDao invoiceDao;
        private readonly IMapper mapper;
        private readonly IMediator mediator;
        private readonly IBackgroundTaskQueue taskQueue;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly Serilog.ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceService"/> class.
        /// </summary>
        /// <param name="mapper">Mapper.</param>
        /// <param name="invoiceDao">Users dao.</param>
        /// <param name="mediator">mediator.</param>
        /// <param name="taskQueue">task queue.</param>
        /// <param name="serviceScopeFactory">serviceScopeFactory.</param>
        /// <param name="logger">logger.</param>
        public InvoiceService(
            IMapper mapper,
            IInvoiceDao invoiceDao,
            IMediator mediator,
            IBackgroundTaskQueue taskQueue,
            IServiceScopeFactory serviceScopeFactory,
            Serilog.ILogger logger)
        {
            this.mapper = mapper;
            this.invoiceDao = invoiceDao;
            this.mediator = mediator.ThrowIfNull(nameof(mediator));
            this.taskQueue = taskQueue.ThrowIfNull(nameof(taskQueue));
            this.serviceScopeFactory = serviceScopeFactory.ThrowIfNull(nameof(serviceScopeFactory));
            this.logger = logger.ThrowIfNull(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> CreateInvoice(CreateInvoiceDto request)
        {
            // primero validar que no se encuentre procesandose
            // crear
            // cambiar el estatus a is processing en true, cambiar estatus a procesandose, y otros campos
            // actualizar cambios
            // enviar a procesar a mediatr
            var result = this.PublishProcessToMediatR(request);
            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, result, null, null);
        }

        private bool PublishProcessToMediatR(CreateInvoiceDto request)
        {
            try
            {
                this.taskQueue.QueueBackgroundWorkItem(async token =>
                {
                    using var scope = this.serviceScopeFactory.CreateScope();
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    await mediator.Publish(
                    new CreateiInvoiceNotification
                    {
                        RequestBody = request,
                    }, token);
                });

                return true;
            }
            catch (Exception ex)
            {
                this.logger.Error(string.Format(LogsConstants.PublishProcessInvoiceToMediatrQueueError, request.ProcessId, ex.Message));
                return false;
            }
        }
    }
}
