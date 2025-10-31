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
        public async Task<IEnumerable<UserModel>> GetAllAsync()
            => await this.context.Users.ToListAsync();

        /// <inheritdoc/>
        public async Task<UserModel> GetByIdAsync(int id)
            => await this.context.Users.FindAsync(id);

        /// <inheritdoc/>
        public async Task InsertAsync(UserModel model)
            => await this.context.AddAsync(model);

        /// <inheritdoc/>
        public UserModel Update(UserModel model)
            => this.context.Update(model).Entity;

        /// <inheritdoc/>
        public void Delete(UserModel model)
            => this.context.Remove(model);

        /// <inheritdoc/>
        public async Task<int> SaveChangesAsync() => await this.context.SaveChangesAsync();

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
                                    err.RequireManualChange == false
                                    || (err.RequireManualChange == true && fac.ManualChangeApplied == true))
                          select fac).ToListAsync();
        }
    }
}
