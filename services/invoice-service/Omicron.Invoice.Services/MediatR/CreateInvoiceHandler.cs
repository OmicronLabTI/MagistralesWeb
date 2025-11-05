// <copyright file="CreateInvoiceHandler.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>

namespace Omicron.Invoice.Services.MediatR
{
    /// <summary>
    /// CreateInvoiceHandler.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="CreateInvoiceHandler"/> class.
    /// </remarks>
    /// <param name="loggerSeriLog">Logger.</param>
    /// <param name="invoiceService">Invoice Service Interface.</param>
    public class CreateInvoiceHandler(Serilog.ILogger loggerSeriLog, IInvoiceService invoiceService) : INotificationHandler<CreateiInvoiceNotification>
    {
        private readonly Serilog.ILogger loggerSeriLog = loggerSeriLog;
        private readonly IInvoiceService invoiceService = invoiceService;

        /// <inheritdoc/>
        public async Task Handle(CreateiInvoiceNotification notification, CancellationToken cancellationToken)
        {
            try
            {
                this.loggerSeriLog.Information(
                    string.Format(LogsConstants.CreateInvoiceLogPushMediatRStart, notification.RequestBody.ProcessId, JsonConvert.SerializeObject(notification.RequestBody)));
                var requestOrderDto = notification.RequestBody;
                await this.invoiceService.CreateInvoice(requestOrderDto);
                this.loggerSeriLog.Information(string.Format(LogsConstants.CreateInvoiceLogPushMediatRFinish, notification.RequestBody.ProcessId));
            }
            catch (Exception ex)
            {
                this.loggerSeriLog.Error(
                    string.Format(
                        LogsConstants.CreateInvoiceLogPushMediatRError,
                        notification.RequestBody.ProcessId,
                        JsonConvert.SerializeObject(notification),
                        ex.Message));
            }
        }
    }
}
