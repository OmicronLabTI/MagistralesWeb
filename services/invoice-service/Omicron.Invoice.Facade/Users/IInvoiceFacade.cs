// <summary>
// <copyright file="IInvoiceFacade.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Facade.Users
{
    /// <summary>
    /// Interface IInvoiceFacade.
    /// </summary>
    public interface IInvoiceFacade
    {
        /// <summary>
        /// Method to Create invoice.
        /// </summary>
        /// <param name="request"> request invoice. </param>
        /// <returns> order data. </returns>
        Task<ResultDto> CreateInvoice(CreateInvoiceDto request);

        /// <summary>
        /// Method to Create invoice.
        /// </summary>
        /// <param name="remissions"> request invoice. </param>
        /// <returns> order data. </returns>
        Task<ResultDto> GetInvoicesByRemissionId(List<int> remissions);
    }
}