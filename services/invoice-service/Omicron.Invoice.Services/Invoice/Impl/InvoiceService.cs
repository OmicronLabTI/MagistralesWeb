// <summary>
// <copyright file="InvoiceService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Services.Invoice.Impl
{
    /// <summary>
    /// InvoiceService class.
    /// </summary>
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceDao invoiceDao;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceService"/> class.
        /// </summary>
        /// <param name="mapper">Mapper.</param>
        /// <param name="invoiceDao">Users dao.</param>
        public InvoiceService(IMapper mapper, IInvoiceDao invoiceDao)
            => (this.mapper, this.invoiceDao) = (mapper, invoiceDao);

        /// <inheritdoc/>
        public async Task<IEnumerable<UserDto>> GetAllAsync()
            => this.mapper.Map<IEnumerable<UserDto>>(
                await this.invoiceDao.GetAllAsync());
    }
}
