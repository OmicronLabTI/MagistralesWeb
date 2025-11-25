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
        public async Task<List<InvoiceModel>> GetInvoicesNotCreatedByStatus(
            List<string> status,
            List<string> typeInvoices,
            List<string> billingTypes,
            int offset,
            int limit)
        {
            var filter = this.BuildInvoiceFilter(status, typeInvoices, billingTypes);

            return await filter
                .OrderBy(x => x.CreateDate)
                .Skip(offset)
                .Take(limit)
                .Include(x => x.Remissions)
                .Include(x => x.SapOrders)
                .Include(x => x.InvoiceError)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<int> GetInvoicesCount(
            List<string> status,
            List<string> typeInvoices,
            List<string> billingTypes)
        {
            var filter = this.BuildInvoiceFilter(status, typeInvoices, billingTypes);
            return await filter.CountAsync();
        }

        /// <inheritdoc/>
        public async Task<List<InvoiceModel>> GetInvoicesById(List<string> id)
        {
            return await this.context.Invoice
                .Where(x => id.Contains(x.Id))
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<int> SaveChangesAsync() => await this.context.SaveChangesAsync();

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
            return await (
                    from fac in this.context.Invoice
                    join err in this.context.InvoiceError
                        on fac.IdInvoiceError equals err.Id into errJoin
                    from err in errJoin.DefaultIfEmpty()
                    where !fac.IsProcessing
                          && fac.Status == status
                          && (
                                err == null ||
                                (!err.RequireManualChange) ||
                                (err.RequireManualChange
                                 && (fac.ManualChangeApplied == true
                                     || fac.ManualChangeApplied == null)))
                    select fac).ToListAsync();
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

        /// <summary>
        /// Retrieves the base dataset for the AutoBilling grid, applying optional filters and pagination.
        /// </summary>
        /// <param name="status">
        /// A list of invoice statuses used to filter results.
        /// If null or empty, all invoices are returned.
        /// </param>
        /// <param name="offset">The number of records to skip, used for pagination.</param>
        /// <param name="limit">The maximum number of records to retrieve.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains a <see cref="List{InvoiceModel}"/> collection representing
        /// the filtered invoices ordered by creation date (descending).
        /// </returns>
        public async Task<List<InvoiceModel>> GetAutoBillingBaseAsync(List<string> status, int offset, int limit)
        {
            return await this.context.Invoice
                .AsNoTracking()
                .Where(x => status == null || status.Count == 0 || status.Contains(x.Status))
                .OrderByDescending(x => x.InvoiceCreateDate)
                .Skip(offset)
                .Take(limit)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves all SAP order entities associated with a collection of invoices.
        /// Results are grouped by invoice ID and returned as a dictionary.
        /// </summary>
        /// <param name="invoiceIds">A list of invoice identifiers for which to load SAP orders.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains a dictionary where each key represents an invoice ID and the value
        /// is a list of <see cref="InvoiceSapOrderModel"/> objects linked to that invoice.
        /// </returns>
        public async Task<Dictionary<string, List<InvoiceSapOrderModel>>> GetSapOrdersByInvoiceIdsAsync(List<string> invoiceIds)
        {
            return await this.context.InvoiceSapOrderModel
                .AsNoTracking()
                .Where(s => invoiceIds.Contains(s.IdInvoice))
                .GroupBy(s => s.IdInvoice)
                .ToDictionaryAsync(g => g.Key, g => g.ToList())
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves all remission entities associated with a collection of invoices.
        /// Results are grouped by invoice ID and returned as a dictionary.
        /// </summary>
        /// <param name="invoiceIds">A list of invoice identifiers for which to load remission data.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains a dictionary where each key represents an invoice ID and the value
        /// is a list of <see cref="InvoiceRemissionModel"/> objects linked to that invoice.
        /// </returns>
        public async Task<Dictionary<string, List<InvoiceRemissionModel>>> GetRemissionsByInvoiceIdsAsync(List<string> invoiceIds)
        {
            return await this.context.Remissions
                .AsNoTracking()
                .Where(r => invoiceIds.Contains(r.IdInvoice))
                .GroupBy(r => r.IdInvoice)
                .ToDictionaryAsync(g => g.Key, g => g.ToList())
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves the total number of invoices matching the provided AutoBilling status filters.
        /// </summary>
        /// <param name="status">
        /// A list of invoice statuses used to filter results.
        /// If null or empty, all invoices are counted.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains the total number of matching invoices.
        /// </returns>
        public async Task<int> GetAutoBillingCountAsync(List<string> status)
        {
            return await this.context.Invoice
                .AsNoTracking()
                .Where(x => status == null || status.Count == 0 || status.Contains(x.Status))
                .CountAsync()
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<List<InvoiceErrorModel>> GetExistingErrorsByCodes(List<string> codes)
        {
            return await this.context.InvoiceError
                .Where(x => codes.Contains(x.Code))
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task InsertInvoiceErrors(List<InvoiceErrorModel> invoiceErrors)
        {
            this.context.InvoiceError.AddRange(invoiceErrors);
            await this.context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task UpdateInvoiceErrors(List<InvoiceErrorModel> invoiceErrors)
        {
            this.context.InvoiceError.UpdateRange(invoiceErrors);
            await this.context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task<List<InvoiceModel>> GetInvoicesByPedidoSap(List<int> pedidosSap, int offset, int limit)
        {
            return await this.context.Invoice
                .Where(i => i.SapOrders.Any(so => pedidosSap.Contains(so.SapOrderId)))
                .OrderBy(x => x.CreateDate)
                .Skip(offset)
                .Take(limit)
                .Include(i => i.Remissions)
                .Include(i => i.SapOrders)
                .Include(i => i.InvoiceError)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<List<InvoiceModel>> GetInvoicesByPedidoDxp(string pedidoDxp, int offset, int limit)
        {
            return await this.context.Invoice
                .Where(x => x.DxpOrderId.Contains(pedidoDxp))
                .OrderBy(x => x.CreateDate)
                .Skip(offset)
                .Take(limit)
                .Include(x => x.Remissions)
                .Include(x => x.SapOrders)
                .Include(x => x.InvoiceError)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<List<InvoiceModel>> GetInvoicesByInvoiceId(string id, int offset, int limit)
        {
            return await this.context.Invoice
                .Where(x => x.Id.Contains(id))
                .OrderBy(x => x.CreateDate)
                .Skip(offset)
                .Take(limit)
                .Include(x => x.Remissions)
                .Include(x => x.SapOrders)
                .Include(x => x.InvoiceError)
                .ToListAsync();
        }

        private IQueryable<InvoiceModel> BuildInvoiceFilter(
            List<string> status,
            List<string> typeInvoices,
            List<string> billingTypes)
        {
            var filter = this.context.Invoice
                .Where(x => status.Contains(x.Status));

            if (typeInvoices != null && typeInvoices.Any())
            {
                filter = filter.Where(x => typeInvoices.Contains(x.TypeInvoice));
            }

            if (billingTypes != null && billingTypes.Any())
            {
                filter = filter.Where(x => billingTypes.Contains(x.BillingType));
            }

            return filter;
        }
    }
}
