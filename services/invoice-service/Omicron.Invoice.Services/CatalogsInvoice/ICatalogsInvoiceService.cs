// <summary>
// <copyright file="ICatalogsInvoiceService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Services.CatalogsInvoice
{
    /// <summary>
    /// Interface ICatalogsInvoiceService.
    /// </summary>
    public interface ICatalogsInvoiceService
    {
        /// <summary>
        /// PostInvoiceErrorsFromExcel.
        /// </summary>
        /// <returns> A <see cref="Task{ResultDto}"/> representing the result of the asynchronous operation. </returns>
        Task<ResultDto> InvoiceErrorsFromExcel();
    }
}
