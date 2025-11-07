// <summary>
// <copyright file="IInvoiceService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Services.Invoice
{
    using System.Threading.Tasks;
    using Omicron.SapAdapter.Dtos.Models;

    /// <summary>
    /// interface for get invoice.
    /// </summary>
    public interface IInvoiceService
    {
        /// <summary>
        /// Sends a POST request to the specified route with the provided order data.
        /// </summary>
        /// <param name="dataToSend"> An object containing the data to be sent in the request body. </param>
        /// <param name="route"> The API endpoint route where the POST request will be sent. </param>
        /// <returns>
        /// A <see cref="ResultDto"/> containing the response from the server, including any relevant status or returned data.
        /// </returns>
        Task<ResultDto> PostAsync(object dataToSend, string route);
    }
}
