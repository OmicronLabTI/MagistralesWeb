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
        private readonly ISapServiceLayerAdapterService serviceLayerService;
        private readonly ISapAdapter sapAdapter;
        private IInvoiceDao usersDao;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceService"/> class.
        /// </summary>
        /// <param name="mapper">Mapper.</param>
        /// <param name="invoiceDao">Users dao.</param>
        /// <param name="mediator">mediator.</param>
        /// <param name="taskQueue">task queue.</param>
        /// <param name="serviceScopeFactory">serviceScopeFactory.</param>
        /// <param name="logger">logger.</param>
        /// <param name="sapAdapter">sap adapter.</param>
        /// <param name="serviceLayerService">the serviceLayerService.</param>
        public InvoiceService(
            IMapper mapper,
            IInvoiceDao invoiceDao,
            IMediator mediator,
            IBackgroundTaskQueue taskQueue,
            IServiceScopeFactory serviceScopeFactory,
            Serilog.ILogger logger,
            ISapAdapter sapAdapter,
            ISapServiceLayerAdapterService serviceLayerService)
        {
            this.mapper = mapper;
            this.invoiceDao = invoiceDao;
            this.mediator = mediator.ThrowIfNull(nameof(mediator));
            this.taskQueue = taskQueue.ThrowIfNull(nameof(taskQueue));
            this.serviceScopeFactory = serviceScopeFactory.ThrowIfNull(nameof(serviceScopeFactory));
            this.logger = logger.ThrowIfNull(nameof(logger));
            this.sapAdapter = sapAdapter.ThrowIfNull(nameof(sapAdapter));
            this.serviceLayerService = serviceLayerService.ThrowIfNull(nameof(serviceLayerService));
        }

        public InvoiceService(IMapper mapper, IInvoiceDao usersDao)
        {
            this.mapper = mapper;
            this.usersDao = usersDao;
        }

        /// <inheritdoc/>
        public async Task<ResultDto> RegisterInvoice(CreateInvoiceDto request)
        {
            var remissions = request.IdDeliveries.Select(x => new InvoiceRemissionModel()
            {
                RemissionId = x,
                IdInvoice = request.ProcessId,
            }).ToList();

            var sapOrders = request.IdSapOrders.Select(x => new InvoiceSapOrderModel()
            {
                SapOrderId = x,
                IdInvoice = request.ProcessId,
            }).ToList();

            var invoice = new InvoiceModel()
            {
                Id = request.ProcessId,
                DxpOrderId = request.DxpOrderId,
                CreateDate = DateTime.Now,
                AlmacenUser = request.CreateUserId,
                Status = ServiceConstants.SendToCreateInvoice,
                TypeInvoice = request.InvoiceType,
                BillingType = request.BillingType,
                RetryNumber = 0,
                Type = "Automático",
                IsProcessing = false,
            };

            invoice.Remissions = request.IdDeliveries.Select(x => new InvoiceRemissionModel()
            {
                RemissionId = x,
                IdInvoice = invoice.Id,
                Invoice = invoice,
            }).ToList();

            invoice.SapOrders = request.IdSapOrders.Select(x => new InvoiceSapOrderModel()
            {
                SapOrderId = x,
                IdInvoice = invoice.Id,
                Invoice = invoice,
            }).ToList();

            await this.invoiceDao.InsertInvoices(new List<InvoiceModel>() { invoice });
            this.PublishProcessToMediatR(request);
            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, null, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultDto> CreateInvoice(CreateInvoiceDto request)
        {
            var invoice = await this.invoiceDao.GetInvoiceById(request.ProcessId);
            if (invoice == default)
            {
                this.logger.Error($"No se encontró la factura con el id {request.ProcessId}");
                return ServiceUtils.CreateResult(false, (int)HttpStatusCode.NotFound, $"No se encontró la factura con el id {request.ProcessId}", null, null, null);
            }

            if (invoice.IsProcessing)
            {
                this.logger.Error($"Ya se encuentra procesandose la factura {request.ProcessId}");
                return ServiceUtils.CreateResult(false, (int)HttpStatusCode.BadRequest, $"Ya se encuentra procesandose la factura {request.ProcessId}", null, null, null);
            }

            invoice.IsProcessing = true;
            invoice.Status = ServiceConstants.CreatingInvoiceStatus;
            await this.invoiceDao.UpdateInvoices(new List<InvoiceModel>() { invoice });

            try
            {
                var hasInvoice = await this.ValidateHasInvoice(request.IdDeliveries);
                if (hasInvoice.HasInvoice)
                {
                    await this.UpdateSuccessResult(invoice, hasInvoice.InvoiceId);
                    return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, null, null, null);
                }

                var slResponse = await this.serviceLayerService.PostAsync(ServiceConstants.SLCreateInvoiceUrl, JsonConvert.SerializeObject(request));
                var invoiceId = JsonConvert.DeserializeObject<int>(slResponse.Response.ToString());
                await this.UpdateSuccessResult(invoice, invoiceId);
                return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, null, null, null);
            }
            catch (Exception ex)
            {
                invoice.IsProcessing = false;
                invoice.Status = ServiceConstants.InvoiceCreationErrorStatus;
                await this.invoiceDao.UpdateInvoices(new List<InvoiceModel>() { invoice });
                return ServiceUtils.CreateResult(false, (int)HttpStatusCode.NotFound, ex.Message, null, ex.Message, null);
            }
        }

        private async Task UpdateSuccessResult(InvoiceModel invoice, int invoiceId)
        {
            invoice.InvoiceCreateDate = DateTime.Now;
            invoice.IsProcessing = false;
            invoice.IdFacturaSap = invoiceId;
            invoice.Status = ServiceConstants.SuccessfulInvoiceCreationStatus;

            // borrar key de redis de hu jose
            await this.invoiceDao.UpdateInvoices(new List<InvoiceModel> { invoice });
        }

        private async Task<InvoicesDataDto> ValidateHasInvoice(List<int> remissions)
        {
            var result = await this.sapAdapter.PostSapAdapter(remissions, ServiceConstants.ValidateInvoiceUrl);
            return JsonConvert.DeserializeObject<InvoicesDataDto>(result.Response.ToString());
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
