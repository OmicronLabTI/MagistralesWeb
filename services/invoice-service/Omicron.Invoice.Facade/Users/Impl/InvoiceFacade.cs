// <summary>
// <copyright file="InvoiceFacade.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Facade.Users.Impl
{
    /// <summary>
    /// Class InvoiceFacade.
    /// </summary>
    public class InvoiceFacade : IInvoiceFacade
    {
        private readonly IInvoiceService invoiceService;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceFacade"/> class.
        /// </summary>
        /// <param name="invoiceService">Interface InvoiceService.</param>
        public InvoiceFacade(IInvoiceService invoiceService) =>
            this.invoiceService = invoiceService ?? throw new ArgumentNullException(nameof(invoiceService));

        /// <inheritdoc/>
        public async Task<IEnumerable<UserDto>> GetAllAsync()
            => await this.invoiceService.GetAllAsync();

        /// <inheritdoc/>
        public async Task<UserDto> GetByIdAsync(int id)
            => await this.invoiceService.GetByIdAsync(id);

        /// <inheritdoc/>
        public async Task<UserDto> InsertAsync(string user, CreateUserDto userRequest)
            => await this.invoiceService.InsertAsync(user, userRequest);

        /// <inheritdoc/>
        public async Task<UserDto> UpdateAsync(
            int id, string user, UpdateUserDto userRequest)
            => await this.invoiceService.UpdateAsync(id, user, userRequest);

        /// <inheritdoc/>
        public async Task DeleteAsync(int id)
            => await this.invoiceService.DeleteAsync(id);
    }
}