// <summary>
// <copyright file="CatalogsInvoiceService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Services.CatalogsInvoice.Impl
{
    /// <summary>
    /// Class CatalogsInvoiceService.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="CatalogsInvoiceService"/> class.
    /// </remarks>
    /// <param name="logger">Logger.</param>
    /// <param name="redisService">Redis Service.</param>
    /// <param name="invoiceService">Invoice Service.</param>
    /// <param name="invoiceDao">Invoice dao.</param>
    /// <param name="configuration">configuration.</param>
    /// <param name="azureService">azureService.</param>
    public class CatalogsInvoiceService(IConfiguration configuration, Serilog.ILogger logger, IRedisService redisService, IInvoiceService invoiceService, IInvoiceDao invoiceDao, IAzureService azureService)
        : ICatalogsInvoiceService
    {
        private readonly IAzureService azureService = azureService.ThrowIfNull(nameof(azureService));
        private readonly Serilog.ILogger logger = logger.ThrowIfNull(nameof(logger));
        private readonly IRedisService redisService = redisService.ThrowIfNull(nameof(redisService));
        private readonly IInvoiceService invoiceService = invoiceService.ThrowIfNull(nameof(invoiceService));
        private readonly IInvoiceDao invoiceDao = invoiceDao.ThrowIfNull(nameof(invoiceDao));
        private readonly IConfiguration configuration = configuration.ThrowIfNull(nameof(configuration));

        /// <inheritdoc/>
        public async Task<ResultDto> InvoiceErrorsFromExcel()
        {
            try
            {
                var invoiceErrors = await this.GetInvoiceErrorsFromExcel();

                invoiceErrors = invoiceErrors
                    .GroupBy(p => p.Code)
                    .Select(g => g.First())
                    .ToList();

                var codes = invoiceErrors.Select(x => x.Code).ToList();
                var existingRecords = await this.invoiceDao.GetExistingErrorsByCodes(codes);
                var existingCodes = existingRecords.Select(x => x.Code).ToList();

                var recordsToUpdate = invoiceErrors.Where(x => existingCodes.Contains(x.Code)).ToList();
                var recordsToInsert = invoiceErrors.Where(x => !existingCodes.Contains(x.Code)).ToList();

                await this.UpdateExistingRecords(recordsToUpdate, existingRecords);

                if (recordsToInsert.Any())
                {
                    await this.invoiceDao.InsertInvoiceErrors(recordsToInsert);
                }

                await this.redisService.WriteToRedis(
                    ServiceConstants.InvoiceErrorsCatalogs,
                    JsonConvert.SerializeObject(invoiceErrors),
                    TimeSpan.FromHours(12));

                return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, null, null, null);
            }
            catch (Exception ex)
            {
                this.logger.Error(ex, ServiceConstants.LogsInvoiceErrorsCatalogs);
                return ServiceUtils.CreateResult(
                    false,
                    (int)HttpStatusCode.InternalServerError,
                    ServiceConstants.InternalServerError,
                    null,
                    null,
                    null);
            }
        }

        private async Task UpdateExistingRecords(
        List<InvoiceErrorModel> recordsToUpdate,
        List<InvoiceErrorModel> existingRecords)
        {
            if (!recordsToUpdate.Any())
            {
                return;
            }

            var existingDict = existingRecords.ToDictionary(x => x.Code);

            recordsToUpdate
                .Where(record => existingDict.ContainsKey(record.Code))
                .ToList()
                .ForEach(record =>
                {
                    existingDict[record.Code].ErrorMessage = record.ErrorMessage;
                    existingDict[record.Code].RequireManualChange = record.RequireManualChange;
                });

            await this.invoiceDao.UpdateInvoiceErrors(existingRecords);
        }

        private async Task<List<InvoiceErrorModel>> GetInvoiceErrorsFromExcel()
        {
            var table = await this.ObtainDataFromExcel(ServiceConstants.InvoiceErrorsCatalogsFileUrl, 1);
            var columns = table.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();

            var code = columns[0];
            var error = columns[1];
            var errorMessage = columns[2];
            var requireManualChange = columns[3];

            var invoiceErrors = table.AsEnumerable()
                .Select(row => new InvoiceErrorModel
                {
                    Code = row[code].ToString()?.Trim(),
                    Error = row[error].ToString()?.Trim(),
                    ErrorMessage = row[errorMessage].ToString()?.Trim(),
                    RequireManualChange = row[requireManualChange].ToString()?.Trim().ToUpper() == "SI",
                })
                .Where(x => !string.IsNullOrWhiteSpace(x.Code) &&
                            !string.IsNullOrWhiteSpace(x.Error) &&
                            !string.IsNullOrWhiteSpace(x.ErrorMessage))
                .ToList();

            return invoiceErrors;
        }

        private async Task<DataTable> ObtainDataFromExcel(string url, int sheet)
        {
            var key = this.configuration[ServiceConstants.AzureAccountKey];
            var account = this.configuration[ServiceConstants.AzureAccountName];
            var file = this.configuration[url];

            using var streamWoorkbook = new MemoryStream();

            await this.azureService.GetElementsFromAzure(account, key, file, streamWoorkbook);
            using var workbook = new XLWorkbook(streamWoorkbook);

            DataTable table = ServiceUtils.ReadSheet(workbook, sheet);

            return table;
        }
    }
}
