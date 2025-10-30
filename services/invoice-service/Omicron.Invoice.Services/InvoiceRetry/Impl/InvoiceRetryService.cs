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
    public class InvoiceRetryService(ILogger logger, IInvoiceDao invoiceDao, IRedisService redisService)
        : IInvoiceRetryService
    {
        private readonly ILogger logger = logger.ThrowIfNull(nameof(logger));
        private readonly IInvoiceDao invoiceDao = invoiceDao.ThrowIfNull(nameof(invoiceDao));
        private readonly IRedisService redisService = redisService.ThrowIfNull(nameof(redisService));

        /// <inheritdoc/>
        public async Task<ResultDto> GetDataToRetryCreateInvoicesAsync()
        {
            var lockKey = "invoices:automaticretry";
            var lockValue = Guid.NewGuid().ToString();
            bool locked = await this.redisService.SetKeyIfNotExists(lockKey, lockValue, TimeSpan.FromMinutes(60));
            if (!locked)
            {
                this.logger.Information("El proceso automático ya se encuentra en proceso");
                return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, 0, null, null);
            }




            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, null, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultDto> RetryCreateInvoicesAsync(InvoiceRetryRequestDto invoiceRetry)
        {
            var idsToProcess = await this.GetIdsToProcess(invoiceRetry);
            if (idsToProcess.Count > 0)
            {
                this.logger.Information("No hay información que procesar.");
            }

            var result = new InvoiceRetryResponseDto();
            var invoicesToProcess = new List<InvoiceModel>();
            foreach (var id in idsToProcess)
            {
                var lockKey = $"lock:retryinvoice:{id}";
                var lockValue = $"{id}:{invoiceRetry.ExecutionType}:{Guid.NewGuid()}";
                await this.ValidateTheRetryDataAndUpdateFoProcess(id, lockKey, lockValue, invoiceRetry.ExecutionType, result, invoicesToProcess);
            }

            await this.RetryInvoiceCreationAsync(invoicesToProcess);

            if (idsToProcess.Count < invoiceRetry.Limit)
            {
                await this.redisService.DeleteKey("invoices:automaticretry");
            }

            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, result, null, null);
        }

        private async Task<List<string>> GetIdsToProcess(InvoiceRetryRequestDto invoiceRetry)
        {
            if (invoiceRetry.InvoiceIds.Count > 0)
            {
                return invoiceRetry.InvoiceIds;
            }

            return await this.redisService.ReadListAsync<string>("invoices:automaticretry", invoiceRetry.Offset, invoiceRetry.Limit);
        }

        private async Task<(List<InvoiceModel>, InvoiceRetryResponseDto)> ValidateTheRetryDataAndUpdateFoProcess(
            string id,
            string lockKey,
            string lockValue,
            string executionType,
            InvoiceRetryResponseDto result,
            List<InvoiceModel> invoicesToProcess)
        {
            try
            {
                bool locked = await this.redisService.SetKeyIfNotExists(lockKey, lockValue, TimeSpan.FromMinutes(5));

                if (!locked)
                {
                    result.SkippedIds.Add(id);
                    return (invoicesToProcess, result);
                }

                var invoiceData = await this.invoiceDao.GetInvoiceModelById(id);

                if (invoiceData.IsProcessing || invoiceData.IdFacturaSap.HasValue || invoiceData.Status == "Creación exitosa")
                {
                    result.SkippedIds.Add(id);
                    await this.redisService.DeleteKey(lockKey);
                    return (invoicesToProcess, result);
                }

                invoiceData.IsProcessing = true;
                invoiceData.Type = executionType;
                invoiceData.Status = "Creando factura";
                await this.invoiceDao.UpdateInvoiceAsync(invoiceData);
                result.ProcessedIds.Add(id);
                invoicesToProcess.Add(invoiceData);
                return (invoicesToProcess, result);
            }
            catch (Exception)
            {
                await this.redisService.DeleteKey(lockKey);
                return (invoicesToProcess, result);
            }
        }

        private async Task RetryInvoiceCreationAsync(List<InvoiceModel> invoiceData)
        {
            // Llamar el método de Dani.
        }
    }
}
