// <summary>
// <copyright file="ISapInvoiceService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Services.Sap
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Omicron.SapAdapter.Entities.Model;

    /// <summary>
    /// Interface for sap invoices.
    /// </summary>
    public interface ISapInvoiceService
    {
        /// <summary>
        /// Gets the deliveries to return.
        /// </summary>
        /// <param name="parameters">the parameters to look.</param>
        /// <returns>The data.</returns>
        Task<ResultModel> GetInvoice(Dictionary<string, string> parameters);

        /// <summary>
        /// Get the invoice products.
        /// </summary>
        /// <param name="invoiceId">the invoice id.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetInvoiceProducts(int invoiceId);
    }
}
