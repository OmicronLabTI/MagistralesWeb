// <summary>
// <copyright file="InvoiceDao.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Persistence.DAO.Invoice.Impl
{
    /// <summary>
    /// Class UsersDao.
    /// </summary>
    public class InvoiceDao : IInvoiceDao
    {
        private readonly DatabaseContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceDao"/> class.
        /// </summary>
        /// <param name="context">DataBase Context.</param>
        public InvoiceDao(DatabaseContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            this.context = context;
        }

        /// <inheritdoc/>
        public async Task InsertInvoices(List<InvoiceModel> invoices)
        {
            this.context.Invoices.AddRange(invoices);
            await ((DatabaseContext)this.context).SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task<InvoiceModel> GetInvoiceById(string id)
        {
            return await this.context.Invoice.Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        /// <inheritdoc/>
        public async Task<InvoiceModel> GetInvoiceModelById(string id)
        {
            return await this.context.Invoice.FirstOrDefaultAsync(x => x.Id == id);
        }

        /// <inheritdoc/>
        public async Task UpdateInvoiceAsync(InvoiceModel invoiceModel)
        {
            this.context.Invoice.Update(invoiceModel);
            await ((DatabaseContext)this.context).SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<InvoiceModel>> GetInvoicesForRetryProcessAsync(string status)
        {
            return await (from fac in this.context.Invoice
                          join err in this.context.InvoiceError on fac.IdInvoiceError equals err.Id
                          where fac.IsProcessing == false
                                && fac.Status == status
                                && (
                                      (err.RequireManualChange == true
                                          && (fac.ManualChangeApplied == true || fac.ManualChangeApplied == null))
                                   || err.RequireManualChange == false)
                          select fac)
                   .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task UpdateInvoices(List<InvoiceModel> invoices)
        {
            this.context.Invoices.UpdateRange(invoices);
            await ((DatabaseContext)this.context).SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<InvoiceErrorModel>> GetAllErrors()
        {
            return await this.context.InvoiceError.ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<InvoiceRemissionJoinModel>> GetInvoicesByRemissionId(List<long> remissionId)
        {
            return await (from invoice in this.context.Invoice
                          join remission in this.context.Remissions on invoice.Id equals remission.IdInvoice
                          where remissionId.Contains(remission.RemissionId)
                          select new InvoiceRemissionJoinModel
                          {
                              RemissionId = remission.RemissionId,
                              Status = invoice.Status,
                              InvoiceId = invoice.IdFacturaSap,
                              ProcessId = invoice.Id,
                          }).ToListAsync();
        }
    }
}
