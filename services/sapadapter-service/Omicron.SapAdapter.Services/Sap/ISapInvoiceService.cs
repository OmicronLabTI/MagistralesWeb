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
    using Omicron.SapAdapter.Entities.Model.AlmacenModels;

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

        /// <summary>
        /// Gets the data of the delivey.
        /// </summary>
        /// <param name="code">the code.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetDeliveryScannedData(string code);

        /// <summary>
        /// Gets the scanned data for magitral product.
        /// </summary>
        /// <param name="code">the code.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetMagistralProductInvoice(string code);

        /// <summary>
        /// Gets the scanned data for magitral product.
        /// </summary>
        /// <param name="code">the code.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetLineProductInvoice(string code);

        /// <summary>
        /// Gets the data To look.
        /// </summary>
        /// <param name="dataToLook">the data to look.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetInvoiceHeader(InvoicePackageSapLookModel dataToLook);

        /// <summary>
        /// Gets the code for the invoice.
        /// </summary>
        /// <param name="code">the invoice.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetInvoiceData(string code);

        /// <summary>
        /// Gets the delivery and invoice id by sales id.
        /// </summary>
        /// <param name="salesIds">the sales id separated by coma.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetSapIds(List<int> salesIds);

        /// <summary>
        /// Gets the cancelled invoices.
        /// </summary>
        /// <param name="days">the days.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetCancelledInvoices(int days);
    }
}
