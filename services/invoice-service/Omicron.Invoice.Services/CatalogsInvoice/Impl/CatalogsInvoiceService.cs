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
    public class CatalogsInvoiceService(Serilog.ILogger logger, IRedisService redisService, IInvoiceService invoiceService)
        : ICatalogsInvoiceService
    {
        private readonly Serilog.ILogger logger = logger.ThrowIfNull(nameof(logger));
        private readonly IRedisService redisService = redisService.ThrowIfNull(nameof(redisService));
        private readonly IInvoiceService invoiceService = invoiceService.ThrowIfNull(nameof(invoiceService));

        /// <inheritdoc/>
        public Task<ResultDto> InvoiceErrorsFromExcel()
        {
            return Task.FromResult(ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, null, null, null));
        }
    }
}
