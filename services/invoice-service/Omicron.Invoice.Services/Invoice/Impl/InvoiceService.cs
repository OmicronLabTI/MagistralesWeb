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

        private ICatalogsService catalogsService;

        private IRedisService redisService;

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
        /// <param name="catalogsService">the catalog.</param>
        /// <param name="redisService">the redis.</param>
        public InvoiceService(
            IMapper mapper,
            IInvoiceDao invoiceDao,
            IMediator mediator,
            IBackgroundTaskQueue taskQueue,
            IServiceScopeFactory serviceScopeFactory,
            Serilog.ILogger logger,
            ISapAdapter sapAdapter,
            ISapServiceLayerAdapterService serviceLayerService,
            ICatalogsService catalogsService,
            IRedisService redisService)
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
            try
            {
                if (invoice == null)
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

                var hasInvoice = await this.ValidateHasInvoice(request.IdDeliveries);
                if (hasInvoice.HasInvoice)
                {
                    await this.UpdateSuccessResult(invoice, hasInvoice.InvoiceId);
                    return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, null, null, null);
                }

                var cfdiVersion = await ResponseUtils.GetCfdiVersion(this.redisService, this.catalogsService);
                request.CfdiDriverVersion = cfdiVersion;

                var slResponse = await this.serviceLayerService.PostAsync(ServiceConstants.SLCreateInvoiceUrl, JsonConvert.SerializeObject(request));
                var invoiceId = JsonConvert.DeserializeObject<int>(slResponse.Response.ToString());
                await this.UpdateSuccessResult(invoice, invoiceId);
                return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, null, null, null);
            }
            catch (Exception ex)
            {
                invoice.IsProcessing = false;
                invoice.Status = ServiceConstants.InvoiceCreationErrorStatus;
                invoice.UpdateDate = DateTime.Now;
                if (invoice.ErrorMessage != null)
                {
                    invoice.RetryNumber = invoice.RetryNumber++;
                }

                var error = await this.GetDataBaseInvoiceError(ex.Message);
                invoice.ErrorMessage = error.Item1.ErrorMessage;
                if (error.Item2)
                {
                    invoice.ManualChangeApplied = error.Item1.RequireManualChange ? false : null;
                    invoice.IdInvoiceError = error.Item1.Id;
                }

                await this.invoiceDao.UpdateInvoices(new List<InvoiceModel>() { invoice });
                return ServiceUtils.CreateResult(false, (int)HttpStatusCode.NotFound, ex.Message, null, ex.Message, null);
            }
            finally
            {
                   await this.redisService.DeleteKey(ServiceConstants.GetRetryInvoiceLockKey(request.ProcessId));
            }
        }

        /// <inheritdoc/>
        public bool PublishProcessToMediatR(CreateInvoiceDto request)
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

        private async Task<(InvoiceErrorModel, bool)> GetDataBaseInvoiceError(string exceptionMessage)
        {
            var errors = await this.invoiceDao.GetAllErrors();
            var selectedError = errors.Where(x => this.ContainsExact(exceptionMessage, x.Code)).FirstOrDefault();
            return (selectedError ?? new InvoiceErrorModel() { ErrorMessage = exceptionMessage }, selectedError != default);
        }

        private bool ContainsExact(string source, string search)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(search))
            {
                return false;
            }

            var pattern = $@"\b{System.Text.RegularExpressions.Regex.Escape(search)}\b";
            return System.Text.RegularExpressions.Regex.IsMatch(
                source,
                pattern,
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }

        private async Task UpdateSuccessResult(InvoiceModel invoice, int invoiceId)
        {
            invoice.InvoiceCreateDate = DateTime.Now;
            invoice.IsProcessing = false;
            invoice.IdFacturaSap = invoiceId;
            invoice.Status = ServiceConstants.SuccessfulInvoiceCreationStatus;

            await this.invoiceDao.UpdateInvoices(new List<InvoiceModel> { invoice });
            await this.redisService.DeleteKey(ServiceConstants.GetRetryInvoiceLockKey(invoice.Id));
        }

        private async Task<InvoicesDataDto> ValidateHasInvoice(List<int> remissions)
        {
            var result = await this.sapAdapter.PostSapAdapter(remissions, ServiceConstants.ValidateInvoiceUrl);
            return JsonConvert.DeserializeObject<InvoicesDataDto>(result.Response.ToString());
        }
    }
}
