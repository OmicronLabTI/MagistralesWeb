// <summary>
// <copyright file="InvoiceService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

using Microsoft.Extensions.Azure;

namespace Omicron.Invoice.Services.Invoice.Impl
{
    /// <summary>
    /// InvoiceService class.
    /// </summary>
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceDao invoiceDao;
        private readonly IBackgroundTaskQueue taskQueue;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly Serilog.ILogger logger;
        private readonly ISapServiceLayerAdapterService serviceLayerService;
        private readonly ISapAdapter sapAdapter;
        private readonly IUsersService usersService;
        private ICatalogsService catalogsService;

        private IRedisService redisService;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceService"/> class.
        /// </summary>
        /// <param name="invoiceDao">Users dao.</param>
        /// <param name="taskQueue">task queue.</param>
        /// <param name="serviceScopeFactory">serviceScopeFactory.</param>
        /// <param name="logger">logger.</param>
        /// <param name="sapAdapter">sap adapter.</param>
        /// <param name="serviceLayerService">the serviceLayerService.</param>
        /// <param name="catalogsService">the catalog.</param>
        /// <param name="redisService">the redis.</param>
        /// <param name="usersService">the usersService.</param>
        public InvoiceService(
            IInvoiceDao invoiceDao,
            IBackgroundTaskQueue taskQueue,
            IServiceScopeFactory serviceScopeFactory,
            Serilog.ILogger logger,
            ISapAdapter sapAdapter,
            ISapServiceLayerAdapterService serviceLayerService,
            ICatalogsService catalogsService,
            IRedisService redisService,
            IUsersService usersService)
        {
            this.invoiceDao = invoiceDao;
            this.taskQueue = taskQueue.ThrowIfNull(nameof(taskQueue));
            this.serviceScopeFactory = serviceScopeFactory.ThrowIfNull(nameof(serviceScopeFactory));
            this.logger = logger.ThrowIfNull(nameof(logger));
            this.sapAdapter = sapAdapter.ThrowIfNull(nameof(sapAdapter));
            this.serviceLayerService = serviceLayerService.ThrowIfNull(nameof(serviceLayerService));
            this.catalogsService = catalogsService.ThrowIfNull(nameof(catalogsService));
            this.redisService = redisService.ThrowIfNull(nameof(redisService));
            this.usersService = usersService.ThrowIfNull(nameof(usersService));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> RegisterInvoice(CreateInvoiceDto request)
        {
            var remissions = request.IdDeliveries.Distinct().Select(x => new InvoiceRemissionModel()
            {
                RemissionId = x,
                IdInvoice = request.ProcessId,
            }).ToList();

            var sapOrders = request.IdSapOrders.Distinct().Select(x => new InvoiceSapOrderModel()
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
                Payload = JsonConvert.SerializeObject(request),
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
                if (!slResponse.Success)
                {
                    var dataError = slResponse.UserError ?? slResponse.Code.ToString();
                    throw new CustomServiceException(dataError);
                }

                var invoiceId = JsonConvert.DeserializeObject<int>(slResponse.Response.ToString());
                await this.UpdateSuccessResult(invoice, invoiceId);
                return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, null, null, null);
            }
            catch (Exception ex)
            {
                invoice.IsProcessing = false;
                invoice.Status = ServiceConstants.InvoiceCreationErrorStatus;
                if (invoice.ErrorMessage != null)
                {
                    invoice.RetryNumber = invoice.RetryNumber++;
                    invoice.UpdateDate = DateTime.Now;
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
        public async Task<ResultDto> GetInvoices(Dictionary<string, string> parameters)
        {
            var (listInvoices, total) = await this.GetInvoiceModels(parameters);

            var usersids = listInvoices.Select(x => x.AlmacenUser).Distinct().ToList();
            var listUsers = await this.GetUsersById(usersids);

            var listToReturn = CreateListInvoicesToReturn(listInvoices, listUsers);

            return ServiceUtils.CreateResult(true, 200, null, listToReturn, null, total);
        }

        /// <inheritdoc/>
        public async Task<ResultDto> UpdateManualChange(UpdateManualChangeDto id)
        {
            var invoice = (await this.invoiceDao.GetInvoicesById([id.Id])).FirstOrDefault();

            if (invoice == null || invoice.ManualChangeApplied != false)
            {
                return ServiceUtils.CreateResult(false, (int)HttpStatusCode.BadRequest, ServiceConstants.ErrorUpdateInvoice, null, null, null);
            }

            invoice.ManualChangeApplied = true;
            await this.invoiceDao.SaveChangesAsync();

            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, null, null, null);
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

        /// <inheritdoc/>
        public async Task<ResultDto> GetInvoicesByRemissionId(List<int> remissions)
        {
            var invoices = await this.invoiceDao.GetInvoicesByRemissionId(remissions.Select(x => (long)x).ToList());
            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, invoices, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultDto> GetAutoBillingAsync(Dictionary<string, string> parameters)
        {
            var offset = int.Parse(ServiceUtils.GetDictionaryValueString(parameters, ServiceConstants.Offset, ServiceConstants.OffsetDefault));
            var limit = int.Parse(ServiceUtils.GetDictionaryValueString(parameters, ServiceConstants.Limit, ServiceConstants.Limit));
            var status = ServiceUtils.SplitStringList(ServiceUtils.GetDictionaryValueString(parameters, ServiceConstants.Status, ServiceConstants.SuccessfulInvoiceCreationStatus));
            var now = DateTime.Now;
            var startDateStr = ServiceUtils.GetDictionaryValueString(parameters, "startDate", null);
            var endDateStr = ServiceUtils.GetDictionaryValueString(parameters, "endDate", null);
            var startDate = string.IsNullOrWhiteSpace(startDateStr)
                ? now.Date.AddDays(-4)
                : DateTime.Parse(startDateStr);

            var endDate = string.IsNullOrWhiteSpace(endDateStr)
                ? now.Date.AddDays(1).AddTicks(-1)
                : DateTime.Parse(endDateStr);
            var invoices = await this.invoiceDao.GetAutoBillingBaseAsync(status, offset, limit);
            var total = await this.invoiceDao.GetAutoBillingCountAsync(status);
            var userIds = invoices
                .Select(x => x.AlmacenUser)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToList();

            var listUsers = await this.GetUsersById(userIds);
            var userMap = listUsers.ToDictionary(u => u.Id, u => u);
            var invoiceIds = invoices.Select(x => x.Id).ToList();
            var sapByInvoice = await this.invoiceDao.GetSapOrdersByInvoiceIdsAsync(invoiceIds);
            var remByInvoice = await this.invoiceDao.GetRemissionsByInvoiceIdsAsync(invoiceIds);
            var catalog = await this.invoiceDao.GetAllErrors();
            var rows = new List<AutoBillingRowDto>();
            foreach (var inv in invoices)
            {
                userMap.TryGetValue(inv.AlmacenUser, out var user);
                var sapOrders = sapByInvoice.ContainsKey(inv.Id) ? sapByInvoice[inv.Id] : new List<InvoiceSapOrderModel>();
                var remissions = remByInvoice.ContainsKey(inv.Id) ? remByInvoice[inv.Id] : new List<InvoiceRemissionModel>();
                string lastErrorMessage;
                if (string.IsNullOrEmpty(inv.ErrorMessage))
                {
                    lastErrorMessage = "NO APLICA";
                }
                else
                {
                    var match = catalog.FirstOrDefault(e => inv.ErrorMessage.Contains(e.Code));
                    lastErrorMessage = match != null ? match.ErrorMessage : inv.ErrorMessage;
                }

                rows.Add(new AutoBillingRowDto
                {
                    Id = inv.Id,
                    IdFacturaSap = inv.IdFacturaSap?.ToString(),
                    InvoiceCreateDate = inv.InvoiceCreateDate?.ToString("dd/MM/yy HH:mm:ss"),
                    TypeInvoice = inv.TypeInvoice,
                    BillingType = inv.BillingType,
                    AlmacenUser = user == null ? string.Empty : $"{user.FirstName} {user.LastName}",
                    DxpOrderId = inv.DxpOrderId,
                    ShopTransaction = "T001", // Temporal
                    SapOrdersCount = sapOrders.Count,
                    RemissionsCount = remissions.Count,
                    RetryNumber = inv.RetryNumber,
                    SapOrders = sapOrders,
                    Remissions = remissions,
                    LastErrorMessage = lastErrorMessage,
                    LastUpdateDate = inv.UpdateDate?.ToString("dd/MM/yy HH:mm:ss"),
                });
            }

            rows = rows.OrderBy(r => r.Id).ToList();
            return ServiceUtils.CreateResult(
                success: true,
                code: 200,
                userError: null,
                responseObj: rows,
                exceptionMessage: null,
                comments: new
                {
                    total,
                    startDate = startDate,
                    endDate = endDate,
                });
        }

        private static List<InvoiceErrorDto> CreateListInvoicesToReturn(List<InvoiceModel> listInvoices, List<UserModel> users)
        {
            var userDictionary = users.ToDictionary(u => u.Id, u => u);
            var listToReturn = new List<InvoiceErrorDto>();

            listInvoices.ForEach(x =>
            {
                userDictionary.TryGetValue(x.AlmacenUser, out var user);

                var invoice = new InvoiceErrorDto
                {
                    Id = x.Id,
                    CreateDate = x.CreateDate.ToString("dd/MM/yyyy HH:mm:ss"),
                    AlmacenUser = user == null ? string.Empty : $"{user.FirstName} {user.LastName}",
                    Status = x.Status,
                    DxpOrderId = x.DxpOrderId,
                    TypeInvoice = x.TypeInvoice,
                    BillingType = x.BillingType,
                    ErrorMessage = GetErrorMessageForInvoice(x.InvoiceError, x),
                    UpdateDate = x.UpdateDate?.ToString("dd/MM/yyyy HH:mm:ss"),
                    RetryNumber = x.RetryNumber,
                    ManualChangeApplied = x.ManualChangeApplied,
                    IsProcessing = x.IsProcessing,
                    RemissionId = x.Remissions == null ? new List<int>() : x.Remissions.Select(x => x.RemissionId).Order().Distinct().ToList(),
                    SapOrderId = x.SapOrders == null ? new List<int>() : x.SapOrders.Select(x => x.SapOrderId).Order().Distinct().ToList(),
                };
                listToReturn.Add(invoice);
            });

            return listToReturn.OrderBy(i => ServiceConstants.StatusOrder[i.Status]).ToList();
        }

        private static string GetErrorMessageForInvoice(InvoiceErrorModel error, InvoiceModel invoice)
        {
            return error?.ErrorMessage
                ?? error?.Error
                ?? invoice.ErrorMessage
                ?? null;
        }

        private static bool ContainsExact(string source, string search)
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

        private async Task<List<UserModel>> GetUsersById(List<string> userIds)
        {
            var users = await this.usersService.GetUsersById(userIds, ServiceConstants.GetUsersById);
            return JsonConvert.DeserializeObject<List<UserModel>>(users.Response.ToString());
        }

        private async Task<(InvoiceErrorModel, bool)> GetDataBaseInvoiceError(string exceptionMessage)
        {
            var errors = await this.invoiceDao.GetAllErrors();
            var selectedError = errors.Where(x => ContainsExact(exceptionMessage, x.Code)).FirstOrDefault();
            return (selectedError ?? new InvoiceErrorModel() { ErrorMessage = exceptionMessage }, selectedError != default);
        }

        private async Task UpdateSuccessResult(InvoiceModel invoice, int invoiceId)
        {
            invoice.InvoiceCreateDate = DateTime.Now;
            invoice.IsProcessing = false;
            invoice.IdFacturaSap = invoiceId;
            invoice.Status = ServiceConstants.SuccessfulInvoiceCreationStatus;
            if (invoice.ErrorMessage != null)
            {
                invoice.UpdateDate = DateTime.Now;
            }

            await this.invoiceDao.UpdateInvoices(new List<InvoiceModel> { invoice });
        }

        private async Task<InvoicesDataDto> ValidateHasInvoice(List<int> remissions)
        {
            var result = await this.sapAdapter.PostSapAdapter(remissions, ServiceConstants.ValidateInvoiceUrl);
            return JsonConvert.DeserializeObject<InvoicesDataDto>(result.Response.ToString());
        }

        private async Task<(List<InvoiceModel>, int)> GetInvoiceModels(Dictionary<string, string> parameters)
        {
            List<InvoiceModel> listInvoices;
            int total = 0;
            var typeId = ServiceUtils.GetDictionaryValueString(parameters, ServiceConstants.IdType, null);
            var offset = int.Parse(ServiceUtils.GetDictionaryValueString(parameters, ServiceConstants.Offset, ServiceConstants.OffsetDefault));
            var limit = int.Parse(ServiceUtils.GetDictionaryValueString(parameters, ServiceConstants.Limit, ServiceConstants.Limit));

            if (typeId == null)
            {
                var status = ServiceUtils.SplitStringList(ServiceUtils.GetDictionaryValueString(parameters, ServiceConstants.Status, ServiceConstants.InvoiceCreationErrorStatus));

                var typeInvoices = ServiceUtils.SplitStringList(ServiceUtils.GetDictionaryValueString(parameters, ServiceConstants.TypeInvoice, string.Empty));
                var billingTypes = ServiceUtils.SplitStringList(ServiceUtils.GetDictionaryValueString(parameters, ServiceConstants.BillingType, string.Empty));

                listInvoices = await this.invoiceDao.GetInvoicesNotCreatedByStatus(status, typeInvoices, billingTypes, offset, limit);
                total = await this.invoiceDao.GetInvoicesCount(status, typeInvoices, billingTypes);

                return (listInvoices, total);
            }

            var id = ServiceUtils.GetDictionaryValueString(parameters, ServiceConstants.Id, string.Empty);

            listInvoices = typeId switch
            {
                ServiceConstants.IdPedidoSapType => await this.invoiceDao.GetInvoicesByPedidoSap(ServiceUtils.SplitIntList(id), offset, limit),

                ServiceConstants.IdPedidoDxpType => await this.invoiceDao.GetInvoicesByPedidoDxp(id, offset, limit),

                ServiceConstants.IdInvoiceType => await this.invoiceDao.GetInvoicesByInvoiceId(id, offset, limit),

                _ => new List<InvoiceModel>()
            };

            listInvoices = listInvoices.Where(x => x.Status != ServiceConstants.SuccessfulInvoiceCreationStatus).ToList();
            total = listInvoices.Count;
            return (listInvoices, total);
        }
    }
}
