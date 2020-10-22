// <summary>
// <copyright file="ISapFileService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapFile.Services.SapFile
{
    using Omicron.SapFile.Entities.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface to create the pdfs.
    /// </summary>
    public interface ISapFileService
    {
        /// <summary>
        /// Generates the pdf and send to append them.
        /// </summary>
        /// <param name="finalizaGeneratePdfs">the data to create.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> CreatePdfs(List<FinalizaGeneratePdfModel> finalizaGeneratePdfs);

        /// <summary>
        /// Creates the sale order pdf.
        /// </summary>
        /// <param name="ordersId">the orders id.</param>
        /// <returns>the data to return.</returns>
        Task<ResultModel> CreateSaleOrderPdf(List<int> ordersId);

        /// <summary>
        /// Deetes the files.
        /// </summary>
        /// <returns>the data.</returns>
        Task<ResultModel> DeleteFiles();
    }
}
