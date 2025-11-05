// <summary>
// <copyright file="InvoiceRetryService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Services.InvoiceRetry.Impl
{
    /// <summary>
    /// Class InvoiceRetryService.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="InvoiceRetryService"/> class.
    /// </remarks>
    /// <param name="logger">Logger.</param>
    /// <param name="invoiceDao">Invoice Dao.</param>
    /// <param name="redisService">Redis Service.</param>
    /// <param name="invoiceService">Invoice Service.</param>
    public class InvoiceRetryService(Serilog.ILogger logger, IInvoiceDao invoiceDao, IRedisService redisService, IInvoiceService invoiceService)
        : IInvoiceRetryService
    {
        private readonly Serilog.ILogger logger = logger.ThrowIfNull(nameof(logger));
        private readonly IInvoiceDao invoiceDao = invoiceDao.ThrowIfNull(nameof(invoiceDao));
        private readonly IRedisService redisService = redisService.ThrowIfNull(nameof(redisService));
        private readonly IInvoiceService invoiceService = invoiceService.ThrowIfNull(nameof(invoiceService));

        /// <inheritdoc/>
        public async Task<ResultDto> GetDataToRetryCreateInvoicesAsync()
        {
            var processId = Guid.NewGuid().ToString();
            var logBase = LogsConstants.GetDataToRetryCreateInvoicesAsyncLogBase(processId);

            bool locked = await this.redisService.SetKeyIfNotExists(
                ServiceConstants.InvoiceLockAutomaticRetryKey,
                processId,
                TimeSpan.FromMinutes(60));

            if (!locked)
            {
                this.logger.Information(LogsConstants.AutomaticProcessAlreadyRunning(logBase));
                return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, 0, null, null);
            }

            var invoicesToProcess = await this.invoiceDao.GetInvoicesForRetryProcessAsync(ServiceConstants.InvoiceCreationErrorStatus);

            if (invoicesToProcess.Any())
            {
                await this.redisService.StoreListAsync(
                    ServiceConstants.InvoiceToProcessAutomaticRetryKey,
                    invoicesToProcess.OrderBy(x => x.CreateDate).Select(x => x.Id),
                    new TimeSpan(2, 0, 0));
            }

            this.logger.Information(LogsConstants.RetryWillProcessRecords(logBase, invoicesToProcess.Count()));
            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, invoicesToProcess.Count(), null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultDto> RetryCreateInvoicesAsync(InvoiceRetryRequestDto invoiceRetry, string executionType)
        {
            var processId = Guid.NewGuid().ToString();
            var logBase = LogsConstants.RetryCreateInvoicesAsync(processId);
            var idsToProcess = await this.GetIdsToProcess(invoiceRetry);
            if (idsToProcess.Count <= 0)
            {
                this.logger.Information(LogsConstants.ThereIsNoInformationToProcess(logBase));
            }

            var result = new InvoiceRetryResponseDto
            {
                ProcessedIds = new List<string>(),
                SkippedIds = new List<string>(),
            };

            var invoicesToProcess = new List<InvoiceModel>();
            foreach (var id in idsToProcess)
            {
                var lockKey = ServiceConstants.GetRetryInvoiceLockKey(id);
                var lockValue = ServiceConstants.GetRetryInvoiceLockValue(id, executionType);
                await this.ValidateTheRetryDataAndUpdateFoProcess(
                    id,
                    lockKey,
                    lockValue,
                    executionType,
                    invoiceRetry.RequestingUser,
                    result,
                    invoicesToProcess,
                    logBase);
            }

            this.logger.Information(LogsConstants.InvoicesToBeRetried(logBase, JsonConvert.SerializeObject(invoicesToProcess.Select(x => x.Id).ToList())));

            this.RetryInvoiceCreationAsync(invoicesToProcess, logBase);

            if (idsToProcess.Count < invoiceRetry.Limit && executionType.Equals(ServiceConstants.AutomaticExecutionType, StringComparison.CurrentCultureIgnoreCase))
            {
                this.logger.Information(LogsConstants.RedisKeysAreDeletedForRetryControl(logBase));
                await this.redisService.DeleteKey(ServiceConstants.InvoiceToProcessAutomaticRetryKey);
                await this.redisService.DeleteKey(ServiceConstants.InvoiceLockAutomaticRetryKey);
            }

            this.logger.Information(LogsConstants.RetryProcessCompletedSuccessfully(logBase, JsonConvert.SerializeObject(result)));
            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, result, null, null);
        }

        private async Task<List<string>> GetIdsToProcess(InvoiceRetryRequestDto invoiceRetry)
        {
            if (invoiceRetry.InvoiceIds.Count > 0)
            {
                return invoiceRetry.InvoiceIds;
            }

            return await this.redisService.ReadListAsync<string>(ServiceConstants.InvoiceToProcessAutomaticRetryKey, invoiceRetry.Offset, invoiceRetry.Limit);
        }

        private async Task<(List<InvoiceModel>, InvoiceRetryResponseDto)> ValidateTheRetryDataAndUpdateFoProcess(
            string id,
            string lockKey,
            string lockValue,
            string executionType,
            string requestingUser,
            InvoiceRetryResponseDto result,
            List<InvoiceModel> invoicesToProcess,
            string logBase)
        {
            try
            {
                bool locked = await this.redisService.SetKeyIfNotExists(lockKey, lockValue, TimeSpan.FromMinutes(60));

                if (!locked)
                {
                    this.logger.Information(LogsConstants.TheIdIsAlreadyBeingProcessed(logBase, id));
                    result.SkippedIds.Add(id);
                    return (invoicesToProcess, result);
                }

                var invoiceData = await this.invoiceDao.GetInvoiceModelById(id);

                if (invoiceData.IsProcessing || invoiceData.IdFacturaSap.HasValue || invoiceData.Status == ServiceConstants.SuccessfulInvoiceCreationStatus)
                {
                    this.logger.Information(LogsConstants.AlreadyInARetryProcessOrTheInvoiceWasSuccessfullyCreated(
                        logBase,
                        id,
                        invoiceData.IsProcessing,
                        invoiceData.Status,
                        invoiceData.IdFacturaSap.HasValue));

                    result.SkippedIds.Add(id);
                    await this.redisService.DeleteKey(lockKey);
                    return (invoicesToProcess, result);
                }

                invoiceData.Type = executionType;
                invoiceData.AlmacenUser = requestingUser;
                await this.invoiceDao.UpdateInvoiceAsync(invoiceData);
                result.ProcessedIds.Add(id);
                invoicesToProcess.Add(invoiceData);
                return (invoicesToProcess, result);
            }
            catch (Exception ex)
            {
                this.logger.Error(LogsConstants.RetryProcessCompletedWithAnError(logBase, id), ex);
                await this.redisService.DeleteKey(lockKey);
                return (invoicesToProcess, result);
            }
        }

        private void RetryInvoiceCreationAsync(List<InvoiceModel> invoiceData, string logBase)
        {
            foreach (InvoiceModel invoice in invoiceData)
            {
                try
                {
                    this.logger.Information(LogsConstants.RetrySendToCreateInvoice(logBase, invoice.Id, invoice.Payload));
                    this.invoiceService.PublishProcessToMediatR(JsonConvert.DeserializeObject<CreateInvoiceDto>(invoice.Payload));
                }
                catch (Exception ex)
                {
                    this.logger.Error(LogsConstants.RetryErrorSendingToCreateTheInvoice(logBase, invoice.Id), ex);
                }
            }
        }
    }
}
